using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




[CustomEditor(typeof(DelunaryMA))]
public class DelunaryEditor : Editor
{
    bool showRules = false;



    int selStartRoomGenType = 0;
    GUIContent[] selStringStartRoomGenType = { new GUIContent() { text = "Circle room", tooltip = "" }, new GUIContent() { text = "square room", tooltip = "" }, new GUIContent() { text = "random WAll", tooltip = "" } };

    int selGridGenType = 0;
    bool blockGeneration = false;
    string saveMapFileName = "";

    int radius;

    int width;
    int height;

    int numbersOfVertices;
    int ondulation;

    int corridorWidth;

    //first need to gen the main room then everything
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



        switch (mainScript.state)
        {
            case DelunaryMA.UI_STATE.STAGE_1:
                {

                    EditorGUI.BeginDisabledGroup(mainScript.rooms.Count == 1);


                    GUILayout.BeginVertical("Box");
                    selStartRoomGenType = GUILayout.SelectionGrid(selStartRoomGenType, selStringStartRoomGenType, 1);
                    GUILayout.EndVertical();

                    switch (selStartRoomGenType)
                    {
                        case 0:  //sphere

                            radius = (int)EditorGUILayout.Slider(new GUIContent() { text = "radius of sphere", tooltip = "" }, radius, 10, 50);

                            break;

                        case 1: // room

                            height = (int)EditorGUILayout.Slider(new GUIContent() { text = "height", tooltip = "" }, height, 10, 50);
                            width = (int)EditorGUILayout.Slider(new GUIContent() { text = "width", tooltip = "" }, width, 10, 50);

                            break;

                        case 2: // random walk

                            break;

                        //case 3:
                        //    break;

                        default:
                            break;

                    }


                    if (GUILayout.Button(new GUIContent() { text = "Generate start room", tooltip = "" }))
                    {
                        var centerPoint = new Vector2Int(mainScript.pcgManager.gridArray2D[0].Length / 2, mainScript.pcgManager.gridArray2D.Length / 2);

                        switch (selStartRoomGenType)
                        {
                            case 0:  //sphere

                                var sphereRoom = AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, centerPoint, radius + 2);

                                if (sphereRoom != null)
                                {
                                    mainScript.pcgManager.CreateBackUpGrid();
                                    sphereRoom = AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, centerPoint, radius, draw: true);

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);

                                    mainScript.rooms.Add(sphereRoom);
                                }

                                break;

                            case 1: // room

                                var squareRoom = AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, centerPoint, radius + 2);

                                if (squareRoom != null)
                                {
                                    mainScript.pcgManager.CreateBackUpGrid();
                                    squareRoom = AlgosUtils.SpawnRoom(width, height, centerPoint, mainScript.pcgManager.gridArray2D);

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);

                                    mainScript.rooms.Add(squareRoom);
                                }

                                break;

                            case 2: // random walk

                                break;

                            //case 3:
                            //    break;


                            default:
                                break;
                        }
                    }

                    EditorGUI.EndDisabledGroup();

                    if (mainScript.rooms.Count >0)
                        mainScript.allowedForward = true;
                    else
                        mainScript.allowedForward = false;


                }

                break;

            case DelunaryMA.UI_STATE.STAGE_2:
                {
                    numbersOfVertices = (int)EditorGUILayout.Slider(new GUIContent() { text = "number of intersections", tooltip = "" }, numbersOfVertices, 5, 50);
                    ondulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Ondulation", tooltip = "" }, ondulation, 5, 40);

                    corridorWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "corridor thickness", tooltip = "" }, corridorWidth, 3, 6);

                    if (GUILayout.Button(new GUIContent() { text = "gen the branches", tooltip = "" }))
                    {
                        mainScript.generatedCorridors = true;

                        mainScript.pcgManager.CreateBackUpGrid();
                        for (int i = 0; i < numbersOfVertices; i++)
                        {
                            var randomPoint = new Vector2Int(Random.Range(radius + 1, mainScript.pcgManager.gridArray2D[0].Length - 2 - 1), Random.Range(2 + 1, mainScript.pcgManager.gridArray2D.Length - 2 - 1));
                            var sphereRoom = AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, randomPoint, 2);

                            if (sphereRoom != null)
                            {
                                sphereRoom = AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, randomPoint, 2, draw: true);

                                mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);

                                mainScript.rooms.Add(sphereRoom);
                            }
                        }


                        mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.pcgManager.gridArray2D, true);
                        var centerPoints = new List<Vector2>();
                        var roomDict = new Dictionary<Vector2, List<Tile>>();
                        foreach (var room in mainScript.rooms)
                        {
                            roomDict.Add(AlgosUtils.FindMiddlePoint(room), room);
                            centerPoints.Add(AlgosUtils.FindMiddlePoint(room));
                        }


                        var edges = AlgosUtils.DelunayTriangulation2D(centerPoints).Item2;

                        foreach (var edge in edges)
                        {
                            var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                            var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                            AlgosUtils.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), ondulation, mainScript.pcgManager.gridArray2D);
                        }

                        AlgosUtils.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArray2D, corridorWidth);

                        mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);
                    }


                    if (mainScript.generatedCorridors)
                        mainScript.allowedForward = true;
                    else
                        mainScript.allowedForward = false;

                }

                break;

            case DelunaryMA.UI_STATE.GENERATION:

                mainScript.allowedBack = true;

                GeneralUtil.GenerateMeshEditorSection(mainScript.pcgManager, selGridGenType, blockGeneration, saveMapFileName, out selGridGenType, out blockGeneration, out saveMapFileName);

                break;

            default:
                break;
        }



        if (mainScript.state != DelunaryMA.UI_STATE.GENERATION)
        {
            GeneralUtil.SpacesUILayout(4);

            EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

            if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" }))// gen something
            {
                mainScript.pcgManager.ClearUndos();
                mainScript.allowedBack = false;
                mainScript.currStateIndex--;
                mainScript.state = (DelunaryMA.UI_STATE)mainScript.currStateIndex;
            }

            EditorGUI.EndDisabledGroup();



            EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

            if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
            {
                mainScript.pcgManager.ClearUndos();
                mainScript.allowedForward = false;
                mainScript.currStateIndex++;
                mainScript.state = (DelunaryMA.UI_STATE)mainScript.currStateIndex;
            }

            EditorGUI.EndDisabledGroup();
        }




    }
}
