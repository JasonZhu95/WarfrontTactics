using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/* ----------------------------------------------------------------------------
 * Class: MapManager
 * Description: Script is responsible for managing the selected tile overlay.
 * Also responsible for moving characters along the path based on click.
 * ---------------------------------------------------------------------------- */
public class MouseController : MonoBehaviour
{
    public float speed = 3;

    [SerializeField] private GameObject characterPrefab;
    private CharacterData character;
    private PathFinder pathFinder;
    private List<OverlayTile> path = new List<OverlayTile>();

    private void Start()
    {
        pathFinder = new PathFinder();
    }

    private void LateUpdate()
    {
        var focusedTileHit = GetFocusedOnTile();

        // Move the mouse cursor where the overlay tile is selected.
        // Show the overlay tile on mouse press
        if (focusedTileHit.HasValue)
        {
            OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

            if (Input.GetMouseButtonDown(0))
            {
                overlayTile.ShowTile();

                if (character == null)
                {
                    character = Instantiate(characterPrefab).GetComponent<CharacterData>();
                    PositionCharacterOnTile(overlayTile);
                }
                else
                {
                    path = pathFinder.FindPath(character.activeTile, overlayTile);
                }
            }
        }

        if (path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    /* ------------------------------------------------------------------------
    * Function: MoveAlongPath
    * Description: Moves the character's position through the path using
    * Unity's built in MoveTowards.
    * ---------------------------------------------------------------------- */
    private void MoveAlongPath()
    {
        float step = speed * Time.deltaTime;

        float zIndex = path[0].transform.position.z;
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

    }

    /* ------------------------------------------------------------------------
    * Function: GetFocusedOnTile
    * Description: Return the overlay tile hit by raycast.
    * ---------------------------------------------------------------------- */
    public RaycastHit2D? GetFocusedOnTile()
    {
        // Use a raycast to determine which overlaytile is hit
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    /* ------------------------------------------------------------------------
    * Function: PositionCharacterOnTile
    * Description: Move the character object to the tile position that is fed
    * as a parameter. 0.0001f is used to differentiate the sorting layer
    * between the character and the tiles that are in front.
    * ---------------------------------------------------------------------- */
    private void PositionCharacterOnTile(OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        character.activeTile = tile;
    }
}
