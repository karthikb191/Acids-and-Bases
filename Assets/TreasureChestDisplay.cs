using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChestDisplay : MonoBehaviour {

    public static TreasureChestDisplay instance;

    public Treasurechest treasureChest;

    public RectTransform itemPanel;
    public RectTransform selectedItemPanel;

    public GameObject slotPrefab;

    public List<GameObject> slotsList = new List<GameObject>();
    public List<int> index = new List<int>();
    public int maxSlotCount;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void SelectItems()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (itemPanel != null && RectTransformUtility.RectangleContainsScreenPoint(itemPanel, mousePos))
            {
                for (int i = 0; i < slotsList.Count; i++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(slotsList[i].GetComponent<RectTransform>(), mousePos))
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
        /*foreach (ItemBase i in treasureChest.itemsInChest)
        {
            Instantiate(slotPrefab, itemPanel.transform);
            slotPrefab.GetComponentInChildren<Image>().sprite = i.itemProperties.imageSprite;
        }*/

        for (int i = 0; i < maxSlotCount; i++)
        {
            Instantiate(slotPrefab, itemPanel.transform);
            slotPrefab.GetComponent<SpriteRenderer>().enabled = false;
            slotPrefab.GetComponentInChildren<Image>().enabled = false;
            slotPrefab.GetComponentInChildren<Text>().enabled = false;
        }
    }
    public void ActivateDisplay()
    {

    }
    public void AddSelected()
    {
        for (int i = 0; i < index.Count; i++)
        {
            treasureChest.itemBase[index[i]].playerObject = treasureChest.playerObj;
            treasureChest.itemBase[index[i]].AddItem();
        }
    }

    public void AddAll()
    {
        for (int i = 0; i < treasureChest.itemsInChest.Count; i++)
        {
            treasureChest.itemBase[i].playerObject = treasureChest.playerObj;
            treasureChest.itemBase[i].AddItem();
        }
        HideChest();

    }

    public void HideChest()
    {
        this.gameObject.SetActive(false);
    }
}
