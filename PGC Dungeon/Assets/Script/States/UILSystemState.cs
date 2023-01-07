using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UILSystemState : UiBaseState
{
    [Range(2, 10)]
    public int iterations = 2;
    public string axium = "";


    public List<Vector3Int> points = new List<Vector3Int>();
    public Vector3Int currHeadPos;

    //public Dictionary<char, string> rulesDict = new Dictionary<char, string>();

    [SerializeField]
    private List<string> ruleString = new List<string>() { "SA+A+CLBSB---AL", "SA--C+CLBSB--AL", "SC+A--CL" };

    public string endingWord;

    public bool run;

    private int currDirection = 0;

    bool modePath = false;

    private string fileName = "";


    private List<string> A_RuleSet = new List<string>();
    private List<string> B_RuleSet = new List<string>();
    private List<string> C_RuleSet = new List<string>();
    private List<string> S_RuleSet = new List<string>();
    private List<string> L_RuleSet = new List<string>();
    private List<string> P_RuleSet = new List<string>();
    private List<string> N_RuleSet = new List<string>();





    public override void onExit(StateUIManager currentMenu)
    {
    }

    public override void onGizmos(StateUIManager currentMenu)
    {
    }

    public override void onGUI(StateUIManager currentMenu)
    {

        // axium   input field 

        // add key    input field
        // add rule    input field

        if (currentMenu.working)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "LOADING");
        }
        else
        {

            GUI.Box(new Rect(5, 10, 230, 650), "");   //background 

            iterations = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), iterations,2, 8);

            axium = GUI.TextField(new Rect(10, 20, 200, 20), axium, 25);
            fileName = GUI.TextField(new Rect(10, 80, 200, 20), fileName, 25);

            if (GUI.Button(new Rect(10, 140, 120, 30), "Run L-system"))
            {
                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    RunLSystem(axium,currentMenu);
                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                    RunLSystem(axium,currentMenu);



                foreach (var point in points)
                {
                    Debug.Log(point);
                }

            }

            if (GUI.Button(new Rect(10, 200, 120, 30), "Load Ruleset"))
            {
                

            }


            modePath = GUI.Toggle(new Rect(10, 100, 100, 30), modePath, "toggle path way");

            if (GUI.Button(new Rect(10, 360, 100, 30), "Go back"))
                currentMenu.ChangeState(0);


        }

    }

    public override void onStart(StateUIManager currentMenu)
    {
        //ruleString.Clear();

        currHeadPos = Vector3Int.zero;
        //ruleString.Add("SA+A+CLBSB---AL");
        //ruleString.Add("SA--C+CLBSB--AL");
        //ruleString.Add("SC+A--CL");
    }

    public override void onUpdate(StateUIManager currentMenu)
    {


    }






    private void RunLSystem(string axium,StateUIManager currentMenu)
    {

        currHeadPos = new Vector3Int(currentMenu.width/2, 0 , currentMenu.height/2 );

        StringBuilder currentWord = new StringBuilder(axium);
        points.Clear();
        for (int i = 0; i < iterations; i++)
        {
            // currentWord.Replace("A", rulesDict['A']);
            currentWord.Replace("A", ruleString[Random.Range(0, ruleString.Count - 1)]);
        }


       endingWord = currentWord.ToString();

       ProcessSentence();

        SetUpCorridors(currentMenu);
    }


    private void SetUpCorridors(StateUIManager currentMenu) 
    {

        for (int i = 0; i < points.Count; i++)
        {
            if (i != points.Count - 1) 
            {

                var path = AlgosUtils.A_StarPathfinding2DNorm(currentMenu.gridArray2D, new Vector2Int(points[i].x, points[i].z), new Vector2Int(points[i + 1].x, points[i + 1].z), modePath);

                foreach (var tile in path.Item1)
                {
                    tile.tileWeight = 1;
                }
            }

        }

        currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D,true);


    }

    private void ProcessSentence()
    {
        Stack<Vector3Int> lastPositions = new Stack<Vector3Int>();

        for (int i = 0; i < endingWord.Length; i++)
        {
            //seems to have a 0 somewhere
            // Debug.Log(currDirection);

            switch (endingWord[i])
            {
                case 'A':
                    MoveHead(10);
                     if (!points.Contains(currHeadPos))
                        points.Add(currHeadPos);
                    break;

                case 'B':
                    MoveHead(15);
                    if (!points.Contains(currHeadPos))
                        points.Add(currHeadPos);
                    break;

                case 'C':
                    MoveHead(20);
                    if (!points.Contains(currHeadPos))
                        points.Add(currHeadPos);
                    break;


                case 'S':
                    lastPositions.Push(currHeadPos);
                    break;

                case 'L':
                    currHeadPos = lastPositions.Pop();
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
                currHeadPos += new Vector3Int(moveAmount, 0, 0);
                break;


            case 2:
                currHeadPos += new Vector3Int(0, 0, moveAmount);
                break;


            case 3:
                currHeadPos += new Vector3Int(-moveAmount, 0, 0);
                break;


            case 4:
                currHeadPos += new Vector3Int(0, 0, -moveAmount);
                break;


            default:
                break;
        }
    }

    private void LoadRuleSet() 
    {

        //var ruleSet = Resources.Load<LSystemRuleSet>(ruleDec.ruleSetFolderName);
    }


}
