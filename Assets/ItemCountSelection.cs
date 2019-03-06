using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCountSelection : MonoBehaviour {

    public static ItemCountSelection instance;

    public ItemBase item;

    Text count;

    Slider slider;

    Button addButton;

 /*   private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        slider = GetComponentInChildren<Slider>();
        addButton = GetComponentInChildren<Button>();
        count = transform.GetChild(1).GetComponentInChildren<Text>();
    }

    public void UpdateCount()
    {
        count.text =  slider.value.ToString();
    }

    public void AddItems()
    {
        Debug.Log("Slider value:: <><>" + slider.value);
        item.AddItems(slider.value);
    }

    public void Activate(float maxValue)
    {
        slider.maxValue = maxValue;
        gameObject.GetComponent<Canvas>().enabled = true;
    }
    */
}
