using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static ArrowTranslator;

/* ----------------------------------------------------------------------------
 * Class: MapManager
 * Description: Script is responsible for managing the selected tile overlay.
 * Also responsible for moving characters along the path based on click.
 * ---------------------------------------------------------------------------- */
public class MouseController : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    [SerializeField] private float speed = 3;
    [SerializeField] private GameObject characterPrefab;
    private CharacterData character;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private ArrowTranslator arrowTranslator;
    private List<OverlayTile> path;
    private List<OverlayTile> rangeFinderTiles;
    private bool isMoving;
    private bool moveAlongPathFinished = false;
    private bool characterIsSelected = false;
    private RaycastHit2D? focusedTileHit;

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        arrowTranslator = new ArrowTranslator();

        path = new List<OverlayTile>();
        isMoving = false;
        rangeFinderTiles = new List<OverlayTile>();
    }

    private void LateUpdate()
    {
        focusedTileHit = GetFocusedOnTile();

        // Move the mouse cursor where the overlay tile is selected.
        // Show the overlay tile on mouse press
        if (focusedTileHit.HasValue)
        {
            OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            cursor.transform.position = overlayTile.transform.position;
            cursor.gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
            // Check if the selected tile is within range and if the player isn't currently moving
            if(rangeFinderTiles.Contains(overlayTile) && !isMoving && characterIsSelected)
            {
                path = pathFinder.FindPath(character.activeTile, overlayTile, rangeFinderTiles);

                // Hide all arrows when player clicks to start moving
                foreach(OverlayTile tile in rangeFinderTiles)
                {
                    MapManager.Instance.map[tile.grid2DLocation].SetArrowSprite(ArrowDirection.None);
                }

                // Calculate the direction of the arrow and set sprite to visible
                for (int i = 0; i < path.Count; i++)
                {
                    OverlayTile previousTile = i > 0 ? path[i - 1] : character.activeTile;
                    OverlayTile futureTile = i < path.Count - 1 ? path[i + 1] : null;

                    ArrowDirection arrowDir = arrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
                    path[i].SetArrowSprite(arrowDir);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (overlayTile.isOccupied && !overlayTile.characterOnTile.isEnemy && !overlayTile.characterOnTile.movedThisTurn)
                {
                    overlayTile.ShowTile();
                    character = overlayTile.characterOnTile;
                    characterIsSelected = true;
                    GetInRangeTiles();
                }
                else
                {
                    isMoving = true;
                }
            }
        }
        
        // Move the character
        if (path.Count > 0 && isMoving)
        {
            MoveAlongPath();
        }

        // Calculate the tiles in range when player isn't moving
        if (path.Count == 0 && character != null)
        {
            isMoving = false;
        }

        if (moveAlongPathFinished)
        {
            characterIsSelected = false;
            character = null;
            moveAlongPathFinished = false;
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

        foreach (OverlayTile tile in rangeFinderTiles)
        {
            MapManager.Instance.map[tile.grid2DLocation].SetArrowSprite(ArrowDirection.None);
        }

        if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }
        if (path.Count == 0)
        {
            moveAlongPathFinished = true;
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
    * Function: GetInRangeTiles
    * Description: Displays tiles that are only in range of the selected
    * character
    * ---------------------------------------------------------------------- */
    private void GetInRangeTiles()
    {
        rangeFinderTiles = rangeFinder.GetTilesInRange(new Vector2Int(character.activeTile.gridLocation.x, character.activeTile.gridLocation.y), character.range);
        foreach (var item in rangeFinderTiles)
        {
            item.ShowTile();
        }
    }

    /* ------------------------------------------------------------------------
    * Function: PositionCharacterOnTile
    * Description: Move the character object to the tile position that is fed
    * as a parameter. 0.0001f is used to differentiate the sorting layer
    * between the character and the tiles that are in front.
    * ---------------------------------------------------------------------- */
    private void PositionCharacterOnTile(OverlayTile tile)
    {
        character.activeTile.isOccupied = false;
        character.activeTile.characterOnTile = null;
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        character.GetComponent<CharacterData>().activeTile = tile;
        character.movedThisTurn = true;
        tile.characterOnTile = character;
        tile.isOccupied = true;
    }
}
