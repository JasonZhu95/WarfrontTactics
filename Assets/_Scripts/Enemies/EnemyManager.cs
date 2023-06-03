using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform enemyHolder;
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

            enemies.Add(childObject.GetComponent<EnemyData>());

            GetAllChildObjects(child);
        }
    }

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

    public void IncrementEnemyIndex()
    {
        currentIndex++;
    }

    private void RemoveEnemyFromList(EnemyData enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count == 0)
        {
            //TODO: ADD GAME WIN LOGIC
            Debug.Log("All Enemies have been destroyed");
        }
    }
}
