using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ThrowTapInput {

    public ThrowTapInput(Player player, Canvas canvas)
    {
        this.player = player;
        this.raycast = canvas.GetComponent<GraphicRaycaster>();
    }

    //PLayer script
    Player player;
    int count = 0;
    int throwInput = 0;
    List<RaycastResult> result = new List<RaycastResult>();
    public GraphicRaycaster raycast;

    float timer = 0;
    bool startTimer = false;

           
    List<Touch> touchesList = new List<Touch>();

    // Update is called once per frame
    public void Update()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                //     if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                if (touch.phase == TouchPhase.Began)

                {
                    touchesList.Add(touch);
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    touchesList.Remove(touch);
                }
            }
            CheckTouchList();
        }

        if (startTimer)
        {
            TapTimer();
            TargetSet();
        }

    }


    private bool IsCanvasElement(Touch touchItem)
    {
        PointerEventData ped = new PointerEventData(null);

        ped.position = touchItem.position;
        raycast.Raycast(ped, result);

        if (result.Count > 0)
        {
            result.Clear();

            return true;
        }
        else
        {
            result.Clear();

            return false;
        }

    }

    private void CheckTouchList()
    {
        //  Debug.Log(touchesList.Count + "   :   touch list count");
        foreach (Touch touchItem in touchesList)
        {
            if (IsCanvasElement(touchItem))
            {
                Debug.Log("Is canvas element");
            }
            else
            {
                if (startTimer)
                {
                    if (timer > 1f)
                    {
                        Debug.Log("Time Up");

                        startTimer = false;
                        throwInput = 0;
                        timer = 0;
                    }
                    else
                    {
                        Debug.Log("inside throw");
                        throwInput++;
                        timer = 0;
                    }
                }

                if (!startTimer)
                {
                    Debug.Log("Timer started");
                    throwInput++;
                    startTimer = true;
                }



            }
        }



        touchesList.Clear();
    }


    private void TapTimer()
    {
        timer += Time.deltaTime;
    }

    public void TargetSet()
    {
        if (throwInput == 2)
        {
            Debug.Log("Target set");
            throwInput = 0;
            timer = 0;
            startTimer = false;

        if (player.GetComponentInChildren<PlayerInventory>().activeItem != null)

        {
            player.GetComponentInChildren<PlayerInventory>().ThrowItem(Camera.main.ScreenToWorldPoint(Input.mousePosition), 5);

        }
    }
          
    }




    /////On update
    /*
     * raycast = anyCanvas.
     * InputCheck();
     * 
     * if(TargetSet())
     * {
     * call throw....
     * }
     * 
     * 
     */
}
