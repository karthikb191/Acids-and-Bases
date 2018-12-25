using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

public class MemoryManager
{
    //public static string localStoragePath = Application.persistentDataPath;
    
    static string localStoragePath;

    //public static void SetLocalStoragePath(string path)
    //{
    //    localStoragePath = path;
    //}

    public static XDocument SearchDirectoryForXML(string localPath, string xmlFileName)
    {
        localStoragePath = localPath;

        XDocument xmlDocument;

        //If the directory is not present, new one is created
        Directory.CreateDirectory(localStoragePath);

        //Check if there's a file in the specified location
        string[] filePath = Directory.GetFiles(localStoragePath);

        if (filePath.Length > 0)
        {
            for (int i = 0; i < filePath.Length; i++)
            {
                if (Path.GetFileName(Path.Combine(localStoragePath, filePath[i])) == xmlFileName)
                {
                    Debug.Log("File obtained");
                    Debug.Log("File path:" + localStoragePath);
                    Debug.Log("File name: " + filePath[i]);
                    try
                    {
                        xmlDocument = XDocument.Load(Path.Combine(localStoragePath, filePath[i]));
                        Debug.Log("Document contents:" + xmlDocument.ToString());
                        return xmlDocument;
                    }
                    catch(XmlException e)
                    {
                        Debug.Log("Exception occurred. File is Empty");
                    }
                }
            }
            //If no file is found, new file is created
            //File.Create(Path.Combine(localStoragePath, saveFileName));
            xmlDocument = CreateEmptyXMLDocumentAndSave(xmlFileName);
        }
        else
        {
            xmlDocument = CreateEmptyXMLDocumentAndSave(xmlFileName);
        }
        return xmlDocument;
    }

    static XDocument CreateEmptyXMLDocumentAndSave(string fileName)
    {
        Debug.Log("XML file created at the specified path");

        XDocument document = new XDocument();

        SaveDocument(ref document, fileName);

        return document;
    }

    static void SaveDocument(ref XDocument document, string fileName)
    {
        document.Save(Path.Combine(localStoragePath, fileName));
    }

    static void SaveDocument(string stream, ref XDocument document, string fileName)
    {
        document = XDocument.Parse(stream);
        document.Save(Path.Combine(localStoragePath, fileName));
    }

    public static T Save<T>(T obj, XDocument xmlDocument, string localPath, string fileName)
    {
        
        //Serialize the save object to an xml file
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringWriter writer = new StringWriter();
        serializer.Serialize(writer, obj);

        //Converting it to text
        string data = writer.ToString();
        Debug.Log("data: " + data);

        MemoryManager.SaveDocument(data, ref xmlDocument, fileName);
        //xmlDocument = XDocument.Parse(data);
        //xmlDocument.Save(saveFileName);

        Debug.Log("Save operation finished" + xmlDocument.ToString());

        return obj;
    }

    public static T Load<T>(T obj, XDocument xmlDocument, string localPath, string fileName) where T : class
    {
        if (xmlDocument == null)
            xmlDocument = MemoryManager.SearchDirectoryForXML(localPath, fileName);

        if (xmlDocument != null)
        {
            //Load levels
            //This stores the correct XML file to the xmlDocument variable
            xmlDocument = MemoryManager.SearchDirectoryForXML(localPath, fileName);

            //Deserialize the XML document
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            obj = serializer.Deserialize(new StringReader(xmlDocument.ToString())) as T;
            Debug.Log("Load operation completed");
            
        }
        return obj;
    }

    public static void DeleteFile(string path, string fileName)
    {
        File.Delete(Path.Combine(path, fileName));
    }

}

public class SaveObject
{
    public List<Level> levelsInfo = new List<Level>();
    
}

public class SaveManager : MonoBehaviour {
    public static SaveManager Instance { set; get; }

    static string localStoragePath;
    static string folderName = "Save Data";
    static string saveFileName = "SaveData.xml";

    static XDocument xmlDocument;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    
    private void Start()
    {
        localStoragePath = Application.persistentDataPath;
        localStoragePath = Path.Combine(localStoragePath, folderName);
        
        //MemoryManager.SetLocalStoragePath(localStoragePath);

        xmlDocument = MemoryManager.SearchDirectoryForXML(localStoragePath, saveFileName);
    }
    

    //TODO: Add error checking code
    public static void Save()
    {
        SaveObject s = new SaveObject();
        s.levelsInfo = GameManager.Instance.levelsCleared;

        MemoryManager.Save<SaveObject>(s, xmlDocument, localStoragePath, saveFileName);
        
    }

    public static void Load()
    {
        SaveObject s = new SaveObject();

        s = MemoryManager.Load<SaveObject>(s, xmlDocument, localStoragePath, saveFileName);

        if (s != null)
        {
            GameManager.Instance.levelsCleared = s.levelsInfo;
            Debug.Log("levels count: " + s.levelsInfo.Count);
        }
        
    }

}
