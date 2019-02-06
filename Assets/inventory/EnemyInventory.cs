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

        character = GetComponentInParent<Character>();
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

                for (int j = 0; j < slots[i].itemlist.Count; j++)
                {
                    if (!slots[i].itemlist[j].gameObject.activeSelf)
                    {
                        slots[i].itemlist[j].gameObject.SetActive(true);
                        activeItem = slots[i].itemlist[j];
                        activeItem.isFromEnemy = true;
                        activeItem.gameObject.SetActive(true);
                        Debug.Log(activeItem.name + "Active item set in enemy inventory");
                        break;
                    }
                }
            }
        }
    }

    void Add()
    {
        GameObject temp = Instantiate(itemPrefab, GetComponentInParent<Character>().Hand.transform.position, Quaternion.identity) as GameObject;
        ItemStored = temp.GetComponent<ItemBase>();
        temp.transform.parent = GetComponentInParent<Character>().Hand.transform;
        temp.gameObject.SetActive(false);
        ItemStored.GetComponent<ItemBase>().isFromEnemy = true;
        temp.GetComponent<ItemBase>().isFromEnemy = true;
        AddItem(temp.GetComponent<ItemBase>());
    }


    public void ReloadInventory()
    {
        SetActiveItem();
    }
}
