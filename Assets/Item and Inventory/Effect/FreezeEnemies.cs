using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "Freeze enemies effect", menuName = "Data/Item Effect/Freeze enemies")]
public class FreezeEnemies : ItemEffect
{
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform _enemyPositon)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerStats.currentHealth > playerStats.GetMaxHealthValue() * .1f)
            return;

        if(!Inventory.instance.CanUseArmor())
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_enemyPositon.position, 2);
        foreach(var hit in colliders){
            if(hit.GetComponent<Enemy>() != null){

                
                hit.GetComponent<Enemy>().FreezeTimeFor(duration);
            }
        }
    }
}
