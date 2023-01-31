using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class testScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var arrayA = new testingClass[2];

        arrayA[0] = new testingClass() { name= "nameA"  , age = 0};
        arrayA[1] = new testingClass() { name= "nameB"  , age = 1};

      var   arrayB = new List<testingClass>(arrayA).ToArray();


        foreach (var item in arrayA)
        {
            Debug.Log($"{item.age}   +  {item.name}  \n");
        }
        Debug.Log($"This is for the arr A\n\n\n");


        foreach (var item in arrayB)
        {
            Debug.Log($"{item.age}   +  {item.name}  \n");
        }
        Debug.Log($"This is for the arr B\n\n\n");


        arrayB[0].age = 3;

        foreach (var item in arrayA)
        {
            Debug.Log($"{item.age}   +  {item.name}  \n");
        }
        Debug.Log($"This is for the arr A posty change\n\n\n");


        foreach (var item in arrayB)
        {
            Debug.Log($"{item.age}   +  {item.name}  \n");
        }
        Debug.Log($"This is for the arr b post chaten\n\n\n");
    }


    public class testingClass 
    {
       public string name;
       public int age;
    }

}
