using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public OverlayTile activeTile;
    public bool isEnemy;
    public int attack;
    public int defense;
    public int range;

    public int currentHealth;
    public int maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

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
