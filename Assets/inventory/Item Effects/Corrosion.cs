using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Corrosion : MonoBehaviour {

    public float maxCorrosionResistance = 100;
    public float targetResistance = 100;
    float currentResistance = 100;

    public bool breakApart = false;

    Animator corrosionAnimator;
    SpriteRenderer platformRenderer;

    MaterialPropertyBlock propertyBlock;
    

    public float _MaxOverlayMultiplier = 3.5f;
    public float _OverlayMultiplier = 0f;

    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        corrosionAnimator = GetComponent<Animator>();
        platformRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if(platformRenderer == null || propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
            platformRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        else
        {
            platformRenderer.GetPropertyBlock(propertyBlock);
            if(propertyBlock != null)
            {
                propertyBlock.SetFloat("_MaxOverlayMultiplier", _MaxOverlayMultiplier);
                propertyBlock.SetFloat("_OverlayMultiplier", _OverlayMultiplier);
            }
            platformRenderer.SetPropertyBlock(propertyBlock);
        }

        if(targetResistance != currentResistance)
        {
            //Animate the prevResistance
            float iterationValue = 5 * Time.deltaTime;
            currentResistance = (1 - iterationValue) * currentResistance + (iterationValue) * targetResistance;
            currentResistance = Mathf.Clamp(currentResistance, 0, maxCorrosionResistance);
            if (Mathf.Abs(currentResistance - targetResistance) < 0.5f)
            {
                currentResistance = targetResistance;
                if (currentResistance <= 0)
                    StartBreakApart();
            }
        }

        //Animating the corrosion property using overlay multiplier
        _OverlayMultiplier = (1 - (currentResistance / maxCorrosionResistance)) * _MaxOverlayMultiplier;

    }

    public void Corrode(float value)
    {
        Debug.Log("Corroding: " + value);
        //The corrosion should be proportional to max overlay multiplier
        targetResistance -= value;
    }

    void StartBreakApart()
    {
        breakApart = true;
        //Start the animation
        if(corrosionAnimator != null)
            corrosionAnimator.SetBool("Enabled", true);

        //Deactivate the object for now.
        gameObject.SetActive(false);
    }

    public void Save()
    {
        //Save values to a save object
    }

    public void Load()
    {
        //TODO: Load the values from the save object
        if (breakApart)
            StartBreakApart();
    }

}
