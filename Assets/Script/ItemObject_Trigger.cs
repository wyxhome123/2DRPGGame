using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject => GetComponentInParent<ItemObject>();

    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Player>() != null){
            if(other.GetComponent<CharacterStats>().isDead)
                return;
            // Debug.Log("OnTriggerEnter2D");
            myItemObject.PickupItem();
        }
    }
}
