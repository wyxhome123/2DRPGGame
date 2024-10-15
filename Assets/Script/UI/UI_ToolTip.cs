using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;

    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;
    

    public virtual void AdjustPosition(){
        Vector2 mousePosition = Input.mousePosition;

        float newXoffset = 0;
        float newYoffset = 0;
        if(mousePosition.x > 600)
            newXoffset = -100;
        else
            newXoffset = 100;

        
        if(mousePosition.y > 320)
            newYoffset = -100;
        else
            newYoffset = 100;

        transform.position = new Vector2(mousePosition.x + newXoffset, mousePosition.y + newYoffset);
    }

    public void AdjustFontSize(TextMeshProUGUI _text){
        if(_text.text.Length > 12){
            _text.fontSize = _text.fontSize * .8f;
        }
    }
}
