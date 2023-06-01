using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ----------------------------------------------------------------------------
 * Class: EnemyData
 * Description: Handles enemy specific logic and inherits from the CharacterData
 * script.
 * ---------------------------------------------------------------------------- */
public class EnemyData : CharacterData
{
    private GameObject target;
    private List<CharacterData> characterList;
    [SerializeField] private MouseController mouseController;

    private void Start()
    {
        characterList = new List<CharacterData>();
        mouseController = GameObject.Find("MouseController").GetComponent<MouseController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            FindCharacterTarget();
            Debug.Log(target);
        }
    }

    /* ------------------------------------------------------------------------
    * Function: FindCharacterTarget
    * Description: Loops through all the tiles on the map and looks for
    * characters.  The closest character is then set as the target.
    * ---------------------------------------------------------------------- */
    private void FindCharacterTarget()
    {
        foreach (OverlayTile tile in MapManager.Instance.map.Values)
        {
            if (tile.characterOnTile != null && !tile.characterOnTile.isEnemy)
            {
                characterList.Add(tile.characterOnTile);
            }
        }
        target = ReturnClosestCharacter(characterList).gameObject;

    }

    /* ------------------------------------------------------------------------
    * Function: ReturnClosestCharacter
    * Description: Returns the closest charaacter on the grid relative to the
    * enemy's starting position.
    * ---------------------------------------------------------------------- */
    private CharacterData ReturnClosestCharacter(List<CharacterData> listOfCharacters)
    {
        float minDistance = float.MaxValue;
        int minIndex = 0;
        for (int i = 0; i < listOfCharacters.Count; i++)
        {
            float distance = Vector2Int.Distance(activeTile.grid2DLocation, listOfCharacters[i].activeTile.grid2DLocation);
            if (distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }
        return listOfCharacters[minIndex];
    }
}
