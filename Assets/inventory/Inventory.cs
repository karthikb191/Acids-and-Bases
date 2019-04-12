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

    //Item stored is just a reference to the type of item stored. This item must not be instantiated in any way
    //To instantiate an item to the character's hand, use the items from the object pool
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

    //public List<ItemBase> itemlist = new List<ItemBase>();

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
                Debug.Log("Item has been added to the slot");
            }
            else
            {
                Debug.Log("Item is of different type. So, it's not added");

                l_itemBase.gameObject.SetActive(true);
                l_itemBase.transform.parent = null;
                //return;
            }
        }
        else
        {
            Debug.Log("Item reference is null. Adding the reference");
            itemCount++;          
            isActive = true;

            //Placing a new item in the slot. The item stored will be a reference to an item that is somewhere in the scene.
            //NOTE: It is just a reference. It should not be instantiated. Get the reference from the itemManager
            //itemStored = l_itemBase;
            itemStored = ItemManager.instance.GetItemReference(l_itemBase.itemProperties.itemDescription.GetItemType());
            
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
        Debug.Log("item list count Before : " + itemCount);
        itemCount--;
        //   Debug.Log("Remove Item called");
        //itemlist.Remove(l_itemBase);
        Debug.Log("item list count after : " + itemCount);

        if (itemCount < 1)
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
        countText.text = "" + itemCount;
        //countText.text = itemlist.Count.ToString();
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
        //itemlist.Clear();
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

    private void Awake()
    {
        slots = new List<Slot>(maxSlotCount);
    }

    private void Start()
    {
        //  CreateSlot();
        character = this.transform.GetComponentInParent<Character>();

        Debug.Log(character.name + "<<<<<--Character name in Start");

        Debug.Log(character.name + "<<<<<--Character name in Start");
    }

    public virtual void SetActiveSlotCount()
    {
        activeSlotCount = ActiveSlotCount();
    }

    public virtual void UseItem()
    {
        //use the active item and check the slots

        //activeItem.Use();

        if (activeItem.itemProperties.isConsumable)
        {
            // activeItem.gameObject.transform.parent = null;

            // Debug.Log(gameObject.transform.GetComponentInParent<Character>());
            activeItem.Use(gameObject.transform.GetComponentInParent<Character>());
        }

        RemoveItem(activeItem);

        activeSlotCount = ActiveSlotCount();

        if (character.gameObject.GetComponent<Player>() != null)
        {
            if (UpdateActiveItemOnCharacter(activeItem))
            {
                Debug.Log("Active Item is present");
            }
            else
            {
                activeItem = null;
            }
        }
    }

    //This function depends on the activeItem. So, if this function is called, and there are enough items in the
    //slot, active item must have been set before
    public virtual void ThrowItem(Vector3 Target, float speed)
    {
        if(activeItem == null)
        {
            Debug.Log("Active item is null. Select the item first and then throw");
            return;
        }

        //Throw the item and check the slots
        if (activeItem.itemProperties.isThrowable)
        {
            //Setting the active item's parent to null so that it doesn't depend on the character anymore
            activeItem.Throw(Target, speed);

            //Parent is set to null in the throw function of the ItemBase
            //activeItem.transform.parent = null;

            RemoveItem(activeItem);

            activeSlotCount = ActiveSlotCount();


            //After throwing the item, we must update the active item on the character's hand
            if (character != null)
            {
                if (UpdateActiveItemOnCharacter(activeItem))
                {
                    Debug.Log("Updated the active item");
                }
                else
                {
                    Debug.Log("Active item is null. Select a different slot");
                    activeItem = null;
                }
            }
        }
    }

    //Now, active item still refers to the last item. That must change
    public void RemoveItem(ItemBase l_ItemBase)
    {
        // Debug.Log("Update slot called");
        for (int i = 0; i < activeSlotCount; i++)
        {
            if (slots[i].itemStored != null && slots[i].itemStored.itemProperties == l_ItemBase.itemProperties)
            {
                Debug.Log("Remove item called");
                Debug.Log("Itemstored : " + slots[i].itemStored.itemProperties.itemDescription.GetItemType());

                //Decrement the item count in the slot data
                slots[i].RemoveItem(l_ItemBase);

                if (slots[i].itemCount > 0)
                {
                    if (slots[i].itemStored == null)
                    {
                        //slots[i].itemStored = slots[i].itemlist[slots[i].itemlist.Count - 1];
                        //If the item stored has a null reference for some reason, Get the reference back by checking the dictionary
                        slots[i].itemStored = ItemManager.instance.GetItemReference(l_ItemBase.itemProperties.itemDescription.GetItemType());
                    }
                }

                activeSlotCount = ActiveSlotCount();

                Debug.Log("Remove item player call end");
                break;
            }
        }
    }

    //TODO: Convert this into a virtual function. Player inventory should be able to override it
    public bool UpdateActiveItemOnCharacter(ItemBase l_activeItem)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemStored != null && slots[i].itemCount > 0 && slots[i].itemStored.itemProperties.name == l_activeItem.itemProperties.name)
            {
                if (slots[i].itemStored.itemProperties.itemDescription.hasPH && l_activeItem.itemProperties.itemDescription.hasPH)
                {
                    if (CheckForPhValues(slots[i].itemStored, l_activeItem))
                    {
                        Debug.Log("Has ph value and it is checked");
                        if (slots[i].itemCount > 0)
                        {
                            //Get the active item from the object pool. Object pool functions ensures that you receive an object from the pool
                            activeItem = GetItemFromPool(slots[i].itemStored.itemProperties, true);
                            //set the active item's scriptable object using the itemStored in the slot
                            //activeItem.itemProperties = slots[i].itemStored.itemProperties;

                            //activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];
                            activeItem.gameObject.SetActive(true);
                            activeItem.transform.position = GetComponentInParent<Character>().Hand.transform.position;
                            //activeItem.AlignPos(GetComponentInParent<Character>().Hand.transform.position, GetComponentInParent<Character>());
                            activeItem.transform.parent = GetComponentInParent<Character>().Hand.transform;

                            Debug.Log("Item on character's hand is updated");

                            return true;
                        }
                    }
                }
                else
                {
                    Debug.Log("active item doesnt have ph");
                    if (slots[i].itemStored.itemProperties.itemDescription.GetType() == l_activeItem.itemProperties.itemDescription.GetType())
                    {
                        //slots[i].itemStored = slots[i].itemlist[0];
                        //activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];

                        //Get the active item from the object pool
                        activeItem = GetItemFromPool(slots[i].itemStored.itemProperties, true);
                        //activeItem.itemProperties = slots[i].itemStored.itemProperties;

                        activeItem.gameObject.SetActive(true);
                        activeItem.transform.position = GetComponentInParent<Character>().Hand.transform.position;
                        //activeItem.AlignPos(GetComponentInParent<Character>().Hand.transform.position, GetComponentInParent<Character>());
                        activeItem.transform.parent = GetComponentInParent<Character>().Hand.transform;
                        activeItem.gameObject.SetActive(true);
                        return true;
                    }
                }

                Debug.Log(slots[i].itemStored);
                Debug.Log("Active item check call :  " + slots[i].itemCount);
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
        RemoveItem(activeItem);


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
            if (slots[i].itemStored.itemProperties == l_itemBase.itemProperties)
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
            Debug.Log("MAx slot count" + maxSlotCount);

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
            ActiveSlotCount();
        }
    }

    public bool CheckForPhValues(ItemBase item1, ItemBase item2)
    {
        if(item1.itemProperties.itemDescription.hasPH && item2.itemProperties.itemDescription.hasPH)
        {
            if(item1.itemProperties.itemDescription.pHValue == item2.itemProperties.itemDescription.pHValue)
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
                Debug.Log("active slot count: " + activeSlotCount);
                Debug.Log("slot is:.................................................." + slots.Count);
                if(slots[i].itemStored != null)
                {
                    Debug.Log("adding?");
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

            //The purpose of this block is to assign the item for the first time
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


    ///////////////---------------------------------throw anime timer---------------------------///////////////////////////

    //Time - lag before the throw actually starts. 
    //TODO: This throw timer must be added to the item's actual time throw timer
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


    #region Item Pool Retrieve and store functions
    public static void AddItemToPool(ItemBase item)
    {
        //Automatically deactivate the item when placing the item in the pool
        Debug.Log("Adding item to pool");
        item.gameObject.SetActive(false);
        itemsPool.Add(item);
    }

    public static ItemBase GetItemFromPool(ItemProperties properties = null, bool createObjectIfPoolIsEmpty = false)
    {
        Debug.Log("Getting item from item pool");
        if (itemsPool.Count == 0)
        {
            if (createObjectIfPoolIsEmpty)
            {
                ItemBase item = ItemManager.GenerateItemTemplate();
                item.gameObject.SetActive(true);
                item.GetComponent<SpriteRenderer>().sprite = properties.imageSprite;
                if (properties != null)
                    item.itemProperties = properties;
                
                return item;
            }
            return null;
        }
        else
        {
            ItemBase item = itemsPool[0];
            //Item must be removed from the pool
            itemsPool.RemoveAt(0);

            //Automatically activate the item when retrieving from the pool
            item.gameObject.SetActive(true);
            //itemsPool[0].GetComponent<SpriteRenderer>().sprite = properties.imageSprite;
            item.GetComponent<SpriteRenderer>().sprite = properties.imageSprite;

            if (properties != null)
                item.itemProperties = properties;

            return item;
        }
    }
    
    #endregion
}
