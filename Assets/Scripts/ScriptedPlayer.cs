using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class ScriptedPlayer : MonoBehaviour
{
    [SerializeField]
    private string ID;

    private int[] MoveList;
    private List<int> AvailableActionList = new List<int>();
    Grid Grid;
    QStateAsGrid QGrid;
    IQStateInterface qsInterface;
    public bool PlayerTurn;
    private int RoundCount, RoundCountNew;
    public int MoveCount;

    [SerializeField]
    private int MoveListLength = 42;
    [SerializeField]
    private float Delay = 0.5f;

    [SerializeField]
    private float Epsilon = 0.8f;
    [SerializeField]
    private float Gamma = 0.8f;
    [SerializeField]
    private float Alpha = 0.5f;
    [SerializeField]
    private bool HaveGoOn;
    

    private void FillMoveList()
    {
        string[] sa = new string[MoveListLength];
        Random.Range(0, Grid.Columns).ToString();
        for (int i = 0; i < MoveListLength; i++)
        {
            sa[i] = Random.Range(0, Grid.Columns).ToString();

            //PlayerPrefs.SetInt(s, Random.Range(0,Grid.Columns));
        }
        string ss = string.Join("#", sa);
        File.WriteAllText(Application.dataPath + "/AIMoves.txt",ss);
    }
    private void GetMoveList()
    {
        if (File.Exists(Application.dataPath + "/AIMoves.txt")) {
            string ss = File.ReadAllText(Application.dataPath + "/AIMoves.txt");
            string[] sa = ss.Split(new[] { "#" }, System.StringSplitOptions.None);
            
            for (int i = 0; i < MoveListLength; i++)
                {
                    MoveList[i] = int.Parse(sa[i]);
                } 
        }
    }

    private int AvailableActions()
    {
        List<int> Actions = new List<int>();
        int AvailableActionsCount = 0;
        for (int i = 0; i < (Grid.Columns); i++)
        {
            for (int j = 0; j < (Grid.Rows); j++)
            {
                if(Grid.grid[i,j] == 0)
                {
                    AvailableActionsCount++;
                    Actions.Add(i);
                    break;
                }
                else
                {

                }
            }
                
            
        }
        AvailableActionList = Actions;
        return AvailableActionsCount;
    }


    private IEnumerator HaveTurn()
    {
        int a;
        //int b;
        yield return new WaitForSeconds(Delay);
        a = Random.Range(0, Grid.Columns);

        if (!PlayerTurn)
        {
            AvailableActions();
            //b = CheckForWin(AvailableActionList);
           //if (b != -1) // check if there is no winning move
            //{
            //    Grid.Insert(b);
            //}
            //else { 
                Grid.Insert(MoveList[MoveCount],false);
            //}
            MoveCount++;
            //Debug.Log("Insert at " + a);
            PlayerTurn = true;
        }

    }
    private IEnumerator StartTurn()
    {
        int sh = -1;
        int r = -1;
        yield return new WaitForSeconds(Delay);
        HaveTurnRL(ref sh, ref r);
    }

    private void NewRoundChecker()
    {
        RoundCountNew = Grid.RoundCount;
        if (RoundCountNew != RoundCount)
        {
            QGrid.saveQToFile(ID);
            MoveCount = 0;
        }
        RoundCount = RoundCountNew;
    }

    private void HaveTurnRL(ref int sh, ref int r)
    {
        int s2 = 0;
        int r2 = 0;
        int c = 0;
        
        float mq = 0.0f, rwd = 0.0f;
        if (PlayerTurn == HaveGoOn)
        {
            PlayerTurn = !PlayerTurn;
            if (AvailableActions() == 0)
            {
                Debug.Log("No Actions left");
            }
            if (CheckForWin(AvailableActionList,ref c) != -1) // check if there is no winning move
            {
                sh = c;
                isColFree(sh, ref r);
            }
            else
            {
                if (Random.Range(0f, 1f) < Epsilon) // Exploitation
                {
                    mq = getMaxQForState(AvailableActionList, ref sh, ref r);
                }
                else // Exploration
                {
                    sh = AvailableActionList[Random.Range(0, AvailableActionList.Count)];
                    isColFree(sh, ref r);
                    if (sh < 0 || r < 0)
                        Debug.Log("");
                }
            }
            Grid.Insert(sh, false);
            mq = getMaxQForState(AvailableActionList, ref s2, ref r2);  //s2 & r2 NOT used!
            rwd = Grid.BestSeqP2Pos.Count >= 4 ? 5.0f : Grid.BestSeqP2Pos.Count - Grid.BestSeqP1Pos.Count;
            if (r == -1)
                Debug.Log("");
            float cq = qsInterface.getQValueAtIdx(sh, r);
            qsInterface.updateQ(sh, r, (1.0f - Alpha) * cq + Alpha * (rwd + Gamma * mq));  //Non-deterministic Bellman eqn

            NewRoundChecker();
        }
    }

    private int CheckForWin(List<int> Actions,ref int c)
    {
       int a;
        for (int i = 0; i < Actions.Count; i++)
        {
            Grid.FakeInsert(Actions[i]); // could alternativly check for if PlayerWon == true before removing it,
            
            if(Grid.BestSeqP2Pos.Count == 4)
            {
                a = Actions[i];
                c = a;
                return a;
            }
        }
        a = -1;
        return a;
    }

    private bool isColFree(int c,ref int r)
    {
        r = -1;
        for (int i = 0; i < (Grid.Rows); i++) // this might be the wrong way as it goes from up to down, yet it will stop at the top if it is free and wont return the lowest free point and instead returns the highest
        {
            if (Grid.grid[c, i] == 0)
            {
                r = i;
                return true;
            }
        }
        return false;
    }
    private float getMaxQForState(List<int> AllPosActions,ref int HighState,ref int r)
    { 
        float maxQ = -999999;
        //	int freeRow = -1, n1, n2, c, nf1, nf2;
        int freeRow = -1, c;
        int n = AllPosActions.Count;
        for (int i = 0; i < (AllPosActions.Count); i++)
        {
            c =AllPosActions[i];
            if (isColFree(c,ref freeRow)) // c and freerow are swapped at the qValue at index, maybe the wrong way round? ass it seems to only check each row in the 5th column
            {
                //			getStatsForState(c, n1, n2, nf1, nf2);
                Grid.Insert(AllPosActions[i],true); //play for s'
                if (qsInterface.getQValueAtIdx(c, freeRow) > maxQ)
                {
                    maxQ = qsInterface.getQValueAtIdx(c,freeRow);
                    HighState = c;
                    r = freeRow;
                }
                Grid.Remove(AllPosActions[i]);
            }
        }
        return maxQ == -999999 ? -1.0f : maxQ; //-999999 NO actions available! -1.0f is NOMINAL
    }
    

    // Start is called before the first frame update
    void Start()
    {
        MoveList = new int[MoveListLength];
        MoveCount = 0;
        Grid = GameObject.Find("GridScript").GetComponent<Grid>();
        if (!File.Exists(Application.dataPath + "/AIMoves.txt"))
            FillMoveList();
        
        GetMoveList();
        QGrid = GetComponent<QStateAsGrid>();
        QGrid.creategrid();
        QGrid.Columns = Grid.Columns;
        QGrid.Rows = Grid.Rows;
        QGrid.loadQFromFile(ID);
        qsInterface = QGrid;
    }

    // Update is called once per frame
    void Update()
    {

        if (!Grid.PlayerWon)
        {
            if (Grid)
                PlayerTurn = Grid.PlayerTurn;

            if (PlayerTurn == HaveGoOn)
            {

                StartCoroutine(StartTurn());
            }

        }

    }
}
