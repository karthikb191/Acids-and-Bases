using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// The entire purpose of this class is to get the vertices of the sprite and form proper line segments
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class LightBlocker : MonoBehaviour {
    SpriteRenderer sr;
    private Vector2[] spriteVertices;
    public Vector3[] vertices;
    public ushort[] triangles;
    public Vector3[] lineSegments;

    public bool lightReceiving = false;
	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        //Getting the 2d Vertices and converting them to 3d
        spriteVertices = sr.sprite.vertices;
        vertices = new Vector3[spriteVertices.Length];
        for(int i = 0; i< spriteVertices.Length; i++)
        {
            vertices[i] = spriteVertices[i];
        }

        triangles = sr.sprite.triangles;
        lineSegments = new Vector3[4];

        vertices[1] = sr.sprite.vertices[2];
        vertices[2] = sr.sprite.vertices[1];

        //Convert the vertices to world coordinates
        Matrix4x4 trs = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);

        for(int i = 0; i < vertices.Length; i++) {
            vertices[i] = trs.MultiplyPoint3x4(new Vector3(spriteVertices[i].x, spriteVertices[i].y, 0));
        }
        //swap vertices 2 and 3
        Vector3 temp = vertices[1];
        vertices[1] = vertices[2];
        vertices[2] = temp;

        lineSegments[0] = vertices[1] - vertices[0];
        lineSegments[1] = vertices[2] - vertices[1];
        lineSegments[2] = vertices[3] - vertices[2];
        lineSegments[3] = vertices[0] - vertices[3];

        //Debug.Log("Line segment: " + lineSegments[0]);
        //Debug.Log("Line segment: " + lineSegments[1]);
        //Debug.Log("Line segment: " + lineSegments[2]);
        //Debug.Log("Line segment: " + lineSegments[3]);
	}
	
	// Update is called once per frame
	void Update () {
        //Screen coordinates update
        Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, LightMap.dist));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, LightMap.dist));
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, LightMap.dist));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, LightMap.dist));

        //sprite position updates
        Matrix4x4 trs = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.lossyScale);

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = trs.MultiplyPoint3x4(new Vector3(spriteVertices[i].x, spriteVertices[i].y, 0));
        }
        //swap vertices 2 and 3
        Vector3 temp = vertices[1];
        vertices[1] = vertices[2];
        vertices[2] = temp;

        lineSegments[0] = vertices[1] - vertices[0];
        lineSegments[1] = vertices[2] - vertices[1];
        lineSegments[2] = vertices[3] - vertices[2];
        lineSegments[3] = vertices[0] - vertices[3];

        //TODO: Add multiple conditions for possible platform orientations
        if (vertices[0].x < topLeft.x)
        {
            //Debug.Log("vertex 0 less than left");
            //Check the other vertices if they are within the screen coordinates
            //get the intersection of first and third line segments with left world border
            if(vertices[1].x > topLeft.x)
            {
                Vector3 p = vertices[0];
                Vector3 r = lineSegments[0];

                Vector3 q = bottomLeft;
                Vector3 s = (topLeft - bottomLeft);
                float v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                float t = (q.x + v * s.x - p.x) / r.x;
                //Change the point of intersection
                Vector3 intersectionPoint = p + t * r;
                vertices[0] = intersectionPoint;
                lineSegments[0] = vertices[1] - intersectionPoint;

                //Also check if the point is above the screen position
                if(vertices[1].y > topLeft.y)
                {
                    p = vertices[1];
                    r = lineSegments[1];

                    q = topLeft;
                    s = (topRight - topLeft);
                    v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                    t = (q.x + v * s.x - p.x) / r.x;

                    //Change the point of intersection
                    intersectionPoint = p + t * r;
                    vertices[1] = intersectionPoint;
                    vertices[0] = vertices[1];
                    lineSegments[1] = vertices[2] - intersectionPoint;
                }
            }

            if(vertices[2].x > topLeft.x)
            {
                //Inersection of the thirs line segment with border
                Vector3 p = vertices[2];
                Vector3 r = lineSegments[2];

                Vector3 q = bottomLeft;
                Vector3 s = (topLeft - bottomLeft);
                float v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                float t = (q.x + v * s.x - p.x) / r.x;
                //Change the point of intersection
                Vector3 intersectionPoint = p + t * r;
                vertices[3] = intersectionPoint;
                lineSegments[2] = intersectionPoint - vertices[2];
                //Change the line segment here if there are any issues
                if(vertices[2].y < bottomLeft.y)
                {
                    p = vertices[1];
                    r = lineSegments[1];

                    q = bottomLeft;
                    s = (bottomLeft - bottomRight);
                    v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                    t = (q.x + v * s.x - p.x) / r.x;

                    //Change the point of intersection
                    intersectionPoint = p + t * r;
                    vertices[2] = intersectionPoint;
                    vertices[3] = vertices[1];
                    lineSegments[1] = intersectionPoint - vertices[1];
                }
            }
        }

        //TODO: Add more conditions here

    }
}
