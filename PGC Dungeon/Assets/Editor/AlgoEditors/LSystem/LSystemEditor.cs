
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

            if (!AssetDatabase.IsValidFolder("Assets/Resources/L-systemRuleSets"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "L-systemRuleSets");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(ruleSet, $"Assets/Resources/L-systemRuleSets/{ruleDec.fileName}.asset");
            AssetDatabase.SaveAssets();

        }


        if (GUILayout.Button("Load Rule Set"))
        {

            var ruleSet = Resources.Load<LSystemRuleSet>(ruleDec.fileName);

            ruleDec.A_RuleSet = ruleSet.A_RuleSet;
            ruleDec.B_RuleSet = ruleSet.B_RuleSet;
            ruleDec.C_RuleSet = ruleSet.C_RuleSet;
            ruleDec.N_RuleSet = ruleSet.Nsign_RuleSet;
            ruleDec.P_RuleSet = ruleSet.Psign_RuleSet;
            ruleDec.S_RuleSet = ruleSet.S_RuleSet;
            ruleDec.L_RuleSet = ruleSet.L_RuleSet;

        }

        if (GUILayout.Button("Run Iteration"))
        {
            ruleDec.RunIteration();
        }



    }
}

