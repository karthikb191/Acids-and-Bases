using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : ItemBase {

   // SpriteRenderer spriteRenderer;


    private void Awake()
    {
     //   spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

       // spriteRenderer.sprite = itemProperties.imageSprite;
    }



    public override void Use(Character c)
    {


      
        if(this.itemProperties.isConsumable)
        {
            Debug.Log("Healer called");
            c.Heal(itemProperties.HealthRestore);
            gameObject.SetActive(false);
        }
    }


    
}
