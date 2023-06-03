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
        TurnManager.OnPlayerTurnChanged += OnPlayerTurnChanged;        
    }

    private void OnDisable()
    {
        TurnManager.OnPlayerTurnChanged -= OnPlayerTurnChanged;
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
        Debug.Log("Took damage: " + damage);
        Debug.Log("Remaining Life: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Insert dead logic;
            activeTile.isOccupied = false;
            Destroy(gameObject);
        }
    }

    public void OnPlayerTurnChanged(int currentTurn)
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        movedThisTurn = false;
        attackedThisTurn = false;
    }
}
