using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;


    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_equipment, InventoryItem> equipmentDictionary;



    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlots;
    private UI_ItemSlot[] stashItemSlots;
    private UI_EquipmentSlot[] equipmentSlots;
    private UI_StatSlot[] statSlot;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    public float flaskCooldown{get; private set;} = 2;
    private float armorCooldown;

    [Header("Data base")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_equipment> loadedEquipment;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    private void Start() {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_equipment, InventoryItem>();

        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlots = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItems();
    }

    private void AddStartingItems(){

        foreach(ItemData_equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        if(loadedItems.Count > 0)
        {
            foreach(InventoryItem item in loadedItems)
            {
                for(int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }

            return;
        }
        for(int i = 0; i < startingItems.Count; i++){
            if( startingItems[i] != null)
                AddItem(startingItems[i]);
        }
    }

    public void EquipItem(ItemData _item){
        ItemData_equipment newEquipment = _item as ItemData_equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_equipment oldEquipment = null;

        foreach(KeyValuePair<ItemData_equipment, InventoryItem> item in equipmentDictionary){
            if(item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;
        }

        if(oldEquipment != null){
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }


        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();
        RemoveItem(_item);

        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_equipment itemToRemove){
        if(equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value)){
            
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }


    private void UpdateSlotUI(){
        
        for(int i = 0; i < equipmentSlots.Length; i++){

            foreach(KeyValuePair<ItemData_equipment, InventoryItem> item in equipmentDictionary){
                if(item.Key.equipmentType == equipmentSlots[i].slotType)
                    equipmentSlots[i].UpdateSlot(item.Value);
            }

        }
        
        for(int i = 0; i < inventoryItemSlots.Length; i++){
            inventoryItemSlots[i].CleanUpSlot();
        }
        
        for(int i = 0; i < stashItemSlots.Length; i++){
            stashItemSlots[i].CleanUpSlot();
        }


        for(int i = 0; i < inventory.Count; i++){
            inventoryItemSlots[i].UpdateSlot(inventory[i]);
        }
        
        for(int i = 0; i < stash.Count; i++){
            stashItemSlots[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();

    }

    public void UpdateStatsUI(){
        
        for(int i = 0; i < statSlot.Length; i++){
            statSlot[i].UpdateStatValueUI();
        }
    }
    
    public void AddItem(ItemData _item){
        if(_item.itemType == ItemType.Equipment && CanAddItem()){
            AddToInventory(_item);
        }else if(_item.itemType == ItemType.Material){
            AddToStash(_item);
        }

        UpdateSlotUI();
    }

    private void AddToInventory(ItemData _item){
        if(inventoryDictionary.TryGetValue(_item, out InventoryItem value)){
            value.AddStack();
        }else{
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }

    }

    private void AddToStash(ItemData _item){
        if(stashDictionary.TryGetValue(_item, out InventoryItem value)){
            value.AddStack();
        }else{
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }

    }

    public void RemoveItem(ItemData _item){
        if(inventoryDictionary.TryGetValue(_item, out InventoryItem value)){
            if(value.stackSize <= 1){
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }else{
                value.RemoveStack();
            }
        }
        
        if(stashDictionary.TryGetValue(_item, out InventoryItem stashvalue)){
            if(stashvalue.stackSize <= 1){
                stash.Remove(stashvalue);
                stashDictionary.Remove(_item);
            }else{
                stashvalue.RemoveStack();
            }
        }

        UpdateSlotUI();
    }

    public bool CanAddItem(){
        if(inventory.Count >= inventoryItemSlots.Length){
            // Debug.Log("no more space");
            return false;
        }

        return true;
    }

    public List<InventoryItem> GetEquipmentsList() => equipment;
    public List<InventoryItem> GetStarshList() => stash;

    public ItemData_equipment GetEquipment(EquipmentType _type){

        ItemData_equipment equipmentItem = null;
        
        foreach(KeyValuePair<ItemData_equipment, InventoryItem> item in equipmentDictionary){
            if(item.Key.equipmentType == _type)
                equipmentItem = item.Key;
        }

        return equipmentItem;
    }

    public void UseFlask(){
        ItemData_equipment currentFlask = GetEquipment(EquipmentType.Flask);
        if(currentFlask == null)
            return;
        
        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if(canUseFlask){
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }else
            Debug.Log("Flask on cooldown");
    }

    public bool CanUseArmor(){
        ItemData_equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if(Time.time > lastTimeUsedArmor + armorCooldown){
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("Armor on cooldown");
        return false;
    }

    public bool CanCraft(ItemData_equipment _itemToCraft, List<InventoryItem> _requiredMaterials){
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for(int i = 0; i < _requiredMaterials.Count; i++){
            if(stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue)){
                if(stashValue.stackSize < _requiredMaterials[i].stackSize){
                    Debug.Log("not enough materials");
                    return false;
                }else{
                    materialsToRemove.Add(stashValue);
                }
            }else{
                Debug.Log("not enough materials");
                return false;
            }
        }

        for(int i = 0; i < materialsToRemove.Count; i++){
            RemoveItem(materialsToRemove[i].data);
        }

        AddItem(_itemToCraft);
        Debug.Log("here is your item " + _itemToCraft.name);
        return true;

    }

    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach(var item in itemDataBase)
            {
                if(item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;
                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach (string loadedItemId in _data.equipmentId)
        {
            foreach(var item in itemDataBase)
            {
                if(item != null && loadedItemId == item.itemId)
                {
                    loadedEquipment.Add(item as ItemData_equipment);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();
        
        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }

    }


#if UNITY_EDITOR
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] {"Assets/Data/Items"});

        foreach(string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
    
#endif


}
