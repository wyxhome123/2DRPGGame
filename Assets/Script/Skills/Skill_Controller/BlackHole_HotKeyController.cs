using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BlackHole_HotKeyController : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    private Transform myEnemy;
    private BlackHole_Skill_Controller blackHole;

    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, BlackHole_Skill_Controller _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blackHole = _myBlackHole;

        myText.text = _myNewHotKey.ToString();
        myHotKey = _myNewHotKey;

    }

    private void Update() {
        if(Input.GetKeyDown(myHotKey)){
            // Debug.Log("Hot Key is " + myHotKey);
            // blackHole.targets.Add(myEnemy);
            blackHole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
