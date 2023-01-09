using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static TestingScript;
using Random = UnityEngine.Random;

public class TestingScript : MonoBehaviour
{

    [SerializeField]
    public AnimationCurve testAnimCurve;

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


    public Renderer textureRender;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;


    private void Start()
    {

        //GameObject.CreatePrimitive(PrimitiveType.Plane);

        float[,] noiseMap = GenerateNoiseMap(mapWidth, mapHeight, noiseScale);

        DrawNoiseMap(noiseMap);


        // https://www.youtube.com/watch?v=WP-Bm65Q-1Y

    }



    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();


        texture.filterMode = FilterMode.Point;
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(width, 1, height);
    }



    public float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

}
