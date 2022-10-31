using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drunkWalk : MonoBehaviour
{


    //given a 3d array start at the mid point and then do the walk


    public int _x;
    public int _y;
    public int _z;


    public GameObject _gameObject;

    public List<GameObject> positionsNodes;
    public Vector3 midPoint;

    public int iter;

    // Start is called before the first frame update
    void Start()
    {

        int idx = 0;


        for (int z = 0; z < _z; z++)
        {
            for (int y = 0; y < _y; y++)
            {
                for (int x = 0; x < _x; x++)
                {



                    Vector3 position = new Vector3(1 * x, 1 * y, 1 * z);
                    GameObject newRef = Instantiate(_gameObject, this.transform);
                    newRef.transform.position = position;
                    newRef.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    newRef.transform.name = idx.ToString();

                    positionsNodes.Add(newRef);

                    idx++;

                }
            }
        }



        //midPoint = new Vector3((_x - 1) / 2, (_y - 1) / 2, (_z - 1) / 2);

        positionsNodes[(positionsNodes.Count - 1) / 2].GetComponent<MeshRenderer>().material.color = Color.blue;
        Debug.Log($"{midPoint}");


        int currIndex = 0;//(positionsNodes.Count - 1) / 2;

        // this osort of works dont want to spend too much time on it tho htere are issuse with the overloading need to check that

        for (int i = 0; i < 14; i++)
        {
            if ((currIndex + 1) % _x == 0)
            {
                if (currIndex - 1 != 0)
                {

                    Debug.Log($"called overload {currIndex}");
                    currIndex += _x ;

                    Debug.Log($"called overload new index {currIndex}\n\n");

                }
            }
            else { currIndex += 1; Debug.Log($"this si for the normal call {currIndex}");  }


            positionsNodes[currIndex].GetComponent<MeshRenderer>().material.color = Color.blue;
        }



        //int maxnum = positionsNodes.Count - 1;
        //for (int i = 0; i < iter; i++)
        //{
        //    int randNum = Random.Range(1, 6);

        //    switch (randNum)
        //    {
        //        case 1:// above 

        //            //currIndex += _y;


        //            break;

        //        case 2:  // below

        //            //currIndex -= _y;

        //            break;

        //        case 3:  // left


        //            currIndex -= 1;


        //            if (currIndex < 0) { currIndex = 0; }


        //            break;
        //        case 4:  //right

        //            if ((currIndex -1) % _x == 0 ) 
        //            {
        //                if (currIndex -1 != 0) 
        //                {

        //                Debug.Log($"called overload {currIndex}");
        //                currIndex += _x -1;

        //                Debug.Log($"called overload new index {currIndex}\n\n");

        //                }
        //            }
        //            else { currIndex += 1; }


        //            break;

        //        case 5:  //forw


        //            //currIndex += (_y * _x);
        //            break;

        //        case 6:  //back

        //            //currIndex -= (_y * _x);
        //            break;










        //        default:
        //            break;
        //    }


        //Debug.Log($"pointer at {currIndex}, {maxnum}");


            //if (currIndex >= maxnum || currIndex < 0) { }
           // else
           // {
                
            //}



            }



            //for (int i = 0; i < positionsNodes.Count; i++)
            //{
            //    if (positionsNodes[i].GetComponent<MeshRenderer>().material.color != Color.blue)
            //    {
            //        positionsNodes[i].SetActive(false);
            //    }
            //}



        }

    
//}
