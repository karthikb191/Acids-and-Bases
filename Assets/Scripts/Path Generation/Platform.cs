using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    public Vector3 position;
    public Node parent = null;
    //TODO: This is for visualization purposes. Remove this in the final build
    public List<Node> child = new List<Node>();
    public List<Node> leftConnections = new List<Node>();
    public List<Node> rightConnections = new List<Node>();
    public GridCell gridCell = null;
    public Platform platform;
}

[RequireComponent(typeof(BoxCollider2D))]
[ExecuteInEditMode]
public class Platform : MonoBehaviour {
    
    public Node leftNode = new Node();
    
    public Node rightNode = new Node();

    SpriteRenderer spriteRenderer;
    //BoxCollider2D boxCollider;
    public float width = 0;
    public float height = 0;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Vector3 extents = spriteRenderer.sprite.bounds.extents;
        //leftNode.position = new Vector3(gameObject.transform.position.x - extents.x,
        //                                    gameObject.transform.position.y + extents.y,
        //                                    gameObject.transform.position.z);
        //rightNode.position = new Vector3(gameObject.transform.position.x + extents.x,
        //                                    gameObject.transform.position.y + extents.y,
        //                                    gameObject.transform.position.z);

        if(gameObject.tag == "tag_ladder")
        {
            leftNode.position = new Vector3(0, -extents.y / 1.1f, 0);
            rightNode.position = new Vector3(0, extents.y / 1.1f, 0);
        }
        else
        {
            leftNode.position = new Vector3(-extents.x / 1.1f, extents.y, 0);
            rightNode.position = new Vector3(extents.x / 1.1f, extents.y, 0);
        }

        Matrix4x4 platformMatrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);
        leftNode.position = platformMatrix.MultiplyPoint3x4(leftNode.position);
        rightNode.position = platformMatrix.MultiplyPoint3x4(rightNode.position);

        leftNode.platform = this;
        rightNode.platform = this;

        width = extents.x * gameObject.transform.localScale.x * 2;
        height = extents.y * gameObject.transform.localScale.y * 2;

        //Adding grid cells for both the nodes so that they register themselves to a certain part of the world
        GridIndex index;
        index = WorldGrid.GetGridIndex(leftNode.position);
        leftNode.gridCell = WorldGrid.GetTheWorldGridCell(index);
        WorldGrid.AddToCell(leftNode, leftNode.gridCell);

        index = WorldGrid.GetGridIndex(rightNode.position);
        rightNode.gridCell = WorldGrid.GetTheWorldGridCell(index);
        WorldGrid.AddToCell(rightNode, rightNode.gridCell);
        
    }

    Player playerOnFocus = null;

    
    void OnTriggerStay2D(Collider2D collider)
    {
        if(gameObject.tag == "tag_ladder")
        {
            Player p = collider.GetComponent<Player>();
            if (p != null)
            {
                //Debug.Log("Player detected");
                playerOnFocus = p;
                if (p.State.Equals(typeof(IdleState)))
                {
                    //Enable the button
                    DynamicButton d = VirtualJoystick.CreateButton("tag_ladder");
                    if (!d.active)
                    {
                        VirtualJoystick.EnableButton(d);
                        d.button.onClick.AddListener(() =>
                        {

                            p.userInputs.climbPressed = true;

                            if (p.State.Equals(typeof(ClimbingState)))
                            {
                                //Disable the button
                                VirtualJoystick.DisableButton(d);
                            }
                        });
                    }
                }

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (playerOnFocus != null)
        {
            if (collider.gameObject == playerOnFocus.gameObject)
            {
                playerOnFocus = null;

                //Disable the button
                VirtualJoystick.DisableButton("tag_ladder");
            }
        }
    }


    private void Update()
    {
        //TODO: Optimize the node calculation code

        #region Temp
        spriteRenderer = GetComponent<SpriteRenderer>();
        Vector3 extents = spriteRenderer.sprite.bounds.extents;
        //leftNode.position = new Vector3(gameObject.transform.position.x - extents.x,
        //                                    gameObject.transform.position.y + extents.y,
        //                                    gameObject.transform.position.z);
        //rightNode.position = new Vector3(gameObject.transform.position.x + extents.x,
        //                                    gameObject.transform.position.y + extents.y,
        //                                    gameObject.transform.position.z);

        if (gameObject.tag == "tag_ladder")
        {
            leftNode.position = new Vector3(0, -extents.y / 1.1f, 0);
            rightNode.position = new Vector3(0, extents.y / 1.1f, 0);
        }
        else
        {
            leftNode.position = new Vector3(-extents.x / 1.1f, extents.y, 0);
            rightNode.position = new Vector3(extents.x / 1.1f, extents.y, 0);
        }

        Matrix4x4 platformMatrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);
        leftNode.position = platformMatrix.MultiplyPoint3x4(leftNode.position);
        rightNode.position = platformMatrix.MultiplyPoint3x4(rightNode.position);

        #endregion
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(leftNode.position, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(rightNode.position, 0.1f);
    }

}
