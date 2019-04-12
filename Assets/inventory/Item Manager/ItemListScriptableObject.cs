using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ItemTemplate
{
    [SerializeField]
    public string enumObject;
    //public System.Object enumObject;
    [SerializeField]
    public ItemProperties itemProperty;
    public System.Type type;
}

[CreateAssetMenu(fileName = "Item List", menuName = "Items/Create Item List", order = 1)]
[System.Serializable]
public class ItemListScriptableObject : ScriptableObject {

    public System.Object[] acidObjects;
    public System.Object[] baseObjects;
    public System.Object[] saltObjects;
    public System.Object[] indicatorObjects;
    public System.Object[] normalItemObjects;


    public ItemTemplate[] acidList = new ItemTemplate[0];
    public ItemTemplate[] baseList = new ItemTemplate[0];
    public ItemTemplate[] saltList = new ItemTemplate[0];
    public ItemTemplate[] indicatorList = new ItemTemplate[0];
    public ItemTemplate[] normalItemList = new ItemTemplate[0];


    private void OnEnable()
    {
        System.Array acidArray = System.Enum.GetValues(typeof(AcidsList));
        acidObjects = new System.Object[acidArray.Length];
        System.Array.Copy(acidArray, acidObjects, acidArray.Length);

        System.Array baseArray = System.Enum.GetValues(typeof(BasesList));
        baseObjects = new System.Object[baseArray.Length];
        System.Array.Copy(baseArray, baseObjects, baseArray.Length);

        System.Array saltArray = System.Enum.GetValues(typeof(SaltsList));
        saltObjects = new System.Object[saltArray.Length];
        System.Array.Copy(saltArray, saltObjects, saltArray.Length);

        System.Array indicatorArray = System.Enum.GetValues(typeof(IndicatorsList));
        indicatorObjects = new System.Object[indicatorArray.Length];
        System.Array.Copy(indicatorArray, indicatorObjects, indicatorArray.Length);

        System.Array normalItemArray = System.Enum.GetValues(typeof(NormalItemList));
        normalItemObjects = new System.Object[normalItemArray.Length];
        System.Array.Copy(normalItemArray, normalItemObjects, normalItemObjects.Length);

    }
}

