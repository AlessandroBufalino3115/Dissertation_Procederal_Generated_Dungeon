using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




[CustomEditor(typeof(DelunaryMA))]
public class DelunaryEditor : Editor
{
    bool showRules = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DelunaryMA mainScript = (DelunaryMA)target;


        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have chosen delu");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion


        GeneralUtil.SpacesUILayout(4);

    }
}
