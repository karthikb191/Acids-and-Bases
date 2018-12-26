 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour {

    bool thrown = false;
    bool used = false;
    
    bool setFocus;
    GameObject playerObject;

    public ItemProperties itemProperties;

    public float speed = 30.0f;

    Vector3 targetScale;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && setFocus)
        {

            Debug.Log(playerObject.gameObject.name + itemProperties.name);

            if (playerObject.GetComponentInChildren<Inventory>().activeItem == null)
            {
                playerObject.GetComponentInChildren<Inventory>().activeItem = this;

                playerObject.GetComponentInChildren<Inventory>().AddItem(this);

                targetScale = transform.localScale/5;

             //   Debug.Log("Align with pos called" + "________Character player>>>" + playerObject.GetComponentInChildren<Character>());

                StartCoroutine(AlignPos(playerObject.GetComponent<Character>().Hand.transform.position, playerObject.GetComponentInChildren<Character>()));
                transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;

            }
            else
            {
                playerObject.GetComponentInChildren<Inventory>().AddItem(this);

                StartCoroutine(AlignPos(playerObject.GetComponent<Character>().Hand.transform.position, playerObject.GetComponentInChildren<Character>()));


                gameObject.SetActive(false);

                transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;

            }

            setFocus = false;
        }


        if (thrown)
        {
            //do something
        }

       
    }

    public virtual void Use() {
        
        transform.parent = null;
        gameObject.SetActive(false);
    }

    public virtual void Throw(Vector3 target)
    {
        float angle = 45;
        
        StartCoroutine(ThrowProjectile(target, angle));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Character>())
        {
            playerObject = collision.gameObject;

            setFocus = true;
        }

     //   Debug.Log(collision.gameObject.name + itemProperties.name);

     //   Debug.Log(itemProperties.name + "___focus___"+ setFocus);

    }




    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Character>())
        {
  

            setFocus = false;
            playerObject = null;
            //Diable prompt message
        }
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
        Debug.Log("Use called");
    }
    //Overload use function for interaction with environment
    public virtual void Use(GameObject g)
    {
        //This is called from inventory, which is called from player

        Debug.Log("Use called");
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


    IEnumerator ThrowProjectile(Vector3 Target, float firingAngle)
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
        {

            if(elapse_time > 0.1f)
            {
               // Debug.Log("Casting");
                Collider2D hit = Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y), transform.localScale.x, LayerMask.GetMask("",""));
               // Debug.Log("overlap circel" , hit);
              if (hit != null)
              {
                   // Debug.Log("Hit on : " + hit.gameObject.name);
                    if(hit.GetComponent<Character>())
                    {
                        Use(hit.GetComponent<Character>());
                    }

                    else
                    {
                        Use(hit.gameObject);
                    }
              }
            }

            //Depending on sign, the object goes right(1) or left(0)
            int directionOfTranslation = (int)Mathf.Sign(Target.x - gameObject.transform.position.x);

            transform.Translate(Vx * Time.deltaTime * directionOfTranslation, (Vy - (speed * elapse_time)) * Time.deltaTime,0);

            elapse_time += Time.deltaTime;

           // Debug.Log("elapse_time" + elapse_time);          

            yield return null;
        }

        if (elapse_time >= flightDuration)
        {

            transform.rotation = Quaternion.identity;
            elapse_time = 0;
            transform.parent = null;
            gameObject.SetActive(false);
           // Debug.Log("Flight over");
            yield break;
        }

    }

 

      
            

        
    

}


