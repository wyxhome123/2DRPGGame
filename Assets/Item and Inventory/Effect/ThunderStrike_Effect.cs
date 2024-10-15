using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Thunder strike effect", menuName ="Data/Item Effect/Tunder strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyPositon)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPositon.position, Quaternion.identity);
        // base.ExecuteEffect();
        Destroy(newThunderStrike, .5f);
    }
}
