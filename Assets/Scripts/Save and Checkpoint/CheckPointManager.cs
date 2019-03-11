using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;

public class CheckPointData
{
    public List<Vector3> positions = new List<Vector3>();
}

public class CheckPointManager : MonoBehaviour {

    static string checkPointFolderName = "Check Point";
    static string checkPointFileName = "CheckPoint.xml";

    static string localStoragePath;

    static XDocument xmlDocument;

    static List<GameObject> orderOfStorage = new List<GameObject>();

    private void Start()
    {
        localStoragePath = Path.Combine(Application.persistentDataPath, checkPointFolderName);

        xmlDocument = MemoryManager.SearchDirectoryForXML(localStoragePath, checkPointFileName);

        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += ClearCheckPointData;

    }

    public static void RegisterCheckPoint()
    {
        CheckPointData c = new CheckPointData();

        //Storing the positions of the active characters when the player steps on the checkpoint
        Character[] characters = FindObjectsOfType<Character>();
        foreach(Character ch in characters)
        {
            if (ch.gameObject.activeSelf)
            {
                orderOfStorage.Add(ch.gameObject);
                c.positions.Add(ch.gameObject.transform.position);
            }
        }

        if (xmlDocument != null)
            MemoryManager.Save<CheckPointData>(c, xmlDocument, localStoragePath, checkPointFileName);
        else
            Debug.Log("XML file is null");
    }

    public static void LoadCheckPoint()
    {
        CheckPointData c = new CheckPointData();

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
