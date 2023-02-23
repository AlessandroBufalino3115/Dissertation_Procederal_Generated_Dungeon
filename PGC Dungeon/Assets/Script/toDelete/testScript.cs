using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class testScript : MonoBehaviour
{

    [SerializeField]
    private List<Vector2Int> test = new List<Vector2Int>() 
    {
        
        new Vector2Int(0,0), 
        new Vector2Int(1,0), 
        new Vector2Int(2,0), 
        new Vector2Int(3,0), 
        new Vector2Int(4,0) 
    
    };

    public Vector2Int test2 = new Vector2Int(0,0);


    private void Start()
    {
       if (test.Contains(test2)) 
        {
            Debug.Log($"adadadadad");
        }
        else 
        {
            Debug.Log($"bvcbcvbvcbc");
        }
    }

}
