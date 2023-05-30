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
    public bool isEnemy;
    public int attack;
    public int defense;
    public int range;

    public int currentHealth;
    public int maxHealth;

    public bool movedThisTurn;

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
}
