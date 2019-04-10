using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemClassificationValues
{
    System.Enum itemType;
    float pHValue;

    public bool Equals(ItemClassificationValues val)
    {
        if (itemType == val.itemType && pHValue == val.pHValue)
            return true;

        return false;
    }
}

public class Slot
{
    public Image displaySprite;
    public ItemBase itemStored;

    //The description of stored item. When a new item is added, this must be checked and the item must be added to the new slot
    //if required
    public ItemClassificationValues classificationValues = null;    

    public int itemCount;

    public RectTransform panel;

    public Text countText;

    public GameObject imageSlotPrefab;

    public bool isSelected;

    public int fromSlot;

    public int maxStorage = 10;

    public int slotNumber;

    public List<ItemBase> itemlist = new List<ItemBase>();

    public bool isActive = false;

    public virtual void AddItem(ItemBase l_itemBase)
    {
        if (itemStored != null)
        {   
            //If the item is already present in the list
            //Add the item to the respective slot if the item properties of the item are the same
            if (l_itemBase.itemProperties == itemStored.itemProperties && itemCount < maxStorage)
            {
                itemCount++;
                itemlist.Add(l_itemBase);               
                //Debug.Log(this.itemCount + "___" + itemStored.name);
            }
            else
            {
                Debug.Log("Item Not added");

                l_itemBase.gameObject.SetActive(true);
                l_itemBase.transform.parent = null;
                //return;
            }
        }
        else
        {
            itemlist.Add(l_itemBase);
            itemCount++;          
            isActive = true;
            itemStored = l_itemBase;
           //Debug.Log("Item in slot" + itemStored.itemProperties.name);
            imageSlotPrefab.SetActive(true);
            displaySprite.enabled = true;
            displaySprite.sprite = itemStored.itemProperties.imageSprite;
            //Added this..... Needs to be arranged properly          
        }
        UpdateUI();
    }

    public virtual void RemoveItem(ItemBase l_itemBase)
    {
        itemCount--;
        Debug.Log("item list count Before : " + itemlist.Count);
        //   Debug.Log("Remove Item called");
        itemlist.Remove(l_itemBase);
        Debug.Log("item list count after : " + itemlist.Count);

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
        countText.text = itemlist.Count.ToString();
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
        isActive = false;
        itemlist.Clear();
    }
}

public class Inventory : MonoBehaviour {
    public List<ItemBase> itemPool = new List<ItemBase>();

    public Character character;    //The character the inventory belongs to

    public List<Slot> slots = new List<Slot>();

    public GameObject imageSlotPrefab;

    public ItemBase activeItem;

    [SerializeField]
    Sprite sampleTestSprite;

    //Maximum number of slots in the inventory
    public int maxSlotCount = 30;

    //The item pool that's common to enemies and players. Players and enemies can get their items from this pool of objects
    //Change the scriptable object on the item to alter the item
    public static List<ItemBase> itemsPool = new List<ItemBase>();

    private void Start()
    {
        //  CreateSlot();
        Debug.Log(character.name + "<<<<<--Character name in Start");
       
        Debug.Log(character.name + "<<<<<--Character name in Start");
        slots = new List<Slot>(maxSlotCount);

        character = this.transform.GetComponentInParent<Character>();
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

        if(character.gameObject.GetComponent<Player>() != null)
        {
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

    public virtual void ThrowItem(Vector3 Target, float speed)
    {
        //Throw the item and check the slots

        if (activeItem.itemProperties.isThrowable && activeItem != null)
        {

            activeItem.Throw(Target,speed);
            UpdateSlotData(activeItem);
       
            activeSlotCount = ActiveSlotCount();
            // Debug.Log("Active slot Count: " + activeSlotCount);

            //   Debug.Log("Active item name:   " + activeItem.name);

            if (character.gameObject.GetComponent<Player>() != null)
            {
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
    }

    //TODO: Change the name of this function to something meaningful
    public void UpdateSlotData(ItemBase l_ItemBase)
    {
       // Debug.Log("Update slot called");
        for (int i = 0; i < activeSlotCount; i++)
        {
            if (slots[i].itemStored != null  && slots[i].itemStored.itemProperties == l_ItemBase.itemProperties)
            {
                Debug.Log("Remove item called");
                Debug.Log("Itemstored : " + slots[i].itemStored.transform.GetComponent<ItemsDescription>().GetItemType());

                slots[i].RemoveItem(l_ItemBase);

                if(slots[i].itemlist.Count > 0)
                {
                    if (slots[i].itemStored == null)
                    {
                        slots[i].itemStored = slots[i].itemlist[slots[i].itemlist.Count - 1];
                    }
                }

                activeSlotCount = ActiveSlotCount();

                
                Debug.Log("Remove item player call end");
                break;
            }
        }
    }


    public bool ActiveItemCheck(ItemBase l_activeItem)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            //  if(slots[i].itemlist.Count > 0  && slots[i].itemStored.itemProperties == l_activeItem.itemProperties)
            if (slots[i].itemStored != null && slots[i].itemlist.Count > 0 && slots[i].itemStored.itemProperties.name == l_activeItem.itemProperties.name  )
            {
                if (slots[i].itemStored.itemProperties.itemDescription.hasPH &&  l_activeItem.itemProperties.itemDescription.hasPH)
                {
                    if(CheckForPhValues(slots[i].itemStored,l_activeItem))
                    {
                        Debug.Log("Has ph value and it is checked");
                        if (slots[i].itemlist.Count > 0)
                        {
                            activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];
                            activeItem.gameObject.SetActive(true);
                            activeItem.AlignPos(GetComponentInParent<Character>().Hand.transform.position, GetComponentInParent<Character>());
                            activeItem.transform.parent = GetComponentInParent<Character>().Hand.transform;
                            if (slots[i].itemStored == null)
                            {
                                slots[i].itemStored = slots[i].itemlist[0];
                                Debug.Log(slots[i].itemStored.name + "::::: item not present assigned");
                            }
                            else
                            {
                                Debug.Log(slots[i].itemStored.name + "::::: item present");
                            }

                            return true;
                        }
                    }                
                }
                else
                {
                    Debug.Log("active item doesnt have ph");
                    // if (slots[i].itemlist.Count > 0 && slots[i].itemStored.GetComponent<ItemsDescription>().GetType() == l_activeItem.gameObject.GetComponent<ItemsDescription>().GetType())
                    {
                        slots[i].itemStored = slots[i].itemlist[0];
                        activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];
                        activeItem.gameObject.SetActive(true);
                        activeItem.AlignPos(GetComponentInParent<Character>().Hand.transform.position, GetComponentInParent<Character>());
                        activeItem.transform.parent = GetComponentInParent<Character>().Hand.transform;
                        activeItem.gameObject.SetActive(true);
                        return true;
                    }
                }

                Debug.Log(slots[i].itemStored);
                Debug.Log("Active item check call :  " + slots[i].itemlist.Count);                
            }
        }
        Debug.Log("Active item is null");
        return false;
    }
    
    public ItemBase GetActiveItem()
    {
        return activeItem;
    }
   

    public void DropItem()
    {
        //Drop the active item and check the item count in the slots again
        //if the item count is less than 0, rearrange the slots
        UpdateSlotData(activeItem);

       
        activeItem.DropItem(this.transform.position, gameObject.GetComponentInChildren<Character>());   
        activeItem.transform.parent = null;
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
                GameObject temp = Instantiate(imageSlotPrefab);
                slots[i].imageSlotPrefab = temp;
                slots[i].panel = slots[i].imageSlotPrefab.gameObject.GetComponent<RectTransform>();
                slots[i].countText = slots[i].imageSlotPrefab.gameObject.GetComponent<Text>();           
                slots[i].displaySprite = slots[i].imageSlotPrefab.transform.Find("Slot Image").gameObject.GetComponentInChildren<Image>();
                slots[i].imageSlotPrefab.SetActive(false);
                slots[i].displaySprite.sprite = sampleTestSprite;
                slots[i].countText = slots[i].imageSlotPrefab.transform.Find("Count text").gameObject.GetComponent<Text>();
                slots[i].slotNumber = i;
                slots[i].imageSlotPrefab.transform.localScale = Vector3.one;
                //Debug.Log(slots[i].countText.text + "_____" + i);
            }
        }
    }
  
    public bool CheckForPhValues(ItemBase item1, ItemBase item2)
    {
        if(item1.gameObject.GetComponent<ItemsDescription>().hasPH && item2.gameObject.GetComponent<ItemsDescription>().hasPH)
        {
            if(item1.gameObject.GetComponent<ItemsDescription>().pHValue == item2.gameObject.GetComponent<ItemsDescription>().pHValue)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void AddItem(ItemBase l_ItemBase)
    {
        bool assigned = false;

        if (l_ItemBase.itemProperties.isAnInventoryItem)
        {
            for (int i = 0; i < activeSlotCount; i++)
            {
                if(slots[i].itemStored!=null)
                {
                    if(slots[i].itemStored.itemProperties == l_ItemBase.itemProperties)
                    {
                        if (slots[i].itemStored.itemProperties.itemDescription.itemType == l_ItemBase.itemProperties.itemDescription.itemType &&
                        slots[i].itemStored.itemProperties.itemDescription.hasPH == l_ItemBase.itemProperties.itemDescription.hasPH
                        && slots[i].itemStored.itemProperties.itemDescription.pHValue == l_ItemBase.itemProperties.itemDescription.pHValue)
                        {
                            Debug.Log("Added to already present slot");
                            slots[i].AddItem(l_ItemBase);
                            assigned = true;
                            break;

                        }
                    }                     
                    /*   else
                    {
                        Debug.Log("Added to already present slot");
                        slots[i].AddItem(l_ItemBase);
                        assigned = true;
                        break;
                    }*/

                }                  
                    
            }
            Debug.Log("Active Slot Count :  " + activeSlotCount);

            if(activeSlotCount < maxSlotCount && !assigned)
            {
                for (int i = activeSlotCount; i < slots.Count; i++)
                {
                    if (!slots[i].imageSlotPrefab.activeSelf && slots[i].itemStored == null)
                    {
                        
                        Debug.Log("Adding item to slot For first time");

                        slots[i].AddItem(l_ItemBase);
                        slots[i].imageSlotPrefab.SetActive(true);
                        slots[i].displaySprite.sprite = l_ItemBase.GetComponent<SpriteRenderer>().sprite;
                        // slots[i].itemStored.GetComponent<SpriteRenderer>().enabled = false;
                        slots[i].isActive = true;
                        slots[i].itemStored = l_ItemBase;

                        slots[i].imageSlotPrefab.gameObject.transform.localScale = Vector3.one;
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

        SetActiveSlotCount();
    }

    public  int activeSlotCount;

    public virtual int ActiveSlotCount()
    {
        int c = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isActive)
            {
                c++;
                continue;
            }
        }
        //Debug.Log("Active slot COunt:"  + c);
        return c;
        
    }

    //public void DisplaySlots()
    //{
    //    for (int i = 0; i < slots.Count; i++)
    //    {
    //        if(slots[i].itemStored!= null)
    //        {
    //            slots[i].imageSlotPrefab.SetActive(true);
    //        }
    //    }
    //}
    //
    //
    //public void HideSlots()
    //{
    //    for (int i = 0; i < slots.Count; i++)
    //    {           
    //        slots[i].imageSlotPrefab.SetActive(false);
    //        
    //    }
    //}

   
   
   ///////////////---------------------------------throw anime timer---------------------------///////////////////////////
    

    public float throwAnimeTime;
    public void ThrowAnimeWaitTime(Vector3 target,float speed)
    {

        StartCoroutine(ThrowAnimeWaitTime(throwAnimeTime));
        ThrowItem(target, speed);

    }

    IEnumerator ThrowAnimeWaitTime(float animeTime)
    {
        yield return new WaitForSeconds(animeTime);
    }


    ///////////////////////////////-------------------------------------------------------/////////////////////////////////////

    
    public static void AddItemToPool(ItemBase item)
    {
        //Automatically deactivate the item when placing the item in the pool
        item.gameObject.SetActive(false);
        itemsPool.Add(item);
    }

    public static ItemBase GetItemFromPool()
    {
        if (itemsPool.Count == 0)
            return null;
        else
        {
            //Automatically activate the item when retrieving from the pool
            itemsPool[0].gameObject.SetActive(true);
            return itemsPool[0];
        }
    }
}
