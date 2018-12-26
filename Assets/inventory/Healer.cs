using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : ItemBase {


    public override void Use(Character c)
    {


        Debug.Log("Healer used");
        if(this.itemProperties.isConsumable)
        {
            c.Heal(itemProperties.HealthRestore);
            gameObject.SetActive(false);
        }
    }


    
}
