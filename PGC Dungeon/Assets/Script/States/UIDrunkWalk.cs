using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIDrunkWalk : UiBaseState
{

    private int iterations;
    private int iterationsLeft;

    private bool alreadyPassed;

    private bool hardStop;

    private bool crunching;


    public override void onExit(StateUIManager currentMenu)
    {
    }

    public override void onGUI(StateUIManager currentMenu)
    {


        if (crunching) 
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "LOADING");
        }
        else 
        {
            GUI.Box(new Rect(5, 10, 230, 650), "");   //background 


            if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                iterations = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), iterations, 20, (int)((currentMenu.gridArray2D.Length * currentMenu.gridArray2D[0].Length)) * 0.9f);

            else if (currentMenu.dimension == StateUIManager.Dimension.THREED)
                iterations = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), iterations, 20, (int)((currentMenu.gridArray3D.Length * currentMenu.gridArray3D[0].Length * currentMenu.gridArray3D[0][0].Length)) * 0.9f);

            else if (currentMenu.dimension == StateUIManager.Dimension.PLANE) { }





            GUI.Label(new Rect(130, 45, 100, 30), "iterations: " + iterations);

            GUI.Label(new Rect(130, 70, 100, 30), "Iterations Left: " + iterationsLeft);


            alreadyPassed = GUI.Toggle(new Rect(10, 100, 120, 30), alreadyPassed, "Overwrite cells");

            if (GUI.Button(new Rect(10, 140, 120, 30), "Run Drunk Walk"))
            {
                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    DrunkWalk2D(currentMenu);
                else if (currentMenu.dimension == StateUIManager.Dimension.THREED)
                    DrunkWalk3D(currentMenu);
                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE) { }

            }

            hardStop = GUI.Toggle(new Rect(10, 190, 100, 30), hardStop, "Hard stop");


            if (GUI.Button(new Rect(10, 360, 100, 30), "Go back"))
                currentMenu.ChangeState(0);
        }

    }

    public override void onStart(StateUIManager currentMenu)
    {
    }

    public override void onUpdate(StateUIManager currentMenu)
    {
    }





    private void DrunkWalk2D(StateUIManager currentMenu) 
    {
        if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
        {
            crunching = true;
            AlgosUtils.DrunkWalk2DCol(currentMenu.gridArray2D, iterations, alreadyPassed);

            crunching = false;
        }
    }



    private void DrunkWalk3D(StateUIManager currentMenu) 
    {
        if (currentMenu.dimension == StateUIManager.Dimension.THREED)
        {

            crunching = true;
            AlgosUtils.DrunkWalk3DCol(currentMenu.gridArray3D, iterations, alreadyPassed);
            crunching=false;
        }
    }

}
