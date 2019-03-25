using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour {

    public static PathGenerator Instance { get; set; }

    [SerializeField]
    public static List<Platform> platforms;
    //List<Platform> sortedPlatforms;

    public static List<Node> nodes;

    public float xSpacing = 4.0f;
    public float ySpacing = 4.0f;

    public float yTolerance = 0.4f;
    public float xTolerance = 0.5f;

    private void Awake()
    {
        platforms = new List<Platform>();
        
        nodes = new List<Node>();
    }

    // Use this for initialization
    void Start () {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        platforms.AddRange(FindObjectsOfType<Platform>());
		//sort the platforms based on the x distance
        for(int i = 0; i < platforms.Count-1; i++)
        {
            for(int j = i+1; j < platforms.Count; j++)
            {
                if(platforms[j].transform.position.x < platforms[i].transform.position.x)
                {
                    Platform temp = platforms[j];
                    platforms[j] = platforms[i];
                    platforms[i] = temp;
                }
            }
        }

        //Form the links between the nodes after sorting is finished
        for(int i = 0; i < platforms.Count; i++)
        {
            //Go through the sorted platforms one by one. First connect the nodes on the platform
            if(i == 0)
            {
                platforms[i].leftNode.parent = null;
                platforms[i].leftNode.child.Add(platforms[i].rightNode);
                platforms[i].rightNode.parent = platforms[i].leftNode;

                //Adding the left and right connections to the respective nodes
                platforms[i].leftNode.rightConnections.Add(platforms[i].rightNode);
                platforms[i].rightNode.leftConnections.Add(platforms[i].leftNode);

                nodes.Add(platforms[i].leftNode);
                nodes.Add(platforms[i].rightNode);
            }
            else
            {
                platforms[i].leftNode.parent = null;
                platforms[i].leftNode.child.Add(platforms[i].rightNode);
                platforms[i].rightNode.parent = platforms[i].leftNode;

                //Adding the left and right connections to the respective nodes
                platforms[i].leftNode.rightConnections.Add(platforms[i].rightNode);
                platforms[i].rightNode.leftConnections.Add(platforms[i].leftNode);


                nodes.Add(platforms[i].leftNode);
                nodes.Add(platforms[i].rightNode);

                //Loop through previous platforms and check for connection. i has greater x position than j
                for (int j = i-1; j >= 0; j--)
                {
                    if (platforms[i].leftNode.position.x < platforms[j].rightNode.position.x &&
                        platforms[i].leftNode.position.x > platforms[j].leftNode.position.x + xTolerance)
                    {
                        //Debug.Log("positions: " + platforms[i].leftNode.position + "     " + platforms[j].leftNode.position + "    " + platforms[j].rightNode.position);
                        float yDist = platforms[i].leftNode.position.y - platforms[j].leftNode.position.y;
                        if (Mathf.Abs(yDist) < ySpacing)
                        {
                            if (Mathf.Sign(yDist) > 0)
                            {
                                if (platforms[i].leftNode.parent == null)
                                    platforms[i].leftNode.parent = platforms[j].leftNode;
                                //Changed. Check this area if the code doesn't work as expected
                                if (platforms[i].tag == "tag_ladder")
                                {
                                    platforms[i].leftNode.rightConnections.Add(platforms[j].rightNode);
                                    platforms[j].rightNode.leftConnections.Add(platforms[i].leftNode);
                                    platforms[j].rightNode.child.Add(platforms[i].leftNode);
                                }

                                platforms[j].leftNode.child.Add(platforms[i].leftNode);

                                platforms[i].leftNode.leftConnections.Add(platforms[j].leftNode);
                                platforms[j].leftNode.rightConnections.Add(platforms[i].leftNode);

                            }
                            else
                            {
                                float yDist2 = platforms[i].rightNode.position.y - platforms[j].rightNode.position.y;
                                if(Mathf.Abs(yDist2) < ySpacing)
                                {
                                    //if 'i' is below 'j'
                                    if (platforms[i].rightNode.position.x > platforms[j].rightNode.position.x + xTolerance)
                                    {
                                        platforms[i].rightNode.child.Add(platforms[j].rightNode);

                                        platforms[i].rightNode.leftConnections.Add(platforms[j].rightNode);
                                        platforms[j].rightNode.rightConnections.Add(platforms[i].rightNode);
                                    }
                                }
                            }
                        }
                    }

                    if (platforms[i].rightNode.position.x < platforms[j].rightNode.position.x - xTolerance &&
                        platforms[i].rightNode.position.x > platforms[j].leftNode.position.x)
                    {
                        float yDist = platforms[i].rightNode.position.y - platforms[j].rightNode.position.y;
                        if (Mathf.Abs(yDist) < ySpacing)
                        {
                            if (Mathf.Sign(yDist) > 0)
                            {
                                if (platforms[i].rightNode.parent == null)
                                    platforms[i].rightNode.parent = platforms[j].rightNode;

                                //Changed this area. If codes doesn't work as expected, check this block
                                if (platforms[i].tag == "tag_ladder")
                                {
                                    platforms[i].rightNode.leftConnections.Add(platforms[j].leftNode);
                                    platforms[j].leftNode.rightConnections.Add(platforms[i].rightNode);
                                    platforms[j].leftNode.child.Add(platforms[i].rightNode);
                                }

                                platforms[j].rightNode.child.Add(platforms[i].rightNode);
                                platforms[i].rightNode.rightConnections.Add(platforms[j].rightNode);
                                platforms[j].rightNode.leftConnections.Add(platforms[i].rightNode);

                                Debug.Log("Reaching");

                            }

                        }

                    }

                    if (platforms[i].leftNode.position.x > platforms[j].rightNode.position.x)
                    {
                        float yDist = platforms[i].leftNode.position.y - platforms[j].rightNode.position.y;
                        float xDist = platforms[i].leftNode.position.x - platforms[j].rightNode.position.x;
                        if (Mathf.Abs(yDist) < ySpacing)
                        {
                            if(Mathf.Abs(xDist) < xSpacing)
                            {
                                if (platforms[i].leftNode.parent == null)
                                    platforms[i].leftNode.parent = platforms[j].rightNode;
                                platforms[j].rightNode.child.Add(platforms[i].leftNode);


                                platforms[i].leftNode.leftConnections.Add(platforms[j].rightNode);
                                platforms[j].rightNode.rightConnections.Add(platforms[i].leftNode);
                            }
                        }
                    }

                    if (platforms[j].leftNode.position.x > platforms[i].leftNode.position.x + xTolerance &&
                       platforms[j].leftNode.position.x < platforms[i].rightNode.position.x - xTolerance)
                    {
                        float yDist = platforms[j].leftNode.position.y - platforms[i].leftNode.position.y;
                        //float xDist = platforms[j].leftNode.position.x - platforms[i].leftNode.position.x;
                        if (Mathf.Abs(yDist) < ySpacing)
                        {
                            if (Mathf.Sign(yDist) > 0)
                            {
                                if (platforms[j].leftNode.parent == null)
                                    platforms[j].leftNode.parent = platforms[i].leftNode;
                                //If the platform is a ladder, connect to the previous platform's right node also
                                if (platforms[j].tag == "tag_ladder")
                                {
                                    //Connect to the right node of the platform also if the platform is a ladder
                                    platforms[j].leftNode.rightConnections.Add(platforms[i].rightNode);
                                    platforms[i].rightNode.leftConnections.Add(platforms[j].leftNode);
                                    platforms[i].rightNode.child.Add(platforms[j].leftNode);
                                }

                                platforms[i].leftNode.child.Add(platforms[j].leftNode);

                                platforms[j].leftNode.leftConnections.Add(platforms[i].leftNode);
                                platforms[i].leftNode.rightConnections.Add(platforms[j].leftNode);

                                Debug.Log("Reaching");
                            }
                        }
                    }

                    if (platforms[j].rightNode.position.x > platforms[i].leftNode.position.x + xTolerance &&
                       platforms[j].rightNode.position.x < platforms[i].rightNode.position.x - xTolerance)
                    {
                        float yDist = platforms[j].rightNode.position.y - platforms[i].rightNode.position.y;
                        //float xDist = platforms[j].rightNode.position.x - platforms[i].rightNode.position.x;
                        if (Mathf.Abs(yDist) < ySpacing)
                        {
                            if (Mathf.Sign(yDist) > 0)
                            {
                                if (platforms[j].rightNode.parent == null)
                                    platforms[j].rightNode.parent = platforms[i].rightNode;

                                //If the node belongs to a ladder, connect to the left node also
                                if (platforms[j].tag == "tag_ladder")
                                {
                                    platforms[j].rightNode.leftConnections.Add(platforms[i].leftNode);
                                    platforms[i].leftNode.rightConnections.Add(platforms[j].rightNode);
                                    platforms[i].leftNode.child.Add(platforms[j].rightNode);
                                }
                                
                                platforms[i].rightNode.child.Add(platforms[j].rightNode);
                                
                                platforms[j].rightNode.rightConnections.Add(platforms[i].rightNode);
                                platforms[i].rightNode.leftConnections.Add(platforms[j].rightNode);

                            }
                        }
                    }

                    //if (platforms[j].tag == "tag_ladder")
                    //{
                    //    if (platforms[j].leftNode.position.x < platforms[i].leftNode.position.x)
                    //    {
                    //        float yDist = platforms[j].leftNode.position.y - platforms[i].rightNode.position.y;
                    //        float xDist = platforms[j].leftNode.position.x - platforms[i].rightNode.position.x;
                    //        if (Mathf.Abs(yDist) < ySpacing)
                    //        {
                    //            if (Mathf.Abs(xDist) < xSpacing)
                    //            {
                    //                platforms[j].leftNode.rightConnections.Add(platforms[i].leftNode);
                    //                platforms[i].leftNode.leftConnections.Add(platforms[j].leftNode);
                    //                platforms[i].leftNode.child.Add(platforms[j].leftNode);
                    //            }
                    //        }
                    //    }
                    //}

                    if (platforms[i].tag == "tag_ladder")
                    {
                        if (platforms[i].rightNode.position.x > platforms[j].rightNode.position.x)
                        {
                            float yDist = platforms[i].rightNode.position.y - platforms[j].rightNode.position.y;
                            float xDist = platforms[i].rightNode.position.x - platforms[j].rightNode.position.x;
                            if (yDist < 4.0f && yDist > 0)
                            {
                                if (Mathf.Abs(xDist) < 4.0f)
                                {
                                    platforms[i].rightNode.leftConnections.Add(platforms[j].rightNode);
                                    platforms[j].rightNode.rightConnections.Add(platforms[i].rightNode);
                                    platforms[j].rightNode.child.Add(platforms[i].rightNode);
                                }
                            }
                        }
                    }
                }
            }

        }
        CheckAllNodes();
	}

    void CheckAllNodes()
    {
        for(int i = 0; i < nodes.Count; i++)
        {
            //Debug.Log("Right.....");
            for(int j = 0; j < nodes[i].rightConnections.Count-1; j++)
            {
                Node n = nodes[i].rightConnections[j];
                Vector3 d1 = nodes[i].rightConnections[j].position - nodes[i].position;
                float dist1 = Vector3.Magnitude(d1);

                if (n.platform == nodes[i].platform)
                    continue;

                //Debug.Log("node.....");
                for (int k = j+1; k < nodes[i].rightConnections.Count; k++)
                {
                    if (nodes[i].platform.tag == "tag_ladder" || nodes[i].rightConnections[j].platform.tag == "tag_ladder" ||
                                nodes[i].rightConnections[k].platform.tag == "tag_ladder")
                    {
                        continue;
                    }



                    Vector3 d2 = nodes[i].rightConnections[k].position - nodes[i].position;
                    float dist2 = Vector3.Magnitude(d2);
                    Debug.Log("Angle: " + Vector3.Angle(d1, d2));
                    //Angle check
                    if(Mathf.Abs(Vector3.Angle(d1, d2)) < 35 )
                    {
                         if(dist2 > dist1)
                        {
                            
                            nodes[i].rightConnections[k].child.Remove(nodes[i]);
                            nodes[i].child.Remove(nodes[i].rightConnections[k]);

                            nodes[i].rightConnections[k].leftConnections.Remove(nodes[i]);
                            nodes[i].rightConnections.Remove(nodes[i].rightConnections[k]);
                            
                        }
                        else
                        {
                            
                            nodes[i].rightConnections[j].child.Remove(nodes[i]);
                            nodes[i].child.Remove(nodes[i].rightConnections[j]);

                            nodes[i].rightConnections[j].leftConnections.Remove(nodes[i]);
                            nodes[i].rightConnections.Remove(nodes[i].rightConnections[j]);
                            
                        }
                    }

                    //x distance and y direction checks
                    if (k < nodes[i].rightConnections.Count)
                    {
                        if (Mathf.Abs(Vector3.Angle(d1, d2)) < 15)
                        {
                            if (Mathf.Abs(d1.y) < Mathf.Abs(d2.y))
                            {
                                if (Mathf.Abs(d1.x) < 1.5f)
                                {
                                    if (d2.y < 0)
                                    {
                                        nodes[i].rightConnections[k].child.Remove(nodes[i]);
                                        nodes[i].child.Remove(nodes[i].rightConnections[k]);

                                        nodes[i].rightConnections[k].leftConnections.Remove(nodes[i]);
                                        nodes[i].rightConnections.Remove(nodes[i].rightConnections[k]);
                                    }
                                }
                            }
                            else
                            {
                                if (Mathf.Abs(d2.x) < 1.5f)
                                {
                                    if (d1.y < 0)
                                    {
                                        nodes[i].rightConnections[j].child.Remove(nodes[i]);
                                        nodes[i].child.Remove(nodes[i].rightConnections[j]);

                                        nodes[i].rightConnections[j].leftConnections.Remove(nodes[i]);
                                        nodes[i].rightConnections.Remove(nodes[i].rightConnections[j]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes[i].leftConnections.Count - 1; j++)
            {
                Node n = nodes[i].leftConnections[j];
                Vector3 d1 = nodes[i].leftConnections[j].position - nodes[i].position;
                float dist1 = Vector3.Magnitude(d1);

                if (n.platform == nodes[i].platform)
                    continue;

                for (int k = j + 1; k < nodes[i].leftConnections.Count; k++)
                {

                    if (nodes[i].platform.tag == "tag_ladder" || nodes[i].leftConnections[j].platform.tag == "tag_ladder" ||
                                nodes[i].leftConnections[k].platform.tag == "tag_ladder")
                    {
                        continue;
                    }

                    Vector3 d2 = nodes[i].leftConnections[k].position - nodes[i].position;
                    float dist2 = Vector3.Magnitude(d2);

                    if (Mathf.Abs(Vector3.Angle(d1, d2)) < 15)
                    {
                        if (dist2 > dist1)
                        {
                            
                            nodes[i].leftConnections[k].child.Remove(nodes[i]);
                            nodes[i].child.Remove(nodes[i].leftConnections[k]);

                            nodes[i].leftConnections[k].rightConnections.Remove(nodes[i]);
                            nodes[i].leftConnections.Remove(nodes[i].leftConnections[k]);
                            
                        }
                        else
                        {
                            
                            nodes[i].leftConnections[j].child.Remove(nodes[i]);
                            nodes[i].child.Remove(nodes[i].leftConnections[j]);


                            nodes[i].leftConnections[j].rightConnections.Remove(nodes[i]);
                            nodes[i].leftConnections.Remove(nodes[i].leftConnections[j]);
                            
                        }
                    }

                    //x distance and y direction checks
                    if (k < nodes[i].leftConnections.Count)
                    {
                        if (Mathf.Abs(Vector3.Angle(d1, d2)) < 15)
                        {
                            if (Mathf.Abs(d1.y) < Mathf.Abs(d2.y))
                            {
                                if (Mathf.Abs(d1.x) < 1.5f)
                                {
                                    if (d2.y < 0)
                                    {
                                        nodes[i].leftConnections[k].child.Remove(nodes[i]);
                                        nodes[i].child.Remove(nodes[i].leftConnections[k]);

                                        nodes[i].leftConnections[k].rightConnections.Remove(nodes[i]);
                                        nodes[i].leftConnections.Remove(nodes[i].leftConnections[k]);
                                    }
                                }
                            }
                            else
                            {
                                if (Mathf.Abs(d2.x) < 1.5f)
                                {
                                    if (d1.y < 0)
                                    {
                                        nodes[i].leftConnections[j].child.Remove(nodes[i]);
                                        nodes[i].child.Remove(nodes[i].leftConnections[j]);

                                        nodes[i].leftConnections[j].rightConnections.Remove(nodes[i]);
                                        nodes[i].leftConnections.Remove(nodes[i].leftConnections[j]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }

    public bool displayCompletePath = false;
    public bool displayRightPath;
    public bool displayLeftNode;
    public bool displayRightNode;

    public int platformNumber = 0;

    private void OnDrawGizmos()
    {
        if (displayCompletePath)
        {
            if (nodes!=null)
            {
                List<Node> tempNodes = nodes;
                for (int i = 0; i < tempNodes.Count; i++)
                {
                    if (tempNodes[i].child.Count > 0)
                        ChildLooper(tempNodes[i].child, tempNodes[i]);

                }
            }
        }
        else
        {
            Gizmos.color = Color.red;
            if (displayRightPath)
            {
                if(displayLeftNode)
                    for(int i = 0; i < platforms[platformNumber].leftNode.rightConnections.Count; i++)
                    {
                        Gizmos.DrawLine(platforms[platformNumber].leftNode.position, platforms[platformNumber].leftNode.rightConnections[i].position);
                    }

                if(displayRightNode)
                    for (int i = 0; i < platforms[platformNumber].rightNode.rightConnections.Count; i++)
                    {
                        Gizmos.DrawLine(platforms[platformNumber].rightNode.position, platforms[platformNumber].rightNode.rightConnections[i].position);
                    }
            }
            else
            {
                if(displayLeftNode)
                    for (int i = 0; i < platforms[platformNumber].leftNode.leftConnections.Count; i++)
                    {
                        Gizmos.DrawLine(platforms[platformNumber].leftNode.position, platforms[platformNumber].leftNode.leftConnections[i].position);
                    }

                if(displayRightNode)
                    for (int i = 0; i < platforms[platformNumber].rightNode.leftConnections.Count; i++)
                    {
                        Gizmos.DrawLine(platforms[platformNumber].rightNode.position, platforms[platformNumber].rightNode.leftConnections[i].position);
                    }
            }
        }
    }

    void ChildLooper(List<Node> children, Node callingNode)
    {
        if (children.Count > 0)
        {
            for (int i = 0; i < children.Count; i++)
            {
                Node child = children[i];
                if (child.parent != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(callingNode.position, child.position);
                }
                if(child.child.Count > 0)
                    ChildLooper(child.child, children[i]);
            }
        }
        return;
    }

}
