 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour {

    bool thrown = false;
    bool used = false;

   
    public ItemProperties itemProperties;
    Vector3 targetScale;
    public string tagToCompare;


    bool setFocus;

    Player playerObject;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Player p = collider.GetComponent<Player>();
        if (p != null)
        {
            playerObject = p;

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

            setFocus = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(playerObject != null)
        {
            if (collider.gameObject == playerObject.gameObject)
            {
                playerObject = null;

                //Disable the button
                VirtualJoystick.DisableButton("tag_item");
            }
        }
        setFocus = false;
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && setFocus)
        {
            if (playerObject.GetComponentInChildren<Inventory>() != null)
            {
                
                Debug.Log(playerObject.gameObject.name + itemProperties.name);

                if (playerObject.GetComponentInChildren<Inventory>().activeItem == null)
                {
                    playerObject.GetComponentInChildren<Inventory>().activeItem = this;

                    playerObject.GetComponentInChildren<Inventory>().AddItem(this);

                    targetScale = new Vector3(0.3f, 0.3f, 0.3f);

                    //   Debug.Log("Align with pos called" + "________Character player>>>" + playerObject.GetComponentInChildren<Character>());

                    StartCoroutine(AlignPos(playerObject.GetComponent<Transform>().position, playerObject.GetComponentInChildren<Character>()));
                    transform.parent = playerObject.GetComponentInChildren<Character>().transform;

                }
                else
                {
                    playerObject.GetComponentInChildren<Inventory>().AddItem(this);

                    StartCoroutine(AlignPos(playerObject.GetComponent<Transform>().position, playerObject.GetComponentInChildren<Character>()));

                    gameObject.SetActive(false);

                    transform.parent = playerObject.GetComponentInChildren<Character>().transform;

                }
            }
            else
                Debug.Log("No inventory attached to the character");
            setFocus = false;
        }


        if (thrown)
        {
            //do something
        }

       
    }

    public virtual void Use() { }

    public virtual void Throw(Vector3 target,float speed)
    {
        float angle = 45;
  
        
        StartCoroutine(ThrowProjectile(target, angle,speed));

    }

    public IEnumerator AlignPos(Vector3 targetPosition, Character c)
    {
        
        //Animating the rotation
        float angle = Quaternion.Angle(gameObject.transform.rotation, c.transform.rotation);
        //Debug.Log("target rotation: " + targetRotation +" Angle is: " + angle);
        Quaternion rot = gameObject.transform.rotation;

        if (angle > 0.05f)
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, c.transform.rotation, 0.2f);
        else
            gameObject.transform.rotation = c.transform.rotation;

        //Animating scale
        float scaleDiff = Vector3.Distance(gameObject.transform.localScale, targetScale);
        if (scaleDiff > 0.05f)
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, targetScale, 0.2f);
        else
            gameObject.transform.localScale = targetScale;

        if (Vector3.Distance(gameObject.transform.position, targetPosition) > 0.05f)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, 0.2f);

        }

        else
        {
            gameObject.transform.position = targetPosition;

        }
        Debug.Log("Parented");

        if (gameObject.transform.position == targetPosition)
        {

            yield break;
        }
        else
        {
            yield return null;
        }
    }

    public void DropItem(Vector3 targetPosition, Character c)
    {

        targetScale = Vector3.one;
         // transform.rotation = c.gameObject.transform.rotation;
        transform.position = targetPosition;
        transform.parent = null;

        //animation for local trasform positions can come here
    }

    public virtual void Use(Character c)
    {
        
    }
    //Overload use function for interaction with environment
    public virtual void Use(GameObject g)
    {
        //This is called from inventory, which is called from player
    }
    

    public void EnvironmentHit(GameObject g)
    {
        //The environment may have a use function later, which can be called from here

        //This must call the item's use property on the particular character that it hits
    }
    public void ItemHitCharacter(Character c)
    {
        //Call the character's use function here

        //This must call the item's use property on the particular character that it hits
    }
    public void Destroy()
    {
        //Destroy item after use or whenever you see fit
    }


    public void AlignWithPos(Quaternion targetRotation, Vector3 targetScale, Vector3 targetPosition)
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




        if (Vector3.Distance(gameObject.transform.position, targetPosition) > 0.05f)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, 0.2f);

        }

        else
        {
            gameObject.transform.position = targetPosition;

        }

    }


    IEnumerator ThrowProjectile(Vector3 Target, float firingAngle,float speed)
    {
        
        Debug.Log("Target to reach"+Target);
        float target_Distance = Vector3.Distance(gameObject.transform.position, Target);
        Debug.Log("target_Distance" + target_Distance);
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / speed);
        

        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        float flightDuration = target_Distance / Vx;

        //Looking at the target
        Quaternion rotation = Quaternion.LookRotation(Target - transform.position);

        rotation.y = 0;
        rotation.x = 0;
        transform.rotation = rotation;


        float elapse_time = 0;

        while (elapse_time < flightDuration)
      //  while(Vector3.Distance(Target,gameObject.transform.position) >0.5f)
        {
            transform.Translate(Vx * Time.deltaTime, (Vy - (speed * elapse_time)) * Time.deltaTime,0);

            elapse_time += Time.deltaTime;
            Debug.Log("elapse_time" + elapse_time);
           

            yield return null;
        }

        if (elapse_time >= flightDuration)
        {

            transform.rotation = Quaternion.identity;
            elapse_time = 0;
            transform.parent = null;
            Debug.Log("Flight over");
            yield break;
        }

    }
    
}


