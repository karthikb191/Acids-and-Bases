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

        scroll = EditorGUILayout.BeginScrollView(scroll);
        description.itemDescription = EditorGUILayout.TextArea("", GUILayout.Height(300));
        EditorGUILayout.EndScrollView();

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
    }
	
}
