using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum JournalTabType
{
    General,
    Items,
    Acids,
    Bases,
    Indicators,
    Reactions
}

[RequireComponent(typeof(RectTransform))]
public class JournalTab : MonoBehaviour{

    public JournalTabType tabType;

    bool locked = true;

    RectTransform rectTransform;
    

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Journal.Instance.IsOpened())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
                {
                    Journal.Instance.ChangeTab(this);
                }
            }
        }
    }


    public bool IsLocked()
    {
        return locked;
    }
    public void Lock()
    {
        locked = true;
    }
    public void Unlock()
    {
        locked = false;
    }
    
}
