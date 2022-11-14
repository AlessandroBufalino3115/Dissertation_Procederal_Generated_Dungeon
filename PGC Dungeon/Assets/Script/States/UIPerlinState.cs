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

    public override void onGUI(StateUIManager currentMenu)
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

        persistance = GUI.HorizontalSlider(new Rect(10, 150, 100, 20), persistance, 0, 1f);
        Math.Round(persistance, 4);
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
                float[,] noiseMap = AlgosUtils.PerlinNoise2DTileSet(currentMenu.gridArray2D, scale, octaves, persistance, lacunarity, offsetX, offsetY);

                for (int y = 0; y < currentMenu.gridArray2D.Length; y++)
                {
                    for (int x = 0; x < currentMenu.gridArray2D[0].Length; x++)
                    {
                        if (thresholdBool) 
                        {
                            if (threshold > noiseMap[x, y])
                                currentMenu.gridArray2D[y][x].tileObj.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1);
                            else
                                currentMenu.gridArray2D[y][x].tileObj.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);

                        }
                        else
                            currentMenu.gridArray2D[y][x].tileObj.GetComponent<MeshRenderer>().material.color = new Color(noiseMap[x, y], noiseMap[x, y], noiseMap[x, y]);
                    }
                }
            }
            else if (currentMenu.dimension == StateUIManager.Dimension.THREED) 
            {
                
                // eveyrthing is to test out on this 

            }
            else if (currentMenu.dimension == StateUIManager.Dimension.PLANE) 
            {

                AlgosUtils.DrawNoiseMap(currentMenu.plane.GetComponent<MeshRenderer>(), currentMenu.xSize, currentMenu.zSize, scale, octaves, persistance, lacunarity,offsetX,offsetY,threshold,thresholdBool);

            }
        }


        if (GUI.Button(new Rect(10, 300, 120, 30), "Go Back"))
            currentMenu.ChangeState(0);
            





            // scale
            // offset y
            // offset x
            // ocatves
            // pers
            // lacu

            //gen



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
