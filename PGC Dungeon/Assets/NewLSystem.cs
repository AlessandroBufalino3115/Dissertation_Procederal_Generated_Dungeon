using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NewLSystem : MonoBehaviour
{

    //https://www.gamedeveloper.com/design/kastle-dungeon-generation-using-l-systems

    public string axium;
    public int iterations = 2;


    public List<Vector3> points = new List<Vector3>();
    public GameObject head;

    //public Dictionary<char, string> rulesDict = new Dictionary<char, string>();


    public List<string> ruleString = new List<string>();
    
    public string endingWord;

    public bool run;

    private int currDirection = 0;


    private void Start()
    {
        head.transform.position = Vector3.zero;

        //rulesDict.Add('C', "SC-B+C-AL");
        //rulesDict.Add('B', "SC+B+CL");
        //rulesDict.Add('A', "SA+A+CLBSB-AL");

        ruleString.Add("SA+A+CLBSB---AL");
        ruleString.Add("SA--C+CLBSB--AL");
        ruleString.Add("SC+A--CL");


    }


    private string RunLSystem(string axium) 
    {
        StringBuilder currentWord = new StringBuilder(axium);

        for (int i = 0; i < iterations; i++)
        {
           // currentWord.Replace("A", rulesDict['A']);
            currentWord.Replace("A", ruleString[Random.Range(0,ruleString.Count-1)]);
        }


        return currentWord.ToString();

    }


    private void Update()
    {
        if (run) 
        {
            run = false;

            head.transform.position = Vector3.zero;

            points.Clear();
            currDirection = 0;
            endingWord = RunLSystem(axium);
            ProcessSentence();



        }
    }

    private void ProcessSentence() 
    {
        Stack<Vector3> lastPositions = new Stack<Vector3>();

        for (int i = 0; i < endingWord.Length; i++)
        {
            //seems to have a 0 somewhere
           // Debug.Log(currDirection);

            switch (endingWord[i])
            {
                case 'A':
                    MoveHead(10);
                   // if (!points.Contains(head.transform.position))
                        points.Add(head.transform.position);
                    break;

                case 'B':
                    MoveHead(15);
                    //if (!points.Contains(head.transform.position))
                        points.Add(head.transform.position);
                    break;

                case 'C':
                    MoveHead(20);
                    //if (!points.Contains(head.transform.position))
                        points.Add(head.transform.position);
                    break;


                case 'S':
                    lastPositions.Push(head.transform.position);
                    break;

                case 'L':
                    head.transform.position=  lastPositions.Pop();
                    break;


                case '+':

                    if (currDirection + 1 >= 5) 
                    {
                        currDirection = 1;
                    }
                    else 
                    {
                        currDirection += 1;
                    }

                    break;

                case '-':


                    if (currDirection - 1 <= 0)
                    {
                        currDirection = 4;
                    }
                    else
                    {
                        currDirection -= 1;
                    }


                    break;

                default:
                    break;
            }

            
        }
    }




    private void MoveHead(int moveAmount) 
    {
        switch (currDirection)
        {
            case 1:
                head.transform.Translate(new Vector3(moveAmount, 0, 0));
                break;


            case 2:
                head.transform.Translate(new Vector3(0, 0, moveAmount));
                break;


            case 3:
                head.transform.Translate(new Vector3(-moveAmount, 0, 0));
                break;


            case 4:
                head.transform.Translate(new Vector3(0, 0, -moveAmount));
                break;


            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var points in points)
        {
            Gizmos.DrawSphere(points, 1f);
        }

        Gizmos.color = Color.green;


        for (int i = 0; i < points.Count; i++)
        {
            if (i != points.Count - 1)
            Gizmos.DrawLine(points[i], points[i + 1]);

        }

    }

}
