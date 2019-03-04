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

    public float rightNodesSpacingVariable = 1.1f;
    public float leftNodeSpacingVariable = 1.1f;

    public float verticalAdjustment = 0.0f;

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
            leftNode.position = new Vector3(verticalAdjustment, -extents.y / leftNodeSpacingVariable, 0);
            rightNode.position = new Vector3(verticalAdjustment, extents.y / rightNodesSpacingVariable, 0);
        }
        else
        {
            leftNode.position = new Vector3(-extents.x / leftNodeSpacingVariable, extents.y + verticalAdjustment, 0);
            rightNode.position = new Vector3(extents.x / rightNodesSpacingVariable, extents.y + verticalAdjustment, 0);
        }

        //Matrix4x4 parentTransform = Matrix4x4.TRS(transform.parent.position, transform.parent.rotation, transform.parent.localScale);
        Matrix4x4 platformMatrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.lossyScale);
        //platformMatrix = platformMatrix * parentTransform;


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
                    DynamicButton d = VirtualJoystick.CreateDynamicButton("tag_ladder");
                    if (!d.active)
                    {
                        VirtualJoystick.EnableDynamicButton(d);
                        d.button.onClick.AddListener(() =>
                        {

                            p.userInputs.climbPressed = true;

                            if (p.State.Equals(typeof(ClimbingState)))
                            {
                                //Disable the button
                                VirtualJoystick.DisableDynamicButton(d);
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
                VirtualJoystick.DisableDynamicButton("tag_ladder");
            }
        }
    }


    private void Update()
    {
        //TODO: Optimize the node calculation code

        #region Temp

        spriteRenderer = GetComponent<SpriteRenderer>();
        Vector3 extents = spriteRenderer.sprite.bounds.extents;

        if (gameObject.tag == "tag_ladder")
        {
            leftNode.position = new Vector3(verticalAdjustment, -extents.y / leftNodeSpacingVariable, 0);
            rightNode.position = new Vector3(verticalAdjustment, extents.y / rightNodesSpacingVariable, 0);
        }
        else
        {
            leftNode.position = new Vector3(-extents.x / leftNodeSpacingVariable, extents.y + verticalAdjustment, 0);
            rightNode.position = new Vector3(extents.x / rightNodesSpacingVariable, extents.y + verticalAdjustment, 0);
        }

        Matrix4x4 platformMatrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.lossyScale);

        //Matrix4x4 parentTransform = Matrix4x4.TRS(transform.parent.position, transform.parent.rotation, transform.parent.localScale);
        
        //platformMatrix = platformMatrix * parentTransform;

        leftNode.position = platformMatrix.MultiplyPoint3x4(leftNode.position);
        rightNode.position = platformMatrix.MultiplyPoint3x4(rightNode.position);

        #endregion
    }

    public void UseItem(ItemBase item)
    {
        if (item != null)
        {
            Debug.Log("Item is: " + item);
            ItemsDescription description = item.gameObject.GetComponent<ItemsDescription>();
            Corrosion c = GetComponentInParent<Corrosion>();
            if (c != null && description != null)
            {
                if (description.GetItemType().GetType() == (typeof(AcidsList)))
                {
                    //Corrode the item based on it's pH value.....Lesser pH, more corrosion
                    //100 will be taken as a reference to calculate corrosion damage to all the platforms
                    Debug.Log("Trying to corrode");
                    float value = (1 / (float)((description.pHValue) + 1)) * 100;
                    c.Corrode(value);
                }
                else
                if (description.GetItemType().GetType() == (typeof(BasesList)))
                {
                    //Corrode the item based on it's pH value.....Higher pH, more corrosion
                    //100 will be taken as a reference to calculate corrosion damage to all the platforms
                    int alteratedpH = 7 - (description.pHValue - 7);

                    float value = (1 / ((alteratedpH) + 1)) * 100;
                    c.Corrode(value);
                }
            }
            else
            {
                Debug.Log("Dude....There is no corrosion");
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(leftNode.position, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(rightNode.position, 0.1f);
    }

}
