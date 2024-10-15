using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity;
    private RectTransform myTransform;
    private CharacterStats myStats;
    private Slider slider;

    private void Start(){
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        myStats = GetComponentInParent<CharacterStats>();
        slider = GetComponentInChildren<Slider>();


        entity.onFlipped += FlipUI;
        myStats.onHealthChange += UpdateHealthUI;

        UpdateHealthUI();
    }

    private void Update(){
        // UpdateHealthUI();
    }

    private void UpdateHealthUI(){
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }



    private void FlipUI() => myTransform.Rotate(0,180,0);
    private void OnDisable(){
        entity.onFlipped -= FlipUI;
        myStats.onHealthChange -= UpdateHealthUI;
    } 
}
