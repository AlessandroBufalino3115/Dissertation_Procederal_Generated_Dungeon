using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;
using System.Xml;

// we need a list that holds all of the possible gameobjects

// by defualt they hold all of the possible outcomes
// on the first run choose at random 
// run a test on everyone of them and get the closest one 


// every gameobject type has a default tile that can be next

// example the 



[Serializable]
public class WFCtile
{
    public GameObject tileObj;

    public int[] allowedObjLeft;
    public int[] allowedObjRight;
    public int[] allowedObjForward;
    public int[] allowedObjBackwards;

    public Vector2 choosenIndex = new Vector2(-1, -1);

}





public class WaveFunctionCollaps : MonoBehaviour
{

    [SerializeField]
    public WFCtile[] arrayOfModules; 




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}







