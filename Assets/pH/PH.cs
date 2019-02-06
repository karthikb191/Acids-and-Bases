using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PH : ItemBase {

    public Sprite PHIndicatorImage { get; set; }

    public IndicatorsList indicator;

    [SerializeField]
    private int useCount = 3;
    
    // Use this for initialization
    void Start () {
        PHIndicatorImage = GetComponent<SpriteRenderer>().sprite;
	}
	public int GetUseCount()
    {
        return useCount;
    }
    
}
