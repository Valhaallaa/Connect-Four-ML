using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayer : MonoBehaviour
{
    [SerializeField]   
    private Grid Grid;
    [SerializeField]
    private float Delay = 0.5f;
    public bool PlayerTurn;

    private IEnumerator HaveTurn()
    {
        int a;
        yield return new WaitForSeconds(Delay);
        a = Random.Range(0, Grid.Columns);
        
        if (PlayerTurn) { 
            Grid.Insert(a,false);
           // Debug.Log("Insert at " + a);
            PlayerTurn = false;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Grid = GameObject.Find("GridScript").GetComponent<Grid>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Grid.PlayerWon)
        {
            if (Grid)
                PlayerTurn = Grid.PlayerTurn;

            if (PlayerTurn)
            {
                StartCoroutine(HaveTurn());
                
            }

        }
    }
}
