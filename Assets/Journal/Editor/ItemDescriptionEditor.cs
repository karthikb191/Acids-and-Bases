using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemsDescription))]
public class ItemDescriptionEditor : Editor{
    Vector2 scroll;
    ItemType type;
    override public void OnInspectorGUI()
    {
        ItemsDescription description = target as ItemsDescription;

        //scroll = EditorGUILayout.BeginScrollView(scroll);
        description.itemDescription = EditorGUILayout.TextArea(description.itemDescription, GUILayout.Height(150));
        //EditorGUILayout.EndScrollView();

        description.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type: ", description.itemType);
        //description.itemType = type;

        switch (description.itemType)
        {
            case ItemType.Normal:
                description.normalItemType = (NormalItemList)EditorGUILayout.EnumPopup("Item:", description.normalItemType);
                break;
            case ItemType.Acid:
                description.acidType = (AcidsList)EditorGUILayout.EnumPopup("Item:", description.acidType);
                break;
            case ItemType.Base:
                description.baseType = (BasesList)EditorGUILayout.EnumPopup("Item:", description.baseType);
                break;
            case ItemType.Salt:
                description.saltType = (SaltsList)EditorGUILayout.EnumPopup("Item:", description.saltType);
                break;
            case ItemType.Indicator:
                description.indicatorType = (IndicatorsList)EditorGUILayout.EnumPopup("Item:", description.indicatorType);
                break;

        }

        description.hasPH = EditorGUILayout.BeginToggleGroup("Has pH", description.hasPH);
        if (description.hasPH)
            description.pHValue = EditorGUILayout.IntSlider(description.pHValue, 0, 14);
        EditorGUILayout.EndToggleGroup();

        description.extractionPossible = EditorGUILayout.BeginToggleGroup("Extraction Quantity", description.extractionPossible);
        if (description.extractionPossible)
            description.extractQuantity = EditorGUILayout.IntSlider(description.extractQuantity, 1, 50);

        EditorUtility.SetDirty(target);
    }
	
}
