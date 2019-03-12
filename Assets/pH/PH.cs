using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PH : ItemBase {

    public Sprite PHIndicatorImage { get; set; }

    [HideInInspector]
    public IndicatorsList indicator;

    [SerializeField]
    private int useCount = 3;
    
    // Use this for initialization
    void Start () {
        PHIndicatorImage = GetComponent<SpriteRenderer>().sprite;
        indicator = (IndicatorsList)System.Enum.Parse(typeof(IndicatorsList), GetComponent<ItemsDescription>().GetItemType().ToString());
	}

	public int GetUseCount()
    {
        return useCount;
    }
    
}
