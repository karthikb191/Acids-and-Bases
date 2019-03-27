using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public ItemListScriptableObject itemListScriptableObject;
    public Dictionary<System.Object, GameObject> itemDictionary = new Dictionary<System.Object, GameObject>();

    private void Awake()
    {
        instance = instance ?? this;

        if (instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        //get all the information from the scriptable object and make a dictionary
        AddToDictionary(itemListScriptableObject.acidList, typeof(AcidsList));
        AddToDictionary(itemListScriptableObject.baseList, typeof(BasesList));
        AddToDictionary(itemListScriptableObject.saltList, typeof(SaltsList));
        AddToDictionary(itemListScriptableObject.indicatorList, typeof(IndicatorsList));
        AddToDictionary(itemListScriptableObject.normalItemList, typeof(NormalItemList));

        Debug.Log("Getting bromythol blue: " + itemDictionary[IndicatorsList.Bromythol_Blue].ToString());

        Debug.Log("salt:  " + itemDictionary[SaltsList.NaCl]);
        
        //Debug.Log("Testing simple reaction: " + Reactions.React(IndicatorsList.Bromythol_Blue, NormalItemList.Hybiscus).ToString());
    }

    void AddToDictionary(ItemTemplate[] list, System.Type type)
    {
        Debug.Log("Adding: " + list.Length);
        for (int i = 0; i < list.Length; i++)
        {
            if(list[i] != null)
                if(list[i].itemPrefab != null)
                {
                    //Convert the string to enum
                    object o = System.Enum.Parse(type, list[i].enumObject);
                    Debug.Log("adding: " + o.ToString() + "   " + list[i].itemPrefab.name);
                    Debug.Log("type: " + o.GetType());

                    //Instantiate the items for reference
                    GameObject g = Instantiate(list[i].itemPrefab, this.transform);
                    //Deactivate the gameObject
                    g.SetActive(false);

                    itemDictionary.Add(o, g);
                }
            
        }
    }

}
