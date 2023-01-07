
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DS.Windows;
using System.IO;
using System;

[CustomEditor(typeof(NewLSystem))]

public class LSystemEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NewLSystem ruleDec = (NewLSystem)target;


        if (GUILayout.Button("New rule Set"))
        {
            var ruleSet = ScriptableObject.CreateInstance<LSystemRuleSet>();



            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(ruleSet, $"Assets/Resources/{ruleDec.fileName}.asset");
            AssetDatabase.SaveAssets();

        }


        if (GUILayout.Button("Load Rule Set"))
        {

            var ruleSet = Resources.Load<LSystemRuleSet>(ruleDec.fileName);

            ruleDec.A_RuleSet = ruleSet.A_RuleSet;
            ruleDec.B_RuleSet = ruleSet.B_RuleSet;
            ruleDec.C_RuleSet = ruleSet.C_RuleSet;
            ruleDec.N_RuleSet = ruleSet.N_RuleSet;
            ruleDec.P_RuleSet = ruleSet.P_RuleSet;
            ruleDec.S_RuleSet = ruleSet.S_RuleSet;
            ruleDec.L_RuleSet = ruleSet.L_RuleSet;

        }

        if (GUILayout.Button("Run Iteration"))
        {
            ruleDec.RunIteration();
        }



    }
}

