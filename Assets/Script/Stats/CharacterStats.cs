using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum StatType{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}
public class CharacterStats : MonoBehaviour
{

    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;//1点 增加一点crit.power
    public Stat agility;//每点增加1点闪避, crit.chance
    public Stat intelligence;//每点增加1点魔法伤害和魔法抵抗
    public Stat vitality;//没电增加生命3-5点

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance; //defalut value 150%
    public Stat critPower;


    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited;  //随时间收到伤害
    public bool isChilled;//减少20%护甲
    public bool isShocked;//减少20%accuracy


    [SerializeField] private float alimentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float igniteDamageCoolDown = .3f;
    private float igniteDamageTimer;

    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;



    public int currentHealth;

    public System.Action onHealthChange;

    public bool isDead{get; private set;}
    public bool isInvincible{get; private set;}
    private bool isVulnerable;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update() {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        if(ignitedTimer < 0)
            isIgnited = false;

        if(chilledTimer < 0)
            isChilled = false;

        if(shockedTimer < 0)
            isShocked = false;

        if(isIgnited)
            ApplyIgniteDamage();
        
    }


    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableForCorutine(_duration));

    private IEnumerator VulnerableForCorutine(float _duration){
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }
    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify){
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify){
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }


    public virtual void DoDamage(CharacterStats _tartgetStats){
        
        bool criticalStrike = false;

        if(TargetCanAvoidAttack(_tartgetStats))
            return;

        _tartgetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if(CanCrit()){
            totalDamage = CalculateCriticalDamage(totalDamage);
            // Debug.Log("Total crit damage is "+ totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFx(_tartgetStats.transform, criticalStrike);

        totalDamage = CheckTargetArmor(_tartgetStats, totalDamage);

        _tartgetStats.TakeDamage(totalDamage);  //物理伤害
        DoMagicalDamage(_tartgetStats);//检查魔法伤害
    }


    # region Magical damage and ailemnts 魔法伤害与状态
    public virtual void DoMagicalDamage(CharacterStats _targetStats){
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        if(Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;


        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightningDamage);

    }

    private void AttemptyToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage){
        
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _iceDamage && _lightningDamage > _fireDamage;

        while(!canApplyIgnite && !canApplyChill && ! canApplyShock){
            if(Random.value < .3f && _fireDamage > 0){
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied fire");
                return;
            }

            if(Random.value < .5f && _iceDamage > 0){
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied ice");
                return;
            }

            if(Random.value < .5f && _lightningDamage > 0){
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Applied lightning");
                return;
            }
        }

        if(canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if(canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);

    }


    public void ApplyAilments(bool _ignite, bool _chill, bool _shock){
        // if(isIgnited || isChilled || isShocked)
        //     return;

        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if(_ignite && canApplyIgnite){
            isIgnited = _ignite;
            ignitedTimer = alimentsDuration;
            fx.IgniteFxFor(alimentsDuration);
        }

        if(_chill && canApplyChill){
            isChilled = _chill;
            chilledTimer = alimentsDuration;

            float slowPercentage = .2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, alimentsDuration);

            fx.ChillFxFor(alimentsDuration);
        }

        if(_shock && canApplyShock){
            
            if(!isShocked){

                ApplyShock(_shock);

            }else{
                //找到最近敌人

                if(GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithShockStrike();
                
            }
        }

        
    }

    public void ApplyShock(bool _shock){
        if(isShocked)
            return;

        isShocked = _shock;
        shockedTimer = alimentsDuration;
        fx.ShockFxFor(alimentsDuration);
    }

    private void HitNearestTargetWithShockStrike(){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach(var hit in colliders){
            if(hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if(distanceToEnemy < closestDistance)
                {
                    closestEnemy = hit.transform;
                    closestDistance = distanceToEnemy;
                }
            }

            if(closestEnemy == null)
                closestEnemy = transform;
        }
        //实例化雷电
        //调用雷电setup

        if(closestEnemy != null){
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }

    }

    
    private void ApplyIgniteDamage(){
        if(igniteDamageTimer < 0){
            // Debug.Log("Take burn damge");
            igniteDamageTimer = igniteDamageCoolDown;

            currentHealth -= igniteDamage;

            if(currentHealth < 0 && !isDead)
                Die();
            

        }
    }
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion


    public virtual void TakeDamage(int _damage){

        if(isInvincible)
            return;

        DecreaseHealthBy(_damage);

        if(currentHealth < 0 && !isDead){
            Die();
        }
    }

    public virtual void IncreaseHealthBy(int _amount){
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if(onHealthChange != null)
            onHealthChange();
    }

    protected virtual void DecreaseHealthBy(int _damage){
        
        if(isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        currentHealth -= _damage;
        
        if(_damage > 0)
        {
            fx.CreatePopUpText(_damage.ToString());
        }

        if(onHealthChange !=null)
            onHealthChange();
    }

    protected virtual void Die(){
        isDead = true;
    }

    public void KillEntity()
    {
        if (!isDead)
            Die();

    } 

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;


    #region Stat calculations
    protected int CheckTargetArmor(CharacterStats _tartgetStats, int totalDamage){
        if(_tartgetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_tartgetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _tartgetStats.armor.GetValue();
        
        
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    
    private int CheckTargetResistance(CharacterStats _tartgetStats, int totalMagicalDamage){
        totalMagicalDamage -= _tartgetStats.magicResistance.GetValue() + (_tartgetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;

    }

    public virtual void onEvasion(){

    }
    protected bool TargetCanAvoidAttack(CharacterStats _tartgetStats){


        int totalEvasion = _tartgetStats.evasion.GetValue() + _tartgetStats.agility.GetValue();

        if(isShocked)
            totalEvasion += 20;

        if(Random.Range(0,100) < totalEvasion){
            // Debug.Log("Attack Avoided");
            _tartgetStats.onEvasion();
            return true;
        }

        return false;
    }

    protected bool CanCrit(){
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) <= totalCriticalChance){
            return true;
        }

        return false;
    }

    protected int CalculateCriticalDamage(int _damage){
        float totalCirtPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        Debug.Log("total Crit Power: " + totalCirtPower);

        float critDamage = _damage * totalCirtPower;
        Debug.Log("crit damage before round up: " + critDamage);

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue(){
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    #endregion


    
    public Stat StatToModify(StatType _statType){

        if(_statType == StatType.strength) return strength;
        else if(_statType == StatType.agility) return agility;
        else if(_statType == StatType.intelligence) return intelligence;
        else if(_statType == StatType.vitality) return vitality;
        else if(_statType == StatType.damage) return damage;
        else if(_statType == StatType.critChance) return critChance;
        else if(_statType == StatType.critPower) return critPower;
        else if(_statType == StatType.health) return maxHealth;
        else if(_statType == StatType.armor) return armor;
        else if(_statType == StatType.evasion) return evasion;
        else if(_statType == StatType.magicRes) return magicResistance;
        else if(_statType == StatType.fireDamage) return fireDamage;
        else if(_statType == StatType.iceDamage) return iceDamage;
        else if(_statType == StatType.lightingDamage) return lightningDamage;

        return null;
    }

}
