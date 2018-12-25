using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : ItemBase {
    
    
    public override void Use()
    {
        base.Use();
        //Rock Used

        Debug.Log("Item used:" + itemProperties.name);
    }
}
