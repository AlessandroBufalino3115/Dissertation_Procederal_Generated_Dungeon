using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RanRoomGenMA))]
public class RanRoomGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RanRoomGenMA mainScript = (RanRoomGenMA)target;


        mainScript.MaxHeight = (int)EditorGUILayout.Slider(mainScript.MaxHeight, 15, 100);
       
        if (mainScript.MaxHeight <= mainScript.MinHeight) 
        {
            mainScript.MaxHeight = mainScript.MinHeight + 1;
        }

        mainScript.MinHeight = (int)EditorGUILayout.Slider(mainScript.MinHeight, 10, 50);



        mainScript.MaxWidth = (int)EditorGUILayout.Slider(mainScript.MaxWidth , 15, 100);
        if (mainScript.MaxWidth <= mainScript.MinWidth)
        {
            mainScript.MaxWidth = mainScript.MinWidth + 1;
        }
        mainScript.MinWidth = (int)EditorGUILayout.Slider(mainScript.MinWidth, 10, 50);




        mainScript.NumOfRoom = (int)EditorGUILayout.Slider(mainScript.NumOfRoom, 10, 20);



        mainScript.BPSg = EditorGUILayout.Toggle("Which algo", mainScript.BPSg);
        mainScript.Additive = EditorGUILayout.Toggle("additive", mainScript.Additive);






        if (mainScript.BPSg) 
        {
            if (GUILayout.Button("Use BPS algo"))// gen something
            {
                mainScript.RoomList.Clear();


                if (!mainScript.Additive)
                {
                    mainScript.RoomList.Clear();
                    mainScript.PcgManager.gridArray2D = AlgosUtils.RestartArr(mainScript.PcgManager.gridArray2D);
                }


                mainScript.BPSRoomGen(mainScript.PcgManager.gridArray2D);
                mainScript.SetUpWeights(mainScript.PcgManager.gridArray2D);
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);
            }
        }
        else 
        {
            if (GUILayout.Button("Use random Room gen"))// gen something
            {
                mainScript.roomList.Clear();

                if (!mainScript.Additive)
                {
                    mainScript.PcgManager.gridArray2D = AlgosUtils.RestartArr(mainScript.PcgManager.gridArray2D);
                }

                mainScript.RandomRoomGen(mainScript.PcgManager.gridArray2D);
                mainScript.SetUpWeights(mainScript.PcgManager.gridArray2D);
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);
            }
        }




    }
}
