using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIPerlinState : UiBaseState
{

    private int offsetX;
    private int offsetY;
    private int offsetZ;

    private float scale;
    private int octaves;
    private float persistance;
    private float lacunarity;


    private float threshold;
    private bool thresholdBool;


    public override void onExit(StateUIManager currentMenu)
    {
    }
    public override void onGizmos(StateUIManager currentMenu)
    {
    }
    public override void onGUI(StateUIManager currentMenu)
    {

        if (currentMenu.working)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "LOADING");
        }
        else
        {
            GUI.Box(new Rect(5, 10, 260, 650), "");

            offsetX = (int)GUI.HorizontalSlider(new Rect(10, 25, 100, 20), offsetX, 0, 10000);
            GUI.Label(new Rect(140, 20, 100, 30), "Offset X: " + offsetX);

            offsetY = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), offsetY, 0, 10000);
            GUI.Label(new Rect(140, 45, 100, 30), "Offset Y: " + offsetY);

            if (currentMenu.dimension == StateUIManager.Dimension.THREED)
            {
                offsetZ = (int)GUI.HorizontalSlider(new Rect(10, 75, 100, 20), offsetZ, 0, 10000);
                GUI.Label(new Rect(140, 70, 100, 30), "Offset Z: " + offsetZ);
            }

            if (currentMenu.dimension == StateUIManager.Dimension.THREED)
                scale = GUI.HorizontalSlider(new Rect(10, 100, 100, 20), scale, 0, 1);
            else
                scale = (int)GUI.HorizontalSlider(new Rect(10, 100, 100, 20), scale, 0, 35);
            GUI.Label(new Rect(140, 95, 100, 30), "Scale: " + scale);

            octaves = (int)GUI.HorizontalSlider(new Rect(10, 125, 100, 20), octaves, 2, 8);
            GUI.Label(new Rect(140, 120, 120, 30), "Octaves: " + octaves);

            Math.Round(persistance, 4);
            persistance = GUI.HorizontalSlider(new Rect(10, 150, 100, 20), persistance, 0, 1f);
            GUI.Label(new Rect(140, 145, 140, 30), "Persistance: " + persistance);

            lacunarity = GUI.HorizontalSlider(new Rect(10, 175, 100, 20), lacunarity, 0, 10f);
            Math.Round(lacunarity, 4);
            GUI.Label(new Rect(140, 170, 130, 30), "lacunarity: " + lacunarity);



            thresholdBool = GUI.Toggle(new Rect(10, 200, 100, 30), thresholdBool, "threshold");

            if (thresholdBool)
            {
                threshold = GUI.HorizontalSlider(new Rect(10, 230, 100, 20), threshold, 0, 1f);
                GUI.Label(new Rect(140, 230, 130, 30), "threshold: " + threshold); ;
            }


            if (GUI.Button(new Rect(10, 260, 120, 30), "Gen Noise"))
            {
                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                {

                    if (thresholdBool)
                        currentMenu.gridArrayObj2D = AlgosUtils.PerlinNoise2DTileSet(currentMenu.gridArrayObj2D, scale, octaves, persistance, lacunarity, offsetX, offsetY, threshold);

                    else
                        currentMenu.gridArrayObj2D = AlgosUtils.PerlinNoise2DTileSet(currentMenu.gridArrayObj2D, scale, octaves, persistance, lacunarity, offsetX, offsetY);



                    for (int y = 0; y < currentMenu.gridArrayObj2D.Length; y++)
                    {
                        for (int x = 0; x < currentMenu.gridArrayObj2D[0].Length; x++)
                        {
                            currentMenu.gridArrayObj2D[y][x].SetColorBi(0, 1);
                        }
                    }
                }
             
                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                {



                    if (thresholdBool)
                    {
                        currentMenu.gridArray2D = AlgosUtils.PerlinNoise2D(currentMenu.gridArray2D, scale, octaves, persistance, lacunarity, offsetX, offsetY, threshold);

                        currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D, true);
                    }
                    else
                    {
                        currentMenu.gridArray2D = AlgosUtils.PerlinNoise2D(currentMenu.gridArray2D, scale, octaves, persistance, lacunarity, offsetX, offsetY);

                        currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColShade(currentMenu.gridArray2D, 0, 1);
                    }


                }
            }



            if (GUI.Button(new Rect(10, 350, 120, 30), "Run Wall finding"))
            {




                currentMenu.working = true;
                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    AlgosUtils.SetUpTileTypesFloorWall(currentMenu.gridArrayObj2D);

                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                {
                    AlgosUtils.SetUpTileTypesFloorWall(currentMenu.gridArray2D);
                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColShade(currentMenu.gridArray2D, 0, 1, true);
                }


                currentMenu.working = false;

            }



            if (GUI.Button(new Rect(10, 440, 120, 30), "Gen mesh"))
            {


                currentMenu.working = true;

                currentMenu.FormObject(AlgosUtils.MarchingCubesAlgo(AlgosUtils.ExtrapolateMarchingCubes(currentMenu.gridArray2D), false));


                currentMenu.working = false;
            }



            if (GUI.Button(new Rect(10, 300, 120, 30), "Go Back"))
                currentMenu.ChangeState(0);







        }
    }

    public override void onStart(StateUIManager currentMenu)
    {
    }

    public override void onUpdate(StateUIManager currentMenu)
    {
    }






    public static void DrawNoiseMap(StateUIManager currentMenu, Renderer meshRenderer)
    {
        
        
    }










}
