using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

[XmlInclude(typeof(SaveData))]
[XmlInclude(typeof(QuestionBoxSaveData))]
public class SaveData{}

public class MemoryManager
{
    static string localStoragePath;
    
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

        //SaveDocument(ref document, fileName);

        return document;
    }

    static void SaveDocument(ref XDocument document, string fileName)
    {
        try
        {
            document.Save(Path.Combine(localStoragePath, fileName));
        }
        catch
        {
            Debug.Log("Not Saved");
        }
    }

    static void SaveDocument(string stream, ref XDocument document, string fileName)
    {
        document = XDocument.Parse(stream);
        document.Save(Path.Combine(localStoragePath, fileName));
    }

    public static T Save<T>(T obj, XDocument xmlDocument, string localPath, string fileName)
    {
        
        //Serialize the save object to an xml file
        XmlSerializer serializer = new XmlSerializer(typeof(T), new Type[] { typeof(SaveData) });
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

[System.Serializable]
public class SaveObjectType
{
    public string type;
    public List<object> values = new List<object>();

    public void SetType(System.Type t)
    {
        type = t.ToString();
    }
}


/// <summary>
/// This class contains information about all the information that must be saved to the xml file
/// </summary>
public class SaveObject
{
    public List<Level> levelsInfo = new List<Level>();

    public List<SaveObjectType> types = new List<SaveObjectType>();
    
    //Journal Contents
    //public List<Sprite> itemsImages = new List<Sprite>();
    public string journalInformationPath;
    
    public void AddObject(System.Object obj)
    {
        for(int i = 0; i < types.Count; i++)
        {
            if(obj.GetType().ToString() == types[0].type)
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


    public void SaveJournal(JournalSaveData instance, string folderPath, string fileName)
    {
        WriteToFile(instance, folderPath, fileName);

        journalInformationPath = Path.Combine(folderPath, fileName);
    }

    //Create new file for binary Formatting
    void WriteToFile(System.Object o, string folderPath, string fileName)
    {
        if (o == null)
            return;

        BinaryFormatter bf = new BinaryFormatter();

        FileStream f = File.Open(Path.Combine(folderPath, fileName), FileMode.OpenOrCreate);
        
        bf.Serialize(f, o);

        f.Close();
    }
}

public class SaveManager : MonoBehaviour {
    public static SaveManager Instance { set; get; }

    public static SaveObject saveObject;

    static string localStoragePath;
    static string folderName = "Save Data";
    static string saveFileName = "SaveData.xml";

    static XDocument xmlDocument;

    public delegate void SaveDelegate(System.Type t);
    public static event SaveDelegate SaveEvent;

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
        saveObject = new SaveObject();

        SaveEvent(typeof(SaveManager));

        saveObject.levelsInfo = GameManager.Instance.levelsCleared;

        //Save the journal
        JournalSaveData js = new JournalSaveData();
        if(Journal.Instance != null)
        {
            js.itemsInJournal = Journal.Instance.GetAllItemsInJournal();

            //TODO: Add all acids, bases and neutral items as separate images in the resources folder.
            //Change how the images are being referenced
            //for storing the sprites, store all the textures.
            for (int i = 0; i < Journal.Instance.GetAllItemIconsInJournal().Count; i++)
            {
                js.itemIconsPath.Add(AssetDatabase.GetAssetPath(Journal.Instance.GetAllItemIconsInJournal()[i]));
            
                SerializableRect r = new SerializableRect(Journal.Instance.GetAllItemIconsInJournal()[i].rect.x,
                                                            Journal.Instance.GetAllItemIconsInJournal()[i].rect.y,
                                                            Journal.Instance.GetAllItemIconsInJournal()[i].rect.width,
                                                            Journal.Instance.GetAllItemIconsInJournal()[i].rect.height);

            
                Debug.Log("Path: " + AssetDatabase.GetAssetPath(Journal.Instance.GetAllItemIconsInJournal()[i]));
                Debug.Log("x: " + r.x + "y: " + r.y + "width: " + r.width + "Height " + r.height);
                js.spriteRect.Add(r);
            }

            js.itemsDescriptions = Journal.Instance.GetAllItemDescriptionsInJournal();

            saveObject.SaveJournal(js, localStoragePath, "Journal");
        }

        MemoryManager.Save<SaveObject>(saveObject, xmlDocument, localStoragePath, saveFileName);
        
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

        LoadJournal(s.journalInformationPath);
        //get the journal Information back
    }

    static void LoadJournal(string filePath)
    {
        JournalSaveData js;
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = new FileStream(filePath, FileMode.Open);
            js = (JournalSaveData)bf.Deserialize(f);
        }
        catch
        {
            Debug.Log("cannot deserialize the data.....damn.");
            return;
        }

        //Populate the actual journal now.
        Journal.Instance.SetAllItemsInJournal(js.itemsInJournal);
        List<Sprite> icons = new List<Sprite>();

        //Create new sprites from the stored textures
        for (int i = 0; i < js.itemIconsPath.Count; i++)
        {
            //Sprite.Create(js.itemTextures[i], Rect.zero, Vector2.zero);
            //int index = js.itemIconsPath[i].Replace("Assets/Resources/", "");
            string pathWithoutExtension = js.itemIconsPath[i].Replace("Assets/Resources/", "").Split('.')[0];
            Texture2D s = (Texture2D)Resources.Load<Texture2D>(pathWithoutExtension);
            //s = Resources.Load<Texture2D>(pathWithoutExtension);
            //Debug.Log("Texture name: " + s.name);
            //s.Apply();
            Debug.Log("sprite: " + pathWithoutExtension);
            Debug.Log("js::::::" + js.spriteRect[i].x);
            icons.Add(Sprite.Create(s, 
                                new Rect(js.spriteRect[i].x, js.spriteRect[i].y, js.spriteRect[i].width, js.spriteRect[i].height), 
                                    new Vector2(0f, 0f)));

            //s.Apply();
        }


        Journal.Instance.SetAllItemIconsInJournal(icons);
        Journal.Instance.SetAllItemDescriptionsInJournal(js.itemsDescriptions);
        Debug.Log("Load successful");
    }
}
