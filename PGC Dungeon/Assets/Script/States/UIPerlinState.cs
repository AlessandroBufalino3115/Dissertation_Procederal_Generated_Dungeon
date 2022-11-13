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



    public override void onExit(StateUIManager currentMenu)
    {
    }

    public override void onGUI(StateUIManager currentMenu)
    {

        GUI.Box(new Rect(5, 10, 230, 650), "");

        offsetX = (int)GUI.HorizontalSlider(new Rect(10, 25, 100, 20), offsetX, 0, 10000);
        GUI.Label(new Rect(140, 20, 100, 30), "Offset X: " + offsetX);

        offsetY = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), offsetY, 0, 10000);
        GUI.Label(new Rect(140, 45, 100, 30), "Offset Y: " + offsetY);

        if (currentMenu.dimension == StateUIManager.Dimension.THREED) 
        {
            offsetZ = (int)GUI.HorizontalSlider(new Rect(10, 75, 100, 20), offsetZ, 0, 10000);
            GUI.Label(new Rect(140, 70, 100, 30), "Offset Z: " + offsetZ);

            threshold = GUI.HorizontalSlider(new Rect(180, 75, 100, 20), threshold, 0, 1f);
            GUI.Label(new Rect(290, 70, 100, 30), "Threshold " + threshold);
        }

        if (currentMenu.dimension == StateUIManager.Dimension.THREED)
            scale = GUI.HorizontalSlider(new Rect(10, 100, 100, 20), scale, 0, 1);
        else 
            scale = (int)GUI.HorizontalSlider(new Rect(10, 100, 100, 20), scale, 0, 35);
        GUI.Label(new Rect(140, 95, 100, 30), "Scale: " + scale);

        octaves = (int)GUI.HorizontalSlider(new Rect(10, 125, 100, 20), octaves, 2, 8);
        GUI.Label(new Rect(140, 120, 120, 30), "Octaves: " + octaves);

        persistance = GUI.HorizontalSlider(new Rect(10, 150, 100, 20), persistance, 0, 1f);
        GUI.Label(new Rect(140, 145, 130, 30), "Persistance: " + persistance);

        lacunarity = GUI.HorizontalSlider(new Rect(10, 175, 100, 20), lacunarity, 0, 10f);
        GUI.Label(new Rect(140, 170, 130, 30), "lacunarity: " + lacunarity);









        if (GUI.Button(new Rect(10, 200, 120, 30), "Gen Noise"))
        {
            if (currentMenu.dimension == StateUIManager.Dimension.TWOD) 
            {
                float[,] noiseMap = AlgosUtils.PerlinNoise2DTileSet(currentMenu.gridArray2D, scale, octaves, persistance, lacunarity);

                for (int y = 0; y < currentMenu.gridArray2D.Length; y++)
                {
                    for (int x = 0; x < currentMenu.gridArray2D[0].Length; x++)
                    {
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

                AlgosUtils.DrawNoiseMap(currentMenu.plane.GetComponent<MeshRenderer>(), currentMenu.xSize, currentMenu.zSize, scale, octaves, persistance, lacunarity);

            }
        }








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
