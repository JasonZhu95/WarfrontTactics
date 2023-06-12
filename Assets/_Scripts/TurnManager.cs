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

    [SerializeField] private Animator turnCanvasAnim;

    private void Start()
    {
        StartTurn(currentTurn);

        SceneLoader.OnChangeScene += UnsubscribeMethods;
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

    private void UnsubscribeMethods()
    {
        OnEnemyTurnChanged = null;
        OnPlayerTurnChanged = null;
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
            StartCoroutine(InvokeOnEnemyTurnChanged(turn));
        }
        else
        {
            StartCoroutine(InvokeOnPlayerTurnChanged(turn));
        }
    }

    /* ------------------------------------------------------------------------
    * Coroutine: InvokeOnEnemyTurnChanged
    * Description: When its the enemies turn, bring up the turn canvas and set
    * the anim.  The 2 second wait is a workaround to an Animation Event
    * ---------------------------------------------------------------------- */
    private IEnumerator InvokeOnEnemyTurnChanged(int turn)
    {
        turnCanvasAnim.SetBool("enemy", true);
        yield return new WaitForSeconds(2f);
        turnCanvasAnim.SetBool("enemy", false);
        OnEnemyTurnChanged?.Invoke(turn);
    }

    /* ------------------------------------------------------------------------
    * Coroutine: InvokeOnPlayerTurnChanged
    * Description: When its the player turn, bring up the turn canvas and set
    * the anim.  The 2 second wait is a workaround to an Animation Event
    * ---------------------------------------------------------------------- */

    private IEnumerator InvokeOnPlayerTurnChanged(int turn)
    {
        turnCanvasAnim.SetBool("player", true);
        yield return new WaitForSeconds(2f);
        turnCanvasAnim.SetBool("player", false);
        OnPlayerTurnChanged?.Invoke(turn);
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
