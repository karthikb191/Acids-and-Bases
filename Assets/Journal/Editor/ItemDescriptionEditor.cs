using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemsDescription))]
public class ItemDescriptionEditor : PropertyDrawer{
    //Vector2 scroll;
    //ItemType type;
    bool show = true;
    bool showDescription = true;
    override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //ItemsDescription description = target as ItemsDescription;

        //description.hasPH = false;
        bool hasPH = property.FindPropertyRelative("hasPH").boolValue;
        //description.IsApHItem = false;
        bool isApHItem = property.FindPropertyRelative("IsApHItem").boolValue;

        //scroll = EditorGUILayout.BeginScrollView(scroll);
        string itemDescriptionText = property.FindPropertyRelative("itemDescription").stringValue;

        //Getting the reference to the properties....
        var itemDescriptionProperty = property.FindPropertyRelative("itemDescription");
        var hasPHProperty = property.FindPropertyRelative("hasPH");
        var IsApHItemProperty = property.FindPropertyRelative("IsApHItem");
        var pHValueProperty = property.FindPropertyRelative("pHValue");
        var itemTypeProperty = property.FindPropertyRelative("itemType");
        var extractionPossibleProperty = property.FindPropertyRelative("extractionPossible");
        var extractQuantityProperty = property.FindPropertyRelative("extractQuantity");

        var normalItemTypeProperty = property.FindPropertyRelative("normalItemType");
        var acidItemTypeProperty = property.FindPropertyRelative("acidType");
        var baseItemTypeProperty = property.FindPropertyRelative("baseType");
        var saltItemTypeProperty = property.FindPropertyRelative("saltType");
        var indicatorItemTypeProperty = property.FindPropertyRelative("indicatorType");


        Rect drawRect = new Rect(position.x, position.y, 300, 15);
        showDescription = EditorGUI.Foldout(drawRect, showDescription, "Show Description");
        if (showDescription)
        {
            UpdateRect(ref drawRect, 0, 10, 300, 150);
            //description.itemDescription = EditorGUILayout.TextArea(itemDescriptionText, GUILayout.Height(150));
            itemDescriptionProperty.stringValue = EditorGUI.TextArea(drawRect, itemDescriptionText);
        }

        UpdateRect(ref drawRect, 0, 10, 300, 15);
        //EditorGUILayout.EndScrollView();

        //description.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type: ", description.itemType);
        ItemType selectedItem = (ItemType)System.Enum.ToObject(typeof(ItemType), itemTypeProperty.enumValueIndex);
        itemTypeProperty.enumValueIndex = (int)(ItemList)EditorGUI.EnumPopup(drawRect, "Item Type: ", selectedItem);
        

        //description.itemType = type;
        //Debug.Log("selected item: " + selectedItem);
        switch (selectedItem)
        {
            case ItemType.Normal:
                UpdateRect(ref drawRect, 0, 10, 300, 15);
                NormalItemList normalItem = (NormalItemList)normalItemTypeProperty.enumValueIndex;
                //normalItemTypeProperty.enumValueIndex = (int)(NormalItemList)EditorGUI.EnumPopup(drawRect, "Normal Item Type: ", normalItem);
                normalItemTypeProperty.enumValueIndex = (int)(NormalItemList)EditorGUI.EnumPopup(drawRect, "Normal Item Type: ", normalItem);
                //Debug.Log("normal item: " + (int)normalItem);
                //description.normalItemType = (NormalItemList)EditorGUILayout.EnumPopup("Item:", description.normalItemType);
                break;

            case ItemType.Acid:
                UpdateRect(ref drawRect, 0, 10, 300, 15);
                AcidsList acidItem = (AcidsList)acidItemTypeProperty.enumValueIndex;
                acidItemTypeProperty.enumValueIndex = (int)(AcidsList)EditorGUI.EnumPopup(drawRect, "Acid Type: ", acidItem);
                //description.acidType = (AcidsList)EditorGUILayout.EnumPopup("Item:", description.acidType);
                break;

            case ItemType.Base:
                UpdateRect(ref drawRect, 0, 10, 300, 15);
                BasesList baseItem = (BasesList)baseItemTypeProperty.enumValueIndex;
                baseItemTypeProperty.enumValueIndex = (int)(BasesList)EditorGUI.EnumPopup(drawRect, "Base Type: ", baseItem);
                //description.baseType = (BasesList)EditorGUILayout.EnumPopup("Item:", description.baseType);
                break;

            case ItemType.Salt:
                UpdateRect(ref drawRect, 0, 10, 300, 15);
                SaltsList saltItem = (SaltsList)saltItemTypeProperty.enumValueIndex;
                saltItemTypeProperty.enumValueIndex = (int)(SaltsList)EditorGUI.EnumPopup(drawRect, "Salt Type: ", saltItem);
                //description.saltType = (SaltsList)EditorGUILayout.EnumPopup("Item:", description.saltType);
                break;

            case ItemType.Indicator:
                UpdateRect(ref drawRect, 0, 10, 300, 15);
                IndicatorsList indicatorItem = (IndicatorsList)indicatorItemTypeProperty.enumValueIndex;
                indicatorItemTypeProperty.enumValueIndex = (int)(IndicatorsList)EditorGUI.EnumPopup(drawRect, "Indicator Type: ", indicatorItem);
                //description.indicatorType = (IndicatorsList)EditorGUILayout.EnumPopup("Item:", description.indicatorType);
                break;
        }

        if(selectedItem == ItemType.Indicator)
        {
            UpdateRect(ref drawRect, 0, 10, 15, 15);
            IsApHItemProperty.boolValue = EditorGUI.Toggle(drawRect, "Is A pH Item", true);
            if (isApHItem)
            {
                UpdateRect(ref drawRect, 0, 10, 300, 15);
                property.FindPropertyRelative("useCount").intValue = EditorGUI.IntSlider(drawRect, 
                                                                    property.FindPropertyRelative("useCount").intValue, 1, 20);
            }
            //EditorGUILayout.EndToggleGroup();
        }

        if(selectedItem == ItemType.Acid || selectedItem == ItemType.Base)
        {
            UpdateRect(ref drawRect, 0, 10, 15, 15);
            hasPHProperty.boolValue = EditorGUI.Toggle(drawRect, "Has pH", true);
            if (hasPH)
            {
                UpdateRect(ref drawRect, 0, 10, 300, 15);
                pHValueProperty.intValue = EditorGUI.IntSlider(drawRect, pHValueProperty.intValue, 0, 14);
            }
            //EditorGUILayout.EndToggleGroup();
        }

        UpdateRect(ref drawRect, 0, 10, 300, 15);
        //extractionPossibleProperty.boolValue = EditorGUI.Toggle(drawRect, "Extraction Quantity", extractionPossibleProperty.boolValue);
        show = GUI.Toggle(drawRect, show, "Extract Possible?");
        //show = EditorGUI.Toggle(drawRect, "Extraction Quantity", show);

        extractionPossibleProperty.boolValue = show;
        if (extractionPossibleProperty.boolValue)
        {
            UpdateRect(ref drawRect, 0, 10, 300, 15);
            extractQuantityProperty.intValue = EditorGUI.IntSlider(drawRect, extractQuantityProperty.intValue, 0, 50);
        }
        //Setting the properties
        property.serializedObject.ApplyModifiedProperties();

        //EditorUtility.SetDirty(target);
        EditorGUI.EndProperty();
    }
    

    void UpdateRect(ref Rect drawRect, int xPadding, int yPadding, int width, int height)
    {
        drawRect.y += drawRect.height + yPadding;
        drawRect.x += xPadding;
        drawRect.width = width;
        drawRect.height = height;
    }
}


//[CustomEditor(typeof(ItemsDescription))]
//public class ItemDescriptionEditor : Editor{
//    Vector2 scroll;
//    ItemType type;
//    override public void OnInspectorGUI()
//    {
//        ItemsDescription description = target as ItemsDescription;

//        description.hasPH = false;
//        description.IsApHItem = false;

//        //scroll = EditorGUILayout.BeginScrollView(scroll);
//        description.itemDescription = EditorGUILayout.TextArea(description.itemDescription, GUILayout.Height(150));
//        //EditorGUILayout.EndScrollView();

//        description.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type: ", description.itemType);
//        //description.itemType = type;

//        switch (description.itemType)
//        {
//            case ItemType.Normal:
//                description.normalItemType = (NormalItemList)EditorGUILayout.EnumPopup("Item:", description.normalItemType);
//                break;
//            case ItemType.Acid:
//                description.acidType = (AcidsList)EditorGUILayout.EnumPopup("Item:", description.acidType);
//                break;
//            case ItemType.Base:
//                description.baseType = (BasesList)EditorGUILayout.EnumPopup("Item:", description.baseType);
//                break;
//            case ItemType.Salt:
//                description.saltType = (SaltsList)EditorGUILayout.EnumPopup("Item:", description.saltType);
//                break;
//            case ItemType.Indicator:
//                description.indicatorType = (IndicatorsList)EditorGUILayout.EnumPopup("Item:", description.indicatorType);
//                break;

//        }
//        if(description.itemType == ItemType.Indicator)
//        {
//            description.IsApHItem = EditorGUILayout.BeginToggleGroup("Is A pH Item", true);
//            if(description.IsApHItem)
//                description.useCount = EditorGUILayout.IntSlider(description.useCount, 1, 20);
//            EditorGUILayout.EndToggleGroup();
//        }
//        if(description.itemType == ItemType.Acid || description.itemType == ItemType.Base)
//        {
//            description.hasPH = EditorGUILayout.BeginToggleGroup("Has pH", true);
//            if (description.hasPH)
//                description.pHValue = EditorGUILayout.IntSlider(description.pHValue, 0, 14);
//            EditorGUILayout.EndToggleGroup();
//        }

//        description.extractionPossible = EditorGUILayout.BeginToggleGroup("Extraction Quantity", description.extractionPossible);
//        if (description.extractionPossible)
//            description.extractQuantity = EditorGUILayout.IntSlider(description.extractQuantity, 1, 50);

//        EditorUtility.SetDirty(target);
//    }

//}
