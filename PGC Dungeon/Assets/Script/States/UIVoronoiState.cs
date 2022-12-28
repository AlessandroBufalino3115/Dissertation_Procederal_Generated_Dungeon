using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIVoronoiState : UiBaseState
{


    private List<Vector3> veronoiPoints2D = new List<Vector3>();
    private List<Color> listColor = new List<Color>();

    private int points;

    public Vector2 scrollPosition = Vector2.zero;


    public override void onExit(StateUIManager currentMenu)
    {
    }

    public override void onGUI(StateUIManager currentMenu)
    {


        GUI.Box(new Rect(5, 10, 230, 560), "");


        points = (int)GUI.HorizontalSlider(new Rect(10, 25, 100, 20), points, 3, 20);
        GUI.Label(new Rect(140, 20, 100, 30), "Points: " + points);

        if (GUI.Button(new Rect(10, 60, 150, 20), "Gen Voroni Points 2D")) 
        {
            veronoiPoints2D.Clear();
            for (int i = 0; i < points; i++)
            {
                
                //Vector3 topRight = currentMenu.gridArrayObj2D[currentMenu.height - 1][currentMenu.width - 1].position;
                //Vector3 botLeft = currentMenu.gridArrayObj2D[0][0].position;

                Vector2 ranVector = new Vector2(Random.Range(0, currentMenu.width), Random.Range(0, currentMenu.height));

                veronoiPoints2D.Add(new Vector2(ranVector.x, ranVector.y));
            }



            if (currentMenu.dimension == StateUIManager.Dimension.TWOD) 
            {
                currentMenu.gridArrayObj2D = AlgosUtils.VoronoiObj2D(currentMenu.gridArrayObj2D, veronoiPoints2D);
                ColourInObj2D(currentMenu);
            }
            else if (currentMenu.dimension == StateUIManager.Dimension.PLANE) 
            {
                currentMenu.gridArray2D = AlgosUtils.Voronoi2D(currentMenu.gridArray2D, veronoiPoints2D);
                ColourIn2D(currentMenu);
            }
        }

        if (GUI.Button(new Rect(10, 90, 150, 20), "Go back to Main Menu"))
            currentMenu.ChangeState(0);

    }

    public override void onStart(StateUIManager currentMenu)
    {
        for (int i = 0; i < 20; i++)
        {
            listColor.Add(new Color(Random.Range(0.01f, 0.99f), Random.Range(0.01f, 0.99f), Random.Range(0.01f, 0.99f)));
        }
    }

    public override void onUpdate(StateUIManager currentMenu)
    {
    }

    private void ColourInObj2D(StateUIManager currentMenu) 
    {
        for (int y = 0; y < currentMenu.gridArrayObj2D.Length; y++)
        {
            for (int x = 0; x < currentMenu.gridArrayObj2D[y].Length; x++)
            {
                currentMenu.gridArrayObj2D[y][x].SetColor(listColor[currentMenu.gridArrayObj2D[y][x].idx]);
            }
        }
    }


    private void ColourIn2D(StateUIManager currentMenu)
    {
        for (int y = 0; y < currentMenu.gridArray2D.Length; y++)
        {
            for (int x = 0; x < currentMenu.gridArray2D[y].Length; x++)
            {
                currentMenu.gridArray2D[y][x].color = listColor[currentMenu.gridArray2D[y][x].idx];
            }
        }

        currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextSelfCol(currentMenu.gridArray2D);

    }


    /*
    private void CallVoronoiGen2D(StateUIManager currentMenu)
    {
        veronoiPoints2D = new List<Vector2>();
        listColor = new List<Color>();

        GameObject topRight = currentMenu.gridArrayObj2D[currentMenu.gridArrayObj2D.Length - 1][currentMenu.gridArrayObj2D[0].Length - 1].tileObj;
        GameObject botLeft = currentMenu.gridArrayObj2D[0][0].tileObj;


        var topRightCor_X = topRight.transform.position.x;
        var topRightCor_Y = topRight.transform.position.z;

        var botLeftCor_X = botLeft.transform.position.x;
        var botLeftCor_Y = botLeft.transform.position.z;


        for (int i = 0; i < points; i++)
        {

            float ran_r = Random.Range(0.01f, 0.99f);
            float ran_g = Random.Range(0.01f, 0.99f);
            float ran_b = Random.Range(0.01f, 0.99f);

            listColor.Add(new Color(ran_r, ran_g, ran_b));


            Vector2 ranVector = new Vector2(Random.Range(botLeftCor_X, topRightCor_X), Random.Range(botLeftCor_Y, topRightCor_Y));

            veronoiPoints2D.Add(new Vector2(ranVector.x, ranVector.y));
        }





        for (int y = 0; y < currentMenu.gridArrayObj2D.Length; y++)
        {
            for (int x = 0; x < currentMenu.gridArrayObj2D[y].Length; x++)
            {
                int closestIndex = 0;
                float closestDistance = -1;

                for (int i = 0; i < veronoiPoints2D.Count; i++)
                {
                    if (closestDistance < 0)  //therefore minus therefoe we just started
                    {
                        closestDistance = GeneralUtil.EuclideanDistance2D(veronoiPoints2D[i], new Vector2(currentMenu.gridArrayObj2D[y][x].tileObj.transform.position.x, currentMenu.gridArrayObj2D[y][x].tileObj.transform.position.z));
                    }
                    else
                    {
                        float newDist = GeneralUtil.EuclideanDistance2D(veronoiPoints2D[i], new Vector2(currentMenu.gridArrayObj2D[y][x].tileObj.transform.position.x, currentMenu.gridArrayObj2D[y][x].tileObj.transform.position.z));

                        if (closestDistance > newDist)
                        {
                            closestDistance = newDist;
                            closestIndex = i;

                        }
                    }
                }

                currentMenu.gridArrayObj2D[y][x].tileObj.GetComponent<MeshRenderer>().material.color = listColor[closestIndex];

            }
        }
    }
    */

}
