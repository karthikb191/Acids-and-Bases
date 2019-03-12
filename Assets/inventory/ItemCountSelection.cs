using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCountSelection : MonoBehaviour {

    public static ItemCountSelection instance;

    public ItemBase item;

    Text count;

    Slider slider;


    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        slider = GetComponentInChildren<Slider>();

        count = transform.GetChild(1).GetComponentInChildren<Text>();
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
    }

    public void Dectivate()
    {
       
        gameObject.GetComponent<Canvas>().enabled = false;
        slider.value = 0;
        item = null;
        
    }

}
