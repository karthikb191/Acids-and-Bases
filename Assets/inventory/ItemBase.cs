 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemBase : MonoBehaviour {

    public bool thrown = false;


    public ParticleSystem travelParticles;
    public ParticleSystem destroyParticles;

    ParticleSystem travelParticlesTemp;
    ParticleSystem destroyParticlesTemp;


   // bool used = false;
    
    bool setFocus;

    public GameObject playerObject;

    public ItemProperties itemProperties;

    public float speed = 20.0f;

    Vector3 targetScale = Vector3.one;

    public bool isFromEnemy = false;

    public float colliderRadius = 2;

    public float phValue;

    public delegate void ItemPickedUp(ItemBase i);
    public static event ItemPickedUp ItemPickedUpEvent;

    private void Start()
    {
        travelParticlesTemp = Instantiate(travelParticles,transform);
        travelParticlesTemp.transform.position = this.transform.position;
        destroyParticlesTemp = Instantiate(destroyParticles, transform);
        destroyParticlesTemp.transform.position = this.transform.position;
    }


    private void Update()
    {
        if(thrown)
        {
            CheckCollision();
            TravelParticleEffect();
            
        }

     //   TravelParticleEffect();
    }

    public virtual void Use()
    {       
        transform.parent = null;
        gameObject.SetActive(false);
    }

    public virtual void Throw(Vector3 target, float speed)
    {
        // gameObject.GetComponent<BoxCollider2D>().enabled = false;
        if (gameObject.activeSelf)
        { 
            if (isFromEnemy)
            {
                playerObject = transform.GetComponentInParent<Enemy>().gameObject;
                thrown = true;
                StartCoroutine(ThrowProjectile(target, 45));
            }
            else
            {
                playerObject = transform.GetComponentInParent<Player>().gameObject;
                Debug.Log("Thrown from: ____>>>>" + playerObject.name);
                ThrowCalculations(target, speed);
                StartCoroutine(ThrowProjectile(target, angleOfThrow));
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger enter");
        if (collision.gameObject.GetComponent<Player>() && !isFromEnemy)
        {
            playerObject = collision.gameObject;

            if (collision.GetComponent<Player>() != null)
            {
                //Enable the button
                DynamicButton d = VirtualJoystick.CreateDynamicButton("tag_item");
                if (!d.active)
                {
                    VirtualJoystick.EnableDynamicButton(d);
                    d.button.onClick.AddListener(() =>
                    {
                        AddItem();
                       // Debug.Log(collision.gameObject.GetComponentInChildren<PlayerInventory>());
                        VirtualJoystick.DisableDynamicButton(d);                          
                    });
                }
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() && !isFromEnemy)
        {
            VirtualJoystick.DisableDynamicButton("tag_item");
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
           // gameObject.transform.localScale = Vector3.one/2;
            gameObject.transform.localScale = targetScale;

        if (Vector3.Distance(gameObject.transform.position, targetPosition) > 0.05f)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, 0.2f);
            gameObject.transform.position = targetPosition;
            yield break;
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
        transform.position = targetPosition;
        transform.parent = null;

        //animation for local trasform positions can come here
    }
    public virtual void Use(Character c)
    {
        Debug.Log("Use called on character");

        if(itemProperties.isThrowable && itemProperties.damageDealt != 0 && isFromEnemy)
        {
            c.TakeDamage(itemProperties.damageDealt);
           
        }

        if(itemProperties.isThrowable && !isFromEnemy)
        {
            c.gameObject.GetComponent<Enemy>().UseItem();
      
            Debug.Log("Stun is called");
        }
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

    IEnumerator ThrowProjectile(Vector3 Target, float firingAngle)
    {      
        Debug.Log("Target to reach"+Target);
        float target_Distance = Vector3.Distance(gameObject.transform.position, Target);
        Debug.Log("target_Distance" + target_Distance);
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / speed);
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
        float flightDuration = target_Distance / Vx;
        int directionOfTranslation = (int)Mathf.Sign(Target.x - gameObject.transform.position.x);

        //Looking at the target
        Quaternion rotation = Quaternion.LookRotation(Target - transform.position);
        
        rotation.y = 0;
        rotation.x = 0;
        transform.rotation = rotation;       

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
           if(isFromEnemy)
            transform.Translate(Vx * Time.deltaTime, (Vy - (speed * elapse_time)) * Time.deltaTime,0);

           else
            transform.Translate(Vx * Time.deltaTime * directionOfTranslation, (Vy - (speed * elapse_time)) * Time.deltaTime,0);

            elapse_time += Time.deltaTime;

           // Debug.Log("elapse_time" + elapse_time);          

            yield return null;
        }
        if (elapse_time >= flightDuration)
        {
            transform.rotation = Quaternion.identity;
            elapse_time = 0;
           if(isFromEnemy)
           {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                this.AlignPos(playerObject.gameObject.GetComponentInParent<Character>().Hand.transform.position, playerObject.gameObject.GetComponentInParent<Character>());
                this.transform.position = playerObject.gameObject.GetComponentInParent<Character>().Hand.transform.position;
                this.transform.parent = playerObject.gameObject.GetComponentInParent<Character>().Hand.transform;
                playerObject.gameObject.GetComponentInChildren<EnemyInventory>().AddItem(this);
                this.gameObject.SetActive(false);
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true;

            }
           else
           {
                //gameObject.SetActive(false);
                DestroyParticleEffect();
                transform.parent = null;
           }
          
            yield break;
        }
    }

    public void AddItem()
    {
        if (ItemPickedUpEvent != null)
            ItemPickedUpEvent(this);

        //Changed......
        if (GetComponent<PH>())
        {
            playerObject.GetComponentInChildren<Inventory>().AddItem(this);
            //StartCoroutine(AlignPos(playerObject.GetComponent<Character>().Hand.transform.position, playerObject.GetComponentInChildren<Character>()));
            gameObject.SetActive(false);
            transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;
            gameObject.transform.localScale = targetScale;

            //Get the player component. If player is null, log error.
            if (playerObject.GetComponent<Player>())
            {
                if(!playerObject.GetComponent<Player>().GetPlayerStatus().GetpHIndicator())
                    playerObject.GetComponent<Player>().GetPlayerStatus().SetpHIndicator(GetComponent<PH>());
            }
            else
            {
                Debug.LogError("No player detected. Check your code");
            }

            return;
        }

        if (playerObject.GetComponentInChildren<PlayerInventory>().activeItem == null)
        {
            

            playerObject.GetComponentInChildren<PlayerInventory>().activeItem = this;
            playerObject.GetComponentInChildren<PlayerInventory>().AddItem(this);           
            StartCoroutine(AlignPos(playerObject.GetComponent<Character>().Hand.transform.position, playerObject.GetComponentInChildren<Character>()));   
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
            gameObject.transform.localScale = targetScale ;
        }
        // setFocus = false;

        if(playerObject.GetComponent<Player>() != null)
        playerObject.GetComponentInChildren<PlayerInventory>().DeactivateSlotInExtendedDisplay();

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

    void OnDrawGizmosSelected()
    {
#if (UNITY_EDITOR)
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(this.transform.position, new Vector3(0, 0,1), colliderRadius);
#endif
    }


    void CheckCollision()
    {

        timeElapsed += Time.deltaTime;
        RaycastHit2D[] collidedWith = Physics2D.CircleCastAll(new Vector2(transform.position.x, transform.position.y), colliderRadius, new Vector2(0, 1));

        for (int i = 0; i < collidedWith.Length; i++)
        {
            if(collidedWith[i].transform.CompareTag("tag_platform"))
            {
                Debug.Log("HIt Platform & destroyed");
                // Destroy(this.gameObject);
                DestroyParticleEffect();
                break;
            }

            if (collidedWith[i].transform.GetComponent<Switch>())
            {
                Debug.Log("Collided with Switch");
                collidedWith[i].transform.GetComponent<Switch>().ActivateDoor();
                DestroyParticleEffect();
                Destroy(this.gameObject);
                break;
            }

            if (collidedWith[i].transform.GetComponent<Character>() != null && collidedWith[i].transform.GetComponent<Character>().gameObject != playerObject.gameObject)
            {                 
                    Use(collidedWith[i].transform.GetComponent<Character>());
                    if(isFromEnemy)
                    {
                        Debug.Log("Used called from:   " + playerObject.name);
                        Debug.Log("Used on  :    " + collidedWith[i].transform.GetComponentInParent<Character>().gameObject);
                        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        this.AlignPos(playerObject.gameObject.GetComponentInParent<Character>().Hand.transform.position, playerObject.gameObject.GetComponentInParent<Character>());
                        this.transform.position = playerObject.gameObject.GetComponentInParent<Character>().Hand.transform.position;
                        this.transform.parent= playerObject.gameObject.GetComponentInParent<Character>().Hand.transform;
                        playerObject.gameObject.GetComponentInChildren<EnemyInventory>().AddItem(this);
                        this.gameObject.SetActive(false);
                        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        break;
                    }
                    else
                    {
                        DestroyParticleEffect();
                        break;
                    }             
            }
        }      
    }

    void TravelParticleEffect()
    {

     //    travelParticles.transform.position = playerObject.transform.position;
     //   travelParticles.Play();

       // travelParticlesTemp.transform.position = playerObject.transform.position; 
        

        if(!travelParticlesTemp.isPlaying)
        {
            travelParticlesTemp.Play();
          //  Debug.Log("PLaying particle");
        }
    }

    void DestroyParticleEffect()
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;

        // destroyParticlesTemp.transform.position = playerObject.transform.position;

        if (!destroyParticlesTemp.isPlaying)
        {
            Debug.Log("destroy play particle");
            destroyParticlesTemp.Play();
        }

        /*destroyParticles.transform.position = playerObject.transform.position;
        destroyParticles.Play();*/

        Destroy(this.gameObject,1f);
    }

}


