
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Treasurechest : MonoBehaviour {

    public List<GameObject> itemsInChest = new List<GameObject>();
    public List<ItemBase> itemBase = new List<ItemBase>();

    public bool chestOpened = false;

    public GameObject slotPrefab;

    public List<GameObject> slotsList = new List<GameObject>();

    public List<int> index = new List<int>();
    public RectTransform itemPanel;

    public RectTransform viewPort;

    public RectTransform selectedItemPanel;

    public GameObject playerObj;

	//Use this for initialization
	void Start ()
    {
		if(playerObj == null)
        {
            playerObj = FindObjectOfType<Player>().gameObject;
        }
        InitializeItems();
        InitializeSlots();
    }

    //Update is called once per frame
    void Update ()
    {
        SelectItems();
	}

    public void SelectItems()
    {
        if(Input.GetMouseButtonDown(0))
        {                     
            if(itemPanel != null && RectTransformUtility.RectangleContainsScreenPoint(itemPanel, Input.mousePosition))
            {             
                for (int i = 0; i < slotsList.Count; i++)
                {
                    if(RectTransformUtility.RectangleContainsScreenPoint(slotsList[i].transform.GetComponent<RectTransform>(), Input.mousePosition))
                    {
                        slotsList[i].transform.SetParent(selectedItemPanel);
                        index.Add(i);                      
                        continue;
                    }
                }
            }
        }
    }

    public void InitializeSlots()
    {
        foreach(ItemBase i in itemBase)
        {
            GameObject temp =  Instantiate(slotPrefab, itemPanel.transform);

            Debug.Log("<><><><><><><><><><><><>" + i + "_________<><><><>" + i.itemProperties.imageSprite + "<<<<<<<<-------Image sprite");
           
            temp.gameObject.transform.GetChild(0).transform.GetComponent<Image>().sprite = i.itemProperties.imageSprite;
            Debug.Log("<><><><><><><><><><><><>" + slotPrefab.GetComponentInChildren<Image>().sprite + "<<<<<<<<-------slot sprite");
            slotsList.Add(temp);           
        }
    }

    public void AddSelected()
    {
        for(int i = 0;i<index.Count;i++)
        {
            itemBase[index[i]].playerObject = playerObj;
            itemBase[index[i]].AddItem();         
        }
        index.Clear();
    }

    public void AddAll()
    {
        for (int i = 0; i < itemsInChest.Count; i++)
        {
            itemBase[i].playerObject = playerObj;
            itemBase[i].AddItem();
        }
        HideChest();
        index.Clear();
        itemBase.Clear();
    }

    public void HideChest()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>())
        {
            playerObj = collision.gameObject;
        }
    }

    public void InitializeItems()
    {
        foreach(GameObject i in itemsInChest)
        {
            GameObject temp = Instantiate(i, this.transform);
            temp.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            itemBase.Add(temp.GetComponent<ItemBase>());
        }
    }
}
