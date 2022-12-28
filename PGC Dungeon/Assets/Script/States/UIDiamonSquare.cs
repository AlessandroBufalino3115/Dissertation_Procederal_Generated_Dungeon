using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDiamonSquare : UiBaseState
{

    private int minWeight;
    private int maxWeight;
    private int roughness;



    public override void onExit(StateUIManager currentMenu)
    {
    }

    public override void onGUI(StateUIManager currentMenu)
    {
        //need to check for size too

        if (currentMenu.working)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "LOADING");
        }
        else
        {
            GUI.Box(new Rect(5, 10, 230, 650), "");   //background 


           roughness = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), roughness, 2, 8);
           minWeight = (int)GUI.HorizontalSlider(new Rect(10, 75, 100, 20), minWeight, -32, -4);
           maxWeight = (int)GUI.HorizontalSlider(new Rect(10, 100, 100, 20), maxWeight, 4, 32);



            if (GUI.Button(new Rect(10, 140, 120, 30), "Run diamond-Square"))
            {
                switch (currentMenu.dimension)
                {
                
                    case StateUIManager.Dimension.TWOD:
                        AlgosUtils.DiamondSquare(maxWeight, minWeight, roughness, currentMenu.gridArrayObj2D);
                        AlgosUtils.SetColorAllObjBi(currentMenu.gridArrayObj2D,minWeight,maxWeight);

                        break;
              
                    case StateUIManager.Dimension.PLANE:

                        AlgosUtils.DiamondSquare(maxWeight, minWeight, roughness, currentMenu.gridArray2D);

                        currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColShade(currentMenu.gridArray2D,minWeight,maxWeight);


                        break;

                    default:
                        break;
                }

            }


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
}
