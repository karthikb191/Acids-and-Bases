using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUtility : MonoBehaviour
{
    public static void CastRayAndOrientPlayer(Character c, out float angle, out float yCorr)
    {
        //Cast the ray and have the player tile according to the angle of the ground
        //TODO: Add more to this once other types of platforms are ready
        LayerMask mask = LayerMask.NameToLayer("Platform");
        RaycastHit2D hit = Physics2D.CircleCast(c.groundCheck.transform.position, c.groundCheckCircleRadius, -c.gameObject.transform.up, 0.5f, mask);
        
        //Get the angle with the world right vector
        Vector3 dir = c.gameObject.transform.up;

        yCorr = 0;
        angle = 0;
        //Debug.Log("collider is: " + hit.collider);

        if (hit.collider != null)
        {
            angle = Vector3.Angle(Vector3.up, hit.normal);
            
            if (c.State.GetType() != typeof(JumpingState))
            {
                if (angle < 65)
                {
                    if (angle > 0.05f)
                        dir = Vector3.Lerp(c.gameObject.transform.up, hit.normal, 0.2f);
                    else
                        dir = hit.normal;

                    //c.playerUpOrientation = dir;
                }
            }

        }
        else
        {

            //Debug.Log("Player up orientation set to world");
            dir = Vector3.Lerp(c.gameObject.transform.up, Vector3.up, 0.15f);
            c.playerUpOrientation = dir;

        }
        if (c.State.GetType() != typeof(FallingState))
        {
            //Debug.Log("Distance to ground is: " + Vector3.Distance(hit.point, gameObject.transform.position));

            yCorr = -(c.gameObject.transform.position.y - hit.point.y);
        }


    }

    public static void CastRayAndOrientPlayer(Character c, RaycastHit2D[] info, out float angle, out float yCorr)
    {
        //Cast the ray and have the player tile according to the angle of the ground
        //TODO: Add more to this once other types of platforms are ready
        LayerMask mask = LayerMask.NameToLayer("Platform");
        RaycastHit2D? hit = null; //= Physics2D.CircleCast(c.groundCheck.transform.position, c.groundCheckCircleRadius, -c.gameObject.transform.up, 0.5f, mask);
        for(int i = 0; i < info.Length; i++)
        {
            if(info[i].collider.gameObject.layer == mask)
            {
                hit = info[i];
                break;
            }
        }

        //Get the angle with the world right vector
        Vector3 dir = c.playerUpOrientation;

        yCorr = 0;
        angle = 0;
        //Debug.Log("collider is: " + hit.collider);

        if (hit.HasValue && hit.Value.transform.tag != "tag_ladder" && c.State.GetType() != typeof(ClimbingState))
        {
            //Debug.Log("Hit has value");
            if (hit.Value.collider != null)
            {
                angle = Vector3.Angle(Vector3.up, hit.Value.normal);

                if (c.State.GetType() != typeof(JumpingState))
                {
                    if (angle < 65)
                    {
                        if (angle > 0.05f)
                            dir = Vector3.Lerp(c.gameObject.transform.up, hit.Value.normal, 0.2f);
                        else
                            dir = hit.Value.normal;

                        c.playerUpOrientation = dir;
                    }
                }
            }
            else
            {
                //Debug.Log("Player up orientation set to world");
                dir = Vector3.Lerp(c.gameObject.transform.up, Vector3.up, 0.15f);
                c.playerUpOrientation = dir;
            }

            if (c.State.GetType() != typeof(FallingState) && c.State.GetType() != typeof(JumpingState))
            {
                //Debug.Log("Distance to ground is: " + Vector3.Distance(hit.point, gameObject.transform.position));

                yCorr = -((c.gameObject.transform.position.y - hit.Value.point.y) - c.groundCheckCircleRadius * 0.5f);
                //Debug.Log("Y Correction is: " + yCorr);
                //Debug.Log("Hit point: " + hit.Value.point.y);
            }

        }
        else
        {
            dir = Vector3.Lerp(c.gameObject.transform.up, Vector3.up, 0.15f);
            c.playerUpOrientation = dir;
        }
    }

    public static IEnumerator BlockInputs(Character c, bool horizontal = true, bool vertical = true,
                                            bool rotation = true, float duration = 0.5f)
    {

        //Debug.Log("inside the blocks block");
        if (horizontal)
        {
            c.horizontalInputBlock = true;
            c.currentLinearSpeed = 0;
        }
        if (vertical)
        {
            c.verticalInputBlock = true;
            //currentJumpSpeed = 0;
        }

        yield return new WaitForSeconds(duration);

        c.horizontalInputBlock = false;
        c.verticalInputBlock = false;

        c.blockRoutine = null;
        yield break;

    }


}
