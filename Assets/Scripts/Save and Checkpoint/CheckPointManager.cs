using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;

public class CheckPointData
{
    //TODO: Make it take the character save data
    public List<Vector3> positions = new List<Vector3>();

    public List<SaveObjectType> types = new List<SaveObjectType>();

    public void AddObject(System.Object obj)
    {
        for (int i = 0; i < types.Count; i++)
        {
            if (obj.GetType().ToString() == types[0].type)
            {
                types[i].values.Add(obj);
                return;
            }
        }

        //Create new type and add object
        SaveObjectType o = new SaveObjectType();
        types.Add(o);
        o.type = obj.GetType().ToString();
        o.values.Add(obj);
    }
}

public class CheckPointManager : MonoBehaviour {

    static string checkPointFolderName = "Check Point";
    static string checkPointFileName = "CheckPoint.xml";

    static string localStoragePath;

    static XDocument xmlDocument;

    static List<GameObject> orderOfStorage = new List<GameObject>();

    public static CheckPointData checkPointData;

    public delegate void RegisterCheckPointDelegate(System.Type t);
    public static event RegisterCheckPointDelegate RegisterCheckPointEvent;

    public delegate void LoadCheckPointDelegate(System.Type t);
    public static event LoadCheckPointDelegate LoadCheckpointEvent;

    private void Start()
    {
        localStoragePath = Path.Combine(Application.persistentDataPath, checkPointFolderName);

        xmlDocument = MemoryManager.SearchDirectoryForXML(localStoragePath, checkPointFileName);

        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += ClearCheckPointData;

    }

    public static void RegisterCheckPoint()
    {
        checkPointData = new CheckPointData();

        RegisterCheckPointEvent(typeof(CheckPointManager));

        //Storing the positions of the active characters when the player steps on the checkpoint
        //Change this to effect more information
        Character[] characters = FindObjectsOfType<Character>();
        foreach(Character ch in characters)
        {
            if (ch.gameObject.activeSelf)
            {
                orderOfStorage.Add(ch.gameObject);
                checkPointData.positions.Add(ch.gameObject.transform.position);
            }
        }

        if (xmlDocument != null)
            MemoryManager.Save<CheckPointData>(checkPointData, xmlDocument, localStoragePath, checkPointFileName);
        else
            Debug.Log("XML file is null");
    }

    public static void LoadCheckPoint()
    {
        CheckPointData c = new CheckPointData();

        LoadCheckpointEvent(typeof(CheckPointManager));

        if (xmlDocument != null)
            c = MemoryManager.Load<CheckPointData>(c, xmlDocument, localStoragePath, checkPointFileName);
        else
            Debug.Log("XML file is null");


        if (c != null)
        {
            for(int i = 0; i < c.positions.Count; i++)
            {
                orderOfStorage[i].transform.position = c.positions[i];
                if (orderOfStorage[i].GetComponent<Enemy>())
                    orderOfStorage[i].GetComponent<Enemy>().ResetAI();
            }
            //orderOfStorage.Clear();
        }
    }

    void ClearCheckPointData(UnityEngine.SceneManagement.Scene scene)
    {
        DeleteCheckPointData();
    }

    private void OnDestroy()
    {
        DeleteCheckPointData();
    }

    void DeleteCheckPointData()
    {
        if (xmlDocument != null)
        {
            Debug.Log("Checkpoint data removed Successfully");
            MemoryManager.DeleteFile(localStoragePath, checkPointFileName);
        }
    }

}
