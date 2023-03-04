using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



namespace DungeonForge
{
    [CustomEditor(typeof(DelunaryMA))]
    public class DelunaryEditor : Editor
    {
        bool showRules = false;

        int selStartRoomGenType = 0;
        GUIContent[] selStringStartRoomGenType = { new GUIContent() { text = "Circle room", tooltip = "" }, new GUIContent() { text = "square room", tooltip = "" }, new GUIContent() { text = "random WAll", tooltip = "" } };

        //int selGridGenType = 0;
        //bool blockGeneration = false;
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


            DFGeneralUtil.SpacesUILayout(4);

            switch (mainScript.state)
            {
                case DelunaryMA.UI_STATE.STAGE_1:
                    {

                        EditorGUI.BeginDisabledGroup(mainScript.rooms.Count >= 1);


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

                                height = (int)EditorGUILayout.Slider(new GUIContent() { text = "height", tooltip = "" }, height, 10, 50);
                                width = (int)EditorGUILayout.Slider(new GUIContent() { text = "width", tooltip = "" }, width, 10, 50);
                                break;

                            //case 3:
                            //    break;

                            default:
                                break;

                        }


                        if (GUILayout.Button(new GUIContent() { text = "Generate start room", tooltip = "" }))
                        {
                            var centerPoint = new Vector2Int(mainScript.pcgManager.gridArr.GetLength(0) / 2, mainScript.pcgManager.gridArr.GetLength(1) / 2);

                            switch (selStartRoomGenType)
                            {
                                case 0:  //sphere
                                    {
                                        var sphereRoom = DFAlgoBank.DrawCircle(mainScript.pcgManager.gridArr, centerPoint, radius + 2);

                                        if (sphereRoom != null)
                                        {
                                            mainScript.pcgManager.CreateBackUpGrid();
                                            sphereRoom = DFAlgoBank.DrawCircle(mainScript.pcgManager.gridArr, centerPoint, radius, draw: true);

                                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                                            mainScript.rooms.Add(sphereRoom);
                                        }
                                    }

                                    break;

                                case 1: // room
                                    {
                                        var squareRoom = DFAlgoBank.SpawnRoom(width, height, centerPoint, mainScript.pcgManager.gridArr, true);

                                        if (squareRoom != null)
                                        {
                                            mainScript.pcgManager.CreateBackUpGrid();
                                            squareRoom = DFAlgoBank.SpawnRoom(width, height, centerPoint, mainScript.pcgManager.gridArr);

                                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                                            mainScript.rooms.Add(squareRoom);
                                        }
                                    }

                                    break;

                                case 2: // random walk
                                    {
                                        var squareRoom = DFAlgoBank.SpawnRoom(width, height, centerPoint, mainScript.pcgManager.gridArr, true);

                                        if (squareRoom != null)
                                        {

                                            var roomBounds = new BoundsInt() { xMin = centerPoint.x - width / 2, xMax = centerPoint.x + width / 2, zMin = centerPoint.y - height / 2, zMax = centerPoint.y + height / 2 };

                                            var room = DFAlgoBank.CompartimentalisedCA(roomBounds);

                                            for (int y = 0; y < room.GetLength(1); y++)
                                            {
                                                for (int x = 0; x < room.GetLength(0); x++)
                                                {
                                                    if (room[x, y].tileWeight == 1)
                                                    {
                                                        mainScript.pcgManager.gridArr[x + roomBounds.xMin, y + roomBounds.zMin].tileWeight = 1;
                                                    }
                                                    else
                                                    {
                                                        mainScript.pcgManager.gridArr[x + roomBounds.xMin, y + roomBounds.zMin].tileWeight = 0;
                                                    }
                                                }
                                            }

                                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                                            DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr);
                                        }
                                    }

                                    break;

                                default:
                                    break;
                            }
                        }

                        EditorGUI.EndDisabledGroup();

                        if (mainScript.rooms.Count > 0)
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
                                var randomPoint = new Vector2Int(Random.Range(radius + 1, mainScript.pcgManager.gridArr.GetLength(0) - 2 - 1), Random.Range(2 + 1, mainScript.pcgManager.gridArr.GetLength(1) - 2 - 1));
                                var sphereRoom = DFAlgoBank.DrawCircle(mainScript.pcgManager.gridArr, randomPoint, 2);

                                if (sphereRoom != null)
                                {
                                    sphereRoom = DFAlgoBank.DrawCircle(mainScript.pcgManager.gridArr, randomPoint, 2, draw: true);

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                                    mainScript.rooms.Add(sphereRoom);
                                }
                            }


                            mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr);
                            var centerPoints = new List<Vector2>();
                            var roomDict = new Dictionary<Vector2, List<Tile>>();
                            foreach (var room in mainScript.rooms)
                            {
                                roomDict.Add(DFAlgoBank.FindMiddlePoint(room), room);
                                centerPoints.Add(DFAlgoBank.FindMiddlePoint(room));
                            }


                            var edges = DFAlgoBank.DelunayTriangulation2D(centerPoints).Item2;

                            foreach (var edge in edges)
                            {
                                var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), ondulation, mainScript.pcgManager.gridArr);
                            }

                            DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, corridorWidth);

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                        }


                        if (mainScript.generatedCorridors)
                            mainScript.allowedForward = true;
                        else
                            mainScript.allowedForward = false;

                    }

                    break;

                case DelunaryMA.UI_STATE.GENERATION:

                    mainScript.allowedBack = true;

                    DFGeneralUtil.GenerateMeshEditorSection(mainScript.pcgManager, saveMapFileName, out saveMapFileName);

                    break;

                default:
                    break;
            }


            if (mainScript.state != DelunaryMA.UI_STATE.GENERATION)
            {
                DFGeneralUtil.SpacesUILayout(4);

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
}