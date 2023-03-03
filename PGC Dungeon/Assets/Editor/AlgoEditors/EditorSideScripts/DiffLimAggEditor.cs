using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



[CustomEditor(typeof(DiffLimAggMA))]
public class DiffLimAggEditor : Editor
{
    bool showRules = false;

    int corridorThickness;

    int selStartRoomGenType = 0;
    GUIContent[] selStringStartRoomGenType = { new GUIContent() { text = "Circle room", tooltip = "" }, new GUIContent() { text = "square room", tooltip = "" }, new GUIContent() { text = "random WAll", tooltip = "" } };

    int cycles;

    float percOfSpawn;

    int sizeOfRoomSphere;

    int sizeOfRoomSphereWidth;
    int sizeOfRoomSphereHeight;

    string saveMapFileName = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DiffLimAggMA mainScript = (DiffLimAggMA)target;


        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("diff lim agg");
        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion

        GeneralUtil.SpacesUILayout(4);


        if (!mainScript.generatedBool) 
        {
            GUILayout.BeginVertical("Box");
            selStartRoomGenType = GUILayout.SelectionGrid(selStartRoomGenType, selStringStartRoomGenType, 1);
            GUILayout.EndVertical();

            switch (selStartRoomGenType)
            {
                case 0:  //sphere

                    sizeOfRoomSphere = (int)EditorGUILayout.Slider(new GUIContent() { text = "size of room as sphere", tooltip = "" }, sizeOfRoomSphere, 4, 25);

                    break;

                case 1: // room

                    sizeOfRoomSphereWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "width", tooltip = "" }, sizeOfRoomSphereWidth, 10, 30);
                    sizeOfRoomSphereHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "height", tooltip = "" }, sizeOfRoomSphereHeight, 10, 30);

                    break;

                case 2: // random walk
                    sizeOfRoomSphereWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "width", tooltip = "" }, sizeOfRoomSphereWidth, 10, 30);
                    sizeOfRoomSphereHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "height", tooltip = "" }, sizeOfRoomSphereHeight, 10, 30);

                    break;


                default:
                    break;

            }

            GeneralUtil.SpacesUILayout(2);
            cycles = (int)EditorGUILayout.Slider(new GUIContent() { text = "cycles", tooltip = "" }, cycles, 40, 350);

            percOfSpawn = EditorGUILayout.Slider(new GUIContent() { text = "perc of spawn", tooltip = "" }, percOfSpawn, 0.15f, 0.35f);

            GeneralUtil.SpacesUILayout(2);

            corridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "thickness of corrs", tooltip = "" }, corridorThickness, 3, 6);

            GeneralUtil.SpacesUILayout(2);

            if (GUILayout.Button("Generate Diff lim agg Randomisation"))// gen something
            {
                var centerPoint = new Vector2Int(mainScript.pcgManager.gridArray2D[0].Length / 2, mainScript.pcgManager.gridArray2D.Length / 2);
                AlgosUtils.RestartArr(mainScript.pcgManager.gridArray2D);

                bool allowedToContinue = false;

                switch (selStartRoomGenType)
                {
                    case 0:  //sphere
                        {
                            var sphereRoom = AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, centerPoint, sizeOfRoomSphere + 2);

                            if (sphereRoom != null)
                            {
                                AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, centerPoint, sizeOfRoomSphere, draw: true);
                                mainScript.generatedBool = true;
                                allowedToContinue = true;
                            }
                        }
                        break;

                    case 1: // room
                        {
                            var squareRoom = AlgosUtils.SpawnRoom(sizeOfRoomSphereWidth, sizeOfRoomSphereHeight, centerPoint, mainScript.pcgManager.gridArray2D);

                            if (squareRoom != null)
                            {
                                AlgosUtils.SpawnRoom(sizeOfRoomSphereWidth, sizeOfRoomSphereHeight, centerPoint, mainScript.pcgManager.gridArray2D);
                                mainScript.generatedBool = true;
                                allowedToContinue = true;
                            }
                        }
                        break;

                    case 2: // random walk
                        {
                            var squareRoom = AlgosUtils.SpawnRoom(sizeOfRoomSphereWidth, sizeOfRoomSphereHeight, centerPoint, mainScript.pcgManager.gridArray2D,true);

                            if (squareRoom != null)
                            {
                                var roomBounds = new BoundsInt() { xMin = centerPoint.x - sizeOfRoomSphereWidth / 2, xMax = centerPoint.x + sizeOfRoomSphereWidth / 2, zMin = centerPoint.y - sizeOfRoomSphereHeight / 2, zMax = centerPoint.y - sizeOfRoomSphereHeight / 2 };

                                var room = AlgosUtils.CompartimentalisedCA(roomBounds);

                                for (int y = 0; y < room.Length; y++)
                                {
                                    for (int x = 0; x < room[0].Length; x++)
                                    {
                                        if (room[y][x].tileWeight == 1)
                                        {
                                            mainScript.pcgManager.gridArray2D[y + roomBounds.zMin][x + roomBounds.xMin].tileWeight = 1;
                                        }
                                        else
                                        {
                                            mainScript.pcgManager.gridArray2D[y + roomBounds.zMin][x + roomBounds.xMin].tileWeight = 0;
                                        }
                                    }
                                }
                                allowedToContinue = true;
                                mainScript.generatedBool = true;
                            }
                        }
                        break;

                    //case 3:
                    //    break;


                    default:
                        break;
                }
                if (!allowedToContinue)
                {

                    EditorGUILayout.HelpBox("There was an issue with the size asked to generate the rooms please choose another size", MessageType.Error);
                    mainScript.pcgManager.Restart();
                    mainScript.generatedBool = false;
                }
                else 
                {
                    int size = mainScript.pcgManager.gridArray2D.Length * mainScript.pcgManager.gridArray2D[0].Length;

                    AlgosUtils.DiffLimAggregation(mainScript.pcgManager.gridArray2D, (int)(size * 0.25f), 300);

                    AlgosUtils.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArray2D, corridorThickness);

                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);

                }


            }
        }
        else 
        {
            
            if (GUILayout.Button("Restart"))// gen something
            {
                mainScript.pcgManager.Restart();
            }

            GeneralUtil.SpacesUILayout(2);

            GeneralUtil.GenerateMeshEditorSection(mainScript.pcgManager, saveMapFileName, out saveMapFileName);

        }
        
    }
}
