using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{

    private Animator anim;
    private Rigidbody2D rb;

    private CircleCollider2D cd;
    private Player player;

    private bool canRotate;
    private bool isReturning;

    private float freezeTimeDuration;
    private float returnSpeed = 12;

    [Header("Pierce Info")]
    [SerializeField] private float pierceAmount;

    [Header("Bounce Info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;


    [Header("Spin Info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;



    private void Awake() {
        canRotate = true;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;
        // Debug.Log(rb);
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if(pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void ReturnSword()
    {
        //rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;
    }

    private void Update() {
        if(canRotate)
            transform.right = rb.velocity;

        if(isReturning){
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
            if(Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword();
        }

        BounceLogic();

        SpinLogic();

    }

    private void SpinLogic()
    {
        if(isSpinning)
        {
            if(Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if(wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection,transform.position.y), 1.5f * Time.deltaTime);
                if(spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;
                if(hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                    foreach(var hit in colliders){

                        if(hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                    }


                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic(){

        if(isBouncing && enemyTarget.Count > 0){
            // Debug.Log("Bouncing");
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);
            if(Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f){
                
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                targetIndex++;
                bounceAmount--;

                if(bounceAmount <= 0){
                    isBouncing = false;
                    isReturning = true;
                }

                if(targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isReturning)
            return;
        
        if(other.GetComponent<Enemy>() != null){
            Enemy enemy = other.GetComponent<Enemy>();

            SwordSkillDamage(enemy);
        }

        SetupTargetForBounce(other);


        StuckInto(other);

    }

    private void SwordSkillDamage(Enemy enemy){

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        enemy.DamageEffect();

        if(player.skill.sword.timeStopUnlocked)
            enemy.FreezeTimeFor(freezeTimeDuration);

        if(player.skill.sword.volnurableUnlocked)
            enemyStats.MakeVulnerableFor(freezeTimeDuration);

        ItemData_equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if(equipedAmulet != null)
            equipedAmulet.Effect(enemy.transform);
    }

    private void SetupTargetForBounce(Collider2D other){
        if(other.GetComponent<Enemy>() != null){
            if(isBouncing && enemyTarget.Count <= 0){
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
                foreach(var hit in colliders){
                    if(hit.GetComponent<Enemy>() != null){
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }
    }

    private void StuckInto(Collider2D other)
    {
        
        if(pierceAmount > 0 && other.GetComponent<Enemy>() != null){
            pierceAmount--;
            return;
        }

        if(isSpinning){
            //bug: 碰到enemy会重制spinTimer
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponentInChildren<ParticleSystem>().Play();

        if(isBouncing && enemyTarget.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = other.transform;

    }


}
