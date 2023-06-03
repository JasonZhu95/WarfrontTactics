using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static ArrowTranslator;

/* ----------------------------------------------------------------------------
 * Class: MouseController
 * Description: Script is responsible for managing the selected tile overlay.
 * Responsible for most of the game logic involving player interactions such as
 * moving and attacking.
 * ---------------------------------------------------------------------------- */
public class MouseController : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    [SerializeField] private float speed = 3;
    [SerializeField] private GameObject characterPrefab;
    private CharacterData character;
    private TurnManager turnManager;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private ArrowTranslator arrowTranslator;
    private List<OverlayTile> path;
    private List<OverlayTile> rangeFinderTiles;
    public bool isMoving { get; set; }
    private bool moveAlongPathFinished = false;
    private bool characterIsSelected = false;
    private bool attackActivated = false;
    private RaycastHit2D? focusedTileHit;

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        arrowTranslator = new ArrowTranslator();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();

        path = new List<OverlayTile>();
        isMoving = false;
        rangeFinderTiles = new List<OverlayTile>();
    }

    private void LateUpdate()
    {
        if (turnManager.currentTurn % 2 == 1)
        {
            cursor.SetActive(true);
            focusedTileHit = GetFocusedOnTile();
            // Move the mouse cursor where the overlay tile is selected.
            // Show the overlay tile on mouse press
            if (focusedTileHit.HasValue && !isMoving)
            {
                OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
                cursor.transform.position = overlayTile.transform.position;
                cursor.gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

                PlaceArrowsOnPath(overlayTile);

                //Attack the enemy on click
                if (attackActivated && rangeFinderTiles.Contains(overlayTile) && characterIsSelected)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!rangeFinderTiles.Contains(overlayTile) && !isMoving)
                        {
                            DeselectUnit();
                        }
                        else
                        {
                            AttackEnemyOnClick(overlayTile);
                        }
                    }
                }

                // Get the selected tile on Mouse Button Down
                if (Input.GetMouseButtonDown(0))
                {
                    if (characterIsSelected && rangeFinderTiles.Contains(overlayTile) && !attackActivated)
                    {
                        isMoving = true;
                        Debug.Log("Moving started");
                    }

                    // If the current tile is a player owned character and selected
                    if (overlayTile.isOccupied && !overlayTile.characterOnTile.isEnemy && !overlayTile.characterOnTile.attackedThisTurn)
                    {
                        // Check to see if we need to reselect the character
                        if (character != null)
                        {
                            characterIsSelected = false;
                            character.GetComponent<SpriteRenderer>().sprite = character.originalSprite;
                        }

                        overlayTile.ShowTile();
                        character = overlayTile.characterOnTile;
                        characterIsSelected = true;
                        if (!overlayTile.characterOnTile.movedThisTurn)
                        {
                            GetInRangeTiles();
                        }
                        else if (overlayTile.characterOnTile.movedThisTurn)
                        {
                            attackActivated = true;
                            GetInAttackRangeTiles();
                        }
                    }
                    else if (!rangeFinderTiles.Contains(overlayTile) && !isMoving)
                    {
                        DeselectUnit();
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
                character.GetComponent<SpriteRenderer>().sprite = character.originalSprite;
                character = null;
                moveAlongPathFinished = false;
            }

            if (character != null)
            {
                character.GetComponent<SpriteRenderer>().sprite = character.selectedSprite;
            }
        }
        else
        {
            cursor.SetActive(false);
        }
    }

    /* ------------------------------------------------------------------------
    * Function: AttackEnemyOnClick
    * Description: Handles attack logic when called.  Takes in the selected
    * overlay tile.
    * ---------------------------------------------------------------------- */
    private void AttackEnemyOnClick(OverlayTile overlayTile)
    {
        if (overlayTile.characterOnTile != null)
        {
            overlayTile.characterOnTile.TakeDamage(character.attack);
        }
        character.attackedThisTurn = true;
        character.GetComponent<SpriteRenderer>().sprite = character.originalSprite;
        character.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f, 1f);
        character = null;
        attackActivated = false;
        characterIsSelected = false;
    }

    /* ------------------------------------------------------------------------
    * Function: AttackEnemyOnClick
    * Description: Handles attack logic when called.  Takes in the selected
    * overlay tile.
    * ---------------------------------------------------------------------- */

    private void DeselectUnit()
    {
        if (character != null)
        {
            characterIsSelected = false;
            character.GetComponent<SpriteRenderer>().sprite = character.originalSprite;
            character = null;
        }
        rangeFinderTiles.Clear();
        Debug.Log("Deselect logic");
    }

    /* ------------------------------------------------------------------------
    * Function: PlaceArrowsOnPath
    * Description: Find if an overlay tile is in range and place a 
    * corresponding arrow alogn the path
    * ---------------------------------------------------------------------- */
    private void PlaceArrowsOnPath(OverlayTile overlayTile)
    {
        // Place Arrows along path for all tiles in range
        if (rangeFinderTiles.Contains(overlayTile) && characterIsSelected && !attackActivated && rangeFinderTiles.Count > 0)
        {
            path = pathFinder.FindPath(character.activeTile, overlayTile, rangeFinderTiles);

            // Hide all arrows when player clicks to start moving
            foreach (OverlayTile tile in rangeFinderTiles)
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
    * Function: GetInAttackRangeTiles
    * Description: Displays tiles that are in attack range of the selected
    * character
    * ---------------------------------------------------------------------- */
    private void GetInAttackRangeTiles()
    {
        rangeFinderTiles = rangeFinder.GetTilesInRangeForAttack(new Vector2Int(character.activeTile.gridLocation.x, character.activeTile.gridLocation.y), character.attackRange);
        foreach (var item in rangeFinderTiles)
        {
            item.ShowTileRed();
        }

        character.activeTile.HideTile();
        attackActivated = true;
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
