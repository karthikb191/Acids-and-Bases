using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : ItemBase
{
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemProperties.imageSprite;
    }
    public override void Use()
    {
        base.Use();
        //Rock Used
        Debug.Log("Item used:" + itemProperties.name);
    }
    public override void Use(Character c)
    {
        base.Use(c);
      //  c.Heal(itemProperties.HealthRestore);
      //  c.TakeDamage(itemProperties.damageDealt);
    }
}
