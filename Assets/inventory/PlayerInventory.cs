using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInventory : Inventory
{
    Player player;


////////////////////////////////////////Inventory animation//////////////////////////////////////////
     Animator InventoryAnimator;

     Animator InventoryButtonAnimator;

    Animator extendedInventoryShowButtonanim;
    Animator extendedInventoryShowPanelanim;

    bool inventoryShown = false;

    Button extendedinventoryButton;

    bool extendedPanelShow = false;

    bool displayPanelShow = false;

    public void DisplayShortInventory()
    {
       
        if (InventoryButtonAnimator.GetBool("SlideLeft")
        && InventoryAnimator.GetBool("ShowInventory") 
        && extendedInventoryShowPanelanim.GetBool("ExtendedInventoryShow"))
        {
            CloseInventoryDisplay();
            return;

        }

        if (inventoryShown && InventoryAnimator.GetBool("ShowInventory") && InventoryButtonAnimator.GetBool("SlideLeft"))
        {
            displayPanelShow = false;
            ShowDisplayPanelItems();
            HideInventoryPanel();
        }
        else if(!inventoryShown && !extendedPanelShow)
        {
            ShowInventory();
        }

       
    }

    void ShowInventory()
    {
        InventoryButtonAnimator.SetBool("SlideLeft", true);
        RuntimeAnimatorController tempController = InventoryButtonAnimator.runtimeAnimatorController;
        float timer = tempController.animationClips.Length;

        Invoke("ShowInventoryPanel", timer);
    }


    void CloseInventoryDisplay()
    {
        InventoryButtonAnimator.SetBool("SlideLeft", false);
        InventoryAnimator.SetBool("ShowInventory", false);
        extendedPanelShow = false;
        displayPanelShow = false;
        inventoryShown = false;
        inventoryUI.transform.GetChild(3).GetComponent<Image>().enabled = false;

     //   extendedinventoryButton.GetComponent<Image>().enabled = false;
        extendedinventoryButton.GetComponentInParent<Image>().enabled = false;
        extendedInventoryShowButtonanim.SetBool("extendedPanelShow", false);
        extendedInventoryShowPanelanim.SetBool("ExtendedInventoryShow", false);
        ShowDisplayPanelItems();
        ShowExtendedPanelItems();
    }

    void ShowInventoryPanel()
    {
        InventoryAnimator.SetBool("ShowInventory", true);
        inventoryShown = true;
        displayPanelShow = true;

        inventoryUI.transform.GetChild(3).GetComponent<Image>().enabled = true;

        extendedinventoryButton.GetComponent<Image>().enabled = true;

        Debug.Log(extendedinventoryButton.GetComponentInParent<Image>().name + "<><><><><><><>Inventory show");
        extendedInventoryShowButtonanim.SetBool("extendedPanelShow", true);
        ShowDisplayPanelItems();
    }

    void HideInventory()
    {

        Debug.Log("HideInventory");
        InventoryButtonAnimator.SetBool("SlideLeft", false);
        inventoryShown = false;

        inventoryUI.transform.GetChild(3).GetComponent<Image>().enabled = false;
        //extendedinventoryButton.GetComponent<Image>().enabled = false;
        extendedinventoryButton.GetComponentInParent<Image>().enabled = false;
        extendedInventoryShowButtonanim.SetBool("extendedPanelShow", false);
    }

    void HideInventoryPanel()
    {
        InventoryAnimator.SetBool("ShowInventory", false);
        RuntimeAnimatorController tempController = InventoryAnimator.runtimeAnimatorController;
        float timer = tempController.animationClips.Length;
        
        Invoke("HideInventory", timer);
    }


    public void DisplayExtendedPanelShow()
    {
       

        if (extendedInventoryShowPanelanim.GetBool("ExtendedInventoryShow"))
        {
            extendedPanelShow = false;
            extendedInventoryShowPanelanim.SetBool("ExtendedInventoryShow", false);
            ShowExtendedPanelItems();

        }

        else
        {
            extendedPanelShow = true;
            extendedInventoryShowPanelanim.SetBool("ExtendedInventoryShow", true);
            RuntimeAnimatorController tempController = extendedInventoryShowPanelanim.runtimeAnimatorController;
            float timer = tempController.animationClips.Length;
   
            Invoke("ShowExtendedPanelItems", timer);
    

        }
    }

  

    ////////////////////////////////////////Inventory animation//////////////////////////////////////////


    // [SerializeField]
    Canvas inventoryUI;
   public  RectTransform extendedPanel;    //Panel within which the slots are stored   
    public RectTransform displayPanel;  //mini inventory Display panel
    List< Slot>  displaySlotList ;

    public int displaySlotCount;

    int index1;
    int index2;
    string index1From;
    string index2From;

    Slot selectedSlot1;
    Slot selectedSlot2;

    void CreateSlots()
    {
        CreateSlot();
        displaySlotList = new List<Slot>();
        

        CreateDisplaySlots();

        
        for (int i = 0; i < slots.Count; i++)
        {
            //Debug.Log(slots[i].imageSlotPrefab.transform.name);
            slots[i].imageSlotPrefab.transform.SetParent(extendedPanel.transform);
        }

     //   ShowDisplayPanelItems();
       // ShowExtendedPanelItems();
    }

    void AddItems(ItemBase l_ItemBase)
    {
        AddItem(l_ItemBase);
        DeactivateSlotInExtendedDisplay();
    }

    // Use this for initialization
    void Start()
    {

        character = GetComponentInParent<Character>();

        player = GetComponentInParent<Player>();

        inventoryUI = transform.GetChild(0).GetComponent<Canvas>();
        extendedPanel = inventoryUI.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        displayPanel = inventoryUI.transform.GetChild(0).GetComponent<RectTransform>();
        Debug.Log("displayPanel :  " + displayPanel.name);
        InventoryAnimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        InventoryButtonAnimator = transform.GetChild(0).GetChild(1).GetComponent<Animator>();
        extendedInventoryShowPanelanim = inventoryUI.transform.GetChild(2).GetComponent<Animator>();
        extendedInventoryShowButtonanim = inventoryUI.transform.GetChild(3).GetChild(0).GetComponent<Animator>();
        extendedinventoryButton = inventoryUI.transform.GetChild(3).GetChild(0).GetComponent<Button>();
        CreateSlots();
       
    }

    // Update is called once per frame
    void Update()
    {
        if(InventoryAnimator.GetBool("ShowInventory") && InventoryButtonAnimator.GetBool("SlideLeft"))
        {
            //SelectFromSlots();
            SwapPanels();
            //Debug.Log("swap:  " + displaySlotList[index1].panel.position + "   " + displaySlotList[index2].panel.position);
        }

        if (extendedInventoryShowPanelanim.GetBool("ExtendedInventoryShow"))
        {
            //call swap fucntion
        }
       
    }

    void SelectFromSlots()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (displayPanel != null && RectTransformUtility.RectangleContainsScreenPoint(displayPanel, Input.mousePosition))
            {
                Debug.Log("Inside display panel selection slots");
                for (int i = 0; i < displaySlotList.Count; i++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(displaySlotList[i].panel, Input.mousePosition))
                    {

                        if(displaySlotList[i].itemStored != null)
                {
                            Debug.Log("Inside display panel:>>><<><><><>  " + displaySlotList[i].itemStored.name);

                        }
                        else
                        {
                            Debug.Log("Inside display panel:>>><<><><><>  Item is  null");

                        }

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
                            if (slots[displaySlotList[i].fromSlot].itemlist.Count > 0)
                            {

                                activeItem = slots[displaySlotList[i].fromSlot].itemlist[slots[displaySlotList[i].fromSlot].itemlist.Count - 1];
                                
                               // slots[i].itemlist[slots[i].itemlist.Count - 1].gameObject.SetActive(true);
                                activeItem.gameObject.SetActive(true);
                          //    UpdateSlotData(activeItem);
                                SetActiveSlotCount();
                                DeactivateSlotInExtendedDisplay();
                                break;
                            }
                            Debug.Log("Assign from slot");
                        }
                        else if (slots[displaySlotList[i].fromSlot].itemStored!=null && activeItem.itemProperties != slots[displaySlotList[i].fromSlot].itemStored.itemProperties)
                        {
                            Debug.Log("Inside display active item not null  :>>><<><><><>  " + slots[displaySlotList[i].fromSlot].itemStored.name);
                            activeItem.gameObject.SetActive(false);
                            // ItemBase temp = activeItem;
                            if (slots[displaySlotList[i].fromSlot].itemlist.Count > 0)
                            {
                                activeItem = slots[displaySlotList[i].fromSlot].itemlist[slots[displaySlotList[i].fromSlot].itemlist.Count - 1];
                                activeItem.gameObject.SetActive(true);
                                DeactivateSlotInExtendedDisplay();
                                break;
                            }
                            
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

   int ActiveDisplaySlots()
    {
        int c = 0;
        for (int i = 0; i < displaySlotList.Count; i++)
        {
            if(displaySlotList[i].itemlist.Count>1 )
            {
                c++;

                continue;
            }
        }
        return c;
    }
  
    public void DeactivateSlotInExtendedDisplay()
    {
            Debug.Log("Deactivate slot called");
       // Debug.Log("Active slot count<><><><><><><><>" + activeSlotCount);
       // Debug.Log("display slot count<><><><><><><><>" + displaySlotList.Count);

            for (int j = 0; j < slots.Count; j++)
            {
                for (int i = 0; i < displaySlotList.Count; i++)
                {
                    if (slots[j].itemStored != null)
                    {
                        if (displaySlotList[i].itemStored != null && slots[j].itemStored.itemProperties == displaySlotList[i].itemStored.itemProperties)
                        {
                            slots[j].imageSlotPrefab.SetActive(false);
                            displaySlotList[i].fromSlot = slots[j].slotNumber;
                            displaySlotList[i].itemlist = slots[j].itemlist;
                            displaySlotList[i].UpdateUI();
                            Debug.Log(".......................");
                        }
                        else if(displaySlotList[i].itemStored == null)
                    {
                        displaySlotList[i].itemStored = Instantiate(slots[j].itemStored) as ItemBase;
                        displaySlotList[i].fromSlot = slots[j].slotNumber;
                        displaySlotList[i].itemlist = slots[j].itemlist;
                        displaySlotList[i].UpdateUI();
                        displaySlotList[i].imageSlotPrefab.SetActive(false);
                                                    Debug.Log("DIsplay slot list is null");
                                                    Debug.Log("<><><><>Item stored<><><><>" + displaySlotList[i].itemStored);
                                                    Debug.Log("<><><><>From slot<><><><>" + displaySlotList[i].fromSlot);
                                                    Debug.Log("<><><><>Item list count<><><><>" + displaySlotList[i].itemlist.Count);
                                                   // Debug.Log("<><><><>Item stored<><><><>");

                       


                    }
                    else
                        {
                            slots[j].imageSlotPrefab.SetActive(true);
                        }
                    }

                Debug.Log("<><><><><>Checking item properties " + displaySlotList[i].itemStored.itemProperties);
                Debug.Log("<><><><><>Checking item properties" + slots[j].itemStored.itemProperties);
                Debug.Log("<><><><><>Checking image prefab display" + displaySlotList[i].imageSlotPrefab);
                Debug.Log("<><><><><>Checking image prefab slot" + slots[j].imageSlotPrefab);

               /*     if (displaySlotList[i].itemStored != null && slots[j].itemStored.itemProperties!=null && displaySlotList[i].itemlist.Count > 0 && slots[j].itemStored.itemProperties == displaySlotList[i].itemStored.itemProperties)
                    {
                        displaySlotList[i].imageSlotPrefab.SetActive(true);
                        slots[j].imageSlotPrefab.SetActive(false);
                        //return;
                    }    */
                }
            }
            for (int i = 0; i < displaySlotList.Count; i++)
            {
                if(displaySlotList[i].itemStored != null)
                {
                    if (slots[displaySlotList[i].fromSlot].itemlist.Count < 1)
                    {
                        displaySlotList[i].imageSlotPrefab.SetActive(false);
                        displaySlotList[i].itemlist.Clear();
                        displaySlotList[i].countText.text = "";
                        displaySlotList[i].fromSlot = 0;
                        displaySlotList[i].itemStored = null;
                        displaySlotList[i].imageSlotPrefab.GetComponent<Image>().sprite = null;
                    }
                }          
            }

      //  ShowDisplayPanelItems();
    }

    public void CreateDisplaySlots()
    {
        displaySlotList = new List<Slot>(displaySlotCount);
        for (int i = 0; i < displaySlotCount; i++)
        {
            Slot tempSlot = new Slot();
            displaySlotList.Add(tempSlot);
            GameObject temp = Instantiate(imageSlotPrefab, displayPanel.transform);
            displaySlotList[i].imageSlotPrefab = temp;
            displaySlotList[i].panel = displaySlotList[i].imageSlotPrefab.gameObject.GetComponent<RectTransform>();
            displaySlotList[i].countText = displaySlotList[i].imageSlotPrefab.gameObject.GetComponent<Text>();
            displaySlotList[i].displaySprite = displaySlotList[i].imageSlotPrefab.transform.Find("Slot Image").gameObject.GetComponentInChildren<Image>();
            displaySlotList[i].imageSlotPrefab.SetActive(false);          
            displaySlotList[i].countText = slots[i].imageSlotPrefab.transform.Find("Count text").gameObject.GetComponent<Text>();    
        }
        for (int i = 0; i < displaySlotList.Count; i++)
        {
            displaySlotList[i].imageSlotPrefab.transform.SetParent(displayPanel.transform);
        }
    }

    public void DisplaySlotInitialization(ItemBase item)
    {
        Debug.Log("DisplaySlotinitialization is called");
        for (int i = 0; i < displaySlotList.Count;i++)
        {
            if(displaySlotList[i].itemStored == null)
            {          
                displaySlotList[i].imageSlotPrefab.transform.SetParent(displayPanel.transform);       
                displaySlotList[i].itemStored = Instantiate(item);
                displaySlotList[i].panel = displaySlotList[i].imageSlotPrefab.gameObject.GetComponent<RectTransform>();
                displaySlotList[i].displaySprite = displaySlotList[i].imageSlotPrefab.transform.Find("Slot Image").gameObject.GetComponentInChildren<Image>();
                displaySlotList[i].countText = displaySlotList[i].imageSlotPrefab.transform.Find("Count text").gameObject.GetComponent<Text>();
                displaySlotList[i].displaySprite.sprite = displaySlotList[i].itemStored.itemProperties.imageSprite;
                Debug.Log(" new Item added to display slot initialization");
                Debug.Log(" inside the display slot" + displaySlotList[i].itemStored.name);
                break;
            }
            else if(displaySlotList[i].itemStored!=null && displaySlotList[i].itemStored.itemProperties == item.itemProperties)
            {
                Debug.Log("Item present in display slot initialization");
                break;
            }                      
        }
            DeactivateSlotInExtendedDisplay();
    }

    public void ShowDisplayPanelItems()
    {
        for (int i = 0; i < displaySlotList.Count; i++)
        {
            if(displaySlotList[i].itemlist.Count > 0)
            {
                if(displayPanelShow)
                {
                    displaySlotList[i].imageSlotPrefab.SetActive(true);
                }
                else
                {
                    displaySlotList[i].imageSlotPrefab.SetActive(false);
                }
            }
        }
    }

    public void ShowExtendedPanelItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].itemlist.Count > 0)
            {
                if (extendedPanelShow)
                {
                    slots[i].imageSlotPrefab.SetActive(true);
                }
                else
                {
                    slots[i].imageSlotPrefab.SetActive(false);
                }
            }
        }
    }

    void SwapPanels()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (displayPanel != null && RectTransformUtility.RectangleContainsScreenPoint(displayPanel, Input.mousePosition))
            {
                for (int i = 0; i < displaySlotList.Count; i++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(displaySlotList[i].panel, Input.mousePosition))
                    {
                        if (selectedSlot1 == null)
                        {
                            selectedSlot1 =  displaySlotList[i];
                            index1 = i;
                            index1From = "DisplayPanel";
                            Debug.Log("index: " + i);
                            Debug.Log("Slot 1 :>><><><><>" + selectedSlot1.itemStored.name);
                            break;
                        }
                        else if (selectedSlot2 == null)
                        {
                            index2 = i;
                            index2From = "DisplayPanel";
                            Debug.Log("index: " + i);
                            selectedSlot2 =  displaySlotList[i];
                            Debug.Log("Slot 2 :>><><><><>" + selectedSlot2.itemStored.name);
                            break;
                        }
                    }
                }
            }

            if (extendedPanel != null && RectTransformUtility.RectangleContainsScreenPoint(extendedPanel, Input.mousePosition))
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(slots[i].panel, Input.mousePosition))
                    {
                        if (selectedSlot1 == null)
                        {
                            index1 = i;
                            index1From = "ExtendedPanel";
                            selectedSlot1 = slots[i];
                            break;
                        }
                        else if (selectedSlot2 == null)
                        {
                            selectedSlot2 = slots[i];
                            index2 = i;
                            index2From = "ExtendedPanel";
                            break;
                        }
                    }
                }
            }
        }

        if(selectedSlot1 != null && selectedSlot2 !=null)
        {
            Debug.Log("swap:  " + displaySlotList[index1].itemStored + "   " + displaySlotList[index2].itemStored);
            Debug.Log("swap:  " + displaySlotList[index1].panel.position + "   " + displaySlotList[index2].panel.position);

            //Slot temp = displaySlotList[index1];
            //displaySlotList[index1] = displaySlotList[index2];
            //displaySlotList[index2] = temp;

            
        //    displaySlotList[index2].imageSlotPrefab.SetActive(false);
          //  displaySlotList[index2].imageSlotPrefab.SetActive(true);

            Debug.Log("swap:  " + displaySlotList[index1].itemStored + "   " + displaySlotList[index2].itemStored);
            Debug.Log("swap:  " + displaySlotList[index1].panel.position + "   " + displaySlotList[index2].panel.position);


            if(SwapObj(selectedSlot1, selectedSlot2))
            //if(SwapObj(ref displaySlotList[index1],ref displaySlotList[index2]))
            {
            Debug.Log("Swapping called");
                selectedSlot1 = null;
                selectedSlot2 = null;
                index1From = null;
                index2From = null;
            //DeactivateSlotInExtendedDisplay();

            }


        }

    }

   bool SwapObj(Slot slot1, Slot slot2)
    {
         Slot temp = new Slot();
        Debug.Log("Indices:  " + index1 + " " + index2);
     /*   for (int i = 0; i < displaySlotList.Count; i++)
        {
            if(displaySlotList[i].itemStored != null)
            {
                Debug.Log("Display slot item name ><><><><><><><><>" + displaySlotList[i].itemStored);
            }
        }
        Debug.Log("count: " + slot1.countText.text + " " + slot2.countText.text);
        Debug.Log("Item stored: " + slot1.itemStored + " " + slot2.itemStored);
        // Debug.Log("count: " + slot1.countText.text + " " + slot2.countText.text);
        //Debug.Log("temp:  " + temp.itemStored.name);
        //temp = slot1;
        */


        if(index1From == index2From)

        {
            if(index1From == "DisplayPanel")
            {
                temp.imageSlotPrefab = slot1.imageSlotPrefab;
                temp.itemCount = slot1.itemCount;
                temp.itemlist = slot1.itemlist;
                temp.fromSlot = slot1.fromSlot;

                temp.itemStored = slot1.itemStored;
                Sprite sprite = slot1.itemStored.itemProperties.imageSprite;

                displaySlotList[index1].imageSlotPrefab = slot2.imageSlotPrefab;
                displaySlotList[index1].itemCount = slot2.itemCount;
                displaySlotList[index1].itemlist = slot2.itemlist;

                displaySlotList[index1].itemStored = slot2.itemStored;
                displaySlotList[index1].fromSlot = slot2.fromSlot;
                displaySlotList[index1].displaySprite.sprite = slot2.itemStored.itemProperties.imageSprite;

                displaySlotList[index2].imageSlotPrefab = temp.imageSlotPrefab;
                displaySlotList[index2].itemCount = temp.itemCount;
                displaySlotList[index2].itemlist = temp.itemlist;
                displaySlotList[index2].fromSlot = temp.fromSlot;

                displaySlotList[index2].itemStored = temp.itemStored;
                displaySlotList[index2].displaySprite.sprite = sprite;

                displaySlotList[index1].UpdateUI();
                displaySlotList[index2].UpdateUI();
                return true;
            }

            else  if(index1From == "ExtendedPanel")
            {
                temp.imageSlotPrefab = slot1.imageSlotPrefab;
                temp.itemCount = slot1.itemCount;
                temp.itemlist = slot1.itemlist;
                temp.fromSlot = slot1.fromSlot;

                temp.itemStored = slot1.itemStored;
                Sprite sprite = slot1.itemStored.itemProperties.imageSprite;

                slots[index1].imageSlotPrefab = slot2.imageSlotPrefab;
                slots[index1].itemCount = slot2.itemCount;
                slots[index1].itemlist = slot2.itemlist;

                slots[index1].itemStored = slot2.itemStored;
                slots[index1].fromSlot = slot2.fromSlot;
                slots[index1].displaySprite.sprite = slot2.itemStored.itemProperties.imageSprite;

                slots[index2].imageSlotPrefab = temp.imageSlotPrefab;
                slots[index2].itemCount = temp.itemCount;
                slots[index2].itemlist = temp.itemlist;

                slots[index2].fromSlot = temp.fromSlot;
                slots[index2].itemStored = temp.itemStored;
                slots[index2].displaySprite.sprite = sprite;

                slots[index1].UpdateUI();
                slots[index2].UpdateUI();
                return true;

            }

        }

        else
        {
            if(index1From == "DisplayPanel" && index2From == "ExtendedPanel")
            {
                temp.imageSlotPrefab = slot1.imageSlotPrefab;
                temp.itemCount = slot1.itemCount;
                temp.itemlist = slot1.itemlist;
                temp.fromSlot = slot1.fromSlot;

                temp.itemStored = slot1.itemStored;
                Sprite sprite = slot1.itemStored.itemProperties.imageSprite;

                displaySlotList[index1].imageSlotPrefab = slot2.imageSlotPrefab;
                displaySlotList[index1].itemCount = slot2.itemCount;
                displaySlotList[index1].itemlist = slot2.itemlist;

                displaySlotList[index1].itemStored = slot2.itemStored;
                displaySlotList[index1].fromSlot = slot2.fromSlot;
                displaySlotList[index1].displaySprite.sprite = slot2.itemStored.itemProperties.imageSprite;

                slots[index2].imageSlotPrefab = temp.imageSlotPrefab;
                slots[index2].itemCount = temp.itemCount;
                slots[index2].itemlist = temp.itemlist;
                slots[index2].fromSlot = temp.fromSlot;

                slots[index2].itemStored = temp.itemStored;
                slots[index2].displaySprite.sprite = sprite;

                displaySlotList[index1].UpdateUI();
                slots[index2].UpdateUI();
                return true;
            }

            else if (index1From == "ExtendedPanel" && index2From == "DisplayPanel")
            {
                temp.imageSlotPrefab = slot1.imageSlotPrefab;
                temp.itemCount = slot1.itemCount;
                temp.itemlist = slot1.itemlist;
                temp.fromSlot = slot1.fromSlot;

                temp.itemStored = slot1.itemStored;
                Sprite sprite = slot1.itemStored.itemProperties.imageSprite;

                slots[index1].imageSlotPrefab = slot2.imageSlotPrefab;
                slots[index1].itemCount = slot2.itemCount;
                slots[index1].itemlist = slot2.itemlist;

                slots[index1].itemStored = slot2.itemStored;
                slots[index1].fromSlot = slot2.fromSlot;
                slots[index1].displaySprite.sprite = slot2.itemStored.itemProperties.imageSprite;

                displaySlotList[index2].imageSlotPrefab = temp.imageSlotPrefab;
                displaySlotList[index2].itemCount = temp.itemCount;
                displaySlotList[index2].itemlist = temp.itemlist;
                displaySlotList[index2].fromSlot = temp.fromSlot;

                displaySlotList[index2].itemStored = temp.itemStored;
                displaySlotList[index2].displaySprite.sprite = sprite;

                slots[index1].UpdateUI();
                displaySlotList[index2].UpdateUI();
                return true;
            }
        }


        
       

        Debug.Log("count: " + slot1.countText.text + " " + slot2.countText.text);
        Debug.Log("Item stored: " + slot1.itemStored + " " + slot2.itemStored);
   /*     for (int i = 0; i < displaySlotList.Count; i++)
        {
            if (displaySlotList[i].itemStored != null)
            {
                Debug.Log("Display slot item name ><><><><><><><><>" + displaySlotList[i].itemStored);
            }
        }*/
        //   slot2.UpdateUI();
        // slot2.UpdateUI();


        Debug.Log("Swapped");
        return true;
      

    }


    #region Player Inventory Functions
    //The data for this object must be created when the object is being selected
    public class SelectionObjectData
    {
        public System.Enum item;
        public int pH = 0;      //Only for acids and bases
        public int volume = 0;  //Only for acids and bases

        public int slotIndex;
    }

    //Combine acids and bases....Volume calculation is present
    public void Combine(SelectionObjectData object1, SelectionObjectData object2)
    {
        //Two objects react to get a resultant object
        System.Enum result = Reactions.React(object1.item, object2.item);

        //Get the item description from the item manager
        ItemBase item = ItemManager.instance.itemDictionary[result].GetComponent<ItemBase>();
        ItemsDescription description = item.GetComponent<ItemsDescription>();
        

        //Perform the volume and pH calculations here. 
        int pH = 9; //TODO: this must be calculated using the formula

        if(pH != description.pHValue)
        {
            //TODO: Assign a new slot in the inventory and add it
            //TODO: Add new item Description data along with it while adding
        }

        //TODO: Check if the item is present in the inventory
        //If present, compare it with it's slot's item description
        //If not, add it to a new slot along with the item description information to the slot.
    }

    public void Extract(SelectionObjectData item)
    {
        ItemBase i = Extraction.Extract(item.item).GetComponent<ItemBase>();
        ItemsDescription des = i.GetComponent<ItemsDescription>();

        //The extraction will take default information from the prefab object.
        //must check if the item type is already present.
        //IF the item type is already present, it's pH value in the description data must be checked.
        //If the pH is same, the extra volume must be added to the existing item slot
    }
    
    //Same reaction mechanic, but no volume calculations
    public void MakepHIndicators(SelectionObjectData object1, SelectionObjectData object2)
    {
        //Two objects react to get a resultant object
        System.Enum result = Reactions.React(object1.item, object2.item);

        //pH papers doesn't have volume. So, volume calculations is not needed

        //pH calculations is also needed.

        //TODO: Check if the item is present in the inventory
        //If present, compare it with it's slot's item description
        //If not, add it to a new slot along with the item description information to the slot.
    }

    public void AddLiquidToPlayer(SelectionObjectData item)
    {
        //Add the selected object to the player

        //Add the item only if it is acid or base.
        if(item.item.GetType() == typeof(AcidsList) || item.item.GetType() == typeof(BasesList))
        {
            //If player already has a liquid in him, react
            System.Enum result = player.React(item);
            //If the result is not null, place it in the inventory

            //If the player doesn't have liquid in him, then change the liquid type, volume and pH of player appropriately
        }
    }

    public void GetLiquidFromPlayer(int amount)
    {
        System.Enum item = player.chemical;

        //This chemical must be added to the inventory.
        //Check it it's already present. If it is, then check if the pH matches in the slot's item description
        //If it isn't add it
    }

    #endregion
}

