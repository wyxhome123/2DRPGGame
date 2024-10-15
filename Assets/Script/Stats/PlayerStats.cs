using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{

    private Player player;
    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        player.DamageEffect();
    }

    protected override void Die()
    {
        base.Die();

        player.Die();

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

        GetComponent<PlayerItemDrop>()?.GenerateDrop();

    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);
        if(isDead)
            return;
        
        if(_damage > GetMaxHealthValue() * .3f)
        {
            player.SetupKnockbackPower(new Vector2(10, 6));
            player.fx.ScreenShake(player.fx.shakeHighDamage);
        }
        ItemData_equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if(currentArmor != null)
            currentArmor.Effect(player.transform);
    }

    public override void onEvasion()
    {
        player.skill.dodge.CreateMirageonDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier){
        
        if(TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if(_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        if(CanCrit()){
            totalDamage = CalculateCriticalDamage(totalDamage);
            Debug.Log("Total crit damage is "+ totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);  //物理伤害
        DoMagicalDamage(_targetStats);//检查魔法伤害
    }

}
