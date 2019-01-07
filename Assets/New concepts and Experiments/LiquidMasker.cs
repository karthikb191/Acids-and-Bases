using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidMasker : MonoBehaviour {
    SpriteRenderer bodyRenderer;
    Player player;

    [SerializeField]
    public Sprite liquidTextures;

    public int xyz;
    Vector2[] uvs;
	// Use this for initialization
	void Start () {
        bodyRenderer = GetComponent<SpriteRenderer>();
        
        Debug.Log(bodyRenderer.sprite.rect);
	}
	
	// Update is called once per frame
	void Update () {
        //bodyRenderer.material.mainTexture = liquidTextures;
    }
}
