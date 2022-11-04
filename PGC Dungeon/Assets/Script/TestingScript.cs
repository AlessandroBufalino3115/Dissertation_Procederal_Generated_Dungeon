using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static TestingScript;
using Random = UnityEngine.Random;

public class TestingScript : MonoBehaviour
{
    // In this example we show how to invoke a coroutine and
    // continue executing the function in parallel.

    //private IEnumerator coroutine;

    //void Start()
    //{
    //    // - After 0 seconds, prints "Starting 0.0"
    //    // - After 0 seconds, prints "Before WaitAndPrint Finishes 0.0"
    //    // - After 2 seconds, prints "WaitAndPrint 2.0"
    //    print("Starting " + Time.time);

    //    // Start function WaitAndPrint as a coroutine.

    //    coroutine = WaitAndPrint(2.0f);
    //    StartCoroutine(coroutine);

    //    print("Before WaitAndPrint Finishes " + Time.time);
    //}

    //// every 2 seconds perform the print()
    //private IEnumerator WaitAndPrint(float waitTime)
    //{

    //    int num = 0;


    //    while (true)
    //    {
    //        yield return new WaitForSeconds(waitTime);
    //        print("WaitAndPrint " + Time.time);


    //        num++;


    //        if (num == 5) { break; }
    //    }



    //    Debug.Log($"time num is called {num}");

    //}
    //private void Start()
    //{
    //    MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
    //    CombineInstance[] combine = new CombineInstance[meshFilters.Length];

    //    int i = 0;
    //    while (i < meshFilters.Length)
    //    {
    //        combine[i].mesh = meshFilters[i].sharedMesh;
    //        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
    //        meshFilters[i].gameObject.SetActive(false);

    //        i++;
    //    }
    //    transform.GetComponent<MeshFilter>().mesh = new Mesh();
    //    transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    //    transform.gameObject.SetActive(true);
    //}


    private void Start()
    {
        int[] array1 = new int[5] { 1, 2, 3, 4, 5 }; 
        int[] array2 = new int[3] { 9,3,5 }; 
        int[] array3 = new int[6] { 0,8,6,7,4,3 }; 
        int[] array4 = new int[3] { 2,3,9 };


        var commonElements1 = array1.Intersect(array2).ToArray();
        var commonElements2 = array3.Intersect(array4).ToArray();

        var commonElementsFinal = commonElements2.Intersect(commonElements1).ToArray();


        //foreach (var element in commonElementsFinal) 
        //{
        //    Debug.Log($"{element}");
        //}



    }

}
