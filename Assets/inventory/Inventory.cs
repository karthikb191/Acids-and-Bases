﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot
{
    public Image displaySprite;
    public ItemBase itemStored; 
    public int itemCount;

    public RectTransform panel;

    public Text countText;

    public GameObject imageSlotPrefab;

    public bool isSelected;

    public int maxStorage = 10;

    public List<ItemBase> itemlist = new List<ItemBase>();


    public virtual void AddItem(ItemBase l_itemBase)
    {

        if (itemStored != null)
        {
            if (l_itemBase.itemProperties.name == itemStored.itemProperties.name && itemCount < maxStorage)
            {
                itemCount++;
                itemlist.Add(l_itemBase);
                
               // Debug.Log(this.itemCount + "___" + itemStored.name);

            }
        }
        else
        {
            itemlist.Add(l_itemBase);
            itemCount++;

            //Added this..... Needs to be arranged properly
            if (l_itemBase.GetComponent<PH>())
            {
                Debug.Log("PH item");
                return;
            }

            Debug.Log("Out of PH Block");

            itemStored = l_itemBase;
            Debug.Log("Item in slot" + itemStored.itemProperties.name);
            // imageSlotPrefab.SetActive(true);

            displaySprite.enabled = true;
            displaySprite.sprite = itemStored.itemProperties.imageSprite;
           
            
        }
        UpdateUI();
    }

    public virtual void RemoveItem(ItemBase l_itemBase)
    {
        itemCount--;

     //   Debug.Log("Remove Item called");
        itemlist.Remove(l_itemBase);
        Debug.Log("item list count: " + itemlist.Count);

        if (itemlist.Count < 1)
        {
            FlushOut();
        }
        else
        {
            
            UpdateUI();
        }
    }

    public virtual void UpdateUI()
    {
        
       // countText.text = "" + itemCount;
        countText.text = "" + itemlist.Count;
      // Debug.Log("UI Text" + itemlist.Count);
    }

    public void FlushOut()
    {
        Debug.Log("Flush out called");
        itemCount = 0;
        displaySprite.sprite = null;
        displaySprite.enabled = false;
        itemStored = null;
        imageSlotPrefab.gameObject.SetActive(false);
        countText.text = "";
      
        itemlist.Clear();
    }


}

public class Inventory : MonoBehaviour {

  /*  [SerializeField]
    Canvas inventoryUI;
    [SerializeField]
    RectTransform panel;   */ //Panel within which the slots are stored

   //  Character character;    //The character the inventory belongs to

    public List<Slot> slots = new List<Slot>();

    public GameObject imageSlotPrefab;
    

    public ItemBase activeItem;

    [SerializeField]
    Sprite sampleTestSprite;

    public int maxSlotCount = 6;

    private void Start()
    {
        //  CreateSlot();

     //   character = GetComponentInParent<Character>();
        slots = new List<Slot>(maxSlotCount);
    }


    public virtual void SetActiveSlotCount()
    {
        activeSlotCount = ActiveSlotCount();
    }

    public virtual void UseItem()
    {
        //use the active item and check the slots

        //activeItem.Use();

        if(activeItem.itemProperties.isConsumable)
        {
           // activeItem.gameObject.transform.parent = null;

           // Debug.Log(gameObject.transform.GetComponentInParent<Character>());
           activeItem.Use(gameObject.transform.GetComponentInParent<Character>());
        }
        UpdateSlotData(activeItem);
        activeSlotCount = ActiveSlotCount();
        if (ActiveItemCheck(activeItem))
        {
            Debug.Log("Active Item is present");


        }
        else
        {
            activeItem = null;
        }
        
    }

    public virtual void ThrowItem(Vector3 Target, float speed)
    {
        //Throw the item and check the slots 
        if (activeItem.itemProperties.isThrowable)
        {
         
            activeItem.Throw(Target,speed);
            UpdateSlotData(activeItem);
            activeSlotCount = ActiveSlotCount();
            Debug.Log("Active slot Count: " + activeSlotCount);
            if (ActiveItemCheck(activeItem))
            {
                Debug.Log("Active Item is present");
            }
            else
            {
                activeItem = null;
            }
        }
    }

    public void UpdateSlotData(ItemBase l_ItemBase)
    {
        Debug.Log("Update slot called");
        for (int i = 0; i <= activeSlotCount; i++)
        {
            if (slots[i].itemStored != null && slots[i].itemStored.itemProperties == l_ItemBase.itemProperties && slots[i].itemlist.Count > 0)
            {
                Debug.Log("Remove item called");
                slots[i].RemoveItem(l_ItemBase);
                activeSlotCount = ActiveSlotCount();
                break;
            }
        }
    }


    public bool ActiveItemCheck(ItemBase l_activeItem)
    {
        for (int i = 0; i <= activeSlotCount; i++)
        {
            
            if(slots[i].itemStored != null && slots[i].itemStored.itemProperties == l_activeItem.itemProperties )
            {
              /*  Debug.Log(slots[i].itemStored);
                Debug.Log(slots[i].itemlist.Count);*/
                activeItem = slots[i].itemlist[slots[i].itemlist.Count-1];
                activeItem.gameObject.SetActive(true);
                activeItem.AlignPos(GetComponentInParent<Character>().Hand.transform.position, GetComponentInParent<Character>());
                activeItem.transform.parent = GetComponentInParent<Character>().Hand.transform;
                return true;
            }
        }
        return false;
    }

    
    public ItemBase GetActiveItem()
    {
        return activeItem;
    }

    public Slot SetActiveSlot(Vector3 clickPosition)
    {
        //Click on a canvas element can be known using RectTransformUtility function.
        //This function also sets the active item player is holding
        return null;
    }

   

    public void DropItem()
    {
        //Drop the active item and check the item count in the slots again
        //if the item count is less than 0, rearrange the slots
        UpdateSlotData(activeItem);
        activeItem.transform.parent = null;
        activeItem.DropItem(this.transform.position, gameObject.GetComponentInChildren<Character>());        
        activeItem = null;

    }

   // public void DropBatch(System.Type type)
    public void DropBatch(ItemBase l_itemBase)
    {
        //Drop the items of the specified type, if found     
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].itemStored.itemProperties == l_itemBase.itemProperties)
            {
                slots[i].FlushOut();
                activeSlotCount = ActiveSlotCount();
            }
        }
    }

    public virtual void EnableUI()
    {
        //UI component of the inventory can be enabled here
    }

    public virtual void DisableUI()
    {
        //UI component of inventory can be disabled here
    }

    public void CreateSlot()
    {
        //Create a new slot and assign the item.
        //If item is null, just create an empty slot to populate the inventory

        if (slots.Count == 0)
        {
            Debug.Log("MAx slot count"+maxSlotCount);
          

            for (int i = 0; i < maxSlotCount; i++)
            {
                Slot tempSlot = new Slot();
               
                slots.Add(tempSlot);
                slots[i].imageSlotPrefab = Instantiate(imageSlotPrefab);
                slots[i].panel = slots[i].imageSlotPrefab.gameObject.GetComponent<RectTransform>();
                slots[i].countText = slots[i].imageSlotPrefab.gameObject.GetComponent<Text>();           
                slots[i].displaySprite = slots[i].imageSlotPrefab.transform.Find("Slot Image").gameObject.GetComponentInChildren<Image>();
                slots[i].imageSlotPrefab.SetActive(false);
                slots[i].displaySprite.sprite = sampleTestSprite;
                slots[i].countText = slots[i].imageSlotPrefab.transform.Find("Count text").gameObject.GetComponent<Text>();
                //Debug.Log(slots[i].countText.text + "_____" + i);
            }

        }
    }
  

    public virtual void AddItem(ItemBase l_ItemBase)
    {
        bool assigned = false;

        if (l_ItemBase.itemProperties.isAnInventoryItem)
        {
          
                for (int i = 0; i <= activeSlotCount; i++)
                {
                    if(slots[i].itemStored!=null  && slots[i].itemStored.itemProperties == l_ItemBase.itemProperties)
                    {

                        slots[i].AddItem(l_ItemBase);
                        
                    assigned = true;
                        break;
                    }

                    
                }
           // Debug.Log("Active Slot Count :  " + activeSlotCount);

           if( activeSlotCount < maxSlotCount && !assigned)
            {
                for (int i = activeSlotCount; i < slots.Count; i++)
                {
                    if (!slots[i].imageSlotPrefab.activeSelf && slots[i].itemStored == null)
                    {
                        
                        Debug.Log("Adding item to slot");
                        slots[i].AddItem(l_ItemBase);
                      // slots[i].imageSlotPrefab.SetActive(true);
                        slots[i].displaySprite.sprite = l_ItemBase.itemProperties.imageSprite;
                        slots[i].itemStored = l_ItemBase;
                        activeSlotCount = ActiveSlotCount();
                        break;
                    }
                }

            }
            else
            {
               // Debug.Log("Max Slots reached");
            }       
        }
        else
        {
            Debug.Log(l_ItemBase.name + "Is not an Inventory item");
        }
    }

    int activeSlotCount;

    public virtual int ActiveSlotCount()
    {
        int c = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemlist.Count > 0)
            {
                c++;
                continue;
            }
        }
        //Debug.Log("Active slot COunt:"  + c);
        return c;
        
    }

    public void DisplaySlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].itemStored!= null)
            {
                slots[i].imageSlotPrefab.SetActive(true);
            }
        }
    }

    public void HideSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {           
             slots[i].imageSlotPrefab.SetActive(false);
            
        }
    }

}
