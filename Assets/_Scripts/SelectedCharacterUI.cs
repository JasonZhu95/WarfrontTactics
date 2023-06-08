using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* ----------------------------------------------------------------------------
 * Class: SelectedCharacterUI
 * Description: Manages the Character Canvas on selection.
 * ---------------------------------------------------------------------------- */
public class SelectedCharacterUI : MonoBehaviour
{
    [SerializeField] private MouseController mouseController;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI nameText;

    private void Start()
    {
        mouseController.OnCharacterSelect += UpdateCanvas;
        gameObject.SetActive(false);
    }

    /* ------------------------------------------------------------------------
    * Function: UpdateCanvas
    * Description: When called updates the character canvas on selection.
    * ---------------------------------------------------------------------- */
    private void UpdateCanvas()
    {
        if (mouseController.SelectedCharacter == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            healthText.text = ":" + mouseController.SelectedCharacter.currentHealth.ToString() + "/" + mouseController.SelectedCharacter.maxHealth.ToString();
            attackText.text = mouseController.SelectedCharacter.attack.ToString();
            nameText.text = mouseController.SelectedCharacter.characterName;
            characterPortrait.sprite = mouseController.SelectedCharacter.GetComponent<SpriteRenderer>().sprite;
        }
    }

}
