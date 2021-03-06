﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemBase : MonoBehaviour {

    public bool thrown = false;
    
    public ParticleSystem travelParticles;
    public ParticleSystem destroyParticles;

    ParticleSystem travelParticlesTemp;
    ParticleSystem destroyParticlesTemp;

   
    //bool used = false;

    public int pickedCount = 0;

    public int maxCount = 0;

    bool setFocus;

    public GameObject playerObject;

    public ItemProperties itemProperties;

    public float speed = 20.0f;

    public Vector3 targetScale = Vector3.one;

    public bool isFromEnemy = false;

    public float colliderRadius = 2;

    public delegate void ItemPickedUp(ItemBase i);
    public static event ItemPickedUp ItemPickedUpEvent;

    private void Start()
    {
        playerObject = FindObjectOfType<Player>().gameObject;
        itemProperties.itemDescription.PhIndicatorImage = GetComponent<SpriteRenderer>().sprite;

        if(travelParticles != null)
        {
            travelParticlesTemp = Instantiate(travelParticles,transform);
            travelParticlesTemp.transform.position = this.transform.position;
        }

        if(destroyParticles != null)
        {
            destroyParticlesTemp = Instantiate(destroyParticles, transform);
            destroyParticlesTemp.transform.position = this.transform.position;
        }

        if(maxCount == 0)
        {
            maxCount = 1;
        }
    }


    private void Update()
    {
        if(thrown)
        {
            CheckCollision();
            TravelParticleEffect();
        }
    }

    public virtual void Use()
    {       
        transform.parent = null;
        gameObject.SetActive(false);
    }

    public virtual void Throw(Vector3 target, float speed)
    {
        if (gameObject.activeSelf)
        { 
            if (isFromEnemy)
            {
                playerObject = transform.GetComponentInParent<Enemy>().gameObject;
                thrown = true;
                StartCoroutine(ThrowProjectile(target, 45));
                //transform.parent = null;
            }
            else
            {
                // playerObject = transform.GetComponentInParent<Player>().gameObject;
                Debug.Log("Thrown from: ____>>>>" + playerObject.name);
                ThrowCalculations(target, speed);
                StartCoroutine(ThrowProjectile(target, angleOfThrow));
            }
            //Item's parent must be set to null so that it can travel along its path
            transform.parent = null;
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
                        VirtualJoystick.DisableDynamicButton(d);
                        ItemCountSelection.instance.Activate(maxCount);
                        ItemCountSelection.instance.item = this;
                        
                        if(playerObject.GetComponent<Player>())
                        {
                            CheckPointManager.RegisterCheckPointEvent += playerObject.GetComponentInChildren<PlayerInventory>().Save;
                        }

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
            ItemCountSelection.instance.Dectivate();
            ///Testing purpose to be removed later
            if(playerObject.GetComponentInChildren<PlayerInventory>() != null)
            {
              //  playerObject.GetComponentInChildren<PlayerInventory>().SaveInventoryData();
            }
            //////
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
            gameObject.SetActive(false);
            yield break;
        }
        else
        {
            gameObject.transform.position = targetPosition;
        }
        Debug.Log("Parented");

        if (gameObject.transform.position == targetPosition)
        {
            gameObject.SetActive(false);

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
    
    IEnumerator ThrowProjectile(Vector3 Target, float firingAngle)
    {
        //Debug.Log("Target to reach"+Target);
        float target_Distance = Vector3.Distance(gameObject.transform.position, Target);
        //Debug.Log("target_Distance" + target_Distance);
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
                transform.Translate(Vx * GameManager.Instance.DeltaTime, (Vy - (speed * elapse_time)) * GameManager.Instance.DeltaTime, 0);

            else
                transform.Translate(Vx * GameManager.Instance.DeltaTime * directionOfTranslation, (Vy - (speed * elapse_time)) 
                                        * GameManager.Instance.DeltaTime, 0);

            elapse_time += Time.deltaTime;
            //Debug.Log("elapse_time" + elapse_time);

            yield return null;
        }

        if (elapse_time >= flightDuration)
        {
            transform.rotation = Quaternion.identity;
            elapse_time = 0;


            if(isFromEnemy)
            {
                //Adding and item as soon as it hits something to the evemy inventory because we don't want items to be limited on enemies
                //playerObject.gameObject.GetComponentInChildren<EnemyInventory>().AddItem(this);
                //playerObject.gameObject.GetComponentInChildren<EnemyInventory>().SetActiveItem();

                StartCoroutine(DestroyParticleEffect());
                this.gameObject.SetActive(false);

                //this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                //this.AlignPos(playerObject.gameObject.GetComponentInParent<Character>().Hand.transform.position, playerObject.gameObject.GetComponentInParent<Character>());
                //this.transform.position = playerObject.gameObject.GetComponentInParent<Character>().Hand.transform.position;
                //this.transform.parent = playerObject.gameObject.GetComponentInParent<Character>().Hand.transform;

                //this.gameObject.SetActive(false);
                //this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }

            else
            {
                //gameObject.SetActive(false);
                StartCoroutine(DestroyParticleEffect());
                transform.parent = null;
            }
          
            yield break;
        }
    }

    //These two add item functions are intended for the player's pickup
    public void AddItems(float count)
    {
        for(int i = 0; i<count;i++)
        {
            if((maxCount - pickedCount) > 1)
            {
                //GameObject temp = Instantiate(this.gameObject) as GameObject;
                //temp.GetComponent<ItemBase>().playerObject = playerObject;
                //temp.GetComponent<ItemBase>().pickedCount = 0;
                //temp.GetComponent<ItemBase>().maxCount = 1;          
                //AddItem(temp.GetComponent<ItemBase>());                   
                AddItem(this);
                pickedCount++;
                //Debug.Log("instantiated item" + temp.gameObject.name);
                Debug.Log("Picked Count" + pickedCount);
            }
            else
            {
                Debug.Log("LastItem");
                AddItem(this);
                
                //Add the active item to the inventory pool, which automatically deactiavtes the item
                Inventory.AddItemToPool(this);
            }
            
        }

        maxCount = Mathf.Abs(maxCount - pickedCount);

        Debug.Log("Max COunt" + maxCount);
    }

    public void AddItem(ItemBase item)
    {
        if (ItemPickedUpEvent != null)
            ItemPickedUpEvent(item);
        
        if (playerObject.GetComponentInChildren<PlayerInventory>().activeItem == null)
        {
            //If active item is null, then get the item from the pool
            item = Inventory.GetItemFromPool(item.itemProperties, true);

            playerObject.GetComponentInChildren<PlayerInventory>().activeItem = item;
            playerObject.GetComponentInChildren<PlayerInventory>().AddItem(item);
            //item.StartCoroutine(AlignPos(playerObject.GetComponent<Character>().Hand.transform.position, playerObject.GetComponentInChildren<Character>()));
            item.transform.position = playerObject.GetComponent<Character>().Hand.transform.position;

            item.gameObject.transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;
            item.gameObject.transform.localScale = targetScale;
            item.gameObject.SetActive(true);
            Debug.Log("Active item set");
            
        }
        else
        {
            playerObject.GetComponentInChildren<Inventory>().AddItem(item);


            //item.StartCoroutine(AlignPos(playerObject.GetComponent<Character>().Hand.transform.position, playerObject.GetComponentInChildren<Character>()));
            //TODO: Logic to this code must be changed later
            //item.transform.position = playerObject.GetComponent<Character>().Hand.transform.position;
            //item.transform.parent = playerObject.GetComponentInChildren<Character>().Hand.transform;
            //item.gameObject.transform.localScale = targetScale;
            //item.gameObject.SetActive(false);
        }

        //Get the player component. If player is null, log error.
        if (playerObject.GetComponent<Player>() && itemProperties.itemDescription != null)
        {
            if(itemProperties.itemDescription.itemType == ItemType.Indicator)
            {
                if(playerObject.GetComponent<Player>().GetPlayerStatus().GetpHIndicator() == null)
                {
                    playerObject.GetComponent<Player>().GetPlayerStatus().SetpHIndicator(itemProperties.itemDescription);
                }
                else if (//TODO: this is needed (moidfy) !playerObject.GetComponent<Player>().GetPlayerStatus().GetpHIndicator() ||
                    playerObject.GetComponent<Player>().GetPlayerStatus().GetpHIndicator().indicatorType == itemProperties.itemDescription.indicatorType)
                        playerObject.GetComponent<Player>().GetPlayerStatus().SetpHIndicator(itemProperties.itemDescription);
            }
        }
        else 
        {
            Debug.LogError("Ph is not present");
        }
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

       // Debug.Log("target for throw" + target + "<<<<<<====>>>>>>");

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
                StartCoroutine(DestroyParticleEffect());
                break;
            }

            if (collidedWith[i].transform.GetComponent<Switch>())
            {
                Debug.Log("Collided with Switch");
                collidedWith[i].transform.GetComponent<Switch>().ActivateDoor();
                StartCoroutine(DestroyParticleEffect());
                //Destroy(this.gameObject);
                //Inventory.AddItemToPool(this);
                break;
            }

            //If the item is from the enemy, add it back to the enemy
            if(isFromEnemy)
                playerObject.gameObject.GetComponentInChildren<EnemyInventory>().AddItem(this);

            if (collidedWith[i].transform.GetComponent<Character>() != null && collidedWith[i].transform.GetComponent<Character>().gameObject != playerObject.gameObject)
            {
                Use(collidedWith[i].transform.GetComponent<Character>());
                if(isFromEnemy)
                {
                    Debug.Log("Used called from:   " + playerObject.name);
                    Debug.Log("Used on :    " + collidedWith[i].transform.GetComponentInParent<Character>().gameObject);
                    //this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    //this.AlignPos(playerObject.gameObject.GetComponentInParent<Character>().Hand.transform.position, playerObject.gameObject.GetComponentInParent<Character>());
                    Debug.Log("Player object is: " + playerObject);
                    //Get the item back
                    this.transform.position = playerObject.gameObject.GetComponentInParent<Character>().Hand.transform.position;
                    this.transform.parent= playerObject.gameObject.GetComponentInParent<Character>().Hand.transform;

                    //Adding item to the enemy list
                    Debug.Log("slots: " + playerObject.gameObject.GetComponentInChildren<EnemyInventory>());
                    

                    this.gameObject.SetActive(false);
                    //this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    thrown = false;
                    break;
                }
                else
                {
                    //Inventory.AddItemToPool(this);
                    StartCoroutine(DestroyParticleEffect());
                    break;
                }
            }
        }
    }

    void TravelParticleEffect()
    {

        //travelParticles.transform.position = playerObject.transform.position;
        //travelParticles.Play();
        //travelParticlesTemp.transform.position = playerObject.transform.position; 
        

        if(travelParticlesTemp != null && !travelParticlesTemp.isPlaying)
        {
            travelParticlesTemp.Play();
          //  Debug.Log("PLaying particle");
        }
    }

    IEnumerator DestroyParticleEffect()
    {
        //this.gameObject.GetComponent<SpriteRenderer>().enabled = false;

        // destroyParticlesTemp.transform.position = playerObject.transform.position;

        if(destroyParticlesTemp != null)
        {
            if (!destroyParticlesTemp.isPlaying)
            {
                Debug.Log("destroy play particle");
                destroyParticlesTemp.Play();
            }
        }
        /*destroyParticles.transform.position = playerObject.transform.position;
        destroyParticles.Play();*/
        yield return new WaitForSeconds(1);
        Inventory.AddItemToPool(this);
        //Destroy(this.gameObject,1f);
    }

}


