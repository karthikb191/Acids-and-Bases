using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public ItemListScriptableObject itemListScriptableObject;
    
    public Dictionary<System.Object, ItemBase> itemDictionary = new Dictionary<System.Object, ItemBase>();
    public Dictionary<System.Object, ItemProperties> itempropertiesDictionary = new Dictionary<System.Object, ItemProperties>();

    private void Awake()
    {
        instance = instance ?? this;

        if (instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        //get all the information from the scriptable object and make a dictionary
        AddPropertiesToDictionary(itemListScriptableObject.acidList, typeof(AcidsList));
        AddPropertiesToDictionary(itemListScriptableObject.baseList, typeof(BasesList));
        AddPropertiesToDictionary(itemListScriptableObject.saltList, typeof(SaltsList));
        AddPropertiesToDictionary(itemListScriptableObject.indicatorList, typeof(IndicatorsList));
        AddPropertiesToDictionary(itemListScriptableObject.normalItemList, typeof(NormalItemList));

        Debug.Log("Getting ph paper: " + itempropertiesDictionary[IndicatorsList.pH_Paper].ToString());

        //Debug.Log("salt:  " + itemDictionary[SaltsList.NaCl]);
        
        //Debug.Log("Testing simple reaction: " + Reactions.React(IndicatorsList.Bromythol_Blue, NormalItemList.Hybiscus).ToString());
    }

    void AddPropertiesToDictionary(ItemTemplate[] list, System.Type type)
    {
        Debug.Log("Adding: " + list.Length);
        for (int i = 0; i < list.Length; i++)
        {
            if(list[i] != null)
                if(list[i].itemProperty != null)
                {
                    //Convert the string to enum
                    object o = System.Enum.Parse(type, list[i].enumObject);
                    Debug.Log("adding: " + o.ToString() + "   " + list[i].itemProperty.name);
                    Debug.Log("type: " + o.GetType());

                    //Instantiate the items for reference
                    //GameObject g = Instantiate(list[i].itemPrefab, this.transform);
                    //GameObject g = Instantiate(list[i].itemPrefab, this.transform);
                    //Deactivate the gameObject
                    //g.SetActive(false);

                    itempropertiesDictionary.Add(o, list[i].itemProperty);
                }
        }
    }

    /// <summary>
    /// Returns the reference item of a certain type.
    /// If the item is not initially present in the dictionary, it creates a new item and adds it to the dictionary
    /// </summary>
    /// <returns>a new reference item that shouldn't be set active or used. Used as a template to create further instances</returns>
    public ItemBase GetItemReference(System.Object itemType)
    {
        ItemBase item;
        if (itemDictionary.ContainsKey(itemType))
        {
            item = itemDictionary[itemType];
        }
        else
        {
            //Creating a new item reference, adding the properties to it and storing it in the dictionary for future access
            item = GenerateItemTemplate();
            item.itemProperties = itempropertiesDictionary[itemType];
            item.gameObject.SetActive(false);

            itemDictionary.Add(itemType, item);
        }

        return item;
    }

    public ItemProperties GetItemProperties(System.Object itemType)
    {
        if (itempropertiesDictionary.ContainsKey(itemType))
        {
            return itempropertiesDictionary[itemType];
        }
        return null;
    }

    public static ItemBase GenerateItemTemplate()
    {
        Debug.Log("Generating item template.....");

        GameObject g = new GameObject();
        g.AddComponent<SpriteRenderer>();
        g.AddComponent<ItemBase>();
        return g.GetComponent<ItemBase>();
    }

}
