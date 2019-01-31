using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridIndex
{
    public int x;
    public int y;
};

public class GridCell
{
    public GridIndex index;
    public Vector3 worldPosition;
    public List<Character> character = new List<Character>();
    public List<Node> node = new List<Node>();
}

public class SparseArray
{
    Hashtable hashValues = new Hashtable();
    public List<GridCell> this[int index1, int index2]
    {
        get
        {
            
            float code = index1 * 7 + index2 * 9;

            if (hashValues.ContainsKey(code))
            {
                return (List<GridCell>)hashValues[code];
            }
            else
            {
                return null;
            }
            
        }
        set
        {
            float code = index1 * 7 + index2 * 9;
            List<GridCell> gridCells = new List<GridCell>();
            //If the hash table already has a key, then add the grid cell to that hash table
            if (hashValues.ContainsKey(code))
            {
                gridCells = (List<GridCell>)hashValues[code];
                gridCells.AddRange(value);
                //hashValues.Add(code, gridCells);
            }
            else
            {
                hashValues.Add(code, value);
            }
        }
    }
}

[ExecuteInEditMode]
class WorldGrid : MonoBehaviour {
    private static WorldGrid instance;
    public static WorldGrid Instance { get { return instance; } }
    public float gridSize = 5;

    //This table contains all the grid cells populated by the characters and nodes(which technically represents platforms)
    //This creates a hash table internally, within it's class
    public SparseArray gridArray = new SparseArray();


    // Use this for initialization
    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (!Application.isPlaying)
            instance = this;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static GridIndex GetGridIndex(Vector3 pos)
    {
        GridIndex g = new GridIndex();
        float halfGridSize = instance.gridSize / 2;
        if (Mathf.Abs(pos.x) < halfGridSize)
        {
            if(pos.x < 0)
            {
                g.x = Mathf.CeilToInt(pos.x / halfGridSize);
                //Debug.Log("sadfasdfsafsdfas" + Mathf.Ceil(pos.x / (instance.gridSize / 2)));
            }
            else
                g.x = Mathf.FloorToInt(pos.x / halfGridSize);
        }
        else {
            if (pos.x < 0)
                g.x = Mathf.CeilToInt((pos.x - halfGridSize) / (instance.gridSize));
            else
                g.x = Mathf.FloorToInt((pos.x + halfGridSize) / (instance.gridSize));
        }
        if (Mathf.Abs(pos.y) < halfGridSize)
        {
            if(pos.y < 0)
                g.y = Mathf.CeilToInt(pos.y / halfGridSize);
            else
                g.y = Mathf.FloorToInt(pos.y / halfGridSize);
        }
        else
        {
            if(pos.y < 0)
                g.y = Mathf.CeilToInt((pos.y - halfGridSize) / (instance.gridSize));
            else
                g.y = Mathf.FloorToInt((pos.y + halfGridSize) / (instance.gridSize));
        }
        return g;
    }

    //TODO: this might need a few corrections. Take a look at it later
    public static GridCell GetTheWorldGridCell(GridIndex index)
    {
        //Debug.Log("There is no grid right now. But don;t worry. Creating a new cell");
        GridCell g = null;

        GridIndex gridIndex = index;

        if (instance.gridArray[gridIndex.x, gridIndex.y] == null)
        {
            //If the respective grid cell doesn't exist, create one
            GridCell newCell = new GridCell
            {
                index =
                {
                    x = gridIndex.x,
                    y = gridIndex.y
                },
                //worldPosition = new Vector3(gridIndex.x + instance.gridSize/2, gridIndex.y + instance.gridSize / 2)
                worldPosition = new Vector3(gridIndex.x * instance.gridSize, gridIndex.y * instance.gridSize)
            };

            //After the gridcell is created, make it as a list and feed it to the grid array
            List<GridCell> tempList = new List<GridCell>();
            tempList.Add(newCell);
            instance.gridArray[gridIndex.x, gridIndex.y] = tempList;
            
            g = newCell;
            //Debug.Log("Yay....Cell created successfully");
            return g;
        }
        else
        {
            //A list already exists in the grid array. 
            List<GridCell> tempList = instance.gridArray[gridIndex.x, gridIndex.y];
            for(int i = 0; i < tempList.Count; i++)
            {
                if(tempList[i].index.x == gridIndex.x && tempList[i].index.y == gridIndex.y)
                {
                    g = tempList[i];
                    return g;
                }
            }
            //If there's still no grid cell found, create one
            GridCell newCell = new GridCell
            {
                index =
                {
                    x = gridIndex.x,
                    y = gridIndex.y
                },
                //worldPosition = new Vector3(gridIndex.x + instance.gridSize / 2, gridIndex.y + instance.gridSize / 2)
                worldPosition = new Vector3(gridIndex.x * instance.gridSize, gridIndex.y * instance.gridSize)
            };

            //After the gridcell is created, make it as a list and feed it to the grid array
            tempList = new List<GridCell>();
            tempList.Add(newCell);
            instance.gridArray[gridIndex.x, gridIndex.y] = tempList;

            g = newCell;
            //Debug.Log("Yay....Cell created successfully");
            return g;

            Debug.Log("Something fishy is going on");
        }

        return g;
    }

    public static GridCell GetTheWorldGridCellWithoutCreatingNewCells(GridIndex index)
    {
        GridCell g = null;

        GridIndex gridIndex = index;

        if (instance.gridArray[gridIndex.x, gridIndex.y] == null)
        {
            return null;
        }
        else
        {
            List<GridCell> tempList = instance.gridArray[gridIndex.x, gridIndex.y];
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].index.x == gridIndex.x && tempList[i].index.y == gridIndex.y)
                {
                    g = tempList[i];
                    return g;
                }
            }
        }
        return g;
    }


    public static void AddToCell(Character c, GridCell cell)
    {
        //Debug.Log("grid cell is: " + cell.ToString());
        //Debug.Log("Character is: " + c.ToString());
        cell.character.Add(c);
    }
    public static void AddToCell(Node n, GridCell cell)
    {
        cell.node.Add(n);
    }
    public static void RemoveFromCell(Character c, GridCell cell)
    {
        cell.character.Remove(c);
    }
    public static void RemoveFromCell(Node n, GridCell cell)
    {
        cell.node.Remove(n);
    }


    public bool showGrid = true;
    private void OnDrawGizmos()
    {
        if (showGrid)
        {
            Vector3 p = Camera.main.transform.position;

            float minusX = p.x - 20;
            float plusX = p.x + 20;
            float minusY = p.y - 20;
            float plusY = p.y + 20;

            Gizmos.color = Color.yellow;
            GridIndex g = new GridIndex();
            int count = 0;

            for (float i = minusX; i <= plusX; i++)
            {
                float j = minusY + count;
                count++;

                g = GetGridIndex(new Vector3(i, j, 0));


                Vector3 p1 = new Vector3((float)(g.x * gridSize) - gridSize / 2, (float)(g.y * gridSize) - gridSize / 2, 0);

                Gizmos.DrawLine(new Vector3(p1.x, p.y - 10, 0),
                                    new Vector3(p1.x, p.y + 10, 0));

                Gizmos.DrawLine(new Vector3(p.x - 10, p1.y, 0),
                                    new Vector3(p.x + 10, p1.y, 0));
            }
        }
    }
    
}
