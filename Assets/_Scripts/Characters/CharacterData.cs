using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* ----------------------------------------------------------------------------
 * Class: CharacterData
 * Description: Responsible for handling character specific data
 * ---------------------------------------------------------------------------- */
public class CharacterData : MonoBehaviour
{
    public OverlayTile activeTile;
    public Sprite originalSprite;
    public Sprite selectedSprite;
    public bool isEnemy;
    public int attack;
    public int range;
    public int attackRange;

    public int currentHealth;
    public int maxHealth;
    public string characterName;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TextMeshProUGUI damageText;
    [HideInInspector] public Animator anim;

    public bool movedThisTurn;
    public bool attackedThisTurn;

    public static event Action<EnemyData> OnEnemyDeath;
    public static event Action OnCharacterDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        if (!isEnemy)
        {
            TurnManager.OnEnemyTurnChanged += OnPlayerTurnChanged;
        }
    }

    /* ------------------------------------------------------------------------
    * Function: Take Damage
    * Description: Takes a float damage amount and subtracts it from the
    * current health of the corresponding Character
    * ---------------------------------------------------------------------- */
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar(maxHealth, currentHealth);
        if (currentHealth > 0)
        {
            StartCoroutine(ShowDamageText(damage));
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Insert dead logic;
            activeTile.isOccupied = false;
            activeTile.isBlocked = false;
            if (isEnemy)
            {
                OnEnemyDeath?.Invoke((EnemyData)this);
                TurnManager.OnPlayerTurnChanged -= OnPlayerTurnChanged;
            }
            else
            {
                TurnManager.OnEnemyTurnChanged -= OnPlayerTurnChanged;
                OnCharacterDeath?.Invoke();
            }
            Destroy(gameObject);
        }
    }

    /* ------------------------------------------------------------------------
    * Function: OnPlayerTurnChanged
    * Description: Takes care of the logic when the player turn is beginning
    * ---------------------------------------------------------------------- */
    public void OnPlayerTurnChanged(int currentTurn)
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        movedThisTurn = false;
        attackedThisTurn = false;
    }

    /* ------------------------------------------------------------------------
    * Function: UpdateHealthBar
    * Description: Updates the fill amoun for the health bar image based
    * on remaining life total.
    * ---------------------------------------------------------------------- */
    private void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
        if (currentHealth == 0)
        {
            healthBarImage.fillAmount = 0;
        }
    }

    /* ------------------------------------------------------------------------
    * Coroutine: ShowDamageText
    * Description: Turns on the damage text and turns it off after half a
    * second
    * ---------------------------------------------------------------------- */
    private IEnumerator ShowDamageText(int damageAmount)
    {
        damageText.gameObject.SetActive(true);
        damageText.text = "-" + damageAmount.ToString();
        yield return new WaitForSeconds(.5f);

        damageText.gameObject.SetActive(false);
    }
}
