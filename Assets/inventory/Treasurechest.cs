
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Treasurechest : MonoBehaviour {

    public List<GameObject> itemsInChest = new List<GameObject>();

    public List<ItemBase> itemBase = new List<ItemBase>();

    public List<ItemBase> itemsAfterPickUp = new List<ItemBase>();

    public bool chestOpened = false;


    public GameObject playerObj;

	//Use this for initialization
	void Start ()
    {
		if(playerObj == null)
        {
            playerObj = FindObjectOfType<Player>().gameObject;
        }

        InitializeItems();      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>())
        {
            playerObj = collision.gameObject;
            TreasureChestDisplay.instance.ActivateDisplay(itemBase.Count);
        }
    }

    public void InitializeItems()
    {
        foreach(GameObject i in itemsInChest)
        {
            GameObject temp = Instantiate(i, this.transform);
            temp.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            itemBase.Add(temp.GetComponent<ItemBase>());
        }
    }

    public void ItemsPresentUpdate(List<int> index)
    {
        for(int i = 0; i< itemBase.Count;i++)
        {
            for(int j = 0; j<index.Count;j++)
            {
                if(index[j] == i)
                {
                    Debug.Log("Item already added to player inventory");
                }

                else
                {
                    itemsAfterPickUp.Add(itemBase[i]);
                    itemBase.Remove(itemBase[i]);
                    itemsInChest.Remove(itemsInChest[i]);
                    break;
                }
            }
        }
    }
}
