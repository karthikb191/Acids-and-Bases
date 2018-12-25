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

        public bool isAssignedToSlot;

        public bool isActiveItem;

        public bool isConsumable;

        public int slotIn;

        [Range(1, 5)]
        public float itemLifeTime;

        [Range(1, 10)]
        public float damageDealt;

        [Range(1, 10)]
        public float HealthRestore;

        public AudioClip itemSound;

        public bool isThrowable;


    
}
