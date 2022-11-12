using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHomeState : UiBaseState
{


    public Vector2 scrollPosition = Vector2.zero;

    private int scrollX = 0;
    private int scrollY = 0;
    private int scrollZ = 0;

    private bool scaleToggle;
    private bool ghostToggle;


    public override void onExit(StateUIManager currentMenu)
    {

    }

    public override void onGUI(StateUIManager currentMenu)
    {
        scrollPosition = GUI.BeginScrollView(new Rect(10, 300, 300, 300), scrollPosition, new Rect(0, 0, 220, 200));

        scaleToggle = GUI.Toggle(new Rect(10, 120, 100, 30), scaleToggle, "toggle scale");
        ghostToggle = GUI.Toggle(new Rect(10, 90, 100, 30), ghostToggle, "toggle ghost");

        scrollX = (int)GUI.HorizontalSlider(new Rect(25, 25, 100, 30), scrollX, 3, 75);

        scrollY = (int)GUI.HorizontalSlider(new Rect(25, 50, 100, 30), scrollY, 3, 15);

        scrollZ = (int)GUI.HorizontalSlider(new Rect(25, 75, 100, 30), scrollZ, 3, 75);

        if (GUI.Button(new Rect(10, 160, 100, 30), "basic"))
            currentMenu.ChangeState(1);
        if (GUI.Button(new Rect(10, 200, 100, 30), "voroni"))
            currentMenu.ChangeState(2);
        if (GUI.Button(new Rect(10, 240, 100, 30), "path"))
            currentMenu.ChangeState(3);

        if (GUI.Button(new Rect(10, 280, 100, 30), "GEnDAta")) 
        {
            currentMenu.DestroyAllTiles();
            currentMenu.Gen2DVolume(scrollX, scrollZ, ghostToggle, scaleToggle);
        }
        // End the scroll view that we began above.
        GUI.EndScrollView();
    }

    public override void onStart(StateUIManager currentMenu)
    {
    }

    public override void onUpdate(StateUIManager currentMenu)
    {
    }
}
