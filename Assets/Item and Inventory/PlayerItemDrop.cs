using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterial;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialsToLoose = new List<InventoryItem>();

        foreach (InventoryItem item in inventory.GetEquipmentsList())
        {
            if(Random.Range(0, 100) <= chanceToLooseItems){
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }
        for(int i = 0; i < itemsToUnequip.Count; i++){

            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_equipment);
        }

        
        foreach (InventoryItem item in inventory.GetStarshList())
        {
            if(Random.Range(0, 100) <= chanceToLooseMaterial){
                DropItem(item.data);
                materialsToLoose.Add(item);
            }
        }
        for(int i = 0; i < materialsToLoose.Count; i++){

            inventory.UnequipItem(materialsToLoose[i].data as ItemData_equipment);
        }




    }
}
