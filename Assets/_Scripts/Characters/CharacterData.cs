using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ----------------------------------------------------------------------------
 * Class: CharacterData
 * Description: Responsible for handling character specific data
 * ---------------------------------------------------------------------------- */
public class CharacterData : MonoBehaviour
{
    public OverlayTile activeTile;
    public Vector2Int spawningTileLocation;
    public Sprite originalSprite;
    public Sprite selectedSprite;
    public bool isEnemy;
    public int attack;
    public int defense;
    public int range;
    public int attackRange;

    public int currentHealth;
    public int maxHealth;

    public bool movedThisTurn;
    public bool attackedThisTurn;

    private void OnEnable()
    {
        TurnManager.OnTurnChanged += OnTurnChanged;        
    }

    private void OnDisable()
    {
        TurnManager.OnTurnChanged -= OnTurnChanged;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /* ------------------------------------------------------------------------
    * Function: Take Damage
    * Description: Takes a float damage amount and subtracts it from the
    * current health of the corresponding Character
    * ---------------------------------------------------------------------- */
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Insert dead logic;
            Debug.Log("dead");
        }
    }

    private void OnTurnChanged(int currentTurn)
    {
        movedThisTurn = false;
        attackedThisTurn = false;
    }
}
