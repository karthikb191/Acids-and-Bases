using System.Collections;
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

    public int maxStorage = 5;

    public List<ItemBase> itemlist = new List<ItemBase>();

 
    public void AddItem(ItemBase l_itemBase)
    {
        Debug.Log("Add item called in slot" + this.itemStored + "____" + l_itemBase.name + this.itemCount);
        if (itemStored != null)
        {
            if (l_itemBase.itemProperties.name == itemStored.itemProperties.name && itemCount < maxStorage)
            {
                itemCount++;
                itemlist.Add(l_itemBase);
                //UpdateUI();
                Debug.Log(this.itemCount + "___" + itemStored.name);

            }
        }
        else
        {

            itemStored = l_itemBase;
            itemlist.Add(l_itemBase);
            itemStored.itemProperties.isAssignedToSlot = true;
            //imageSlotPrefab.SetActive(true);

            //displaySprite.enabled = true;
            //displaySprite.sprite = itemStored.itemProperties.imageSprite;
            itemCount++;
            //UpdateUI();
            Debug.Log("Add item called in slot" + this.itemStored + "____" + l_itemBase.name + this.itemCount);

        }
      
    }

    public void RemoveItem(ItemBase l_itemBase)
    {
        itemCount--;

        Debug.Log("Remove Item called");
        itemlist.Remove(l_itemBase);

        if (itemlist.Count < 1)
        {
            FlushOut();
        }
        else
        {
            
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        
       // countText.text = "" + itemCount;
        countText.text = "" + itemlist.Count;
       // Debug.Log("UI Text" + itemlist.Count);
    }

    public void FlushOut()
    {
        itemCount = 0;
        displaySprite.sprite = null;
        displaySprite.enabled = false;
        itemStored.itemProperties.isAssignedToSlot = false;
        itemStored = null;
        imageSlotPrefab.gameObject.SetActive(false);
        countText.text = "";
      
        itemlist.Clear();
    }


}

public class Inventory : MonoBehaviour {

    //[SerializeField]
    //Canvas inventoryUI;
    //[SerializeField]
    //RectTransform panel;    //Panel within which the slots are stored

    Character character;    //The character the inventory belongs to

    List<Slot> slots = new List<Slot>();

    public GameObject imageSlotPrefab;


    public ItemBase activeItem;

    //[SerializeField]
    //Sprite sampleTestSprite;

    public int maxSlotCount = 1;

    private void Start()
    {
        CreateSlot();
        ResetItems();
    }

    private void Update()
    {

        //if(Input.GetMouseButtonDown(0))
        //{
        //    if (RectTransformUtility.RectangleContainsScreenPoint(panel, Input.mousePosition))
        //    {
        //        for(int i = 0; i < slots.Count; i++)
        //        {
        //            if(RectTransformUtility.RectangleContainsScreenPoint(slots[i].panel, Input.mousePosition))
        //            {
        //                Debug.Log("HIt panel" + slots[i].displaySprite.name);
        //
        //                if(activeItem == null)
        //                {
        //                    activeItem = new ItemBase();
        //
        //                    slots[i].itemlist[slots[i].itemlist.Count - 1].gameObject.SetActive(true);
        //                    activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];
        //
        //                    Debug.Log(gameObject.GetComponentInChildren<Character>() + "In align pose");
        //                    activeItem.AlignPos(gameObject.transform.position, gameObject.GetComponentInChildren<Character>());
        //
        //                    Debug.Log("Assign from slot");
        //                    
        //                }
        //            
        //                else if(activeItem != slots[i].itemStored)
        //                {
        //                    ItemBase temp = activeItem;
        //
        //                    activeItem = slots[i].itemStored;
        //                  
        //                    UpdateSlotData(temp);
        //                }
        //            }
        //        }
        //    }
        //    ///////////////////////////////////////////////////////////////////Target Setting testing//////////////////////////////////////////////////
        //    //else
        //    //{
        //    //
        //    //    targerSetTest = Camera.main.ScreenToWorldPoint( Input.mousePosition);
        //    //    Debug.Log("Target set" + "_____" + targerSetTest);
        //    //
        //    //    targerSetTest.z = 0;
        //    //        
        //    //}
        //}
        //
        //if (activeItem != null && Input.GetKeyDown(KeyCode.D))
        //{
        //    DropItem();
        //
        //    activeItem = null;
        //}
        //
        ////if (activeItem != null && Input.GetKeyDown(KeyCode.T))
        ////{
        ////    if(activeItem.itemProperties.isThrowable)
        ////        ThrowItem();
        ////}
        //
        //if (activeItem != null && Input.GetKeyDown(KeyCode.U))
        //{
        //    UseItem();
        //}


    }

    Vector3 targerSetTest = Vector3.zero;

    public void UseItem()
    {
        //use the active item and check the slots

        //activeItem.Use();

        if(activeItem.itemProperties.isConsumable)
        {
            activeItem.gameObject.transform.parent = null;

            Debug.Log(gameObject.transform.GetComponentInParent<Character>());
            activeItem.Use(gameObject.transform.GetComponentInParent<Character>());
        }

        UpdateSlotData(activeItem);
        activeItem = null;
        activeSlotCount = ActiveSlotCount();
  
    }

    public void ThrowItem(Vector3 destination)
    {
        //Throw the item and check the slots 
        if (activeItem.itemProperties.isThrowable)
        {
            activeItem.gameObject.transform.parent = null;

            Debug.Log(activeItem.gameObject.transform.position);
            activeItem.Throw(destination, 100f);
     
            UpdateSlotData(activeItem);
            activeSlotCount = ActiveSlotCount();

            //Get the item from the current active slot
            //activeItem = slots[0].itemlist[slots[0].itemlist.Count - 1]; ;
            activeItem = null;
        }
     
    }

    public void UpdateSlotData(ItemBase l_ItemBase)
    {
       // Debug.Log("Update slot called");
        for (int i = 0; i < activeSlotCount; i++)
        {

            if (slots[i].itemStored != null && slots[i].itemStored.itemProperties == l_ItemBase.itemProperties)
            {
             //   Debug.Log("Remove item called");
                slots[i].RemoveItem(l_ItemBase);
                activeSlotCount = ActiveSlotCount();
                break;
            }


        }
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

    public void EnableUI()
    {
        //UI component of the inventory can be enabled here
    }

    public void DisableUI()
    {
        //UI component of inventory can be disabled here
    }

    private void CreateSlot()
    {
        //Create a new slot and assign the item.
        //If item is null, just create an empty slot to populate the inventory

        if (slots.Count == 0)
        {
            for (int i = 0; i < maxSlotCount; i++)
            {
                Slot tempSlot = new Slot();
              //  Debug.Log(tempSlot);
                slots.Add(tempSlot);
               // Debug.Log(slots.Count);
            }

            for (int i = 0; i < slots.Count; i++)
            {
            
                //slots[i].imageSlotPrefab = Instantiate(imageSlotPrefab);
                
                //slots[i].imageSlotPrefab.transform.parent = transform;
            
                //slots[i].panel = slots[i].imageSlotPrefab.gameObject.GetComponent<RectTransform>();
            
                //slots[i].countText = slots[i].imageSlotPrefab.gameObject.GetComponent<Text>();
            
                //slots[i].displaySprite = slots[i].imageSlotPrefab.transform.Find("Slot Image").gameObject.GetComponentInChildren<Image>();
            
                //slots[i].imageSlotPrefab.SetActive(false);
            
                //slots[i].displaySprite.sprite = sampleTestSprite;
            
                //slots[i].countText = slots[i].imageSlotPrefab.transform.Find("Count text").gameObject.GetComponent<Text>();
            
                //  Debug.Log(slots[i].countText.text + "_____" + i);
            
            }

        }
    }
  

    public void AddItem(ItemBase l_ItemBase)
    {
        
        if (l_ItemBase.itemProperties.isAnInventoryItem)
        {
            if (l_ItemBase.itemProperties.isAssignedToSlot)
            {
                for (int i = 0; i <= activeSlotCount; i++)
                {
                    if(slots[i].itemStored!=null  && slots[i].itemStored.itemProperties == l_ItemBase.itemProperties)
                    {
                        slots[i].AddItem(l_ItemBase);
                        break;
                    }
                }
            }

            else if(!l_ItemBase.itemProperties.isAssignedToSlot && activeSlotCount < maxSlotCount)
            {
                for (int i = activeSlotCount; i < slots.Count; i++)
                {
                    //if (!slots[i].imageSlotPrefab.activeSelf && slots[i].itemStored == null)
                    if (slots[i].itemStored == null)
                    {
                        slots[i].AddItem(l_ItemBase);
                      
                        activeSlotCount = ActiveSlotCount();
                        break;
                    }
                }

            }

            else
            {
                Debug.Log("Max Slots reached");
            }
        }
        else
        {
            Debug.Log(l_ItemBase.name + "Is not an Inventory item");
        }

        //Create an instance of the prefab
        if(activeItem == null)
        {
            Debug.Log("Active item is null....instantiation");
            ItemBase g = Instantiate(slots[0].itemlist[slots[0].itemlist.Count - 1]);
            g.transform.parent = transform;
            g.transform.localPosition = new Vector3(0, 1.0f, 0);
            activeItem = g;
        }

        slots[0].itemlist[slots[0].itemlist.Count - 1].gameObject.SetActive(true);
    }

    int activeSlotCount;

    private int ActiveSlotCount()
    {
        int c = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            //if(slots[i].imageSlotPrefab.activeSelf)
            //{
            //    c++;
            //    continue;
            //}
        }
        //Debug.Log("Active slot COunt:"  + c);
        return c;
        
    }



    private void ReArrangeSlots()
    {
        //All the slots will be rearranged depending on 
        //Since the slots contain the Image UI element, if it's value is changed, it automically reflects within the 
        //UI element


        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].itemStored == null || slots[i].itemlist.Count == 0)
            {
                slots[i].imageSlotPrefab.SetActive(false);
            }

            else
            {
                slots[i].imageSlotPrefab.SetActive(true);
            }

            if(slots[i].imageSlotPrefab.activeSelf)
            {
                slots[i].panel = slots[i].imageSlotPrefab.gameObject.GetComponent<RectTransform>();
            }
        }
    }


#if(UNITY_EDITOR)

    public List<ItemProperties> allItems = new List<ItemProperties>();

    private void ResetItems()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            allItems[i].isAssignedToSlot = false;
        }
    }


#endif

}
