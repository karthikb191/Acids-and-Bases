 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBase : MonoBehaviour {

    bool thrown = false;
    bool used = false;
    
    bool setFocus;
   public GameObject playerObject;

    public ItemProperties itemProperties;

    public float speed = 20.0f;

    Vector3 targetScale = Vector3.one;



   public bool isFromEnemy = false;
 

    private void Update()
    {
        if(thrown)
        {
          ThrowPathFollow();
        }


        //Add to item while in throw

        CheckCollision();
    }

    public virtual void Use() {
        
        transform.parent = null;
        gameObject.SetActive(false);
    }

    public virtual void Throw(Vector3 target,float speed)
    {
    //   if(isFromEnemy)
        {
             float angle = 45;

              if ((target.x - transform.position.x) < (target.y - transform.position.y))
              {
                  angle = 60;
              }


              if (isFromEnemy && GetComponentInParent<Enemy>())
              {
                  //   playerObject = GetComponentInParent<Enemy>().gameObject;
                  Debug.Log("From enemy" + playerObject.name);
              }

              StartCoroutine(ThrowProjectile(target, angle));
          //  ThrowCalculations(target, speed);

        }
     //  else
        {
      //      ThrowCalculations(target, speed);
            Debug.Log("New throw Called");
        }

        // StartCoroutine(ThrowPathFollow());

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger enter");
        if (collision.gameObject.GetComponent<Player>() && !isFromEnemy)
        {
            playerObject = collision.gameObject;

           // Player p = collision.GetComponent<Player>();
            if (collision.GetComponent<Player>() != null)
            {
                //Enable the button
                DynamicButton d = VirtualJoystick.CreateButton("tag_item");
                if (!d.active)
                {
                    VirtualJoystick.EnableButton(d);
                    d.button.onClick.AddListener(() =>
                    {
                        AddItem();
                        VirtualJoystick.DisableButton(d);                          
                    });
                }
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() && !isFromEnemy)
        {
            VirtualJoystick.DisableButton("tag_item");
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
                Collider2D hit = Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y), transform.localScale.x, LayerMask.GetMask("Character","Platform"));
               // Debug.Log("overlap circel" , hit);
              if (hit != null)
              {
                   // Debug.Log("Hit on : " + hit.gameObject.name);
                    if(hit.GetComponent<Character>())
                    {
                        Debug.Log("Character hit");
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
           // 
            

           if(isFromEnemy)
            {
                //   transform.position = playerObject.GetComponentInChildren<Character>().Hand.transform.position;
                //   transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;
                gameObject.SetActive(false);
                Destroy(gameObject);
             //   GetComponentInParent<Inventory>().AddItem(this);
            }
           else
            {
                gameObject.SetActive(false);
                transform.parent = null;
            }
           // Debug.Log("Flight over");
            yield break;
        }
    }

    private void AddItem()
    {
        if (playerObject.GetComponentInChildren<PlayerInventory>().activeItem == null)
        {
            playerObject.GetComponentInChildren<PlayerInventory>().activeItem = this;

            playerObject.GetComponentInChildren<PlayerInventory>().AddItem(this);

            targetScale = transform.localScale / 2;

           StartCoroutine(AlignPos(playerObject.GetComponent<Character>().Hand.transform.position, playerObject.GetComponentInChildren<Character>()));
           // StartCoroutine(AlignWithPos( new Quaternion(0,0,0,0),new Vector3(0.1f,0.1f,0.1f),playerObject.GetComponent<Character>().Hand.transform.position));

            transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;
            gameObject.transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;
            gameObject.transform.localScale = targetScale;

        }
        else
        {
            playerObject.GetComponentInChildren<Inventory>().AddItem(this);

            StartCoroutine(AlignPos(playerObject.GetComponent<Character>().Hand.transform.position, playerObject.GetComponentInChildren<Character>()));

            gameObject.SetActive(false);

            transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;
            gameObject.transform.localScale = targetScale;

        }
        setFocus = false;
    }

    public float maxRangeOfThrow = 20;
    public float maxAngle = 45;

    [Range(0, 45)]
   public float angleOfThrow;

    Vector3 directionOfThrow;

    Vector3 angleQuatrenion;

   public float timeElapsed = 0;

   public  float timeToReach = 0;
  public  Vector3 targetToHit;

    Vector3 lastDirection;

    public float throwVelocity = 30;

    private void ThrowCalculations(Vector3 target, float throwVelo)
    {
        /////////// set max range deactive later

        Debug.Log("target for throw" + target + "<<<<<<====>>>>>>");

        directionOfThrow = target - gameObject.transform.position;

        if (Mathf.Abs(directionOfThrow.x) <= maxRangeOfThrow)
        {
            targetToHit = target;

            throwVelocity = throwVelo;

            angleOfThrow = maxAngle * Mathf.Abs(directionOfThrow.x) / maxRangeOfThrow;

            timeToReach = directionOfThrow.magnitude / throwVelocity;

            angleQuatrenion = Quaternion.AngleAxis(angleOfThrow, Vector3.forward) * directionOfThrow;

            lastDirection = angleQuatrenion;

            thrown = true;
        }

    }

   private void ThrowPathFollow()
  //  IEnumerator ThrowPathFollow()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > timeToReach / 3.0f)
        {
            angleQuatrenion = targetToHit - gameObject.transform.position;

        }

        Vector3 tempDirection = Vector3.Lerp(lastDirection.normalized, angleQuatrenion.normalized, 0.02f);

         gameObject.transform.position += tempDirection.normalized * throwVelocity * Time.deltaTime;

      
        lastDirection = tempDirection;

       // if (timeElapsed > timeToReach + 0.2f || Mathf.Abs(targetToHit.x - transform.position.x) < 0.5f)
        if (timeElapsed > timeToReach + 0.2f)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            /// particle effect at end of path
            thrown = false;

          //  yield break;

        }

    }

    void CheckCollision()
    {

    }

}


