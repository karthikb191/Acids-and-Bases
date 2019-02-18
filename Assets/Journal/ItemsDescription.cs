using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Descriptions
{
    //Populate the descriptions here
    public static string GetDescription(AcidsList acid, ItemsDescription des)
    {
        string result = "";
        switch (acid)
        {
            case AcidsList.HCl:
                break;
        }
        return result;
    }

}

public class ItemsDescription : MonoBehaviour{
    //TODO: Consider moving the item description to a separate class
    public string itemDescription;

    public string shortName;    //For displaying in the panel. If not specified, use the same name as enum
    public string fullName;     //For displaying in the description
    public string tags;         //For searching

    public ItemType itemType;
    public IndicatorsList indicatorType;
    public AcidsList acidType;
    public BasesList baseType;
    public SaltsList saltType;
    public NormalItemList normalItemType;

    public bool hasPH = false;
    public int pHValue = 0;

    public bool extractionPossible = false;
    public int extractQuantity = 10;
    
    public System.Enum GetItemType()
    {
        switch (itemType)
        {
            case ItemType.Acid:
                return acidType;
            case ItemType.Base:
                return baseType;
            case ItemType.Salt:
                return saltType;
            case ItemType.Indicator:
                return indicatorType;
            case ItemType.Normal:
                return normalItemType;
        }
        return null;
    }
}
