using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Knockback Info")]
    [SerializeField] protected Vector2 knockbackPower;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;
    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int knockbackDir{get; private set;}
    
    public EntityFX entityFX{get; private set;}


#region 组件
    public Animator anim{get; private set;}
    public Rigidbody2D rb;

    public SpriteRenderer sr{get; private set;}

    public CharacterStats stats{get; private set;}

    public CapsuleCollider2D cd{get; private set;}

#endregion
    
    public int faceDir{get; private set;} = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;
    protected virtual void Awake() {
        
    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        entityFX = GetComponent<EntityFX>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {
        
    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration){

    }

    protected virtual void ReturnDefaultSpeed(){
        anim.speed = 1;
    }

    public virtual void DamageEffect(){
        entityFX.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");
        // Debug.Log(gameObject.name + "was Damaged");
    }

    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;
    }

    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower;
    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        rb.velocity = new Vector2(knockbackPower.x * -faceDir, knockbackPower.y);
        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    
#region 碰撞检测

    public virtual bool isGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool isWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos(){
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

#endregion


#region 翻转
    public virtual void Flip()
    {
        faceDir = faceDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0,180,0);

        if(onFlipped != null)
            onFlipped();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }

#endregion


#region  速度 Velocity
    public void SetZeroVelocity(){
        if(isKnocked)
            return;
        
        rb.velocity = new Vector2(0, 0);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity){
        if(isKnocked)
            return;

        
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

#endregion


    public virtual void Die(){

    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }

}
