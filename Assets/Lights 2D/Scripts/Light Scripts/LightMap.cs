using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class LightMap : MonoBehaviour {
    
    Vector3 lightPosition;
    public static float dist = 0;
    public GameObject lightObject;

    public GameObject lightMapMeshPrefab;
    public Material lightMapMaterialPrefab;

    [HideInInspector]
    public GameObject lightMapMesh;
    [HideInInspector]
    public Material lightMapMaterial;

    public LightBlocker lb;

    public float angle = 180;

    [Range(0, 100)]
    public int horizontalDivisions = 0;
    [Range(0, 100)]
    public int verticalDivisions = 0;

    //List<Vector3> vertices;
    //List<Vector3> lineSegments;
    List<LightBlocker> lightBlockers;

    //Get the Mesh variables
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector3> normals;
    List<Vector2> uvs;

    //Get the world boundaries
    Vector3[] worldVertices;
    Vector3[] worldLineSegments;

    Vector3 screenCenter;

    Coroutine c = null;
    bool loopIsRunning = false;

    Lights2D light;
    // Use this for initialization
    void Awake () {
        light = GetComponent<Lights2D>();
        if (light.isAPixelLight)
        {
            if(transform.childCount==0)
                InstantiateLightMesh();
            else
                AssignLightMesh();
        }
        //TODO: Add more intuitive mesh generation later. But for now, keep it simple
        
	}

    float tempDistance = -1;
    int previousHorizontalDivisions = -1;
    int previousVerticalDivisions = -1;

    void InstantiateLightMesh()
    {
        if (lightMapMesh != null)
            Destroy(lightMapMesh);

        if(gameObject.transform.childCount != 0)
        {
            
            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                Debug.Log("destroyeddddddddddddddddddddddddddddddddddddddddddddddd");
                DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
            }
        }

        //Instantiates the mesh prefab at a certain depth and updates the Game manager's light position variable
        GameManager.Instance.currentZValue += 1;
        lightMapMesh = Instantiate(lightMapMeshPrefab, new Vector3(0, 0, GameManager.Instance.currentZValue), Quaternion.identity);
        lightMapMesh.transform.parent = gameObject.transform;
        lightMapMesh.transform.localRotation = Quaternion.identity;
        //lightMapMesh.transform.SetParent(this.gameObject.transform);


        dist = Vector3.Distance(Camera.main.transform.position,
                        new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0));

        //Get all the Light Blocker Obstacles and add the vertices and linesegments
        lightBlockers = new List<LightBlocker>();
        lightBlockers.AddRange(FindObjectsOfType<LightBlocker>());

        meshRenderer = lightMapMesh.GetComponent<MeshRenderer>();
        meshFilter = lightMapMesh.GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        //Create a new material from the prefab and set it to the mesh
        lightMapMaterial = new Material(lightMapMaterialPrefab);
        meshRenderer.sharedMaterial = lightMapMaterial;

        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();

        worldVertices = new Vector3[4];
        worldLineSegments = new Vector3[4];
        
    }

    void AssignLightMesh()
    {
        lightMapMesh = gameObject.transform.GetChild(0).gameObject;
        
        //Instantiates the mesh prefab at a certain depth and updates the Game manager's light position variable
        GameManager.Instance.currentZValue += 1;
        lightMapMesh.transform.localRotation = Quaternion.identity;
        //lightMapMesh = Instantiate(lightMapMeshPrefab, new Vector3(0, 0, GameManager.Instance.currentZValue), Quaternion.identity);
        //lightMapMesh.transform.parent = gameObject.transform;
        //lightMapMesh.transform.SetParent(this.gameObject.transform);


        dist = Vector3.Distance(Camera.main.transform.position,
                        new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0));

        //Get all the Light Blocker Obstacles and add the vertices and linesegments
        //lightBlockers = new List<LightBlocker>();
        //lightBlockers.AddRange(FindObjectsOfType<LightBlocker>());

        meshRenderer = lightMapMesh.GetComponent<MeshRenderer>();
        meshFilter = lightMapMesh.GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        //Create a new material from the prefab and set it to the mesh
        lightMapMaterial = new Material(lightMapMaterialPrefab);
        meshRenderer.sharedMaterial = lightMapMaterial;

        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();

        worldVertices = new Vector3[4];
        worldLineSegments = new Vector3[4];

    }

    // Update is called once per frame
    void Update () {
        if(light.isAPixelLight && lightMapMesh == null)
        {
            Debug.Log("Instantiating light mesh");
            if (transform.childCount == 0)
                InstantiateLightMesh();
            else
                AssignLightMesh();
        }
        if (tempDistance != light.distance || previousHorizontalDivisions != horizontalDivisions || previousVerticalDivisions != verticalDivisions || 
            previousLeftX != leftX || previousBottomY != bottomY || previousRightX != rightX || previousTopY != topY)
        {
            
            tempDistance = light.distance;  previousHorizontalDivisions = horizontalDivisions; previousVerticalDivisions = verticalDivisions;
            previousRightX = rightX;    previousTopY = topY;    previousLeftX = leftX;  previousBottomY = bottomY;

            if (light.isAPixelLight)
            {
                if (!loopIsRunning)
                {
                    //Debug.Log("Resetting Mesh");
                    //Clear the vertices, triangles, normals and uv list list first
                    meshFilter.sharedMesh.Clear(); vertices.Clear(); triangles.Clear(); normals.Clear(); uvs.Clear();
                    //c = StartCoroutine(RayCastingFromLightAndGenerateMesh());
                    //SortVertices();
                    //GenerateMesh();
                    //Vector3 pos = gameObject.transform.localPosition;
                    Vector3 pos = gameObject.transform.position;
                    //lightMapMesh.transform.position = pos;
                    Vector3 vertex1 = Quaternion.AngleAxis(-45, Vector3.forward) * -Vector3.right * tempDistance * 2;
                    Vector3 vertex2 = Quaternion.AngleAxis(45, Vector3.forward) * Vector3.right * tempDistance * 2;
                    Vector3 vertex3 = Quaternion.AngleAxis(-45, Vector3.forward) * Vector3.right * tempDistance * 2;
                    Vector3 vertex4 = Quaternion.AngleAxis(45, Vector3.forward) * -Vector3.right * tempDistance * 2;
                    //Debug.DrawLine(gameObject.transform.position, vertex1);
                    //Debug.DrawLine(gameObject.transform.position, vertex2);
                    //Debug.DrawLine(gameObject.transform.position, vertex3);
                    //Debug.DrawLine(gameObject.transform.position, vertex4);
                    
                    vertices.Add(vertex1);
                    vertices.Add(vertex2);
                    vertices.Add(vertex3);
                    vertices.Add(vertex4);
                    
                    normals.Add(-Vector3.forward);
                    normals.Add(-Vector3.forward);
                    normals.Add(-Vector3.forward);
                    normals.Add(-Vector3.forward);
                    
                    
                    uvs.Add(new Vector2(1, 1));
                    uvs.Add(new Vector2(1, 1));
                    uvs.Add(new Vector2(1, 1));
                    uvs.Add(new Vector2(1, 1));
                    
                    triangles.Add(1);
                    triangles.Add(3);
                    triangles.Add(0);
                    
                    triangles.Add(1);
                    triangles.Add(2);
                    triangles.Add(3);
                    
                    meshFilter.sharedMesh.vertices = vertices.ToArray();
                    meshFilter.sharedMesh.triangles = triangles.ToArray();
                    meshFilter.sharedMesh.normals = normals.ToArray();
                    meshFilter.sharedMesh.uv = uvs.ToArray();

                    
                    if (horizontalDivisions != 0 || verticalDivisions != 0)
                        CalculateVertices();

                    meshFilter.transform.position = pos;
                }
                //RayCastingFromLightAndGenerateMesh();

            }
        }
        //Compare this ray with all the other line segments and get the possible intersection
        #region trial
        /*float minDist = r.magnitude;
        for (int i = 0; i < lb.lineSegments.Length; i++)
        {
            if (i != 0)
            {
                Vector3 p = lightPosition;
                Vector3 q = lb.vertices[i];
                Vector3 s = lb.lineSegments[i];
                //Get the vertex position and the line segments are rays here
                float v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                float t = (q.x + v * s.x - p.x) / r.x;

                //Debug.Log("i and v and t: " + i + " " + v + " " + t);
                if (0 <= v && v <= 1 && 0 <= t && t <= 1)
                {
                    Vector3 intersectionPoint = p + t * r;
                    float presentDist = Vector3.Distance(lightPosition, intersectionPoint);
                    //Debug.Log("Intersection Found at: " + intersectionPoint);
                    if (presentDist < minDist - 0.1f)
                    {
                        minDist = presentDist;
                        Debug.Log("Breaking the loop");
                        Debug.DrawLine(lightPosition, intersectionPoint, Color.blue);
                        break;
                    }
                    
                    
                }
            }
            if(i == 3)
            {
                //Debug.Log("default line");
                Debug.DrawLine(lightPosition, lb.vertices[0], Color.red);
            }
        }

        r = new Vector3(lb.vertices[1].x, lb.vertices[1].y, 0) - lightPosition;
        //Compare this ray with all the other line segments and get the possible intersection
        minDist = r.magnitude;
        for (int i = 0; i < lb.lineSegments.Length; i++)
        {
            if (i != 1)
            {
                Vector3 p = lightPosition;
                Vector3 q = lb.vertices[i];
                Vector3 s = lb.lineSegments[i];
                //Get the vertex position and the line segments are rays here
                float v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                float t = (q.x + v * s.x - p.x) / r.x;

                Debug.Log("i and v and t: " + i + " " + v + " " + t);
                if (0 <= v && v <= 1 && 0 <= t && t <= 1)
                {
                    Vector3 intersectionPoint = p + t * r;
                    float presentDist = Vector3.Distance(lightPosition, intersectionPoint);
                    Debug.Log("Intersection Found at: " + intersectionPoint);
                    if (presentDist < minDist-0.1f)
                    {
                        minDist = presentDist;
                        Debug.Log("Breaking the loop");
                        Debug.DrawLine(lightPosition, intersectionPoint, Color.blue);
                        break;
                    }
                }
            }
            if (i == 3)
            {
                Debug.Log("default line");
                Debug.DrawLine(lightPosition, lb.vertices[1], Color.red);
            }
        }

        r = new Vector3(lb.vertices[2].x, lb.vertices[2].y, 0) - lightPosition;
        //Compare this ray with all the other line segments and get the possible intersection
        minDist = r.magnitude;
        for (int i = 0; i < lb.lineSegments.Length; i++)
        {
            if (i != 2)
            {
                Vector3 p = lightPosition;
                Vector3 q = lb.vertices[i];
                Vector3 s = lb.lineSegments[i];
                //Get the vertex position and the line segments are rays here
                float v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                float t = (q.x + v * s.x - p.x) / r.x;

                //Debug.Log("i and v and t: " + i + " " + v + " " + t);
                if (0 <= v && v <= 1 && 0 <= t && t <= 1)
                {
                    Vector3 intersectionPoint = p + t * r;
                    float presentDist = Vector3.Distance(lightPosition, intersectionPoint);
                    //Debug.Log("Intersection Found at: " + intersectionPoint);
                    if (presentDist < minDist - 0.1f)
                    {
                        minDist = presentDist;
                        Debug.Log("Breaking the loop");
                        Debug.DrawLine(lightPosition, intersectionPoint, Color.blue);
                        break;
                    }

                }
            }
            if (i == 3)
            {
                //Debug.Log("default line");
                Debug.DrawLine(lightPosition, lb.vertices[2], Color.red);
            }
        }

        r = new Vector3(lb.vertices[3].x, lb.vertices[3].y, 0) - lightPosition;
        //Compare this ray with all the other line segments and get the possible intersection
        minDist = r.magnitude;
        for (int i = 0; i < lb.lineSegments.Length; i++)
        {
            if (i != 3)
            {
                Vector3 p = lightPosition;
                Vector3 q = lb.vertices[i];
                Vector3 s = lb.lineSegments[i];
                //Get the vertex position and the line segments are rays here
                float v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                float t = (q.x + v * s.x - p.x) / r.x;

                //Debug.Log("i and v and t: " + i + " " + v + " " + t);
                if (0 <= v && v <= 1 && 0 <= t && t <= 1)
                {
                    Vector3 intersectionPoint = p + t * r;
                    float presentDist = Vector3.Distance(lightPosition, intersectionPoint);
                    //Debug.Log("Intersection Found at: " + intersectionPoint);
                    if (presentDist < minDist - 0.1f)
                    {
                        minDist = presentDist;
                        Debug.Log("Breaking the loop");
                        Debug.DrawLine(lightPosition, intersectionPoint, Color.blue);
                        break;
                    }

                }
            }
            if (i == 3)
            {
                //Debug.Log("default line");
                Debug.DrawLine(lightPosition, lb.vertices[3], Color.red);
            }
        }

        //Debug.DrawLine(lightPosition, lb.vertices[0], Color.red);
        //Debug.DrawLine(lightPosition, lb.vertices[1], Color.blue);
        //Debug.DrawLine(lightPosition, lb.vertices[2], Color.green);
        //Debug.DrawLine(lightPosition, lb.vertices[3], Color.cyan);

        //Doing the intersection with line segment
        */
        #endregion
        //lightMapMesh.transform.position = gameObject.transform.position;
    }

    public float leftX = 0;
    public float rightX = 0;
    public float topY = 0;
    public float bottomY = 0;

    float previousLeftX = 0;
    float previousRightX = 0;
    float previousTopY = 0;
    float previousBottomY = 0;


    void CalculateVertices()
    {
        int numOfVerts = (horizontalDivisions + 2) * (verticalDivisions + 2);
        float horizontalWidth = Mathf.Abs(meshFilter.sharedMesh.vertices[0].x - meshFilter.sharedMesh.vertices[1].x);
        float verticalWidth = Mathf.Abs(meshFilter.sharedMesh.vertices[1].y - meshFilter.sharedMesh.vertices[2].y);
        float x0_pos = meshFilter.sharedMesh.vertices[0].x;
        float x1_pos = meshFilter.sharedMesh.vertices[1].x;
        float y0_pos = meshFilter.sharedMesh.vertices[1].y;
        float y1_pos = meshFilter.sharedMesh.vertices[2].y;
        float horRectDiv = horizontalWidth / (horizontalDivisions + 1);
        float verRectDiv = verticalWidth / (verticalDivisions + 1);

        Vector3[] verts = new Vector3[numOfVerts];
        Vector3[] norms = new Vector3[numOfVerts];
        Vector2[] uv = new Vector2[numOfVerts];
        List<int> tris = new List<int>();

        Debug.Log("Vertex array length is: " + verts.Length);
        //int tempVert = 0;
        //Defining the vertices
        Debug.Log("horizontal div: " + horizontalDivisions);
        Debug.Log("vertical div: " + verticalDivisions);
        for (int i = 0; i < verticalDivisions + 2; i++)
        {
            for (int j = 0; j < horizontalDivisions + 2; j++)
            {
                //Debug.Log("separation value: " + (j * ((horizontalDivisions+1) - j) / (horizontalDivisions+1)));
                float x = meshFilter.sharedMesh.vertices[0].x + j * horRectDiv;
                if (j != 0 && j != horizontalDivisions + 1)
                {
                    float h1 = (x0_pos - x) * 0.07f * leftX;
                    float h2 = (x1_pos - x) * 0.07f * rightX;
                    x += h1 + h2;
                }
                float y = meshFilter.sharedMesh.vertices[1].y - i * verRectDiv;
                if (i != 0 && i != verticalDivisions+ 1)
                {
                    float h1 = (y0_pos - y) * 0.07f * bottomY;
                    float h2 = (y1_pos - y) * 0.07f * topY;
                    y += h1 + h2;
                }

                verts[i * (horizontalDivisions + 2) + j] = new Vector3(x, y, gameObject.transform.position.z);
                //Debug.Log("Vertex: " + (i * (horizontalDivisions + 2) + j) + "  : " + verts[i * (horizontalDivisions + 2) + j]);
            }

        }

        //Defining the triangles
        for (int i = 0; i <= verticalDivisions; i++)
        {
            for (int j = 0; j <= horizontalDivisions; j++)
            {
                tris.Add((i * (horizontalDivisions + 2) + j));
                tris.Add((i * (horizontalDivisions + 2) + j + 1));
                tris.Add(((i + 1) * (horizontalDivisions + 2) + j));

                tris.Add((i * (horizontalDivisions + 2) + j + 1));
                tris.Add(((i + 1) * (horizontalDivisions + 2) + j + 1));
                tris.Add(((i + 1) * (horizontalDivisions + 2) + j));

            }
        }

        meshFilter.sharedMesh.Clear();

        int[] triangles = tris.ToArray();

        meshFilter.sharedMesh.vertices = verts;
        meshFilter.sharedMesh.triangles = tris.ToArray();
        meshFilter.sharedMesh.normals = norms;
        meshFilter.sharedMesh.uv = uv;

        meshFilter.sharedMesh.RecalculateNormals();
        //for(int i = 0; i < tris.Count; i++)
        //{
        //    Debug.Log(tris[i]);
        //}
        meshFilter.transform.position = gameObject.transform.position;
    }

    IEnumerator RayCastingFromLightAndGenerateMesh()
    {
        loopIsRunning = true;
        

        //lightPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
        //                                                        Input.mousePosition.y, dist));
        lightPosition = gameObject.transform.position;

        Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, dist));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, dist));
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, dist));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, dist));
        worldVertices[0] = topLeft; worldLineSegments[0] = topRight - topLeft;
        worldVertices[1] = topRight; worldLineSegments[1] = bottomRight - topRight;
        worldVertices[2] = bottomRight; worldLineSegments[2] = bottomLeft - bottomRight;
        worldVertices[3] = bottomLeft; worldLineSegments[3] = topLeft - bottomLeft;


        screenCenter = new Vector3((topLeft.x + topRight.x) / 2, (bottomLeft.y + topLeft.y) / 2, dist);
        //Debug.Log("screen center is: " + screenCenter);
        // (p+tr) = (q+vs)
        float rayLength = 10;
        //Vector3 r = new Vector3(lb.vertices[0].x, lb.vertices[0].y, 0) - lightPosition;
        //Rays to corners of the screen
        Vector3 intersectionPoint = Vector3.zero;
        intersectionPoint = CheckForPossibleIntersections(lightPosition, (topLeft - lightPosition), topLeft);
        Debug.DrawLine(lightPosition, intersectionPoint);
        vertices.Add(intersectionPoint);

        intersectionPoint = CheckForPossibleIntersections(lightPosition, (topRight - lightPosition), topRight);
        Debug.DrawLine(lightPosition, intersectionPoint);
        vertices.Add(intersectionPoint);


        intersectionPoint = CheckForPossibleIntersections(lightPosition, (bottomRight - lightPosition), bottomRight);
        Debug.DrawLine(lightPosition, intersectionPoint);
        vertices.Add(intersectionPoint);

        intersectionPoint = CheckForPossibleIntersections(lightPosition, (bottomLeft - lightPosition), bottomLeft);
        Debug.DrawLine(lightPosition, intersectionPoint);
        vertices.Add(intersectionPoint);

        
        for (int i = 0; i < lightBlockers.Count; i++)
        {
            if (lightBlockers[i].gameObject.activeSelf)
            {
                for (int j = 0; j < lightBlockers[i].vertices.Length; j++)
                {
                    Vector3 ray = new Vector3(lightBlockers[i].vertices[j].x, lightBlockers[i].vertices[j].y, 0) - lightPosition;

                    //Check if the angle falls into the range of the given value with the forward vector
                    float rayAnglewithRightVector = Vector3.Angle(lightObject.transform.right, ray.normalized);
                    if (rayAnglewithRightVector <= angle / 2)
                    {

                        intersectionPoint = CheckForPossibleIntersections(lightPosition, ray, lightBlockers[i].vertices[j]);
                        vertices.Add(intersectionPoint);

                        Debug.DrawLine(lightPosition, intersectionPoint);
                        //If the intersection point is the vertex itself, cast two more rays with a slightly offset angle
                        //This is done by simply rotating the current ray
                        Vector3 leftRay = Quaternion.AngleAxis(0.1f, lightObject.transform.forward) * ray * 100;
                        intersectionPoint = CheckForPossibleIntersections(lightPosition, leftRay, lightBlockers[i].vertices[j]);
                        vertices.Add(intersectionPoint);

                        Vector3 rightRay = Quaternion.AngleAxis(-0.1f, lightObject.transform.forward) * ray * 100;
                        intersectionPoint = CheckForPossibleIntersections(lightPosition, rightRay, lightBlockers[i].vertices[j]);
                        vertices.Add(intersectionPoint);

                        Debug.DrawLine(lightPosition, intersectionPoint, Color.magenta);

                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }
        Debug.Log("vertex: " + vertices[0]);
        //SortVertices();
        //
        //GenerateMesh();
        //
        //meshFilter.mesh.triangles = triangles.ToArray();
        //meshFilter.mesh.normals = normals.ToArray();
        //meshFilter.mesh.uv = uvs.ToArray();
        //
        //meshFilter.mesh.RecalculateNormals();
        //Debug.Log("c has been set to null");

        loopIsRunning = false;
        c = null;
        yield return null;
    }

    Vector3 CheckForPossibleIntersections(Vector3 lightPosition, Vector3 r, Vector3 vertex)
    {
        //float minimumDistance = Vector3.Distance(lightPosition, vertex);
        float minimumDistance = r.magnitude;
        //Debug.Log("minimum Distace is: " + minimumDistance);
        Vector3 intersectionPoint = vertex;
        Vector3 tempIntersectionPoint = vertex;

        
        for (int i = 0; i < lightBlockers.Count; i++)
        {
            for (int j = 0; j < lightBlockers[i].vertices.Length; j++)
            {
                if(Vector3.Distance(lightPosition, lightBlockers[i].vertices[j]) < minimumDistance + 5f)
                {
                    //Debug.Log("Distance lesser for: " + j);
                    Vector3 p = lightPosition;
                    Vector3 q = lightBlockers[i].vertices[j];
                    Vector3 s = lightBlockers[i].lineSegments[j];
                    //Get the vertex position and the line segments are rays here
                    float v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
                    float t = (q.x + v * s.x - p.x) / r.x;
                    
                    //Debug.Log("v and t values: " + j + " : " + v + " " + t);
                    
                    //Debug.Log("j and v and t: " + j + " " + v + " " + t);
                    if (0 <= v && v <= 1 && 0 <= t && t <= 1)
                    {
                        //Debug.Log("Intersecting with some line");
                        tempIntersectionPoint = p + t * r;
                        float presentDist = Vector3.Distance(lightPosition, tempIntersectionPoint);
                        //Debug.Log("Intersection Found at: " + intersectionPoint);
                        if (presentDist < minimumDistance - 0.01f)
                        {
                            //Debug.Log("setting the minimum distace");
                            intersectionPoint = tempIntersectionPoint;
                            minimumDistance = presentDist;
                            //Debug.Log("Breaking the loop");
                            //return intersectionPoint;
                            
                        }
                    }
                }
            }
        }


        //Checking for collision with the world boundaries
        for(int i = 0; i < 4; i++)
        {
            //Debug.Log("Distance lesser for: " + j);
            Vector3 p = lightPosition;
            Vector3 q = worldVertices[i];
            Vector3 s = worldLineSegments[i];
            //Get the vertex position and the line segments are rays here
            float v = (r.x * p.y + q.x * r.y - p.x * r.y - q.y * r.x) / (s.y * r.x - s.x * r.y);
            float t = (q.x + v * s.x - p.x) / r.x;

            //Debug.Log("v and t values: " + j + " : " + v + " " + t);

            //Debug.Log("j and v and t: " + j + " " + v + " " + t);
            if (0 <= v && v <= 1 && 0 <= t && t <= 1)
            {
                //Debug.Log("Intersecting with some line");
                tempIntersectionPoint = p + t * r;
                float presentDist = Vector3.Distance(lightPosition, tempIntersectionPoint);
                //Debug.Log("Intersection Found at: " + intersectionPoint);
                if (presentDist < minimumDistance - 0.01f)
                {
                    //Debug.Log("setting the minimum distace");
                    intersectionPoint = tempIntersectionPoint;
                    minimumDistance = presentDist;
                    //Debug.Log("Breaking the loop");
                    //return intersectionPoint;

                }
            }
        }

        //Generate a triangle here

        return intersectionPoint;
    }

    void SortVertices()
    {
        
        //IEnumerable<Vector3> verts = vertices.OrderBy(vertices => vertices.y);
        //vertices.Clear();
        //Debug.Log("vertices order: ");
        
        List<float> angles = new List<float>();
        //TODO: Sort the vertices based on angle
        for(int i = 0; i < vertices.Count; i++)
        {
            angles.Add(Vector3.SignedAngle(Vector3.right, (vertices[i] - lightPosition).normalized, -Vector3.forward) * Mathf.Deg2Rad);
            //angles.Add()
        }

        //Once we have the angles, sort the vertices according to the angles
        for(int i = 0; i < angles.Count-1; i++)
        {
            //float angle = angles[i];
            for(int j = i+1; j < angles.Count; j++)
            {
                if(angles[j] < angles[i])
                {
                    //Perform Swap
                    float temp = angles[j];
                    angles[j] = angles[i];
                    angles[i] = temp;

                    //Swap the vertices
                    Vector3 tempVert = vertices[j];
                    vertices[j] = vertices[i];
                    vertices[i] = tempVert;
                }
            }
        }

        Debug.DrawLine(lightPosition, vertices[0], Color.gray);
        Debug.DrawLine(lightPosition, vertices[1], Color.gray);
        Debug.DrawLine(lightPosition, vertices[2], Color.gray);
        Debug.DrawLine(lightPosition, vertices[3], Color.gray);

        //Debug.Log("Sorted angles are: " );
        //for(int i = 0; i< angles.Count; i++)
        //{
        //    Debug.Log(angles[i]);
        //}

        //vertices = sortedVerts;
        //Add the first position as the light position
        vertices.Insert(0, lightPosition);
        normals.Insert(0, -Vector3.forward);
        uvs.Insert(0, new Vector2(0, 0));
    }

    void GenerateMesh()
    {
        meshFilter.sharedMesh.vertices = vertices.ToArray();
        
        for(int i = 1; i < vertices.Count; i++)
        {
            if(i == vertices.Count-1)
            {
                normals.Add(-Vector3.forward);
                uvs.Add(new Vector2(1, 1));
                break;
            }
            //Set the Triangles
            
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
            //Set the normals
            normals.Add(-Vector3.forward);
            uvs.Add(new Vector2(1, 1));

        }
        triangles.Add(0);
        triangles.Add(vertices.Count-1);
        triangles.Add(1);
    }

}
