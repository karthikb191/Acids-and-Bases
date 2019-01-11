using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : Inventory {

    public GameObject itemPrefab;
    public int maxItem;

    public float reloadTime;

    public float speedOfThrow;
    
    ItemBase ItemStored;

   

    // Use this for initialization
    void Start () {
        
        CreateSlot();
        for(int i = 0; i<slots.Count;i++)
        {
            slots[i].maxStorage = 10;
            slots[i].imageSlotPrefab.transform.SetParent(transform);
        }

        //Instantiating items

        for (int i = 0; i < maxItem; i++)
        {
            Add();
        }
        SetActiveItem();   
    }



   

    void SetActiveItem()
    {
        for (int i = 0; i<slots.Count; i++)
        {
          
            if (slots[i].itemStored != null && slots[i].itemStored.itemProperties == ItemStored.itemProperties)
            {
               
                slots[i].itemlist[slots[i].itemlist.Count - 1].gameObject.SetActive(true);
                activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];
                activeItem.isFromEnemy = true;
                activeItem.gameObject.SetActive(true);

               // Debug.Log("Item in slot"+ slots[i].itemStored);
                Add();
                break;
            }
        }
    }

    void Add()
    {
        GameObject temp = Instantiate(itemPrefab, GetComponentInParent<Character>().Hand.transform.position, Quaternion.identity) as GameObject;
        ItemStored = temp.GetComponent<ItemBase>();
        temp.transform.parent = GetComponentInParent<Character>().Hand.transform;
        temp.transform.localScale = Vector3.one / 2;
        temp.gameObject.SetActive(false);
        ItemStored.GetComponent<ItemBase>().isFromEnemy = true;
        temp.GetComponent<ItemBase>().isFromEnemy = true;

        //Debug.Log("Calling add function" + temp.GetComponent<ItemBase>().itemProperties.name);
        AddItem(temp.GetComponent<ItemBase>());
    }


    public void ReloadInventory()
    {
        SetActiveItem();
    }
}
