using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ----------------------------------------------------------------------------
 * Class: TurnManager
 * Description: Manages the turn order of the game
 * ---------------------------------------------------------------------------- */
public class TurnManager : MonoBehaviour
{
    public delegate void TurnChangedHandler(int currentTurn);
    public static event TurnChangedHandler OnEnemyTurnChanged;
    public static event TurnChangedHandler OnPlayerTurnChanged;

    [SerializeField] private MouseController mouseController;
    public int currentTurn = 1;

    private void Start()
    {
        StartTurn(currentTurn);

        //ENEMY Turns are even
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !mouseController.isMoving)
        {
            EndTurn();
        }
    }

    private void StartTurn(int turn)
    {
        Debug.Log("turn number: " + turn);
        if (turn % 2 == 0)
        {
            OnEnemyTurnChanged?.Invoke(turn);
        }
        else
        {
            OnPlayerTurnChanged?.Invoke(turn);
        }
    }

    public void EndTurn()
    {
        currentTurn++;
        StartTurn(currentTurn);
    }

}
