﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Must be attached to a separate journal canvas
/// </summary>

public class Journal : MonoBehaviour {

    public static Journal Instance { get; set; }

    /// <summary>
    /// List of all the items in the journal.
    /// If a new item is picked up, it will be added to this list
    /// </summary>
    //List<ItemList> itemsInJournal;
    List<System.Object> itemsInJournal;
    List<string> pathToImageAsset;
    List<string> itemDescriptions;

    List<string> itemsDisplayed;

    bool opened = false;

    /// <summary>
    /// Journal tabs contain the index which is used to populate appropriate items
    /// 0 - All Items || 1 - Items || 2 - Acids || 3 - Bases || 4 - Salts || 5 - Indicators
    /// </summary>
    List<JournalTab> journalTabs;

    JournalTab activeTab;

    public GameObject contentPrefab;
    GameObject itemsPanel;
    /// <summary>
    /// Content panel's local transform must be (0, 0, 0) and it's pivot must be top-middle
    /// </summary>
    GameObject contentPanel;    //Content panel must be a child of items panel

    //Object Pooling for tabs
    List<GameObject> activeContentItems;
    List<GameObject> disabledContentItems;

    float heightOfContent;
    float heightOfContentPanel;

    GameObject activeContentItem = null;
    
    private void Awake()
    {
        //This expands to: Instance = Instance != null ? Instance : this;
        Instance = Instance ?? this;

        if (Instance != this)
            Destroy(this);
    }

    private void Start()
    {
        itemsInJournal = new List<System.Object>();
        pathToImageAsset = new List<string>();
        itemDescriptions = new List<string>();

        itemsDisplayed = new List<string>();

        journalTabs = new List<JournalTab>();

        activeContentItems = new List<GameObject>();
        disabledContentItems = new List<GameObject>();

        journalTabs.AddRange(GetComponentsInChildren<JournalTab>());

        activeTab = journalTabs[0];

        itemsPanel = transform.Find("Items Panel").gameObject;
        contentPanel = itemsPanel.transform.Find("Content Panel").gameObject;

        ItemBase.ItemPickedUpEvent += ItemPickedUp;

        gameObject.SetActive(false);

        heightOfContent = contentPrefab.GetComponent<RectTransform>().sizeDelta.y;

        RefreshContentInActiveTab();
    }

    private void Update()
    {
        CheckContentSelection();
        
    }

    void CheckContentSelection()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RectTransform contentPanelRect = contentPanel.GetComponent<RectTransform>();
            Vector2 localPoint;
            //if (RectTransformUtility.RectangleContainsScreenPoint(contentPanelRect, Input.mousePosition))
            localPoint = contentPanelRect.InverseTransformPoint(Input.mousePosition);
            //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(contentPanelRect, Input.mousePosition, 
            //                                                            null, out localPoint))
            if(contentPanelRect.rect.Contains(localPoint))
            {
                //touch / mouse click happened in the contents panel.
                //y coordinate of local point is important for the selection

                localPoint.x = localPoint.x / contentPanelRect.sizeDelta.x;
                localPoint.y = localPoint.y / contentPanelRect.sizeDelta.y;

                if (activeContentItems.Count > 0)
                {
                    float partition = (float)1 / activeContentItems.Count;
                    int indexnumber = Mathf.FloorToInt(-localPoint.y / partition);
                    Debug.Log("partition: " + partition);
                    Debug.Log("local point:  " + localPoint);
                    Debug.Log("Index number is: " + indexnumber);
                    //select the content item of specified index
                    if (activeContentItems[indexnumber] != activeContentItem)
                    {
                        if (activeContentItem != null)
                            activeContentItem.GetComponent<UnityEngine.UI.Image>().color = Color.grey;

                        activeContentItem = activeContentItems[indexnumber];
                        activeContentItem.GetComponent<UnityEngine.UI.Image>().color = Color.green;
                    }
                }
                //List<System.Enum> balh = new List<System.Enum> {
                //    IndicatorsList.Bromythol_Blue,
                //    AcidsList.HCl,
                //    BasesList.NaOh
                //};
                //Debug.Log("Type of blah: " + balh[0].GetType());
            }
        }
    }

    public void ToggleJournal()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf)
        {
            RefreshContentInActiveTab();
            opened = true;
        }
        else
            opened = false;
    }

    void ItemPickedUp(ItemBase item)
    {
        ItemsDescription des = item.GetComponent<ItemsDescription>();
        if (des == null)
            return;
        //Check if the item is already present in the journal
        bool present = IsItemPresentInTheJournal(des);

        //Add the item if not.
        if (!present)
        {
            Debug.Log("Item is not present in the journal.....Attempting to add it");
            AddItem(des);
        }
        else
        {
            Debug.Log("Item is already present in the journal....No need to add again");
        }

        //At the same time, get the path to the image asset and description content

        //Get the search tags also

        //Item base should have all the above information
        
    }

    bool IsItemPresentInTheJournal(ItemsDescription des)
    {
        //Get the current items in the journal

        switch (des.itemType)
        {
            case ItemType.Normal:
                return CheckWith(des.normalItemType);
                break;
            case ItemType.Acid:
                return CheckWith(des.acidType);
                break;
            case ItemType.Base:
                return CheckWith(des.baseType);
                break;
            case ItemType.Salt:
                return CheckWith(des.saltType);
                break;
            case ItemType.Indicator:
                return CheckWith(des.indicatorType);
                break;
        }
        return false;
    }

    bool CheckWith<T>(T item)
    {
        for(int i = 0; i < itemsInJournal.Count; i++)
        {
            if (itemsInJournal[i].Equals(item))
            {
                return true;
            }

        }
        return false;
    }
    
    public void AddItem(ItemsDescription description)
    {
        switch (description.itemType)
        {
            case ItemType.Normal:
                AddCorrectItemTypeToJournal(description.normalItemType);
                break;
            case ItemType.Acid:
                AddCorrectItemTypeToJournal(description.acidType);
                break;
            case ItemType.Base:
                AddCorrectItemTypeToJournal(description.baseType);
                break;
            case ItemType.Salt:
                AddCorrectItemTypeToJournal(description.saltType);
                break;
            case ItemType.Indicator:
                AddCorrectItemTypeToJournal(description.indicatorType);
                break;
        }

    }

    public T Add<T>(T item)
    {
        Debug.Log("Item to string: " + item.ToString());
        T val = (T)System.Enum.Parse(typeof(T), item.ToString());
        itemsInJournal.Add(val);
        return val;
    }

    public void AddCorrectItemTypeToJournal<T>(T item) where T:struct, System.IConvertible
    {
        //Get the enum values of the complete list of items
        if (typeof(T).Equals(typeof(AcidsList)))
        {
            Add(item);
        }
        else if (typeof(T).Equals(typeof(BasesList)))
        {
            Add(item);
        }
        else if (typeof(T).Equals(typeof(SaltsList)))
        {
            Add(item);
        }
        else if (typeof(T).Equals(typeof(IndicatorsList)))
        {
            Add(item);
        }
        else if (typeof(T).Equals(typeof(NormalItemList)))
        {
            Add(item);
        }

        //ItemList[] list = GetEnumValues<ItemList>();
        //
        //
        //
        ////TODO: Convert this to binary search algorithm
        //foreach(ItemList i in list)
        //{
        //    //if (System.Enum.GetName(typeof(ItemList), i) == System.Enum.GetName(typeof(T), itemType))
        //    //{
        //    //    Debug.Log("Item has been found in the list of items. adding it to the journal");
        //    //    itemsInJournal.Add(i);
        //    //    typeOfItems.Add(typeof(T));
        //    //    break;
        //    //}
        //    if (System.Enum.GetName(typeof(System.Enum), i) == System.Enum.GetName(typeof(T), itemType))
        //    {
        //        Debug.Log("Item has been found in the list of items. adding it to the journal");
        //        itemsInJournal.Add(i);
        //        typeOfItems.Add(typeof(T));
        //        break;
        //    }
        //}
        Debug.Log("Items in journal: " + itemsInJournal.Count);
    }

    T[] GetEnumValues<T>()
    {
        return (T[])System.Enum.GetValues(typeof(T));
    }
    
    public void ChangeTab(JournalTab tab)
    {
        activeTab.GetComponent<UnityEngine.UI.Image>().color = Color.gray;
        tab.GetComponent<UnityEngine.UI.Image>().color = Color.green;

        //set all the active content items to false
        Debug.Log("Active items list: " + activeContentItems.Count);
        for(int i = 0; i < activeContentItems.Count; i++)
        {
            disabledContentItems.Add(activeContentItems[i]);
            activeContentItems[i].SetActive(false);
            //activeContentItems.RemoveAt(0);
            //Debug.Log("Removing..................");
        }
        activeContentItems.Clear();
        activeTab = tab;
        itemsDisplayed.Clear();
        RefreshContentInActiveTab();
    }

    void RefreshContentInActiveTab()
    {
        Debug.Log("Refreshing Content");
        if(activeTab != null)
            switch (activeTab.tabType)
            {
                case JournalTabType.General:
                    CheckForTypeAndInstantiateContent(typeof(NormalItemList));
                    break;
                case JournalTabType.Acids:
                    CheckForTypeAndInstantiateContent(typeof(AcidsList));
                    break;
                case JournalTabType.Bases:
                    CheckForTypeAndInstantiateContent(typeof(BasesList));
                    break;
                case JournalTabType.Indicators:
                    CheckForTypeAndInstantiateContent(typeof(IndicatorsList));
                    break;
            }
    }

    void CheckForTypeAndInstantiateContent(System.Type type)
    {
        //for (int i = 0; i < typeOfItems.Count; i++)
        //{
        //    Debug.Log("Type 1: " + typeOfItems[i] + " type 2: " + type);
        //    if (typeOfItems[i] == type)
        //    {
        
        for (int i = 0; i < itemsInJournal.Count; i++)
        {
            Debug.Log("Type 1: " + itemsInJournal[i].GetType() + " type 2: " + type);
            if(itemsInJournal[i].GetType() == type)
            {
                if (disabledContentItems.Count == 0)
                {
                    Debug.Log("Attempting to instantiate new content");
                    GameObject g = Instantiate(contentPrefab, contentPanel.transform);
                    g.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = itemsInJournal[i].ToString();
                    activeContentItems.Add(g);
                }
                else
                {
                    disabledContentItems[0].SetActive(true);
                    disabledContentItems[0].transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = itemsInJournal[i].ToString();
                    activeContentItems.Add(disabledContentItems[0]);
                    disabledContentItems.RemoveAt(0);
                }
            }
        }
        ArrangeContentItems();
    }

    void ArrangeContentItems()
    {
        float topPadding = 10.0f;

        heightOfContentPanel = activeContentItems.Count * (heightOfContent + topPadding);
        Debug.Log("active content items: " + activeContentItems.Count + " height: " + heightOfContentPanel);
        contentPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(contentPanel.GetComponent<RectTransform>().sizeDelta.x,
                                                                heightOfContentPanel);
        for (int i = 0; i < activeContentItems.Count; i++)
        {
            activeContentItems[i].transform.localPosition = new Vector3(0, -i * heightOfContent - topPadding, 0);
        }
    }

    void SortContent()
    {

    }

    public bool IsOpened()
    {
        return opened;
    }

    private void OnDisable()
    {
        for(int i = 0; i < activeContentItems.Count; i++)
        {
            activeContentItems[i].SetActive(false);
            disabledContentItems.Add(activeContentItems[i]);
            activeContentItems.RemoveAt(i);
        }
    }

    private void OnDestroy()
    {
        ItemBase.ItemPickedUpEvent -= ItemPickedUp;
    }

}
