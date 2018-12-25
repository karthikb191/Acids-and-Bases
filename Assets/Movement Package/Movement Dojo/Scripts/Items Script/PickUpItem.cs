using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour {
    public Item item;

    public Material defaultMaterialPrefab;
    public Material pickUpMaterialPrefab;
    private Material defaultMaterial;
    private Material pickUpMaterial;

    public bool pickedUp = false;
    public bool promptPickUp;
    
    public bool north;    //false for south and true for north

    public float outlineAnimationVariable = 0;
    public float maxMaskValue = 0;
    public float minMaskValue = 0;
    public Vector3 targetScale = Vector3.one;
    public Vector3 localtargetPosition = Vector3.zero;
    public Vector3 targetRotation = Vector3.zero;

    private PlayerMechanics playerMechanics;
    private SpriteRenderer spriteRenderer;


    private Quaternion defaultOrientation;
    private Quaternion pickUpOrientation;

    private Vector3 defaultScale;

    Vector3 previousPickUpPosition;

    public Animator itemAnimator;
    //TODO: Remove the virtual joystick controls from this class and place it in a separate class with all the controls

    Player playerOnFocus = null;
    private void Start()
    {
        //itemAnimator = transform.GetChild(0).GetComponent<Animator>();
        //Debug.Log("Item animator is" + itemAnimator);

        
        defaultMaterial = Instantiate(defaultMaterialPrefab);
        pickUpMaterial = Instantiate(pickUpMaterialPrefab);

        if (transform.GetChild(0) != null)
        {
            spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenderer.material = defaultMaterial;
        }

        defaultOrientation = gameObject.transform.rotation;
        defaultScale = gameObject.transform.localScale;

        //This makes object stay in it's place
        previousPickUpPosition = gameObject.transform.position + localtargetPosition;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        Player p = collider.GetComponent<Player>();
        if (p != null)
        {
            playerOnFocus = p;

            //Enable the button
            DynamicButton d = VirtualJoystick.CreateButton("tag_item");
            if (!d.active)
            {
                VirtualJoystick.EnableButton(d);
                d.button.onClick.AddListener(() =>
                {
                    //p.userInputs.doorOpenPressed = true;

                    //Disable the button
                    VirtualJoystick.DisableButton(d);
                });
            }

        }

        //if(collider.tag == "tag_player")
        //{
        //    promptPickUp = true;
        //    playerMechanics = collider.gameObject.GetComponent<PlayerMechanics>();
        //
        //    //Enable the button on the controls canvas
        //    VirtualJoystick.pickUpButton.SetActive(true);
        //    //Set the sprite to pick up sprite
        //    if(playerMechanics.itemPickedUp == null)
        //        VirtualJoystick.pickUpButton.GetComponent<UnityEngine.UI.Image>().sprite = VirtualJoystick.pickUpButton.GetComponent<PickUpButton>().pickupSprite;
        //}
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if(playerOnFocus != null)
            if (collider.gameObject == playerOnFocus.gameObject)
            {
                playerOnFocus = null;

                //Disable the button
                VirtualJoystick.DisableButton("tag_item");
            }

        //if(collider.tag == "tag_player")
        //{
        //
        //    promptPickUp = false;
        //    //Set the player to null only of the object is not picked up
        //    if (!pickedUp && collider.gameObject.GetComponent<PlayerMechanics>().itemPickedUp == null)
        //    {
        //        VirtualJoystick.pickUpButton.SetActive(false);
        //        playerMechanics = null;
        //    }
        //}
    }

    void Update() {

        //bool tempPickUp = false;
        //if (pickedUp)
        //    CheckForSpecialActionButtonPress();
        //
        ////If the item is not already picked up and the pickup prompt is enabled, the check for the button press for item pickup
        //if (!pickedUp && promptPickUp)
        //{
        //    tempPickUp = PickUp();
        //}
        //
        ////If the item is picked up, check for the drop
        //if (pickedUp)
        //{
        //    
        //    //Debug.Log("pickup");
        //    previousPickUpPosition = new Vector3(playerMechanics.gameObject.transform.position.x,
        //                                        playerMechanics.gameObject.transform.position.y + playerMechanics.GetComponent<Collider2D>().bounds.size.y, 
        //                                        playerMechanics.gameObject.transform.position.z);
        //    OrientAndAbsorbItem (Quaternion.Euler(targetRotation), targetScale, localtargetPosition);
        //
        //    SetAnimation();
        //    ModifyForces();
        //    if(!tempPickUp)
        //        Drop();
        //    
        //}
        //else{
        //    //Debug.Log("Not picked up");
        //    OrientAndAbsorbItem (defaultOrientation, defaultScale, previousPickUpPosition - localtargetPosition);
        //}
        //
        //
        //if (pickedUp)
        //{
        //    //Give the center position value to the shader
        //    spriteRenderer.sharedMaterial.SetVector("_Center", new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0));
        //    spriteRenderer.sharedMaterial.SetFloat("_MaxDistance", maxMaskValue);
        //    spriteRenderer.sharedMaterial.SetFloat("_MinDistance", minMaskValue);
        //}

    }

    void SetAnimation()
    {
        if (itemAnimator != null)
        {
            MovementScript2D playerMovementScript = transform.parent.GetComponent<MovementScript2D>();
            if (playerMovementScript != null)
            {
                if (playerMovementScript.playerState != PlayerStates.falling && playerMovementScript.playerState != PlayerStates.jumping)
                    itemAnimator.SetBool("Open", false);
            }
            else
            {
                    itemAnimator.SetBool("Open", false);
            }

            spriteRenderer.material = pickUpMaterial;
        }
    }

    void ModifyForces()
    {
        switch (item)
        {
            case Item.parachute:
                //If the parachute is equipped, then cut off the movement accordingly
                if(playerMechanics.GetComponent<MovementScript2D>().CurrentJumpSpeed < 0)
                {
                    if (itemAnimator != null)
                    {
                        if (itemAnimator.GetBool("Open"))
                        {
                            playerMechanics.GetComponent<MovementScript2D>().externalVerticalMovementDamp = 0.1f;
                            playerMechanics.GetComponent<MovementScript2D>().externalHorizontalMovementDamp = 0.7f;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    void Drop()
    {
        if(playerMechanics != null && playerMechanics.GetComponent<MovementScript2D>().playerState == PlayerStates.idle)
        {
            //Check for the button press
            if (playerMechanics.itemPickedUp == this.gameObject)   
            {
                //If the item is picked up, then check for the button press for pickup
                if (Input.GetKeyDown(KeyCode.P) || VirtualJoystick.pickUpButtonDown)
                {
                    Debug.Log("Dropping");
                    //Material must be set to the default material on drop
                    spriteRenderer.material = defaultMaterial;


                    //If all the conditions are satisfied, then pick up the item
                    pickedUp = false;
                    playerMechanics.itemPickedUp = null;

                    //disable the trigger and set it's parent as the player
                    gameObject.GetComponent<BoxCollider2D>().enabled = true;
                    gameObject.transform.parent = null;
                    playerMechanics = null;

                    //Change virtual joystick sprite
                    VirtualJoystick.pickUpButton.GetComponent<UnityEngine.UI.Image>().sprite = VirtualJoystick.pickUpButton.GetComponent<PickUpButton>().pickupSprite;
                    
                    //After Dropping, disable the special actions button and activate the pickup button
                    VirtualJoystick.itemSpecialActionButton.SetActive(false);
                    VirtualJoystick.pickUpButton.SetActive(false);
                    //VirtualJoystick.pickUpButton.SetActive(true);
                }
            }
        }
    }

    bool PickUp()
    {
        bool res = false;
        if(playerMechanics != null)
        {
            //Check for the button press
            if (Input.GetKeyDown(KeyCode.P) || VirtualJoystick.pickUpButtonDown)
            {
                Debug.Log("Picking up");
                //If the item is not picked up, then check for the button press for pickup
                if (playerMechanics.itemPickedUp == null)
                {
                    //Object's material must change upon pickup
                    spriteRenderer.material = pickUpMaterial;
                    

                    //If all the conditions are satisfied, then pick up the item
                    pickedUp = true;
                    playerMechanics.itemPickedUp = this.gameObject;

                    //disable the trigger and set it's parent as the player
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    gameObject.transform.parent = playerMechanics.transform;
                    //gameObject.transform.localPosition = Vector3.zero;

                    //change the pickup sprite of virtual joystick to drop
                    VirtualJoystick.pickUpButton.GetComponent<UnityEngine.UI.Image>().sprite = VirtualJoystick.pickUpButton.GetComponent<PickUpButton>().dropSprite;
                    res = true;
                }

                //After pickup, enable the special action control button and deactivate the pickup button
                //VirtualJoystick.pickUpButton.SetActive(false);
                VirtualJoystick.itemSpecialActionButton.SetActive(true);
            }

        }
        return res;
    }


    void CheckForSpecialActionButtonPress() {
        //press F for flipping the special action
        
        if (Input.GetKeyDown(KeyCode.F) || VirtualJoystick.itemSpecialActionButtonDown) {
            switch (item)
            {
                case Item.magnet:
                    {
                        //Filp the magnet
                        north = !north;
                        if (north)
                        {
                            targetRotation = new Vector3(targetRotation.x, targetRotation.y, targetRotation.z - 180);
                        }
                        else
                        {
                            targetRotation = new Vector3(targetRotation.x, targetRotation.y, targetRotation.z + 180);

                        }
                        break;
                    }
                case Item.parachute:
                    {
                        //Debug.Log("Animator is: " + itemAnimator);
                        if (itemAnimator != null)
                        {
                            itemAnimator.SetBool("Open", !itemAnimator.GetBool("Open"));
                        }
                        break;
                    }
                default:
                    Debug.Log("No item found");
                    break;
            }
            
        }
        
    }

    void OrientAndAbsorbItem(Quaternion targetRotation, Vector3 targetScale, Vector3 targetPosition)
    {
        //Animating the rotation
        float angle = Quaternion.Angle(gameObject.transform.rotation, targetRotation);
        //Debug.Log("target rotation: " + targetRotation +" Angle is: " + angle);
        //Quaternion rot = gameObject.transform.rotation;
        
        if (angle > 0.05f)
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, 0.2f);
        else
            gameObject.transform.rotation = targetRotation;

        //Animating scale
        float scaleDiff = Vector3.Distance(gameObject.transform.localScale, targetScale);
        if (scaleDiff > 0.05f)
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, targetScale, 0.2f);
        else
            gameObject.transform.localScale = targetScale;

        //Animating the position
        if (pickedUp)
        {
            //Debug.Log("local tagert position: " + localPosition);
            if (Vector3.Distance(gameObject.transform.localPosition, targetPosition) > 0.05f)
                gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, targetPosition, 0.2f);

            else
                gameObject.transform.localPosition = targetPosition;
        }
        else
        {
            if (Vector3.Distance(gameObject.transform.position, targetPosition) > 0.05f)
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, 0.2f);

            else
                gameObject.transform.position = targetPosition;
        }
        
    }

}

/*[CustomEditor(typeof(PickUpItem))]
[CanEditMultipleObjects]
public class PickUpItemEditor : Editor {

}*/
