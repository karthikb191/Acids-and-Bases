﻿using System.Collections;
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
        itemPanel = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        selectedItemPanel = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        SelectItems();
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


    void InitializeSlots(int slotCount)
    {
        
        if(slotsList.Count < slotCount)
        {
            for (int i = slotsList.Count; i < slotCount; i++)
            {
                GameObject temp = Instantiate(slotPrefab, itemPanel.transform);
                temp.GetComponent<SpriteRenderer>().enabled = false;
                temp.GetComponentInChildren<Image>().enabled = false;
                temp.GetComponentInChildren<Text>().enabled = false;
                slotsList.Add(temp);
            }
        }
        
    }

    public void ActivateDisplay(int slotsCount)
    {
        gameObject.SetActive(true);
        InitializeSlots(slotsCount);
    }

    public void AddSelected()
    {
        for (int i = 0; i < index.Count; i++)
        {
            treasureChest.itemBase[index[i]].playerObject = treasureChest.playerObj;
            treasureChest.itemBase[index[i]].AddItem(treasureChest.itemBase[index[i]]);

            slotsList[index[i]].gameObject.SetActive(false);
        }
        treasureChest.ItemsPresentUpdate(index);
        index.Clear();
    }

    public void AddAll()
    {
        for (int i = 0; i < treasureChest.itemsInChest.Count; i++)
        {
            treasureChest.itemBase[i].playerObject = treasureChest.playerObj;
            treasureChest.itemBase[i].AddItem(treasureChest.itemBase[index[i]]);
        }
        HideChest();        
    }

    public void HideChest()
    {
        this.gameObject.SetActive(false);
    }

}
