using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCountSelection : MonoBehaviour {

    public static ItemCountSelection instance;

    public ItemBase item;

    Text count;

    Slider slider;

    public GameObject removeButton;
    public GameObject addButtton;

    PlayerInventory playerInventory;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }

        if(instance!= this)
        {
            Destroy(this);
        }

        slider = GetComponentInChildren<Slider>();

        count = transform.GetChild(1).GetComponentInChildren<Text>();

        removeButton = transform.GetChild(3).gameObject;

        addButtton = transform.GetChild(2).gameObject;

        playerInventory = FindObjectOfType<PlayerInventory>();

    }
    
    public void UpdateCount()
    {
        count.text =  slider.value.ToString();
    }

    public void AddItems()
    {
        item.AddItems(slider.value);
        Dectivate();
    }

    public void Activate(float maxValue)
    {
        slider.maxValue = maxValue;
        gameObject.GetComponent<Canvas>().enabled = true;
        removeButton.SetActive(false);
        addButtton.SetActive(true);
    }

    public void Dectivate()
    {     
        gameObject.GetComponent<Canvas>().enabled = false;
        slider.maxValue = 1;
        slider.value = 0;

        item = null;        
    }

    public void RemoveItemsActivate(float maxValue)
    {
        slider.maxValue = maxValue;
        gameObject.GetComponent<Canvas>().enabled = true;
        removeButton.SetActive(true);
        addButtton.SetActive(false);
        Debug.Log("Remove count called");
    }

    public void RemoveButtonIsPressed()
    {
        playerInventory.RemoveItems(slider.value);
        gameObject.GetComponent<Canvas>().enabled = false;
    }

}
