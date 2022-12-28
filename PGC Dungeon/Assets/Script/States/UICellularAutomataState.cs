using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UICellularAutomataState : UiBaseState
{

    private float startingPerc = 0;
    // private int scrollY = 0;
    private int aliveNeighbours = 0;


    // need an auto run iter for button
    // we can implement a back for the if we store the number of run implemenations
    // we would also have to save the random


    public override void onExit(StateUIManager currentMenu)
    {
    }

    public override void onGUI(StateUIManager currentMenu)
    {

        GUI.Box(new Rect(5, 10, 230, 650), "");

        startingPerc = GUI.HorizontalSlider(new Rect(10, 25, 100, 20), startingPerc, 0.2f, 0.8f);
        GUI.Label(new Rect(140, 20, 100, 30), "Starting %: " + startingPerc);

        aliveNeighbours = (int)GUI.HorizontalSlider(new Rect(10, 75, 100, 20), aliveNeighbours, 3, 5);
        GUI.Label(new Rect(140, 70, 100, 30), "Height: " + aliveNeighbours);


        if (GUI.Button(new Rect(10, 240, 120, 30), "Spawn random points"))
        {
            switch (currentMenu.dimension)
            {
              
                case StateUIManager.Dimension.TWOD:
                    AlgosUtils.SpawnRandomPointsObj2D(currentMenu.gridArrayObj2D, startingPerc);
                    AlgosUtils.SetColorAllObjAnchor(currentMenu.gridArrayObj2D);

                    break;
              
                case StateUIManager.Dimension.PLANE:
                    AlgosUtils.SpawnRandomPointsObj2D(currentMenu.gridArray2D, startingPerc);
                   
                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D);

                    break;
                default:
                    break;
            }
        }

        if (GUI.Button(new Rect(10, 280, 100, 30), "Run Iter"))
        {
            switch (currentMenu.dimension)
            {

                case StateUIManager.Dimension.TWOD:
                    AlgosUtils.RunCaIteration2D(currentMenu.gridArrayObj2D, aliveNeighbours);
                    AlgosUtils.SetColorAllObjAnchor(currentMenu.gridArrayObj2D);
                    break;

                case StateUIManager.Dimension.PLANE:
                    AlgosUtils.RunCaIteration2D(currentMenu.gridArray2D, aliveNeighbours);
                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D);

                    break;
                default:
                    break;
            }
        }

        if (GUI.Button(new Rect(10, 320, 100, 30), "clean up"))
        {
            switch (currentMenu.dimension)
            {

                case StateUIManager.Dimension.TWOD:
                    AlgosUtils.CleanUp2dCA(currentMenu.gridArrayObj2D, aliveNeighbours);
                    AlgosUtils.SetColorAllObjAnchor(currentMenu.gridArrayObj2D);
                    break;

                case StateUIManager.Dimension.PLANE:
                    AlgosUtils.CleanUp2dCA(currentMenu.gridArray2D, aliveNeighbours);
                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D);

                    break;
                default:
                    break;
            }
        }








        if (GUI.Button(new Rect(10, 360, 100, 30), "Go back"))
            currentMenu.ChangeState(0);

        // graph grammar and WFC on their own
    }

    public override void onStart(StateUIManager currentMenu)
    {
    }

    public override void onUpdate(StateUIManager currentMenu)
    {
    }
}
