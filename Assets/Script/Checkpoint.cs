using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string Id;
    public bool activationStatus;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();    
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId(){
        Id = System.Guid.NewGuid().ToString();
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        activationStatus = true;
        anim.SetBool("active", true);
    }

}
