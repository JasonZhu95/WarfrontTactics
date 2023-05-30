using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ----------------------------------------------------------------------------
 * Class: CharacterSpawnManager
 * Description: Manages the spawning of characters on Scene Creation.
 * ---------------------------------------------------------------------------- */
public class CharacterSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;

    private void Start()
    {
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            GameObject character = Instantiate(characterPrefabs[i]);
            Vector2Int locationToSpawn = character.GetComponent<CharacterData>().spawningTileLocation;
            if (MapManager.Instance.map.ContainsKey(locationToSpawn))
            {
                PositionCharacterOnTile(MapManager.Instance.map[locationToSpawn], character);
            }
        }
    }

    /* ------------------------------------------------------------------------
    * Function: PositionCharacterOnTile
    * Description: Move the character object to the tile position that is fed
    * as a parameter. 0.0001f is used to differentiate the sorting layer
    * between the character and the tiles that are in front.
    * ---------------------------------------------------------------------- */
    private void PositionCharacterOnTile(OverlayTile tile, GameObject characterToMove)
    {
        characterToMove.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        characterToMove.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        characterToMove.GetComponent<CharacterData>().activeTile = tile;
        tile.characterOnTile = characterToMove.GetComponent<CharacterData>();
        tile.isOccupied = true;
    }
}
