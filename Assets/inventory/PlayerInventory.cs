using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory
{
    Player player;


////////////////////////////////////////Inventory animation//////////////////////////////////////////
     Animator InventoryAnimator;

     Animator InventoryButtonAnimator;

    public void ShowInventory()
    {
        if (InventoryButtonAnimator.GetBool("SlideLeft"))
        {
            InventoryButtonAnimator.SetBool("SlideLeft", false);

           
        }
        else
        {
            InventoryButtonAnimator.SetBool("SlideLeft", true);
           
            
        }
        ShowInventoryPanel();
    }

    private void ShowInventoryPanel()
    {
        if (InventoryAnimator.GetBool("ShowInventory"))
        {
            InventoryAnimator.SetBool("ShowInventory", false);
          
        }
        else
        {
            InventoryAnimator.SetBool("ShowInventory", true);
            
        }
    }


////////////////////////////////////////Inventory animation//////////////////////////////////////////


   // [SerializeField]
    Canvas inventoryUI;
   // [SerializeField]
    RectTransform panel;    //Panel within which the slots are stored   
  


    void CreateSlots()
    {
        CreateSlot();

        for (int i = 0; i < slots.Count; i++)
        {
         Debug.Log(slots[i].imageSlotPrefab.transform.name);
           // slots[i].imageSlotPrefab.transform.parent = panel.transform;

            slots[i].imageSlotPrefab.transform.SetParent(panel.transform);

        }

    }

    void AddItems(ItemBase l_ItemBase)
    {
        AddItem(l_ItemBase);
        
    }

    // Use this for initialization
    void Start()
    {
        player = GetComponentInParent<Player>();
        inventoryUI = transform.GetChild(0).GetComponent<Canvas>();
        panel = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        InventoryAnimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        InventoryButtonAnimator = transform.GetChild(0).GetChild(1).GetComponent<Animator>();
        CreateSlots();
    }

    // Update is called once per frame
    void Update()
    {
        SelectFromSlots();
        if (InventoryAnimator.GetBool("ShowInventory"))
        {
            Invoke("DisplaySlots",1.5f);
        }
        else
        { HideSlots(); }
    }

    void SelectFromSlots()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (panel != null && RectTransformUtility.RectangleContainsScreenPoint(panel, Input.mousePosition))
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(slots[i].panel, Input.mousePosition))
                    {
                        //Check if the selected item is an indicator item
                        if (slots[i].itemStored.GetComponent<PH>())
                        {
                            //If it is, and player doesn't have any active indicator, add it to his indicator list
                            if(player.GetPlayerStatus().pHIndicator == null)
                            {
                                player.GetPlayerStatus().SetpHIndicator(slots[i].itemStored.GetComponent<PH>());
                                Debug.Log("New pH Indicator has been set.");
                            }
                        }


                        if (activeItem == null)
                        {
                            if (slots[i].itemlist.Count > 0)
                            {

                                activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];
                                
                                slots[i].itemlist[slots[i].itemlist.Count - 1].gameObject.SetActive(true);
                                UpdateSlotData(activeItem);
                                SetActiveSlotCount();
                            }
                            Debug.Log("Assign from slot");
                        }
                        else if (activeItem.itemProperties != slots[i].itemStored.itemProperties)
                        {
                            activeItem.gameObject.SetActive(false);
                            // ItemBase temp = activeItem;
                            if (slots[i].itemlist.Count > 0)
                            {
                                activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];
                            }
                            activeItem.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        if (activeItem != null && Input.GetKeyDown(KeyCode.Y))
        {
           DropItem();
           activeItem = null;
        }

        if (activeItem != null && Input.GetKeyDown(KeyCode.U))
        {
            UseItem();
        }
    }

    public void ThrowItem(Vector3 target)
    {
        if (activeItem.itemProperties.isThrowable)
        {
           
           // ThrowItem(target, this.gameObject.GetComponent<Character>());
           
        }
    }
}

