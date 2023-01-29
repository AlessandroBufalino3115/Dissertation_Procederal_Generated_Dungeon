using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RuleSpawnerEditor : Editor
{

    //https://hugecalf-studios.github.io/unity-lessons/lessons/editor/menuitem/
    [MenuItem("PCG Algorithms/Spawn WFC Rule", priority = 24)]
    private static void SpawnWFCRule() 
    {
        var GVcont = ScriptableObject.CreateInstance<GraphViewDataCont>();

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.Refresh();
        }


        if (!AssetDatabase.IsValidFolder("Assets/Resources/WFC RuleSets"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "WFC RuleSets");
            AssetDatabase.Refresh();
        }

        AssetDatabase.CreateAsset(GVcont, $"Assets/Resources/WFC RuleSets/NewWFCRuleSet.asset");
        AssetDatabase.SaveAssets();
    }



    [MenuItem("PCG Algorithms/Spawn L-System Rule", priority = 25)]
    private static void SpawnLSystemRule()
    {
        var ruleSet = ScriptableObject.CreateInstance<LSystemRuleObj>();

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

        AssetDatabase.CreateAsset(ruleSet, $"Assets/Resources/L-systemRuleSets/NewLSystemRuleSet.asset");
        AssetDatabase.SaveAssets();
    }




    [MenuItem("PCG Algorithms/Spawn Weight Rule", priority = 26)]
    private static void SpawnWeightRule()
    {
        var GVcont = ScriptableObject.CreateInstance<WeightRuleSet>();

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.Refresh();
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources/WeightPathfindingRuleSet"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "WeightPathfindingRuleSet");
            AssetDatabase.Refresh();
        }

        AssetDatabase.CreateAsset(GVcont, $"Assets/Resources/WeightPathfindingRuleSet/NewWeightRuleSet.asset");
        AssetDatabase.SaveAssets();
    }




    [MenuItem("PCG Algorithms/Spawn TileSet Rule", priority = 27)]
    private static void SpawnTileSetRule()
    {
        var GVcont = ScriptableObject.CreateInstance<ScriptableOBJUtil.TilesRuleSet>();

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.Refresh();
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources/Tile_Sets_Ruleset"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Tile_Sets_Ruleset");
            AssetDatabase.Refresh();
        }

        AssetDatabase.CreateAsset(GVcont, $"Assets/Resources/Tile_Sets_Ruleset/NewTileSetRuleSet.asset");
        AssetDatabase.SaveAssets();
    }


}
