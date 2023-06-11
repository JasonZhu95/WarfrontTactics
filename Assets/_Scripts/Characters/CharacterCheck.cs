using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCheck : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;

    private void OnEnable()
    {
        CharacterData.OnCharacterDeath += CheckCount;
    }

    private void CheckCount()
    {
        if (gameObject.transform.childCount == 1)
        {
            StartCoroutine(StartGameOverCanvas());
        }
    }

    private IEnumerator StartGameOverCanvas()
    {
        yield return new WaitForSeconds(.5f);
        gameOverCanvas.SetActive(true);
    }
}
