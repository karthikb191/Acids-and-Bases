using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DynamicButton
{
    public bool active = false;
    public bool touchingPlayer = true;
    public string tag;  //tag will be the same as the tag of the interactable object
    
    public Button button;
    public GameObject objectInContactWith;
}

[System.Serializable]
public class ButtonGraphics
{
    public Sprite defaultImage;
    public Sprite ladderImage;
    public Sprite itemImage;
    public Sprite doorImage;
}


public class VirtualJoystick : MonoBehaviour {
    
    public static bool usingJoystick;
    public static bool jumpButtonDown;
    public static bool jumpButtonUp;

    //Item Pickup Button
    public static GameObject pickUpButton;
    public static bool pickUpButtonDown;
    public static bool pickUpButtonUp;

    //Item special Action Button
    public static GameObject itemSpecialActionButton;
    public static bool itemSpecialActionButtonDown;
    public static bool itemSpecialActionButtonUp;

    //Item special Action Button
    public static GameObject throwButton;
    public static bool throwButtonButtonDown;
    public static bool throwButtonButtonUp;

    public static float horizontalValue;
    public static float verticalValue;

    static Transform controlCanvas;
    public static List<DynamicButton> dynamicButtonsStore;
    public static List<DynamicButton> activeDynamicButtons;
    public GameObject dynamicButtonPrefab;  //Must be set in the inspector
    
    [SerializeField]
    public ButtonGraphics buttonGraphics;

    //Arrows Holder
    public static RectTransform arrowsHolder;

    private void Awake()
    {

        VirtualJoystick[] virtualJoysticks;
        virtualJoysticks = FindObjectsOfType<VirtualJoystick>();

        foreach (VirtualJoystick v in virtualJoysticks)
        {
            if (GameManager.Instance.virtualJoystick != this.gameObject && GameManager.Instance.virtualJoystick != null)
            {
                DestroyImmediate(this.gameObject);
            }
            //if (v.gameObject != this.gameObject)
            //    Destroy(this.gameObject);
        }
    }

    void Start () {

        jumpButtonUp = true;
        jumpButtonDown = false;

        pickUpButtonUp = true;

        itemSpecialActionButtonUp = true;
        
        
        DontDestroyOnLoad(this.gameObject);

        activeDynamicButtons = new List<DynamicButton>();
        dynamicButtonsStore = new List<DynamicButton>();

        controlCanvas = transform.GetChild(0);
        pickUpButton = transform.GetChild(0).Find("PickUp").gameObject;
        itemSpecialActionButton = transform.GetChild(0).Find("ItemSpecialAction").gameObject;
        throwButton = transform.GetChild(0).Find("ThrowItemButton").gameObject;

        pickUpButton.SetActive(false);
        itemSpecialActionButton.SetActive(false);

        arrowsHolder = gameObject.transform.Find("Controls").Find("Arrows").GetComponent<RectTransform>();

        GameManager.Instance.virtualJoystick = this;

        Debug.Log("control canvas initiated");
    }
    
	// Update is called once per frame
	void LateUpdate() {
        if (jumpButtonUp) {
            jumpButtonUp = false;
            Debug.Log("jump to false");
        }
        if (pickUpButtonDown)
            pickUpButtonDown = false;
        if (itemSpecialActionButtonDown)
            itemSpecialActionButtonDown = false;
	}
    
    #region Dynamic Button Functions
    //Button creation and destruction will be done on the respective target object
    //This makes editing them easier
    public static DynamicButton CreateButton(string tag)
    {
        float buttonSize = 50.0f;
        //Debug.Log("enabling");
        bool buttonFound = false;

        //If the button is present in the active dynamic buttons list
        for(int i = 0; i < activeDynamicButtons.Count; i++)
        {
            if (activeDynamicButtons[i].tag == tag)
                return activeDynamicButtons[i];
        }

        if(dynamicButtonsStore.Count > 0)
        {
            for(int i = 0; i < dynamicButtonsStore.Count; i++)
            {
                //If a button is already present in the list with that tag, enable it
                if(dynamicButtonsStore[i].tag == tag)
                {
                    buttonFound = true;
                    dynamicButtonsStore[i].button.gameObject.SetActive(true);
                    
                    //Set the position of thr button
                    return dynamicButtonsStore[i];
                }
            }
        }
        else
        {
            Debug.Log("Button Created");
            GameObject g = new GameObject();
            DynamicButton d = new DynamicButton();
            

            g.AddComponent<Button>();
            g.AddComponent<Image>();
            d.button = g.GetComponent<Button>();
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            d.button.navigation = nav;
            g.transform.SetParent(controlCanvas);

            d.tag = tag;
            dynamicButtonsStore.Add(d);

            d.button.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonSize, buttonSize);

            //Assign an image to the button depending on the tag
            AssignImage(d, tag);

            return d;
        }
        if (!buttonFound)
        {
            Debug.Log("Button Created");
            GameObject g = new GameObject();
            DynamicButton d = new DynamicButton();

            

            g.AddComponent<Button>();
            g.AddComponent<Image>();
            d.button = g.GetComponent<Button>();
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            d.button.navigation = nav;
            g.transform.SetParent(controlCanvas);
            
            d.tag = tag;

            dynamicButtonsStore.Add(d);

            //Assign an image to the button depending on the tag
            AssignImage(d, tag);

            return d;
        }
        else
            return null;
    }

    public static void AssignImage(DynamicButton d, string tag)
    {
        switch (tag)
        {
            case "tag_ladder":
                d.button.GetComponent<Image>().sprite = GameManager.Instance.virtualJoystick.buttonGraphics.ladderImage;
                break;

            case "tag_item":
                Debug.Log("Button graphics: " + GameManager.Instance.virtualJoystick.buttonGraphics.itemImage);
                Debug.Log("Button graphics: " + d.button);
                d.button.GetComponent<Image>().sprite = GameManager.Instance.virtualJoystick.buttonGraphics.itemImage;
                break;

            case "tag_door":
                d.button.GetComponent<Image>().sprite = GameManager.Instance.virtualJoystick.buttonGraphics.doorImage;
                break;

            default:
                d.button.GetComponent<Image>().sprite = GameManager.Instance.virtualJoystick.buttonGraphics.defaultImage;
                break;
        }       
    }

    public static void EnableButton(DynamicButton b)
    {
        if(b!= null)
        {
            //enable the button
            b.active = true;
            b.button.gameObject.SetActive(true);

            //Reset the positions of active buttons
            b.button.transform.position = new Vector3(Screen.width / 2, Screen.height / 3);

            //Add to the list of active buttions
            Debug.Log("active buttons list: " + activeDynamicButtons.Count);

            if(!activeDynamicButtons.Contains(b))
                activeDynamicButtons.Add(b);

            ArrangeButtons();
        }
    }

    public static void DisableButton(string tag)
    {
        Debug.Log("Disabling button");
        if(activeDynamicButtons.Count > 0)
        {
            for(int i = 0; i < activeDynamicButtons.Count; i++)
            {
                if(activeDynamicButtons[i].tag == tag)
                {
                    activeDynamicButtons[i].button.onClick.RemoveAllListeners();
                    activeDynamicButtons[i].active = false;
                    activeDynamicButtons[i].button.gameObject.SetActive(false);
                    activeDynamicButtons.RemoveAt(i);
                }
            }
            ArrangeButtons();
        }
    }
    public static void DisableButton(DynamicButton b)
    {
        if (b != null)
        {
            //Debug.Log("Disabling button " + b.tag);
            b.active = false;
            b.button.gameObject.SetActive(false);
            b.button.onClick.RemoveAllListeners();
            activeDynamicButtons.Remove(b);
            ArrangeButtons();
        }

    }
    
    static void ArrangeButtons()
    {
        float angle = 60 * Mathf.Deg2Rad;
        float radius = 150;

        float initialAngle = Mathf.PI / 2 - angle/2 * activeDynamicButtons.Count;

        Vector3 intialPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        for(int i = 0; i < activeDynamicButtons.Count; i++)
        {
            float x = intialPosition.x + radius * Mathf.Cos(initialAngle + (i+1) * angle);
            float y = intialPosition.y + radius * Mathf.Sin(initialAngle + (i+1) * angle);

            activeDynamicButtons[i].button.gameObject.transform.position = new Vector3(x, y, 0);
        }

    }
    #endregion

    #region Arrows Rotation Functions
    static Coroutine arrowRotationRoutine;
    static Quaternion initialRotation;
    //Quaternion previousTragetRotation;
    public static void RotateArrows(float targetAngle, Character c)
    {
        if (c.GetComponent<Player>())
        {
            initialRotation = arrowsHolder.localRotation;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));
            if (arrowRotationRoutine == null)
                arrowRotationRoutine = GameManager.Instance.virtualJoystick.StartCoroutine(RotateArrows(targetRotation));
            else
            {
                GameManager.Instance.virtualJoystick.StopCoroutine(arrowRotationRoutine);
                arrowRotationRoutine = null;
                arrowRotationRoutine = GameManager.Instance.virtualJoystick.StartCoroutine(RotateArrows(targetRotation));
            }
        }
    }
    public static void ResetArrows(Character c)
    {
        RotateArrows(0, c);
    }

    static IEnumerator RotateArrows(Quaternion targetRotation)
    {
        float stepSize = 0.05f;
        while(Quaternion.Angle(arrowsHolder.localRotation, targetRotation) > 1.0f)
        {
            //Arrows are rotated here
            arrowsHolder.rotation = Quaternion.Lerp(arrowsHolder.rotation, targetRotation, stepSize);
            yield return new WaitForFixedUpdate();
        }
        arrowRotationRoutine = null;
        
        yield return null;
    }

    #endregion
}
