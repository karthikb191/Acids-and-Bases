using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class Light : MonoBehaviour {
    Material lightMaterial;
    public float multiplier;

    MaterialPropertyBlock propertyBlock;
    SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
        
        propertyBlock = new MaterialPropertyBlock();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lightMaterial = spriteRenderer.sharedMaterial;
        
	}
	
	// Update is called once per frame
	void Update () {
        
        spriteRenderer.GetPropertyBlock(propertyBlock);
        if(propertyBlock != null)
        {
            propertyBlock.SetFloat("_Multiplier", multiplier);
            //lightMaterial.SetFloat("_Multiplier", multiplier);
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
        
	}
}
