using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform enemyHolder;
    [SerializeField] private GameObject LevelCompletedCanvas;
    private TurnManager turnManager;
    public List<EnemyData> enemies;
    public int currentIndex = 0;

    private void OnEnable()
    {
        CharacterData.OnEnemyDeath += RemoveEnemyFromList;
    }

    private void OnDisable()
    {
        CharacterData.OnEnemyDeath -= RemoveEnemyFromList;
    }

    private void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        GetAllChildObjects(enemyHolder);
        TurnManager.OnEnemyTurnChanged += StartEnemyTurnActions;
    }

    /* ------------------------------------------------------------------------
    * Function: GetAllChildObjects
    * Description: Gets all the child objects in a transform and places it
    * in the enemy list.
    * ---------------------------------------------------------------------- */
    private void GetAllChildObjects(Transform parent)
    {
        int childCount = parent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            GameObject childObject = child.gameObject;
            if (childObject.GetComponent<EnemyData>() != null)
            {
                enemies.Add(childObject.GetComponent<EnemyData>());
            }

            GetAllChildObjects(child);
        }
    }

    /* ------------------------------------------------------------------------
    * Function: StartEnemyTurnActions
    * Description: A filler function to help bypass the need for a turn 
    * parameter for the event action.
    * ---------------------------------------------------------------------- */
    private void StartEnemyTurnActions(int turn)
    {
        PerformNextAction();
    }

    /* ------------------------------------------------------------------------
    * Function: PerformNextAction
    * Description: Performs the next enemy Action.
    * ---------------------------------------------------------------------- */
    public void PerformNextAction()
    {
        if (currentIndex < enemies.Count)
        {
            EnemyData currentEnemy = enemies[currentIndex];
            currentEnemy.PerformAction();
        }
        else if (currentIndex == enemies.Count)
        {
            currentIndex = 0;
            turnManager.EndTurn();
        }
    }

    /* ------------------------------------------------------------------------
    * Function: IncrementEnemyIndex
    * Description: when called increments the enemy Index
    * ---------------------------------------------------------------------- */
    public void IncrementEnemyIndex()
    {
        currentIndex++;
    }

    /* ------------------------------------------------------------------------
    * Function: RemoveEnemyFromList
    * Description: When called deletes teh enemies from list of enemies.
    * Checks for win condition of all enemies gone.
    * ---------------------------------------------------------------------- */
    private void RemoveEnemyFromList(EnemyData enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count == 0)
        {
            StartCoroutine(StartLevelCompletedCanvas());
        }
    }

    private IEnumerator StartLevelCompletedCanvas()
    {
        yield return new WaitForSeconds(0.5f);
        LevelCompletedCanvas.SetActive(true);
    }
}
