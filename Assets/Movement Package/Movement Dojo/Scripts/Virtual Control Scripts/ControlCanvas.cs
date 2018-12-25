﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ControlCanvas : MonoBehaviour {
    ControlCanvas instance;
    // Use this for initialization

    public static bool leftButtonPressed;
    public static bool rightButtonPressed;
    public static bool middleButtonPressed;

    public static bool leftButtonReleased;
    public static bool rightButtonReleased;
    public static bool middleButtonReleased;

    [Range(0, 1)]
    public static float counter = 0;

    public static bool clickControls;

    Image leftArea;
    Image rightArea;
    Image middleArea;

    private float defaultWidth;
    private float extraWidth = 200;

    void Awake() {
        
        leftArea = FindObjectOfType<LeftButton>().GetComponent<Image>();
        rightArea = FindObjectOfType<RightButton>().GetComponent<Image>();
        middleArea = FindObjectOfType<Jump>().GetComponent<Image>();

        defaultWidth = middleArea.rectTransform.sizeDelta.x;
        extraWidth = defaultWidth + extraWidth;
        
    }
    private void Start()
    {
        instance = this;
    }

    public static float StartCounter() {
        counter += Time.deltaTime;
        return Mathf.Clamp(counter, 0, 1);
    }

    public static float ResetCounter() {
        counter = 0;
        return counter;
    }

	
    
}
