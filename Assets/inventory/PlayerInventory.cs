using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory
{

    [SerializeField]
    Canvas inventoryUI;
    [SerializeField]
    RectTransform panel;    //Panel within which the slots are stored   
    Vector3 targerSetTest;


    void CreateSlots()
    {
        CreateSlot();

        for (int i = 0; i < slots.Count; i++)
        {
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
        CreateSlots();
    }

    // Update is called once per frame
    void Update()
    {
        SelectFromSlots();
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
            ///////////////////////////////////////////////////////////////////Target Setting testing//////////////////////////////////////////////////
            else
            {
                targerSetTest = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targerSetTest.z = 0;
                Debug.Log("Target set" + "_____" + targerSetTest);
            }
        }
        if (activeItem != null && Input.GetKeyDown(KeyCode.D))
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
            ThrowItem(target, this.gameObject.GetComponent<Character>());
        }
    }
}

