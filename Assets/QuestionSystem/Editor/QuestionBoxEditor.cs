using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ExpectedItem))]
public class QuestionBoxEditor : PropertyDrawer{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);

    }
}
