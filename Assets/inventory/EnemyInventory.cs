using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : Inventory {

    public GameObject itemPrefab;

    public int maxItem;

    public float reloadTime;

    public float speedOfThrow;
    
    ItemBase itemStored;

   

    // Use this for initialization
   public void Initialize () {

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
        GameObject temp = Instantiate(itemPrefab, GetComponentInParent<Character>().Hand.transform.position, Quaternion.identity) as GameObject;

        itemStored = temp.GetComponent<ItemBase>();
        itemStored.GetComponent<ItemBase>().isFromEnemy = true;

        SetActiveItem();   
    }

    void SetActiveItem()
    {
        Debug.Log("Set active item is called");

        /*  for (int i = 0; i < slots.Count; i++)
          {

            if(slots[i].itemlist.Count > 0 && slots[i].itemStored == null)
            {
                slots[i].itemStored = slots[i].itemlist[0];
            }

            Debug.Log("item description" + slots[i].itemStored.gameObject.GetComponent<ItemsDescription>().GetItemType());
            Debug.Log("item description item stored" + itemStored.gameObject.GetComponent<ItemsDescription>().GetItemType());
              if (slots[i].itemStored != null && slots[i].itemStored.gameObject.GetComponent<ItemsDescription>() == itemStored.gameObject.GetComponent<ItemsDescription>())
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
          }*/

        for (int j = 0; j < slots[0].itemlist.Count; j++)
        {
            if(!slots[0].itemlist[j].gameObject.activeSelf)
            activeItem = slots[0].itemlist[j];
            activeItem.isFromEnemy = true;
            activeItem.gameObject.SetActive(true);
            break;

        }

    }

    void Add()
    {
        GameObject temp = Instantiate(itemPrefab, GetComponentInParent<Character>().Hand.transform.position, Quaternion.identity) as GameObject;

        itemStored = temp.GetComponent<ItemBase>();
        itemStored.GetComponent<ItemBase>().isFromEnemy = true;

        temp.transform.parent = GetComponentInParent<Character>().Hand.transform;
        temp.gameObject.SetActive(false);
        temp.GetComponent<ItemBase>().isFromEnemy = true;
        AddItem(temp.GetComponent<ItemBase>());
    }


    public void ReloadInventory()
    {
        SetActiveItem();
    }
}
