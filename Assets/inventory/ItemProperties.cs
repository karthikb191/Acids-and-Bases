using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "Inventory/Item")]


public class ItemProperties : ScriptableObject {



        public new string name;

        public int maxHoldingCapacity;

        public Sprite imageSprite;

        public bool isEquipable;

        public bool isAnInventoryItem;

        public bool isConsumable;

        [Range(1, 5)]
        public float itemLifeTime;

        [Range(1, 10)]
        public float damageDealt;

        [Range(1, 10)]
        public float HealthRestore;

        public AudioClip itemSound;

        public bool isThrowable;

   

    public ItemType itemtype;

    public ItemList itemList;
    
    
}
