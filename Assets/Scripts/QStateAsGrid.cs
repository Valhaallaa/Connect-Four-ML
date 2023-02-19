using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
interface IQStateInterface
{
    void initQ();
    float getQValueAtIdx(int r, int c);
    bool updateQ(int r, int c, float value);
    void loadQFromFile(string FileNum);
    void saveQToFile(string FileNum);
}
public class QStateAsGrid : MonoBehaviour, IQStateInterface
{
    public float[,] Q;
    [SerializeField]
    public int Columns, Rows;
    public void creategrid()
    {

        Q = new float[Columns, Rows];
        for (int columns = 0; columns < Columns; columns++)
        {
            for (int rows = 0; rows < Rows; rows++)
            {
                Q[columns, rows] = 0f;

            }
        }
    }


    public float getQValueAtIdx(int c, int r)
    {
        if (c < 0 || r < 0)
            Debug.Log("");
        return Q[c, r];
    }

    public void initQ()
    {
        //throw new System.NotImplementedException();
    }

    public void loadQFromFile(string FileNum)
    {
         // once trained Q file to be loaded for use
                  // throw new System.NotImplementedException();

        if (File.Exists(Application.dataPath + "/QArray" + FileNum + ".txt"))
        {
            string ss = File.ReadAllText(Application.dataPath + "/QArray" + FileNum + ".txt");
            string[] sa = ss.Split(new[] { "#" }, System.StringSplitOptions.None);
            int ArrayPos = 0;
            for (int C = 0; C < Columns; C++)
            {
                for (int R = 0; R < Rows; R++)
                {
                    Q[C,R] = float.Parse(sa[ArrayPos]);
                    ArrayPos++;
                }
            }
        }
    }

    public void saveQToFile(string FileNum) // save Q Grid to save training
    {
        //  throw new System.NotImplementedException();
        string[] sa = new string[(Columns * Rows)];
        int ArrayPos = 0;
        for (int C = 0; C < Columns; C++)
        {
            for (int R = 0; R < Rows; R++)
            {
                sa[ArrayPos] = Q[C,R].ToString();
                ArrayPos++;

            }
        }
        string ss = string.Join("#", sa);
        File.WriteAllText(Application.dataPath + "/QArray" + FileNum + ".txt", ss);
    }


    public bool updateQ(int r, int c, float value)
    {
        if (r >= 0 && r < Columns && c >= 0 && c < Rows)
            Q[r, c] = value;
        return false;
    }
}

