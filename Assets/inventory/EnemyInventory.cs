using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : Inventory {

    public GameObject itemPrefab;   //The prefab of the item enemy can throw

    public int maxItem;

    public float reloadTime;

    public float speedOfThrow;
    
    ItemBase itemStored;
    
    // Use this for initialization
    public void Initialize () {

        character = GetComponentInParent<Character>();
        CreateSlot();

        itemStored = ItemManager.instance.GetItemReference(itemPrefab.GetComponent<ItemBase>().itemProperties.itemDescription.GetItemType());
        itemStored.GetComponent<ItemBase>().isFromEnemy = true;

        for (int i = 0; i<slots.Count; i++)
        {
            slots[i].maxStorage = 10;
            slots[i].imageSlotPrefab.transform.SetParent(transform);
        }

        //adding the item once is enough
        //Add();
        //Adding items to inventory
        for (int i = 0; i < maxItem; i++)
        {
            Add();
        }

        //GameObject temp = Instantiate(itemPrefab, GetComponentInParent<Character>().Hand.transform.position, Quaternion.identity) as GameObject;
        //itemStored = temp.GetComponent<ItemBase>();
        

        SetActiveItem();   
    }

    public void SetActiveItem()
    {
        Debug.Log("Set active item is called in enemy" );

        for (int j = 0; j < slots[0].itemCount; j++)
        {
            //if (!slots[0].itemlist[j].gameObject.activeSelf)
            if (activeItem != null)
            {
                activeItem.gameObject.SetActive(true);
                activeItem.isFromEnemy = true;

                //Setting the parent
                activeItem.transform.parent = GetComponentInParent<Character>().Hand.transform;
                activeItem.transform.localPosition = Vector3.zero;
            }
            else if (activeItem == null)
            {
                activeItem = GetItemFromPool(itemStored.itemProperties, true);
                activeItem.gameObject.SetActive(true);
                activeItem.isFromEnemy = true;

                //Setting the parent
                activeItem.transform.parent = GetComponentInParent<Character>().Hand.transform;
                activeItem.transform.localPosition = Vector3.zero;
                
                
                break;
            }
        }
        


    }

    void Add()
    {
        AddItem(itemStored);
        //Getting a game object from the pool
        //GameObject temp = GetItemFromPool(itemStored.itemProperties, true).gameObject;
        //
        //itemStored = temp.GetComponent<ItemBase>();
        //itemStored.GetComponent<ItemBase>().isFromEnemy = true;
        //
        //temp.transform.parent = GetComponentInParent<Character>().Hand.transform;
        //temp.transform.localPosition = Vector3.zero;
        //temp.gameObject.SetActive(false);
        //temp.GetComponent<ItemBase>().isFromEnemy = true;
        //AddItem(temp.GetComponent<ItemBase>());
    }


    public void ReloadInventory()
    {
        SetActiveItem();
    }
}
