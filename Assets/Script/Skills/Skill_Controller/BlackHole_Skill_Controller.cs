using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeTimer;


    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool playerCanDisapear = true;


    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createHotKey = new List<GameObject>();

    public bool playerCanExitState{get; private set;}

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shringkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _blackholeDuration){
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shringkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;

        blackholeTimer = _blackholeDuration;
        
        if(SkillManager.instance.clone.crystalInsteadOfClone)
            playerCanDisapear = false;
    }

    private void Update() {

        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if(blackholeTimer < 0){
            blackholeTimer = Mathf.Infinity;

            if(targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackHoleAbility();
        }

        if(Input.GetKeyDown(KeyCode.R)){
            ReleaseCloneAttack();
        }

        CloneAttackLogic();


        if(canGrow && !canShrink){
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);

        }

        if(canShrink){
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1,-1), shrinkSpeed * Time.deltaTime);

            if(transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if(targets.Count <= 0)
            return;

        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;

        if(playerCanDisapear){
            playerCanDisapear = false;
            PlayerManager.instance.player.fx.Maketransprent(true);
        }

    }
    private void CloneAttackLogic(){
        if(cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0){
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;
            if(Random.Range(0, 100) > 50){
                xOffset = 2;
            }else{
                xOffset = -2;
            }

            if(SkillManager.instance.clone.crystalInsteadOfClone){
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }else{
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));

            }
            amountOfAttacks--;

            if(amountOfAttacks <= 0){
                Invoke("FinishBlackHoleAbility", 1f);
            }

        }
    }

    private void FinishBlackHoleAbility(){
        DestroyHotKeys();
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void DestroyHotKeys(){
        if(createHotKey.Count <= 0)
            return;

        for(int i = 0; i < createHotKey.Count; i++){
            Destroy(createHotKey[i]);
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Enemy>() != null){
            other.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(other);


            // targets.Add(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.GetComponent<Enemy>() != null){
            other.GetComponent<Enemy>().FreezeTime(false);
        }
        
    }
    private void CreateHotKey(Collider2D other){
        if(keyCodeList.Count <= 0){
            Debug.Log("Not enough hot keys in a key code list");
            return;
        }

        if(!canCreateHotKeys)
            return;
        
        GameObject newHotKey = Instantiate(hotKeyPrefab, other.transform.position + new Vector3(0,2), Quaternion.identity);
        createHotKey.Add(newHotKey);

        KeyCode chooseKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(chooseKey);

        BlackHole_HotKeyController newHotKeyScript = newHotKey.GetComponent<BlackHole_HotKeyController>();

        newHotKeyScript.SetupHotKey(chooseKey, other.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
