using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class Grid : MonoBehaviour
{
    [SerializeField]
    public int Columns, Rows;
    public int[,] grid;
    [SerializeField]
    private GameObject cube;
    private GameObject turnimage,RedText,YellowText,NobodyText,WinText,RedWinCount,YellowWinCount;
    public bool PlayerTurn, PlayerWon, RoundOver;
    [SerializeField]
    private bool AutoRestart = false;
    public int RoundCount;

    private GameObject InputField;
    private ScriptedPlayer SP;
    QStateAsGrid QGrid;
    public int MoveCount, MaxMoves,RedWins,YellowWins;
    public int BestSeqP1,BestSeqP2;
    //public int[] BestSeqP1Pos, BestSeqP2Pos;
    public List<Vector2> BestSeqP1Pos = new List<Vector2>();
    public List<Vector2> BestSeqP2Pos = new List<Vector2>();
    public int WinningPlayer;

    // Start is called before the first frame update
    void Start()
    {
        InputField = GameObject.Find("InputField");
        turnimage = GameObject.Find("TurnImage");
        RedText = GameObject.Find("RedText");
        YellowText = GameObject.Find("YellowText");
        WinText = GameObject.Find("WinText");
        NobodyText = GameObject.Find("NobodyText");
        RedWinCount = GameObject.Find("RedWinCount");
        YellowWinCount = GameObject.Find("YellowWinCount");
        //SP = GameObject.Find("ScriptedPlayer").GetComponent<ScriptedPlayer>();
        //QGrid = GameObject.Find("ScriptedPlayer").GetComponent<QStateAsGrid>();
        WinText.SetActive(false);
        RedText.SetActive(false);
        YellowText.SetActive(false);
        NobodyText.SetActive(false);
        MaxMoves = Columns * Rows;
        creategrid();
        RoundCount = 0;
        if(File.Exists(Application.dataPath + "/Stats.txt"))
            File.AppendAllText(Application.dataPath + "/Stats.txt", "-------------------------------------------------------" + "\n");

    }
    // creates the grid used on start
    private void creategrid()
    {
        grid = new int[Columns,Rows];
        for (int columns = 0; columns < Columns; columns++)
            {
                for (int rows = 0; rows < Rows; rows++)
                {
                    grid[columns,rows] = 0;
                    var gridpos = Instantiate(cube, new Vector3(columns,rows), transform.rotation, null);
                    gridpos.GetComponent<Griddisplay>().RowPos = rows;
                    gridpos.GetComponent<Griddisplay>().ColumnPos = columns;
                }
        }
    }

    public void ResetGrid()
    {
        PlayerWon = false;
        PlayerTurn = false;
        RoundOver = false;
        WinText.SetActive(false);
        RedText.SetActive(false);
        YellowText.SetActive(false);
        NobodyText.SetActive(false);
        MoveCount = 0;
        BestSeqP1Pos.Clear(); 
        BestSeqP2Pos.Clear();
        BestSeqP1 = 0;
        BestSeqP2 = 0;
        RoundCount++;
        /*
        if (SP)
        
            SP.MoveCount = 0;
        if (QGrid)
            QGrid.saveQToFile();
        */
        for (int columns = 0; columns < Columns; columns++)
        {
            for (int rows = 0; rows < Rows; rows++)
            {
                grid[columns, rows] = 0;
            }
        }
        SaveStats();
    }

    private void SaveStats()
    {
       File.AppendAllText(Application.dataPath + "/Stats.txt", "Red Player wins: " + RedWins + " | Yellow Player Wins: " + YellowWins + " | Round Count: " + RoundCount + "\n");
    }
    // checks if the players in that grid pos has won or not
    private void CheckForFour(int a,int b)
    {
        int checkingfor = grid[a, b];
        int SeqLength = 1;
        List<Vector2> BestSeqPos = new List<Vector2>();
        BestSeqPos.Add(new Vector2(a, b));
        //straight line checks
        //down
        if (b >= 3) {
            for (int c = 1; c < 4; c++)
            {
                if (grid[a, b - c] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a, b - c));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
                else {
                    SeqLength = c;
                    break; }
            }
        }
        if (checkingfor == 1) {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            } 
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));

        //left
        if (a >= 3)
        {
            for (int c = 1; c < 4; c++)
            {

                if (grid[a - c, b] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a- c, b));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
                else { break; }

            } 
        }
        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));
        // left +1 right
        if (a >= 2 && a<Columns-1) { 
            for (int c = 1; c < 4; c++)
            {
                if(c == 1)
                {
                    if(grid[a +c,b] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a+c, b));
                    }
                    else
                    {
                        break;
                    }
                    if (grid[a - c, b] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a - c, b));
                        c++;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (grid[a - (c-1), b] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a - (c - 1), b));
                    if (c == 3)
                    {
                        PlayerWon = true;
                       
                    }
                }
                else { break; }

            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));

        //right
        if (a < Columns - 3)
        {
            for (int c = 1; c < 4; c++)
            {
                if (grid[a + c, b] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a +c, b));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
                else { break; }

            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));
        //right +1 left
        if (a < Columns - 2 && a >= 1)
        {
            for (int c = 1; c < 4; c++)
            {
                if (c == 1)
                {
                    if (grid[a + c, b] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a + c, b));
                    }
                    else
                    {
                        break;
                    }
                    if (grid[a - c, b] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a - c, b));
                        c++;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (grid[a + (c - 1), b] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a - (c-1), b));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
                else { break; }

            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
       BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));
        //diagonal line  checks
        //upright
        if (a < Columns - 3 && b < Rows - 3)
        {
            for (int c = 1; c < 4; c++)
            {
                if (grid[a + c, b + c] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a +c, b+c));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
                else { break; }

            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));
        //upright +1downleft
        if (a < Columns - 2 && b < Rows - 2 && a>=1 && b>=1)
        {
            for (int c = 1; c < 4; c++)
            {
                if(c==1){ 
                    if (grid[a + c, b + c] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a + c, b + c));
                    }
                    else { break; }
                    if (grid[a - c, b - c] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a - c, b - c));
                        c++;
                    }
                    else { break; }
                }
                else if(grid[a + (c-1), b + (c-1)] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a + (c - 1), b + (c - 1)));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));

        //downright
        if (a < Columns - 3 && b >= 3)
        {
            for (int c = 1; c < 4; c++)
            {
                if (grid[a + c, b - c] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a + c, b - c));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
                else { break; }

            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
       BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));
        //downright +upleft
        if (a < Columns - 2 && b >1 && a >= 1 && b < Rows-1)
        {
            for (int c = 1; c < 4; c++)
            {
                if (c == 1)
                {
                    if (grid[a + c, b - c] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a + c, b - c));
                    }
                    else { break; }
                    if (grid[a - c, b + c] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a - c, b + c));
                        c++;
                    }
                    else { break; }
                }
                else if (grid[a + (c - 1), b - (c - 1)] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a + (c - 1), b - (c - 1)));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));
        //upleft
        if (a >= 3 && b < Rows - 3)
        {
            for (int c = 1; c < 4; c++)
            {
                if (grid[a - c, b + c] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a - c, b + c));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
                else { break; }

            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));

        //upleft +1downright
        if (a >=2 && b < Rows - 2 && a < Columns -1 && b >=1)
        {
            for (int c = 1; c < 4; c++)
            {
                if (c == 1)
                {
                    if (grid[a - c, b + c] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a - c, b + c));
                    }
                    else { break; }
                    if (grid[a + c, b - c] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a + c, b - c));
                        c++;
                    }
                    else { break; }
                }
                else if (grid[a - (c - 1), b + (c - 1)] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a - (c - 1), b + (c - 1)));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));

        //downleft
        if (a >= 3 && b >= 3)
        {
            for (int c = 1; c < 4; c++)
            {
                if (grid[a - c, b - c] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a - c, b - c));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
                else { break; }

            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));

        //downleft +1upright
        if (a >= 2 && b >= 2 && a < Columns -1 && b < Rows - 1)
        {
            for (int c = 1; c < 4; c++)
            {
                if (c == 1)
                {
                    if (grid[a + c, b + c] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a + c, b + c));
                    }
                    else { break; }
                    if (grid[a - c, b - c] == checkingfor)
                    {
                        BestSeqPos.Add(new Vector2(a - c, b - c));
                        c++;
                    }
                    else { break; }
                }
                else if (grid[a - (c - 1), b - (c - 1)] == checkingfor)
                {
                    BestSeqPos.Add(new Vector2(a - (c - 1), b - (c - 1)));
                    if (c == 3)
                    {
                        PlayerWon = true;
                        
                    }
                }
            }
        }

        if (checkingfor == 1)
        {
            if (BestSeqPos.Count >= BestSeqP1)
            {
                BestSeqP1Pos = BestSeqPos;
                BestSeqP1 = BestSeqPos.Count;
            }
        }
        else if (checkingfor == 2)
            if (BestSeqPos.Count >= BestSeqP2)
            {
                BestSeqP2Pos = BestSeqPos;
                BestSeqP2 = BestSeqPos.Count;
            }
        BestSeqPos.Clear();
        BestSeqPos.Add(new Vector2(a, b));

    }

    private void CallChecks(int a,int b, bool Win)
    {
        FourCheckerHorizontal(a, b, Win);
        FourCheckDown(a, b,Win);
        FourCheckDiagonal(a, b,Win);
    }

    private void FourCheckerHorizontal(int a,int b,bool Win)
    {
        //Horizontal
        int LookFor = grid[a, b];
        List<Vector2> BestSeqPos = new List<Vector2>();
        int c4 = 0;
        //right
        for (int r = 0; r < 4; r++)
        {
            if ((a + r) <= (Columns - 1))
            {
                if (grid[a + r, b] == LookFor)
                {
                    BestSeqPos.Add(new Vector2(a + r, b));
                    c4++;
                    if (c4 == 4 && Win == true)
                    {
                        PlayerWon = true;
                        WinningPlayer = LookFor;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        //left
        for (int l = 1; l < 4; l++)
        {
            if((a - l) >= 0)
            {
                if (grid[a - l, b] == LookFor)
                {
                    BestSeqPos.Add(new Vector2(a - l, b));
                    c4++;
                    if (c4 == 4 && Win == true)
                    {
                        PlayerWon = true;
                        WinningPlayer = LookFor;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        BestSequence(BestSeqPos, LookFor);
    }

    private void FourCheckDown(int a,int b,bool Win)
    {
        int LookFor = grid[a, b];
        List<Vector2> BestSeqPos = new List<Vector2>();
        int c4 = 0;
        //down
        for (int d = 0; d < 4; d++)
        {
            if ((b - d) >= 0) { 
                if (grid[a, b - d] == LookFor)
                {
                    BestSeqPos.Add(new Vector2(a, b - d));
                    c4++;
                    if (c4 == 4 && Win == true)
                    {
                        PlayerWon = true;
                        WinningPlayer = LookFor;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        BestSequence(BestSeqPos, LookFor);
    }

    private void FourCheckDiagonal(int a,int b,bool Win)
    {
        int LookFor = grid[a, b];
        List<Vector2> BestSeqPos = new List<Vector2>();
        int c4 = 0;
        //upright + down left
        //right
        for (int r = 0; r < 4; r++)
        {
            if ((a + r) <= (Columns -1) && (b + r) < (Rows - 1))
            {
                if (grid[a + r, b + r] == LookFor)
                {
                    BestSeqPos.Add(new Vector2(a + r, b + r));
                    c4++;
                    if (c4 == 4 && Win == true)
                    {
                        PlayerWon = true;
                        WinningPlayer = LookFor;
                    }
                }
                else
                {
                    break;
                } 
            }
        }
        //left
        for (int l = 1; l < 4; l++)
        {
            if ((a - l) >= 0 && (b - l) >= 0)
            {
                if (grid[a - l, b - l] == LookFor)
                {
                    BestSeqPos.Add(new Vector2(a - l, b - l));
                    c4++;
                    if (c4 == 4 && Win == true)
                    {
                        PlayerWon = true;
                        WinningPlayer = LookFor;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        BestSequence(BestSeqPos, LookFor);
        BestSeqPos.Clear();
        c4 = 0;
        //downright + upleft
        //right
        for (int r = 0; r < 4; r++)
        {
            if ((b - r) >= 0 && (a + r) <= (Columns - 1))
            {
                if (grid[a + r, b - r] == LookFor)
                {
                    BestSeqPos.Add(new Vector2(a + r, b - r));
                    c4++;
                    if (c4 == 4 && Win == true)
                    {
                        PlayerWon = true;
                        WinningPlayer = LookFor;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        //left
        for (int l = 1; l < 4; l++)
        {
            if ((a - l) >= 0 && (b + l) <= (Rows - 1))
            {
                if (grid[a - l, b + l] == LookFor)
                {
                    BestSeqPos.Add(new Vector2(a - l, b + l));
                    c4++;
                    if (c4 == 4 && Win == true)
                    {
                        PlayerWon = true;
                        WinningPlayer = LookFor;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        BestSequence(BestSeqPos, LookFor);
    }

    private void BestSequence(List<Vector2> a,int b) // 
    {
        if (b == 1)
        {
            if (a.Count >= BestSeqP1)
            {
                BestSeqP1Pos = a;
                BestSeqP1 = a.Count;
            }
        }
        else if (b == 2)
            if (a.Count >= BestSeqP2)
            {
                BestSeqP2Pos = a;
                BestSeqP2 = a.Count;
            }
        
    }

    // inserting a players counter into the grid
    public void Insert(int a,bool isFake)
    {
        
        
        for(int j = 0;j < Rows; j++)
        {   if (a < 0 || a >= Columns)
            {
                break;
            }
            else if(grid[a,j] == 0)
            {
                if (j >= 4)
                {
                    //Debug.Log("");
                }
                if (PlayerTurn)
                {
                    grid[a, j] = 1;
                                       
                    if (!isFake)
                    {
                        MoveCount++;
                        CallChecks(a, j, true);
                        PlayerTurn = !PlayerTurn;
                    }
                    else
                        CallChecks(a, j, false);
                }
                else if (!PlayerTurn)
                {
                    grid[a, j] = 2;
                    if (!isFake)
                    {
                        MoveCount++;
                        CallChecks(a, j, true);
                        PlayerTurn = !PlayerTurn;
                    }
                    else
                        CallChecks(a, j, false);
                }
                break;
            }

        }
        
    }
    
    public void FakeInsert(int a)
    {
        int c = 0;
        bool placed = false;
        for (int j = 0; j < Rows; j++)
        {
            if (a < 0 || a >= Columns)
            {
                break;
            }
            else if (grid[a, j] == 0)
            {
                if (j >= 4)
                {

                }
                if (PlayerTurn)
                {
                    grid[a, j] = 1;
                    c = j;
                    CallChecks(a, j,false);
                    placed = true;
                }
                else if (!PlayerTurn)
                {
                    grid[a, j] = 2;
                    c = j;
                    CallChecks(a, j,false);
                    placed = true;
                }
                break;
            }

        }
        if (placed)
        {
            grid[a, c] = 0;
        }
    }

    public void Remove(int a) {

        for (int j = 0; j < Rows; j++)
        {
            if (a < 0 || a >= Columns)
            {

                break;
            }
            
            else if (grid[a, j] == 0)
            {
                grid[a, (j-1)] = 0;
            }
            //playerturn = !playerturn;
            //playerwon = false;
            if(j == (Rows - 1))
            {
                grid[a, (Rows - 1)] = 0;
            }
        }
    }
  

    public void AutoTestLine()
    {
        Insert(0,false);
        Insert(0, false);
        Insert(1, false);
        Insert(1, false);
        Insert(2, false);
        Insert(2, false);
        Insert(3, false);
    }
    public void AutoTestBackLine()
    {
        Insert(0, false);
        Insert(0, false);
        Insert(1, false);
        Insert(1, false);
        Insert(3, false);
        Insert(3, false);
        Insert(2, false);
        
    }

    // gets the player textfield input
    public void getint()
    {
        if (!PlayerWon)
        {
            int b;
            b = int.Parse(InputField.GetComponent<InputField>().text);
            Insert(b,false);
        }
    }
    private void PlayerHasWon()
    {
        RoundOver = true;
        if (!PlayerTurn)
        {
            RedText.SetActive(true);
            RedWins++;
        }

        else
        {
            YellowText.SetActive(true);
            YellowWins++;
        }
        WinText.SetActive(true);
    }

    private void MaxMovesReached()
    {
        NobodyText.SetActive(true);
        WinText.SetActive(true);
        PlayerWon = true;
    }
    // Update is called once per frame
    void Update()
    {
       
        if(!RoundOver && PlayerWon)
            PlayerHasWon();
        if(AutoRestart && PlayerWon)
            ResetGrid();

        

        if (!PlayerWon)
        {
            YellowWinCount.GetComponent<Text>().text = YellowWins.ToString();
            RedWinCount.GetComponent<Text>().text = RedWins.ToString();
            if (PlayerTurn)
                turnimage.GetComponent<Image>().color = Color.red;
            else
                turnimage.GetComponent<Image>().color = Color.yellow;


            if (MoveCount == MaxMoves)
                MaxMovesReached();

        }
           
    }
}
