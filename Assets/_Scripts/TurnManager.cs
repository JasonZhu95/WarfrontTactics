using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Button endTurnButton;
    public int currentTurn = 1;

    private void Start()
    {
        StartTurn(currentTurn);

        //ENEMY Turns are even
    }

    private void Update()
    {
        if (currentTurn % 2 == 0 || mouseController.isMoving || mouseController.SelectedCharacter != null)
        {
            endTurnButton.interactable = false;
        }
        else
        {
            endTurnButton.interactable = true;
        }
    }

    /* ------------------------------------------------------------------------
    * Function: StartTurn
    * Description: Starts the next turn and checks if it is a player turn
    * or an enemy turn
    * ---------------------------------------------------------------------- */
    private void StartTurn(int turn)
    {
        if (turn % 2 == 0)
        {
            OnEnemyTurnChanged?.Invoke(turn);
        }
        else
        {
            OnPlayerTurnChanged?.Invoke(turn);
        }
    }

    /* ------------------------------------------------------------------------
    * Function: EndTurn
    * Description: Ends the turn and increases the turn index
    * ---------------------------------------------------------------------- */
    public void EndTurn()
    {
        currentTurn++;
        StartTurn(currentTurn);
    }

}
