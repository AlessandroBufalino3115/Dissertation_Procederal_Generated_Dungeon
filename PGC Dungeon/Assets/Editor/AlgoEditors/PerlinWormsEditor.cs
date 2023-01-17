using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PerlinWormsMA))]
public class PerlinWormsEditor : Editor
{

    bool showCA = false;

    bool showPath = false;
    bool showRooms = false;

    bool showRules = false;

    bool useWeights = false;


    int selGridPathType = 0;
    GUIContent[] selStringsPathType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualr path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have manu corridors" }, new GUIContent() { text = "Prim's algo + random", tooltip = "Create a singualr path that traverses the whole dungeon, with some random diviation" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };


    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algortihm Marhcing cubes create a mesh object whihc can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tielset provided" } };

    bool started = false;





    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PerlinWormsMA mainScript = (PerlinWormsMA)target;


        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have choosen RandomWalk algorithm as your initial algorithm\n\nExplenation: a \"Head\" moves in a random direction at each step\n\nStep 1: Decide how many iterations the algorithm will have to generate the base map and its sub parameters." +
                "\n\nStep 2: To round up the rough edges you can decide to use Cellular Automata to help smooth things out." +
                "\n\nStep 3: It is possible small rooms that are not your what you are looking for have been geenrated, delete them using by setting up the minimum amount of tiles a room should have." +
                "\n\nStep 4: Depending on the amount of rooms you are able to create corridors by choosing the wanted pathFinding algorithm and the algortihm which decideds which room is connected to which." +
                "\n\nStep 5: Generate the algorithm using the tileSet provided or create the blank gameobject whihc can then be exported and manipulated");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion




        GeneralUtil.Spaces(4);

        #region Main algo region




        mainScript.OffsetX = (int)EditorGUILayout.Slider(mainScript.OffsetX, 0, 10000);
        mainScript.OffsetY = (int)EditorGUILayout.Slider(mainScript.OffsetY, 0, 10000);


        mainScript.Scale = EditorGUILayout.Slider(mainScript.Scale, 3f, 35f);
        mainScript.Octaves = (int)EditorGUILayout.Slider(mainScript.Octaves, 1, 8);

        mainScript.Persistance = EditorGUILayout.Slider(mainScript.Persistance, 0.1f, 0.9f);
        mainScript.Lacunarity = EditorGUILayout.Slider(mainScript.Lacunarity, 0.5f, 10f);

        mainScript.MinThreshold = EditorGUILayout.Slider(mainScript.MinThreshold, 0.1f, 0.9f);
        mainScript.MaxThreshold = EditorGUILayout.Slider(mainScript.MaxThreshold, 0.1f, 0.9f);

        if (GUILayout.Button("Run worm Gen"))
        {
            mainScript.PcgManager.gridArray2D = AlgosUtils.PerlinWorms(mainScript.PcgManager.gridArray2D, mainScript.Scale, mainScript.Octaves, mainScript.Persistance, mainScript.Lacunarity, mainScript.OffsetX, mainScript.OffsetY, mainScript.MaxThreshold, mainScript.MinThreshold);

            mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);

            started = true;
        }




        #endregion

    }
}
