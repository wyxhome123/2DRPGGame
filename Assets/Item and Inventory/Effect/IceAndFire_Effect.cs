using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;



[CreateAssetMenu(fileName ="Ice and Fire effect", menuName ="Data/Item Effect/Ice and Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _respawnPositon)
    {
        Player player = PlayerManager.instance.player;
        bool thirdAttack = player.GetComponent<Player>().primaryAttack.comboCounter == 2;
        
        if(thirdAttack){
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respawnPositon.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.faceDir, 0);

            Destroy(newIceAndFire, 10);

        }

    }
    
}
