using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour {

    public static ItemSelection Instance;

    public GameObject imagePreafab;

    List<ItemsDescription> itemDescriptions = new List<ItemsDescription>();
    List<GameObject> activeContentItems = new List<GameObject>();
    GameObject itemDisplayPanel;

    float heightOfImage;
    float widthOfImage;
    float heightOfSelectionPanel = 0;
    float widthOfSelectionPanel = 0;

    List<int> selectedIndices = new List<int>();
    Button button;

    QuestionBox targetQuestionBox;
    PlayerInventory inventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(this);
        }

        widthOfImage = imagePreafab.GetComponent<RectTransform>().sizeDelta.y;
        heightOfImage = imagePreafab.GetComponent<RectTransform>().sizeDelta.y;

        itemDisplayPanel = transform.Find("ItemDisplayPanel").GetChild(0).gameObject;
        button = transform.Find("Button").GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(PassSelection);
    }

    void PassSelection()
    {
        for (int i = 0; i < selectedIndices.Count; i++)
        {
            if(inventory.slots[selectedIndices[i]].itemStored != null)
            {
                ItemsDescription des = inventory.slots[selectedIndices[i]].itemStored.GetComponent<ItemsDescription>();
                if(des != null)
                {
                    itemDescriptions.Add(des);
                }
            }
        }

        if (targetQuestionBox != null)
        {
            //Sets the selected items in the question box
            targetQuestionBox.SetSelectedItems(itemDescriptions);
        }

        ToggleActivation(inventory, targetQuestionBox);
    }

    private void Update()
    {
        CheckContentSelection();
    }

    bool opened = false;
    public void ToggleActivation(PlayerInventory inventory, QuestionBox target = null)
    {
        this.inventory = inventory;
        gameObject.SetActive(!gameObject.activeSelf);
        targetQuestionBox = target;
        if (gameObject.activeSelf)
        {
            //RefreshContentInActiveTab();
            opened = true;
            ActivateCanvas(inventory);
        }
        else
        {
            DeactivateCanvas();
            opened = false;
        }
    }

    public void ActivateCanvas(PlayerInventory inventory)
    {
        RectTransform displayPanelRect = itemDisplayPanel.GetComponent<RectTransform>();
        //TODO: animation must be done here
        //for(int i = 0; i < inventory.slots.Count; i++)
        //{
        //
        //}
        InstantiateImages(inventory);
        ArrangeContentItems();
    }
    
    List<GameObject> disabledImages = new List<GameObject>();

    void InstantiateImages(PlayerInventory inventory)
    {
        Debug.Log("inventory slots: " + inventory.activeSlotCount);
        for (int i = 0; i < inventory.activeSlotCount; i++)
        {
            if (disabledImages.Count <= 0)
            {
                GameObject g = Instantiate(imagePreafab, itemDisplayPanel.transform);
                g.GetComponent<Image>().sprite = inventory.slots[i].itemStored.GetComponent<SpriteRenderer>().sprite;
                activeContentItems.Add(g);
            }
            else
            {
                activeContentItems.Add(disabledImages[0]);
                disabledImages[0].GetComponent<Image>().sprite = inventory.slots[i].itemStored.GetComponent<SpriteRenderer>().sprite;
                disabledImages[0].SetActive(true);
                disabledImages.RemoveAt(0);
            }
        }
    }
    int rows = 0;
    int cols = 0;
    //Assuming 6 items per row
    void ArrangeContentItems()
    {
        int itemsPerRow = 4;
        cols = itemsPerRow;

        rows = Mathf.CeilToInt(activeContentItems.Count / (float)(itemsPerRow));
        Debug.Log("rows: " + rows);
        
        //float initialYValue = Mathf.FloorToInt(activeContentItems.Count / (itemsPerRow + 1)) * heightOfImage * 0.5f;  //This initial value sets the top position of the elements

        float topPadding = 10.0f;
        float leftPadding = 10.0f;

        float initialYValue = (rows - 1) * (heightOfImage + topPadding) * 0.5f;  //This initial value sets the top position of the elements
        float initialXValue = -(cols - 1) * (widthOfImage + leftPadding) * 0.5f;  

        Debug.Log("initial y value: " + initialYValue);
        Debug.Log("initial x value: " + initialXValue);


        float vertDisplacement = Mathf.FloorToInt(activeContentItems.Count / itemsPerRow);

        //Setting the width and height of the parent panel
        //heightOfSelectionPanel = heightOfImage + Mathf.FloorToInt(activeContentItems.Count / (itemsPerRow + 1)) * (heightOfImage + topPadding);
        //heightOfSelectionPanel = heightOfImage + rows * (heightOfImage + topPadding);
        heightOfSelectionPanel = rows * (heightOfImage + topPadding);
        widthOfSelectionPanel = itemsPerRow * (widthOfImage + leftPadding);
        //Debug.Log("active content items: " + activeContentItems.Count + " height: " + heightOfContentPanel);
        Debug.Log("Arranging items");
        itemDisplayPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfSelectionPanel, heightOfSelectionPanel);

        //Arranging the items
        if(activeContentItems.Count > 0)
        {
            for (int i = 0; i < activeContentItems.Count; i++)
            {
                //int mult = 1;
                //if ((i % itemsPerRow) == 0)
                //    mult = 0;
                Debug.Log("i mod rows: " + i % itemsPerRow);
                //activeContentItems[i].transform.localPosition = new Vector3(((i % itemsPerRow) - itemsPerRow * 0.5f) * widthOfImage + leftPadding * (i % itemsPerRow),
                //                                                (initialYValue - Mathf.FloorToInt(i / itemsPerRow) * heightOfImage) - topPadding * (i / itemsPerRow), 0);

                activeContentItems[i].transform.localPosition = new Vector3(initialXValue + Mathf.FloorToInt(i % itemsPerRow) * widthOfImage + leftPadding * (i % itemsPerRow),
                                                                (initialYValue - Mathf.FloorToInt(i / itemsPerRow) * heightOfImage) - topPadding * Mathf.FloorToInt(i / itemsPerRow), 0); //* (i / itemsPerRow), 0);
            }
        }
    }


    void CheckContentSelection()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RectTransform contentPanelRect = itemDisplayPanel.GetComponent<RectTransform>();
            Vector2 localPoint;

            localPoint = contentPanelRect.InverseTransformPoint(Input.mousePosition);

            //Check if the input is inside the selection panel
            if (contentPanelRect.rect.Contains(localPoint))
            {
                //touch / mouse click happened in the contents panel.
                //y coordinate of local point is important for the selection
                
                localPoint.x = (localPoint.x / contentPanelRect.sizeDelta.x) + 0.5f;
                localPoint.y = -((localPoint.y / contentPanelRect.sizeDelta.y) - 0.5f);

                
                Debug.Log("local point:  " + localPoint);

                if (activeContentItems.Count > 0)
                {
                    float partition = (float)1 / activeContentItems.Count;
                    int indexnumber = Mathf.FloorToInt(localPoint.x / partition);

                    int x = Mathf.FloorToInt(localPoint.x * cols);
                    int y = Mathf.FloorToInt(localPoint.y * rows);

                    indexnumber = y * cols + x;


                    Debug.Log("index number is: " + indexnumber);

                    //check the index number. If it is already present, remove it
                    if (selectedIndices.Contains(indexnumber))
                    {
                        activeContentItems[indexnumber].GetComponent<Image>().color = Color.white;
                        selectedIndices.Remove(indexnumber);
                    }
                    else
                    {
                        activeContentItems[indexnumber].GetComponent<Image>().color = Color.green;
                        selectedIndices.Add(indexnumber);
                        Debug.Log("selected : " + inventory.slots[indexnumber].itemStored.GetComponent<ItemsDescription>().GetItemType());
                    }

                    //Debug.Log("partition: " + partition);
                    //Debug.Log("Index number is: " + indexnumber);
                    //select the content item of specified index
                    //if (activeContentItems[indexnumber] != activeContentItem)
                    //{
                    //    if (activeContentItem != null)
                    //        activeContentItem.GetComponent<Image>().color = Color.grey;
                    //
                    //    activeContentItem = activeContentItems[indexnumber];
                    //    activeContentItemJournalIndex = indexLinksToAllItemsInJournal[indexnumber];
                    //    activeContentItem.GetComponent<Image>().color = Color.green;
                    //    UpdateDescription();
                    //}
                }
            }
        }
    }

    public void DeactivateCanvas()
    {
        while(selectedIndices.Count > 0)
        {
            activeContentItems[selectedIndices[0]].GetComponent<Image>().color = Color.white;
            selectedIndices.RemoveAt(0);
        }
        itemDescriptions.Clear();
        //TODO: Flush out all the items
        while (activeContentItems.Count > 0)
        { 
            activeContentItems[0].SetActive(false);
            disabledImages.Add(activeContentItems[0]);
            activeContentItems.RemoveAt(0);
        }
    }

}
