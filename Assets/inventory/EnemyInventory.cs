using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : Inventory {

    public GameObject itemPrefab;
    public int maxItem;

    public float reloadTime;

    public float speedOfThrow;
    
    ItemBase ItemStored;

    ItemBase temp;

    //TODO: Remove this after this is integrated with alignpos
    Vector3 tempLocalScale = new Vector3(0.1f, 0.1f, 0.1f);

    // Use this for initialization
    void Start () {
        
        CreateSlot();
        for(int i = 0; i<slots.Count;i++)
        {
            slots[i].maxStorage = 10;
        }

        //Instantiating items
        for(int i = 0;i<maxItem;i++)
        {
            GameObject temp = Instantiate(itemPrefab, GetComponentInParent<Character>().Hand.transform.position, Quaternion.identity) as GameObject;
            ItemStored = temp.GetComponent<ItemBase>();
            temp.transform.parent = GetComponentInParent<Character>().Hand.transform;
            temp.transform.localScale = tempLocalScale;
            temp.gameObject.SetActive(false);
            ItemStored.GetComponent<ItemBase>().isFromEnemy = true;
           // ItemStored.GetComponent<ItemBase>().playerObject = GetComponentInParent<GameObject>();
            AddItem(ItemStored);
        }
        SetActiveItem();   
    }
	

   public override void ThrowItem(Vector3 target, Character c)
    {

        base.ThrowItem(target, c);
        
        activeItem = null;

        Invoke("Reload",reloadTime);

    }

    void Reload()
    {
        SetActiveItem();
    }

    void SetActiveItem()
    {
        for (int i = 0; i<slots.Count; i++)
        {
           /* for (int j = 0; j < slots[i].itemlist.Count; j++)
            {
                if (slots[i].itemlist[j].gameObject !=null && !slots[i].itemlist[j].gameObject.activeSelf)
                {
                    activeItem = slots[i].itemlist[j];
                    activeItem.isFromEnemy = true;
                    activeItem.AlignPos(this.transform.position, GetComponentInParent<Enemy>());
                    activeItem.gameObject.SetActive(true);
                    break;

                }
            }*/
            if (slots[i].itemStored != null && slots[i].itemStored.itemProperties == ItemStored.itemProperties)
            {
               
                slots[i].itemlist[slots[i].itemlist.Count - 1].gameObject.SetActive(true);
                activeItem = slots[i].itemlist[slots[i].itemlist.Count - 1];
                activeItem.isFromEnemy = true;
                activeItem.gameObject.SetActive(true);
                break;
            }
        }

        GameObject temp = Instantiate(itemPrefab, GetComponentInParent<Character>().Hand.transform.position, Quaternion.identity) as GameObject;
        ItemStored = temp.GetComponent<ItemBase>();
        temp.transform.parent = GetComponentInParent<Character>().Hand.transform;
        temp.transform.localScale = tempLocalScale;
        AddItem(ItemStored);
       
    }
}
