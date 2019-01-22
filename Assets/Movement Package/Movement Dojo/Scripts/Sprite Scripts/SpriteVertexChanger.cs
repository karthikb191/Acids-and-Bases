using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteVertexChanger : MonoBehaviour {

    [Range(0, 30)]
    public uint horizontalDivisions = 0;
    [Range(0, 30)]
    public uint verticalDivisions = 0;

    private SpriteRenderer spriteRenderer;
    private uint previousHorizontalDivisions;
    private uint previousVerticalDivisions;
    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (horizontalDivisions < 0 || verticalDivisions < 0)
        {
            Debug.LogError("Error!!!!!! Don't use negative values");
        }
        else
        {
            CalculateVertices();
        }
	}

    private void Update()
    {
        if(previousHorizontalDivisions != horizontalDivisions || previousVerticalDivisions != verticalDivisions)
        {
            CalculateVertices();
        }
        previousVerticalDivisions = verticalDivisions;
        previousHorizontalDivisions = horizontalDivisions;
    }

    void CalculateVertices()
    {
        uint numOfVerts = (horizontalDivisions + 2) * (verticalDivisions + 2);

        float horRectDiv = (spriteRenderer.sprite.rect.size.x) / (horizontalDivisions + 1);
        float verRectDiv = (spriteRenderer.sprite.rect.size.y) / (verticalDivisions + 1);

        Vector2[] vertices = new Vector2[numOfVerts];
        List<ushort> tris = new List<ushort>();
        Debug.Log("Vertex array length is: " + vertices.Length);
        //int tempVert = 0;
        //Defining the vertices
        for (int i = 0; i < verticalDivisions + 2; i++)
        {
            for(int j = 0; j < horizontalDivisions + 2; j++)
            {
                float x = j * horRectDiv;
                float y = i * verRectDiv;
                vertices[i * (horizontalDivisions + 2) + j] = new Vector2(x, y);
                //Debug.Log("Vertex: " + (i * (horizontalDivisions + 2) + j) + "  : " + vertices[i * (horizontalDivisions + 2) + j]);
            }

        }

        //Defining the triangles
        for(ushort i = 0; i <= verticalDivisions; i++)
        {
            for(ushort j = 0; j <= horizontalDivisions; j++)
            {
                tris.Add((ushort)(i * (horizontalDivisions + 2) + j));
                tris.Add((ushort)((i+1) * (horizontalDivisions + 2) + j));
                tris.Add((ushort)(i * (horizontalDivisions + 2) + j + 1));

                tris.Add((ushort)(i * (horizontalDivisions + 2) + j + 1));
                tris.Add((ushort)((i+1) * (horizontalDivisions + 2) + j));
                tris.Add((ushort)((i+1) * (horizontalDivisions + 2) + j + 1));

            }
        }

        ushort[] triangles = tris.ToArray();

        spriteRenderer.sprite.OverrideGeometry(vertices, triangles);

        //for(int i = 0; i < tris.Count; i++)
        //{
        //    Debug.Log(tris[i]);
        //}

    }

}
