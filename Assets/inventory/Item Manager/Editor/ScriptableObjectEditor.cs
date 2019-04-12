using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemListScriptableObject))]
public class ScriptableObjectEditor : Editor {

    public List<ItemTemplate> tempList;

    System.Object[] acidObjects;
    System.Object[] baseObjects;
    System.Object[] saltObjects;
    System.Object[] indicatorObjects;
    System.Object[] normalItemObjects;

    bool acidsFoldout = true;
    bool basesFoldout = true;
    bool saltsFoldout = true;
    bool indicatorsFoldout = true;
    bool normalItemsFoldout = true;

    ItemListScriptableObject obj;

    private void OnEnable()
    {
        obj = target as ItemListScriptableObject;

        System.Array acidArray = System.Enum.GetValues(typeof(AcidsList));
        acidObjects = new System.Object[acidArray.Length];
        System.Array.Copy(acidArray, acidObjects, acidArray.Length);

        System.Array baseArray = System.Enum.GetValues(typeof(BasesList));
        baseObjects = new System.Object[baseArray.Length];
        System.Array.Copy(baseArray, baseObjects, baseArray.Length);

        System.Array saltArray = System.Enum.GetValues(typeof(SaltsList));
        saltObjects = new System.Object[saltArray.Length];
        System.Array.Copy(saltArray, saltObjects, saltArray.Length);

        System.Array indicatorArray = System.Enum.GetValues(typeof(IndicatorsList));
        indicatorObjects = new System.Object[indicatorArray.Length];
        System.Array.Copy(indicatorArray, indicatorObjects, indicatorArray.Length);

        System.Array normalItemArray = System.Enum.GetValues(typeof(NormalItemList));
        normalItemObjects = new System.Object[normalItemArray.Length];
        System.Array.Copy(normalItemArray, normalItemObjects, normalItemObjects.Length);

        
    }

    public override void OnInspectorGUI()
    {
        ArrangeItems("Acid Items", ref obj.acidObjects, ref obj.acidList, ref acidsFoldout);
        ArrangeItems("Base Items", ref obj.baseObjects, ref obj.baseList, ref basesFoldout);
        ArrangeItems("Salt Items", ref obj.saltObjects, ref obj.saltList, ref saltsFoldout);
        ArrangeItems("Indicator Items", ref obj.indicatorObjects, ref obj.indicatorList, ref indicatorsFoldout);
        ArrangeItems("Normal Items", ref obj.normalItemObjects, ref obj.normalItemList, ref normalItemsFoldout);

        
        EditorUtility.SetDirty(obj);
        //base.OnInspectorGUI();
    }

    void ArrangeItems(string name, ref System.Object[] objects, ref ItemTemplate[] itemTemplates, ref bool foldout)
    {
        //EditorGUILayout.Foldout

        if (itemTemplates.Length < objects.Length)
        {
            //Debug.Log("resize called: asdfajkdhdalfkghadkjgahdghdlgjhfdlgskdjfhsldkf");
            System.Array.Resize(ref itemTemplates, objects.Length);
        }


        foldout = EditorGUILayout.Foldout(foldout, name);
        if(foldout)
            for (int i = 0; i < objects.Length; i++)
            {
                EditorGUILayout.BeginHorizontal(GUIStyle.none);

                EditorGUILayout.LabelField(objects[i].ToString());
                itemTemplates[i].itemProperty = EditorGUILayout.ObjectField(itemTemplates[i].itemProperty,
                                                    typeof(ItemProperties), false) as ItemProperties;//UnityEngine.GameObject;
                itemTemplates[i].enumObject = objects[i].ToString();
                itemTemplates[i].type = objects[i].GetType();
                //Debug.Log("asdfasfasdfasdfasdfasdfadfasdfasdfasdfasdf: " + objects[i].ToString());
                EditorGUILayout.EndHorizontal();
            }


    }

}
