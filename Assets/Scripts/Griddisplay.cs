using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Griddisplay : MonoBehaviour
{
    public int RowPos,ColumnPos,gridcontains;
    private GameObject grid;
    [SerializeField]
    private Material Player1Mat, Player2Mat,EmptyMat;

  
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("GridScript");
    }

    // Update is called once per frame
    void Update()
    {
        gridcontains = grid.GetComponent<Grid>().grid[ColumnPos, RowPos];
        if (gridcontains == 1)
            GetComponent<Renderer>().material = Player1Mat;
        else if(gridcontains == 2){
            GetComponent<Renderer>().material = Player2Mat;
        }
        else
            GetComponent<Renderer>().material = EmptyMat;
    }
}
