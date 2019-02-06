using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsDescription : MonoBehaviour{
    //TODO: Consider moving the item description to a separate class
    [TextArea]
    public string itemDescription;

    public ItemType itemType;
    public IndicatorsList indicatorType;
    public AcidsList acidType;
    public BasesList baseType;
    public SaltsList saltType;
    public NormalItemList normalItemType;
    
}
