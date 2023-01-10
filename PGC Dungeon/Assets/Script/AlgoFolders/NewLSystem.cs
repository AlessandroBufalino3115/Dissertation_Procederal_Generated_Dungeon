using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class NewLSystem : MonoBehaviour
{
    //https://www.gamedeveloper.com/design/kastle-dungeon-generation-using-l-systems

    public string axium;
    public int iterations = 2;

    public string fileName = "";

    public List<Vector3> points = new List<Vector3>();
    public GameObject head;
    
    public string endingWord;

    public bool run;

    private int currDirection = 0;

    public List<string> A_RuleSet = new List<string>();
    public List<string> B_RuleSet = new List<string>();
    public List<string> C_RuleSet = new List<string>();
    public List<string> S_RuleSet = new List<string>();
    public List<string> L_RuleSet = new List<string>();
    public List<string> P_RuleSet = new List<string>();
    public List<string> N_RuleSet = new List<string>();


    private PCGManager pcgManager;
    public PCGManager PcgManager
    {
        get { return pcgManager; }
    }


    public void InspectorAwake()
    {
        head.transform.position = Vector3.zero;
        pcgManager = this.transform.GetComponent<PCGManager>();
    }


    private string RunLSystem(string axium) 
    {
        string currentWord = axium;

        for (int i = 0; i < iterations; i++)
        {
            Queue<string> newWordQue = new Queue<string>();

            for (int x = 0; x < currentWord.Length; x++)
            {
                switch (currentWord[x])
                {
                    case 'A':

                        if (A_RuleSet.Count == 0)  
                            newWordQue.Enqueue(currentWord[x].ToString()); 

                        else if (A_RuleSet.Count == 1) 
                            newWordQue.Enqueue(A_RuleSet[0]);
                 
                        else   
                            newWordQue.Enqueue(A_RuleSet[Random.Range(0, A_RuleSet.Count - 1)]);

                        break;

                    case 'B':

                        if (B_RuleSet.Count == 0)
                            newWordQue.Enqueue(currentWord[x].ToString());

                        else if (B_RuleSet.Count == 1)
                            newWordQue.Enqueue(B_RuleSet[0]);

                        else
                            newWordQue.Enqueue(B_RuleSet[Random.Range(0, B_RuleSet.Count - 1)]);

                        break;

                    case 'C':

                        if (C_RuleSet.Count == 0)
                            newWordQue.Enqueue(currentWord[x].ToString());

                        else if (C_RuleSet.Count == 1)
                            newWordQue.Enqueue(C_RuleSet[0]);

                        else
                            newWordQue.Enqueue(C_RuleSet[Random.Range(0, C_RuleSet.Count - 1)]);

                        break;

                    case 'S':

                        if (S_RuleSet.Count == 0)
                            newWordQue.Enqueue(currentWord[x].ToString());

                        else if (S_RuleSet.Count == 1)
                            newWordQue.Enqueue(S_RuleSet[0]);

                        else
                            newWordQue.Enqueue(S_RuleSet[Random.Range(0, S_RuleSet.Count - 1)]);

                        break;

                    case 'L':

                        if (L_RuleSet.Count == 0)
                            newWordQue.Enqueue(currentWord[x].ToString());

                        else if (L_RuleSet.Count == 1)
                            newWordQue.Enqueue(L_RuleSet[0]);

                        else
                            newWordQue.Enqueue(L_RuleSet[Random.Range(0, L_RuleSet.Count - 1)]);

                        break;

                    case 'P':

                        if (P_RuleSet.Count == 0)
                            newWordQue.Enqueue(currentWord[x].ToString());

                        else if (P_RuleSet.Count == 1)
                            newWordQue.Enqueue(P_RuleSet[0]);

                        else
                            newWordQue.Enqueue(P_RuleSet[Random.Range(0, P_RuleSet.Count - 1)]);

                        break;

                    case 'N':

                        if (N_RuleSet.Count == 0)
                            newWordQue.Enqueue(currentWord[x].ToString());

                        else if (N_RuleSet.Count == 1)
                            newWordQue.Enqueue(N_RuleSet[0]);

                        else
                            newWordQue.Enqueue(N_RuleSet[Random.Range(0, N_RuleSet.Count - 1)]);

                        break;


                    default:
                        break;
                }
            }

            currentWord = string.Empty;
            while (newWordQue.Count > 0)
            {
                currentWord = currentWord +  newWordQue.Dequeue();
            }

        }

        Debug.Log($"{currentWord}");
        return currentWord.ToString();

    }




    public void RunIteration() 
    {
        head.transform.position = Vector3.zero;

        points.Clear();
        currDirection = 0;
        endingWord = RunLSystem(axium);
        ProcessSentence();
    }


    private void ProcessSentence() 
    {
        Stack<Vector3> lastPositions = new Stack<Vector3>();

        for (int i = 0; i < endingWord.Length; i++)
        {

            switch (endingWord[i])
            {
                case 'A':
                    MoveHead(10);
                    if (!points.Contains(head.transform.position))
                        points.Add(head.transform.position);
                    break;

                case 'B':
                    MoveHead(15);
                    if (!points.Contains(head.transform.position))
                        points.Add(head.transform.position);
                    break;

                case 'C':
                    MoveHead(20);
                    if (!points.Contains(head.transform.position))
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
