using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PerlinWormsMA))]
public class PerlinWormsEditor : Editor
{

    bool showRules = false;

    int length = 40;
    float turnMulti = 0.5f;

    
   
    //int thicknessWorm = 2;



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PerlinWormsMA mainScript = (PerlinWormsMA)target;


        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have chosen perlin worms");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion

        GeneralUtil.SpacesUILayout(4);

        #region Main algo region

        length = (int)EditorGUILayout.Slider(new GUIContent() { text = "length", tooltip = "" }, length, 0, 500);
        turnMulti = EditorGUILayout.Slider(new GUIContent() { text = "turn Multiplier", tooltip = "" }, turnMulti, 0.2f, 0.8f);

        mainScript.offsetX = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset X", tooltip = "" }, mainScript.offsetX, 0, 10000);
        mainScript.offsetY = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset Y", tooltip = "" }, mainScript.offsetY, 0, 10000);


        mainScript.scale = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Scale", tooltip = "" }, mainScript.scale, 3f, 35f);
        mainScript.octaves = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Octaves", tooltip = "" }, mainScript.octaves, 1, 8);

        mainScript.persistance = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Persitance", tooltip = "" }, mainScript.persistance, 0.1f, 0.9f);
        mainScript.lacunarity = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Lacunarity", tooltip = "" }, mainScript.lacunarity, 0.5f, 10f);


        if (GUILayout.Button("Add one Worm"))
        {
            var worm = AlgosUtils.PerlinWorms(mainScript.pcgManager.gridArray2D, mainScript.scale, mainScript.octaves, mainScript.persistance, mainScript.lacunarity, mainScript.offsetX, mainScript.offsetY, length, turnMulti );

            mainScript.wormsTiles.UnionWith(worm);

            for (int y = 0; y < mainScript.pcgManager.gridArray2D.Length; y++)
            {
                for (int x = 0; x < mainScript.pcgManager.gridArray2D[0].Length; x++)
                {
                    if (mainScript.wormsTiles.Contains(mainScript.pcgManager.gridArray2D[y][x])) 
                    {
                        mainScript.pcgManager.gridArray2D[y][x].tileWeight = 1;
                    }
                    else 
                    {
                        mainScript.pcgManager.gridArray2D[y][x].tileWeight = 0;
                    }
                }
            }

            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.pcgManager.gridArray2D);
        }

        if (GUILayout.Button("Restart"))
        {
            mainScript.pcgManager.Restart();
        }

        #endregion

    }
}
