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
    public static event TurnChangedHandler OnTurnChanged;

    [SerializeField] private MouseController mouseController;
    private int currentTurn = 1;

    private void Start()
    {
        StartTurn(currentTurn);
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
        OnTurnChanged?.Invoke(turn);
    }

    private void EndTurn()
    {
        currentTurn++;
        StartTurn(currentTurn);
    }
}
