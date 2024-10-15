using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LostCurrencyController : MonoBehaviour
{

    public int currency;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Player>() != null)
        {
            PlayerManager.instance.currency += currency;
            Destroy(this.gameObject);
        }
    }
    
}
