using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class InventorySaveData : SaveData
{
   // public static InventorySaveData instance = new InventorySaveData();
    public List<SlotSaveData> slots = new List<SlotSaveData>();
    public string activeItem;
    public int activeDisplayCount;
    
}

[System.Serializable]
public class SlotSaveData
{
    public string itemStored;
    public int numberOfItemsStored;
    public string parent;
    public bool isActive;
    public int siblingIndex;
  //  public SerializableRect position;
}


public class PlayerInventory : Inventory
{

    public InventorySaveData inventorySaveData = new InventorySaveData();

    Player player;

    bool inventoryShown = false;

    Canvas inventoryUI;

    public RectTransform displayPanel;
    public RectTransform extendedPanel;

    public Button inventoryButton;
    public Button extendedInventoryButton;
    public Image extendedInventoryButtonHolder;

    public Scrollbar verticalScrollBar;

    public Button selectButton;

    public Button extractButton;
    public Button removeButton;

    public Button swapButton;
    public Button combineButton;
    public Button specialActionButton;

    public Animator inventoryHolderAnimator;

    public Animator extendedInventoyHolderAnimator;

    public int maxDisplaySlots;

    public bool showInventoryItems = false;

    public bool allowSelection = false;

    public bool allowSwap = false;

    public Slot selectedSlot1;

    public Slot selectedSlot2;

    public GameObject highlightSelection1;
    public GameObject highlightSelection2;

    public bool allowCombine = false;
    public bool allowExtract = false;
    public bool allowRemove = false;

    public List<GameObject> highLightGameObjectList;


    

    // Use this for initialization
    void Start()
    {



        character = GetComponentInParent<Character>();

        player = GetComponentInParent<Player>();

        inventoryUI = transform.GetChild(0).GetComponent<Canvas>();

        extendedPanel = inventoryUI.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        displayPanel = inventoryUI.transform.GetChild(0).GetComponent<RectTransform>();
        inventoryHolderAnimator = displayPanel.GetComponent<Animator>();
        extendedInventoyHolderAnimator = inventoryUI.transform.GetChild(2).GetComponent<Animator>();

      
        extendedInventoryButtonHolder = inventoryUI.transform.GetChild(3).GetComponent<Image>();
        verticalScrollBar = inventoryUI.GetComponentInChildren<Scrollbar>();
        verticalScrollBar.gameObject.SetActive(false);

        ////////BUttons/////////
        inventoryButton = inventoryUI.transform.GetChild(1).GetComponent<Button>();
        extendedInventoryButton = inventoryUI.transform.GetChild(3).GetComponentInChildren<Button>();

        selectButton = inventoryUI.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Button>();
        extractButton = inventoryUI.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Button>();
        swapButton = inventoryUI.transform.GetChild(2).GetChild(2).GetChild(2).GetComponent<Button>();
        combineButton = inventoryUI.transform.GetChild(2).GetChild(2).GetChild(3).GetComponent<Button>();

        specialActionButton = inventoryUI.transform.GetChild(2).GetChild(3).GetComponent<Button>();
        removeButton = inventoryUI.transform.GetChild(2).GetChild(4).GetComponent<Button>();

        highlightSelection1 = inventoryUI.transform.GetChild(4).gameObject;

        highlightSelection2 = inventoryUI.transform.GetChild(5).gameObject;

        ///////////////////////////buttons end//////////

        CreateSlot();

        for(int i = 0; i<slots.Count;i++)
        {
            slots[i].imageSlotPrefab.transform.SetParent(extendedPanel);
        }

        SelectButtonIsPressed();
        //////load inventory items///////
        SaveManager.SaveEvent += Save;

        CheckPointManager.RegisterCheckPointEvent += Save;
        CheckPointManager.LoadCheckpointEvent += Load;

        /////////////////////////////////
    }

    // Update is called once per frame
    void Update()
    {
        DisplaySlotUpdate();
        InventoryAnimate();
        
    }




    #region Inventory Display and animation functions

    public void InventoryAnimate()
    {
        if (showInventoryItems)
        {
                ShowDisplayItems();
           // Debug.Log(" Getting called" + extendedInventoyHolderAnimator.GetBool("ExtendedInventoryShow"));

            if (extendedInventoyHolderAnimator.GetBool("ExtendedInventoryShow"))
            {
                 ShowAllItems();
            }
            else
            {
                HideExtendedItems();
            }
            // ButtonActivation();
        }

        else
        {
            HideDisplayItems();
        }

        if (allowSelection || allowExtract || allowRemove)
        {
            SelectOneItem();
            if (allowSelection && selectedSlot1 != null)
            {
                // set as active item
                SelectSpecialActionButton();
            }
        }

        if (allowSwap)
        {
            SelectTwoItems();
            if (selectedSlot1 != null && selectedSlot2 != null)
            {
                SwapSlots();
            }
        }
    }

    public void InventoryButtonPressed()
    {
        if(inventoryButton.GetComponent<Animator>().GetBool("InventoryButton") && extendedInventoyHolderAnimator.GetBool("ExtendedInventoryShow"))
        {
            HideAll();
        }
        else if(inventoryButton.GetComponent<Animator>().GetBool("InventoryButton"))
        {
            inventoryHolderAnimator.SetBool("InventoryHolderShow", false);
            RuntimeAnimatorController tempController = inventoryHolderAnimator.runtimeAnimatorController;
            float timer = tempController.animationClips.Length / 2;
            showInventoryItems = false;
            allowSelection = false;
            Invoke("DisplayPanelHide", timer);
        }
        else
        {
            inventoryButton.GetComponent<Animator>().SetBool("InventoryButton", true);
            RuntimeAnimatorController tempController = inventoryButton.GetComponent<Animator>().runtimeAnimatorController;
            float timer = tempController.animationClips.Length;
            Invoke("DisplayPanelShow", timer);
        }
    }

    private void DisplayPanelShow()
    {       
        inventoryHolderAnimator.SetBool("InventoryHolderShow", true);
        extendedInventoryButtonHolder.enabled = true;       
        extendedInventoryButton.GetComponent<Image>().enabled = true;
        extendedInventoryButton.GetComponent<Animator>().SetBool("ExtendedInventoryButton", true);
        showInventoryItems = true;
        allowSelection = true;
        allowSwap = true;
    }

    private void DisplayPanelHide()
    {
        inventoryButton.GetComponent<Animator>().SetBool("InventoryButton", false);
        extendedInventoryButtonHolder.enabled = false;
        extendedInventoryButton.GetComponent<Image>().enabled = false;
    }

    public void ExtendedInventoryButtonPressed()
    {
        if (extendedInventoyHolderAnimator.GetBool("ExtendedInventoryShow"))
        {
            extendedInventoyHolderAnimator.SetBool("ExtendedInventoryShow", false);
            verticalScrollBar.gameObject.SetActive(false);

        }
        else
        {
            extendedInventoyHolderAnimator.SetBool("ExtendedInventoryShow", true);
            verticalScrollBar.gameObject.SetActive(true);
            allowSelection = false;
          //  ShowAllItems();
        }
    }

    public void HideAll()
    {
        verticalScrollBar.gameObject.SetActive(false);
        extendedInventoyHolderAnimator.SetBool("ExtendedInventoryShow", false);
        RuntimeAnimatorController tempController = extendedInventoyHolderAnimator.runtimeAnimatorController;
        float timer = tempController.animationClips.Length;       
        Invoke("InventoryButtonPressed", timer);
        showInventoryItems = false;
        allowSelection = false;
        allowSwap = false;
    }

    int displaySlotCount = 0;

    public void DisplaySlotUpdate()
    {
        if(displaySlotCount < maxDisplaySlots)
        {
            int count = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].isActive && count < maxDisplaySlots)
                {
                    slots[i].imageSlotPrefab.transform.SetParent(displayPanel);
                    // slots[i].imageSlotPrefab.SetActive(false);
                    count++;
                }

                else
                {
                    slots[i].imageSlotPrefab.transform.SetParent(extendedPanel);
                    if (extendedInventoyHolderAnimator.GetBool("ExtendedInventoryShow") && slots[i].isActive)
                    {
                      //  Debug.Log("Function is getting called");

                        slots[i].imageSlotPrefab.SetActive(true);
                    }
                    else
                    {
                        slots[i].imageSlotPrefab.SetActive(false);
                    }
                }
            }
            SetDisplayCount();          
        }       
    ///display count shuld be updated.....  

    }

    public void SetDisplayCount()
    {
        int c = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].isActive && slots[i].imageSlotPrefab.transform.parent == displayPanel)
            {
                c++;
            }
        }
        displaySlotCount = c;
    }
   
    public void ShowDisplayItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isActive && slots[i].imageSlotPrefab.transform.parent == displayPanel)
            {
                slots[i].imageSlotPrefab.SetActive(true);
            }


        }
    }

    public void ShowAllItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isActive)
            {
                slots[i].imageSlotPrefab.SetActive(true);
            }


        }
    }
    public void HideExtendedItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isActive && slots[i].imageSlotPrefab.transform.parent == extendedPanel)
            {
                slots[i].imageSlotPrefab.SetActive(false);
            }


        }

    }

    public void HideDisplayItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isActive)
            {
                slots[i].imageSlotPrefab.SetActive(false);
            }
        }
    }

    public void SelectOneItem()
    {
        if(Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].isActive && slots[i].imageSlotPrefab.transform.parent == displayPanel)
                {
                    //Debug.Log("Active slot" + slots[i].itemStored.name);

                    if (RectTransformUtility.RectangleContainsScreenPoint(slots[i].panel, Input.mousePosition))
                    {
                       if(selectedSlot1 == null)
                       {
                            selectedSlot1 = slots[i];
                            HighLightSlot(selectedSlot1, highlightSelection1);
                       }

                       else if(selectedSlot1 != null && selectedSlot1 != slots[i])
                        {
                            selectedSlot1 = slots[i];
                            HighLightSlot(selectedSlot1, highlightSelection1);
                        }
                    }
                }
            }
        }
    }
    
    public void HighLightSlot(Slot selectedSlot,GameObject hightlighter)
    {
        hightlighter.transform.position = selectedSlot.imageSlotPrefab.transform.position;
        hightlighter.gameObject.SetActive(true);
    }   

    public void SelectSpecialActionButton()
    {       
        if (selectedSlot1.itemlist.Count > 0)
        {
            if(activeItem == null)
            {
                activeItem = selectedSlot1.itemlist[selectedSlot1.itemlist.Count - 1];

            }
            else
            {
                activeItem.gameObject.SetActive(false);
                activeItem = selectedSlot1.itemlist[selectedSlot1.itemlist.Count - 1];
                activeItem.gameObject.SetActive(true);
            }
          

            if(selectedSlot1.imageSlotPrefab.transform.parent != displayPanel)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if(slots[i].imageSlotPrefab.transform.parent == displayPanel)
                    {
                        selectedSlot2 = slots[i];
                        SwapSlots();
                    }
                }
            }

            selectedSlot1 = null;
         //   highlightSelection1.SetActive(false);

        }
    }

    public void ExtractSpecialActionButton()
    {
        if(selectedSlot1 != null)
        {
            if (selectedSlot1.itemlist.Count > 0)
            {
                Debug.Log( selectedSlot1.itemlist[selectedSlot1.itemlist.Count - 1].gameObject.GetComponent<ItemsDescription>().GetItemType());
                selectedObj1.item = selectedSlot1.itemlist[selectedSlot1.itemlist.Count - 1].gameObject.GetComponent<ItemsDescription>().GetItemType();
                Extract(selectedObj1);
            }
        }
    }

    public void CombineSpecialActionButton()
    {
        if (selectedSlot1 != null && selectedSlot2 != null)
        {
            if (selectedSlot1.itemlist.Count > 0)
            {
                selectedObj1.item = selectedSlot1.itemlist[selectedSlot1.itemlist.Count - 1].gameObject.GetComponent<ItemsDescription>().GetItemType();             
            }

            if (selectedSlot2.itemlist.Count > 0)
            {
                selectedObj2.item = selectedSlot2.itemlist[selectedSlot1.itemlist.Count - 1].gameObject.GetComponent<ItemsDescription>().GetItemType();
            }
        }
    }

    public void RemoveSpecialActionButton()
    {
        if(selectedSlot1 != null)
        {
            ItemCountSelection.instance.RemoveItemsActivate(selectedSlot1.itemlist.Count);
        }
    }

    public void SelectTwoItems()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].isActive && RectTransformUtility.RectangleContainsScreenPoint(slots[i].panel, Input.mousePosition))
               // if (RectTransformUtility.RectangleContainsScreenPoint(slots[i].panel, Input.mousePosition))
                {
                    if (selectedSlot1 == null && selectedSlot2 != slots[i])
                    {
                        selectedSlot1 = slots[i];
                        HighLightSlot(selectedSlot1, highlightSelection1);
                        Debug.Log(selectedSlot1.itemStored.name);                        
                        break;
                    }
                    else if(selectedSlot1 != null && selectedSlot1 == slots[i])
                    {
                        selectedSlot1 = null;
                        highlightSelection1.SetActive(false);
                        Debug.Log("Removed selection of 1");
                        break;
                    }
                    else if (selectedSlot2 == null && selectedSlot1 != slots[i])
                    {
                        selectedSlot2 = slots[i];
                        HighLightSlot(selectedSlot2, highlightSelection2);
                        Debug.Log(selectedSlot2.itemStored.name);                     
                        break;
                    }
                    else if (selectedSlot2 != null && selectedSlot2 == slots[i])
                    {
                        selectedSlot2 = null;
                        highlightSelection2.SetActive(false);
                        Debug.Log("Removed selection of 2");
                        break;
                    }
                }
            }
        }
    }

    public void SwapSlots()
    {
        if(selectedSlot1 != null && selectedSlot2 != null)
        {
            int clildIndex1 = selectedSlot1.imageSlotPrefab.transform.GetSiblingIndex();
            int clildIndex2 = selectedSlot2.imageSlotPrefab.transform.GetSiblingIndex();

            if (selectedSlot1.imageSlotPrefab.transform.parent != selectedSlot2.imageSlotPrefab.transform.parent)
            {                         
                Transform temp = selectedSlot1.imageSlotPrefab.transform.parent;
                selectedSlot1.imageSlotPrefab.transform.SetParent(selectedSlot2.imageSlotPrefab.transform.parent);
                selectedSlot2.imageSlotPrefab.transform.SetParent(temp);

            }             
            selectedSlot1.imageSlotPrefab.transform.SetSiblingIndex(clildIndex2);
            selectedSlot2.imageSlotPrefab.transform.SetSiblingIndex(clildIndex1);

            selectedSlot1 = selectedSlot2 = null;
            highlightSelection1.SetActive(false);
            highlightSelection2.SetActive(false);
        }
    }


    public void ButtonActivation()
    {
        if (selectedSlot1 != null && selectedSlot2 != null)
        {
            swapButton.gameObject.SetActive(true);
            combineButton.gameObject.SetActive(true);


            selectButton.gameObject.SetActive(false);
            extractButton.gameObject.SetActive(false);

        }

       else if (selectedSlot1 != null || selectedSlot2 != null)
        {
            selectButton.gameObject.SetActive(true);

            selectButton.interactable = true;

            extractButton.gameObject.SetActive(true);

            extractButton.interactable = true;

            removeButton.interactable = true;

            swapButton.gameObject.SetActive(false);
            combineButton.gameObject.SetActive(false);           
        }
        
    }
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    SelectionObjectData selectedObj1 = new SelectionObjectData();
    SelectionObjectData selectedObj2 = new SelectionObjectData();

    public void SelectButtonIsPressed()
    {
        allowSelection = true;
        allowSwap = false;
        specialActionButton.gameObject.SetActive(false);
        specialActionButton.GetComponentInChildren<Text>().text = "Set As Active Item";
        selectButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
        extractButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        combineButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        swapButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        removeButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
        removeButton.gameObject.SetActive(true);
    }

    public void ExtractButtonIsPressed()
    {
        allowSelection = false;
        allowSwap = false;
        allowCombine = false;
        allowRemove = false;
        allowExtract = true;
        specialActionButton.gameObject.SetActive(true);

        specialActionButton.GetComponentInChildren<Text>().text = "Extract from Item";

        selectButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        extractButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
        combineButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        swapButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        removeButton.GetComponent<Image>().color = new Color(255, 255, 255, 0f);
        removeButton.gameObject.SetActive(false);


    }

    public void ComineButtonIsPressed()
    {
        allowSelection = false;
        allowSwap = false;
        allowExtract = false;
        allowCombine = true;
        allowRemove = false;
        specialActionButton.gameObject.SetActive(true);
        specialActionButton.GetComponentInChildren<Text>().text = "Combine these Item";

        selectButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        extractButton.GetComponent<Image>().color = new Color(255, 255, 255,0.5f);
        combineButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
        swapButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        removeButton.GetComponent<Image>().color = new Color(255, 255, 255, 0f);
        removeButton.gameObject.SetActive(false);

    }


    public void SwapButtonIsPressed()
    {
        allowSelection = false;
        allowSwap = true;
        allowExtract = false;
        allowCombine = false;
        allowRemove = false;

        specialActionButton.gameObject.SetActive(false);

        selectButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        extractButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        combineButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        swapButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
        removeButton.GetComponent<Image>().color = new Color(255, 255, 255, 0f);
        removeButton.gameObject.SetActive(false);

    }


    public void RemoveButtonIsPressed()
    {
     /*   allowSelection = false;
        allowSwap = false ;
        allowExtract = false;
        allowCombine = false;
        allowRemove = true;


        specialActionButton.gameObject.SetActive(true);
        specialActionButton.GetComponentInChildren<Text>().text = "Remove item";
        selectButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        extractButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        combineButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        swapButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        removeButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);*/

        RemoveSpecialActionButton();


    }

    public void RemoveItems(float itemCount)
    {
         if(selectedSlot1 != null )
         {
            for (int i = 0; i < itemCount; i++)
            {
                selectedSlot1.RemoveItem(selectedSlot1.itemlist[selectedSlot1.itemlist.Count - 1]);
            }
         }

       
    }

    public void SpecialActionButtonPressed()
    {
      
        if(allowExtract)
        {
            // call extract functions
            ExtractSpecialActionButton();
        }

        else if(allowCombine)
        {

            //call combine function
            CombineSpecialActionButton();
        }

       if(allowRemove)
        {
           // RemoveSpecialActionButton();
        }
       

    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion

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
        des.GetItemType();

        //The extraction will take default information from the prefab object.
        //must check if the item type is already present.
        int phVal = des.pHValue;
      
        AddItem(i); //invenetory will take care of everything checking for ph if present.

        
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
            if(result != null)
            {
          //      if(player.GetComponent<PlayerMechanics>().volume == result)
            }
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


    #region Save and Load

   public void Save(System.Type type)
    {
        inventorySaveData = new InventorySaveData();
    


        
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemlist.Count > 0 && slots[i].isActive)
            {

                SlotSaveData slotdata = new SlotSaveData();
                slotdata.itemStored = slots[i].itemlist[0].gameObject.GetComponent<ItemsDescription>().GetType().ToString();
                slotdata.parent = slots[i].imageSlotPrefab.transform.parent.name;
                
               // slotdata.position.x = slots[i].imageSlotPrefab.transform.position.x;
                //slotdata.position.y = slots[i].imageSlotPrefab.transform.position.y;
                
                slotdata.isActive = slots[i].isActive;
                slotdata.siblingIndex = slots[i].imageSlotPrefab.transform.GetSiblingIndex();

                // InventorySaveData.instance.slots.Add(slotdata);

             //   inventorySaveData.slots.Add(slotdata);
            }
        }

        for (int i = 0; i < inventorySaveData.slots.Count; i++)
        {
            Debug.Log(inventorySaveData.slots[i].itemStored + "Inventory slot data item stored");
            Debug.Log(inventorySaveData.slots[i].isActive + "Inventory slot data active status");
            Debug.Log(inventorySaveData.slots[i].numberOfItemsStored + "Inventory slot data number of items stored");
            Debug.Log(inventorySaveData.slots[i].siblingIndex + "Inventory sibling index");
        }
        
        if (activeItem != null)
        {
            inventorySaveData.activeItem = activeItem.gameObject.GetComponent<ItemsDescription>().GetType().ToString();
          //  inventorySaveData.activeItem = activeItem.itemProperties.name;
        }
        else
        {
            //  Debug.LogError("Active item is null");
            inventorySaveData.activeItem = null;
        }

      

        Debug.Log("Created inventory save obj>>>>>>"  + inventorySaveData.activeItem);

        if (type.Equals(typeof(SaveManager)))
            SaveManager.saveObject.AddObject(inventorySaveData);
        else if (type.Equals(typeof(CheckPointManager)))
            CheckPointManager.checkPointData.AddObject(inventorySaveData);

        Debug.Log("Added to the save object list successfully");

       
    }



    public virtual void LoadData(InventorySaveData saveData)
    {

       // if(InventorySaveData.instance != null)
        if(saveData != null)
        {
            // for (int i = 0; i < InventorySaveData.instance.slots.Count; i++)
             for (int i = 0; i < saveData.slots.Count; i++)
             {
                  for (int j = 0; j < slots.Count; j++)
                  {
                      if(slots[j].itemStored == null)
                      {
                          System.Type type = System.Type.GetType(saveData.slots[i].itemStored);

                          GameObject temp = ItemManager.instance.itemDictionary[type];


                          for (int k = 0; k< saveData.slots[i].numberOfItemsStored;k++)
                          {
                              GameObject tempItem = Instantiate(temp);
                              slots[j].AddItem(tempItem.GetComponent<ItemBase>());
                              tempItem.GetComponent<ItemBase>().transform.parent = player.Hand.transform;
                              slots[j].itemStored = slots[j].itemlist[0];
                              slots[j].isActive = saveData.slots[i].isActive;
                              slots[j].imageSlotPrefab.transform.parent = this.transform.Find(saveData.slots[i].parent);
                          }
                      }
                  } 
             }
            GameObject tempActiveItem = Instantiate(ItemManager.instance.itemDictionary[saveData.activeItem]);
            activeItem = tempActiveItem.GetComponent<ItemBase>();
        }
    }

    void Load(System.Type type)
    {
        //Debug.Log(CheckPointManager.checkPointData.types.Count + "check point data count");

        //Get the appropriate value from the save data
        //TODO: This part might need a little tweaking

        InventorySaveData saveData = new InventorySaveData();

        if (type.Equals(typeof(SaveManager)))
        {
            Debug.Log("Loading");
            for (int i = 0; i < SaveManager.saveObject.types.Count; i++)
            {
                if (SaveManager.saveObject.types[i].type == typeof(InventorySaveData).ToString())
                {

                    saveData = (InventorySaveData)SaveManager.saveObject.types[i].values[0];
                    break;
                }
            }      
        }
        else
        {
            if (type.Equals(typeof(CheckPointManager)))
            {
                Debug.Log("Loading the checkpoint");

                Debug.Log(CheckPointManager.checkPointData.types.Count + "check point data count");

                for (int i = 0; i < CheckPointManager.checkPointData.types.Count; i++)
                {

                    if (CheckPointManager.checkPointData.types[i].type == typeof(InventorySaveData).ToString())
                    {
                        Debug.Log((InventorySaveData)CheckPointManager.checkPointData.types[i].values[0] + "value of check point");
                        saveData = (InventorySaveData)CheckPointManager.checkPointData.types[i].values[0];

                        break;
                    }
                }

                LoadData(saveData);
             
            }
        }
    }

    private void OnDestroy()
    {
        SaveManager.SaveEvent -= Save;

        CheckPointManager.RegisterCheckPointEvent -= Save;

        CheckPointManager.LoadCheckpointEvent -= Load;

    }

    #endregion
}




