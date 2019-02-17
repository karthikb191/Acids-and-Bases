using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Dialogues))]
public class DialoguesDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        label=EditorGUI.BeginProperty(position,label,property);
		Rect contentPosition=EditorGUI.PrefixLabel(position,label);        
        position.height = 16f;        
        EditorGUI.indentLevel += 1;
        contentPosition = EditorGUI.IndentedRect(position);
        contentPosition.y += 18f;		
        contentPosition.width*=0.75f;
        EditorGUI.indentLevel=0;
        EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("dialogue"),GUIContent.none);
        contentPosition.x+=contentPosition.width;
        contentPosition.width/=3;
        EditorGUIUtility.labelWidth=18f;
        EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("isQuestion"),new GUIContent("Q?"));
        if(property.FindPropertyRelative("isQuestion").boolValue){
            
            EditorGUI.indentLevel += 1;
            contentPosition = EditorGUI.IndentedRect(position);
            contentPosition.y+=36f;           
            EditorGUI.LabelField(contentPosition,"Answer: ");
            contentPosition.width*=0.15f;
            contentPosition.x+=50f;            
            EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("currentAnswerVal"),GUIContent.none);
            contentPosition.x+=50f;
            EditorGUI.LabelField(contentPosition,"C:");
            contentPosition.x+=20f;
            contentPosition.width*=1.5f;
            EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("currentChoice"),GUIContent.none);
            int options=property.FindPropertyRelative("currentChoice").enumValueIndex+2;
            property.FindPropertyRelative("answerOptions").arraySize=options;
            if(options>=2){
                EditorGUI.indentLevel+=1;
                contentPosition=EditorGUI.IndentedRect(position);
                contentPosition.y+=54f;
                contentPosition.width*=.5f;
                contentPosition.x-=30.0f;
                EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("answerOptions").GetArrayElementAtIndex(0),GUIContent.none);
                contentPosition.x+=contentPosition.width;
                EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("answerOptions").GetArrayElementAtIndex(1),GUIContent.none);
                if(options>=3){
                    EditorGUI.indentLevel+=1;
                    contentPosition=EditorGUI.IndentedRect(position);
                    contentPosition.y+=72f;
                    contentPosition.width*=0.6f;
                    contentPosition.x-=60.0f;
                    EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("answerOptions").GetArrayElementAtIndex(2),GUIContent.none);
                    if(options==4){
                        contentPosition.x+=contentPosition.width-12.0f;
                        EditorGUI.PropertyField(contentPosition,property.FindPropertyRelative("answerOptions").GetArrayElementAtIndex(3),GUIContent.none);
                    }
                }
            }
        }
        EditorGUI.EndProperty();
	}
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		//return property.FindPropertyRelative("isQuestion").boolValue?52f:34f;
        int options=property.FindPropertyRelative("currentChoice").enumValueIndex+2;
        if(property.FindPropertyRelative("isQuestion").boolValue){
            return options==2?70.0f:88.0f;
        }else{
            return 34f;
        }
	}
}
