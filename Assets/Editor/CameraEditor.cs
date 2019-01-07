using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraScript))]
public class CameraEditor : Editor{
    override public void OnInspectorGUI()
    {
        
        //target is the object under inspection
        CameraScript cameraScript = target as CameraScript;

        cameraScript.speed = EditorGUILayout.Slider("Speed", cameraScript.speed, 0, 1);
        cameraScript.xPosMultiplier = EditorGUILayout.Slider("X Pos Multiplier", cameraScript.xPosMultiplier, -50, 50);
        cameraScript.yPosMultiplier = EditorGUILayout.Slider("Y Pos Multiplier", cameraScript.yPosMultiplier, -50, 50);


        cameraScript.shakeEnabled = EditorGUILayout.Toggle("ShakeEnabled", cameraScript.shakeEnabled);

        if (cameraScript.shakeEnabled)
        {
            cameraScript.shakeAmount = EditorGUILayout.Slider("Shake Amount", cameraScript.shakeAmount, 0, 2);
            cameraScript.shakeDamping = EditorGUILayout.Slider("Shake Damping", cameraScript.shakeDamping, 0, 2);
            cameraScript.shakeSmoothness = EditorGUILayout.Slider("Shake Smoothness", cameraScript.shakeSmoothness, 0, 1);
            if(GUILayout.Button("Shake"))
            {
                cameraScript.ShakeCamera(cameraScript.shakeAmount, cameraScript.shakeDamping, cameraScript.shakeSmoothness);
            }
        }

        cameraScript.focusEnabled = EditorGUILayout.Toggle("Focus Enabled", cameraScript.focusEnabled);

        if (cameraScript.focusEnabled)
        {
            cameraScript.focusPosition = EditorGUILayout.Vector3Field("Focus Position", cameraScript.focusPosition);
            cameraScript.focusSpeed = EditorGUILayout.Slider("Focus Speed", cameraScript.focusSpeed, 1, 10);
            cameraScript.focusDuration = EditorGUILayout.Slider("Focus Duration", cameraScript.focusDuration, 0, 10);
            if (GUILayout.Button("Focus"))
            {
                cameraScript.FocusCameraAt(cameraScript.focusPosition, cameraScript.focusSpeed, cameraScript.focusDuration);
            }
        }

        cameraScript.boundariesContainer = EditorGUILayout.ObjectField("Boundary Container", cameraScript.boundariesContainer, typeof(UnityEngine.GameObject)) as UnityEngine.GameObject;


    }
}
