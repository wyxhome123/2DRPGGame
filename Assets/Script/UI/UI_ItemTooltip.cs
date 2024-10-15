using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private int defaultFontSize = 32;

    public void ShowToolTip(ItemData_equipment item){
        if(item == null)
            return;
        
        itemNameText.text = item.itemName;
        itemTypeText.text = item.equipmentType.ToString();
        itemDescription.text = item.GetDescription();

        AdjustPosition();
        AdjustFontSize(itemNameText);


        gameObject.SetActive(true);
    }

    public void HideToolTip(){
        itemNameText.fontSize = defaultFontSize;
        gameObject.SetActive(false);
        
    }
    
}
