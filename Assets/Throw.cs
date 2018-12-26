using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Throw : MonoBehaviour {

   // LineRenderer lr;
    public float lineVelocity;
    public float lineAngle;
    public int numOfLineSeg;

    float radianAngle;

    public bool activated;

    public Vector3 Target;
    public float firingAngle = 45.0f;
    public float gravity = 19.8f;

    public Transform Projectile;
    private Transform myTransform;

    void Awake()
    {
        myTransform = transform;
      //  lr = GetComponent<LineRenderer>();
    }

    void Start()
    {
       // StartCoroutine(SimulateProjectile());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            activated = true;
        }

        if(activated)
        {
             Target = Camera.main.ScreenToWorldPoint( Input.mousePosition);

        }
    }





    IEnumerator SimulateProjectile()
    {
        
        

        // Move projectile to the position of throwing object + add some offset if needed.
        Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(Projectile.position, Target);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.

        Quaternion rotation = Quaternion.LookRotation(Target - Projectile.position);

        rotation.y = 0;
        rotation.x = 0;
        Projectile.rotation = rotation;

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            Projectile.Translate(Vx * Time.deltaTime,(Vy - (gravity * elapse_time)) * Time.deltaTime,0 );

            elapse_time += Time.deltaTime;

            yield return null;
        }

        if(elapse_time > flightDuration + 1f)
        {
            transform.position = myTransform.position;
        }
    }








}
