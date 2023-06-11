using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArrowTranslator;

/* ----------------------------------------------------------------------------
 * Class: EnemyData
 * Description: Handles enemy specific logic and inherits from the CharacterData
 * script.
 * ---------------------------------------------------------------------------- */
public class EnemyData : CharacterData
{
    private CharacterData target;
    private List<CharacterData> characterList;
    [SerializeField] private MouseController mouseController;
    [SerializeField] private EnemyManager enemyManager;

    private RangeFinder rangeFinder;
    private PathFinder pathFinder;
    private ArrowTranslator arrowTranslator;
    private List<OverlayTile> rangeFinderTiles;
    private List<OverlayTile> path;
    private List<CharacterData> charactersInRange = new List<CharacterData>();
    private bool isMoving = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        characterList = new List<CharacterData>();
        mouseController = GameObject.Find("MouseController").GetComponent<MouseController>();
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();

        rangeFinder = new RangeFinder();
        pathFinder = new PathFinder();
        arrowTranslator = new ArrowTranslator();
        rangeFinderTiles = new List<OverlayTile>();
        path = new List<OverlayTile>();

    }

    private void OnEnable()
    {
        OnCharacterDeath += FindCharacterTarget;
        TurnManager.OnPlayerTurnChanged += OnPlayerTurnChanged;
    }

    private void OnDisable()
    {
        OnCharacterDeath -= FindCharacterTarget;
    }

    private void Update()
    {
        if (path.Count > 0 && isMoving)
        {
            MoveAlongPath();
        }

        if (path.Count == 0 && isMoving)
        {
            isMoving = false;
            ShowTilesInAttackRange();
        }
    }

    /* ------------------------------------------------------------------------
    * Function: GetTilesInRangeForAttack
    * Description: Starts the Artifical intelligence of the enemy Data when
    * their turn starts.
    * ---------------------------------------------------------------------- */
    public void PerformAction()
    {
        SetSelectedSprite(true);
        FindCharacterTarget();
        ShowTilesInRange();
        PlaceArrowsOnPath();
    }

    private void SetSelectedSprite(bool selected)
    {
        if (selected)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = selectedSprite;
            anim.SetBool("selected", true);

        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f, 1f);
            gameObject.GetComponent<SpriteRenderer>().sprite = originalSprite;
            anim.SetBool("selected", false);
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
        target = ReturnClosestCharacter(characterList);
        characterList.Clear();
    }

    /* ------------------------------------------------------------------------
    * Function: ShowTilesInRange
    * Description: Finds all tiles in range and highlights them
    * ---------------------------------------------------------------------- */
    private void ShowTilesInRange()
    {
        SoundManager.instance.Play("CharacterSelected");
        rangeFinderTiles = rangeFinder.GetTilesInRange(new Vector2Int(activeTile.gridLocation.x, activeTile.gridLocation.y), range);
        foreach(var item in rangeFinderTiles)
        {
            item.ShowTile();
        }
    }

    /* ------------------------------------------------------------------------
    * Function: ShowTilesInAttackRange
    * Description: Finds all tiles in attack range and highlts them red.
    * ---------------------------------------------------------------------- */
    private void ShowTilesInAttackRange()
    {
        rangeFinderTiles = rangeFinder.GetTilesInRangeForAttack(new Vector2Int(activeTile.gridLocation.x, activeTile.gridLocation.y), attackRange);
        foreach (var item in rangeFinderTiles)
        {
            item.ShowTileRed();
            if (item.isOccupied && !item.characterOnTile.isEnemy)
            {
                charactersInRange.Add(item.characterOnTile);
            }
        }
        StartCoroutine(DealDamage());
    }

    /* ------------------------------------------------------------------------
    * Function: DealDamage
    * Description: Create a buffer in between enemy movement and attack to get
    * it to feel like a player.
    * ---------------------------------------------------------------------- */
    private IEnumerator DealDamage()
    {
        yield return new WaitForSeconds(1f);

        foreach (OverlayTile tile in rangeFinderTiles)
        {
            tile.HideTile();
        }
        if (charactersInRange.Count > 0)
        {
            switch (attackRange)
            {
                case 1:
                    SoundManager.instance.Play("Sword");
                    break;
                case 2:
                    SoundManager.instance.Play("Spear");
                    break;
                case 3:
                    SoundManager.instance.Play("Bow");
                    break;
                default:
                    SoundManager.instance.Play("Sword");
                    break;
            }

            int randomIndex = UnityEngine.Random.Range(0, charactersInRange.Count);
            StartCoroutine(AttackAnimation());
            charactersInRange[randomIndex].TakeDamage(attack);
            charactersInRange.Clear();
        }
        else
        {
            SetSelectedSprite(false);
            StartCoroutine(FinishTurn());
        }
    }

    /* ------------------------------------------------------------------------
    * Function: FinishTurn
    * Description: Ends current Enemy turn and goes to the next enemies turn.
    * ---------------------------------------------------------------------- */
    private IEnumerator AttackAnimation()
    {
        anim.SetBool("attack", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("attack", false);
        SetSelectedSprite(false);
        StartCoroutine(FinishTurn());
    }

    /* ------------------------------------------------------------------------
    * Function: FinishTurn
    * Description: Ends current Enemy turn and goes to the next enemies turn.
    * ---------------------------------------------------------------------- */
    private IEnumerator FinishTurn()
    {
        yield return new WaitForSeconds(.5f);
        enemyManager.IncrementEnemyIndex();
        enemyManager.PerformNextAction();
    }

    /* ------------------------------------------------------------------------
    * Function: PlaceArrowsOnPath
    * Description: Finds the best path to get to the target character.
    * Places the arrow sprite along the path.
    * ---------------------------------------------------------------------- */
    private void PlaceArrowsOnPath()
    {
        path = pathFinder.FindPath(activeTile, ReturnClosestTileInRangeToTarget(), rangeFinderTiles);
        foreach (OverlayTile tile in rangeFinderTiles)
        {
            MapManager.Instance.map[tile.grid2DLocation].SetArrowSprite(ArrowDirection.None);
        }

        StartCoroutine(PlaceArrowAfterTime());
    }

    /* ------------------------------------------------------------------------
    * Function: PlaceArrowAfterTime
    * Description: Simulate a small delay in between the arrows to emulate
    * a player-like movement
    * ---------------------------------------------------------------------- */
    private IEnumerator PlaceArrowAfterTime()
    {
        for (int i = 0; i < path.Count; i++)
        {
            OverlayTile previousTile = i > 0 ? path[i - 1] : activeTile;
            OverlayTile futureTile = i < path.Count - 1 ? path[i + 1] : null;

            ArrowDirection arrowDir = arrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
            path[i].SetArrowSprite(arrowDir);
            yield return new WaitForSeconds(.3f);
        }

        foreach (OverlayTile tile in rangeFinderTiles)
        {
            tile.HideTile();
        }
        rangeFinderTiles.Clear();
        MoveAlongPath();
    }

    /* ------------------------------------------------------------------------
    * Function: MoveAlongPath
    * Description: Moves the enemy along the given path
    * ---------------------------------------------------------------------- */
    private void MoveAlongPath()
    {
        isMoving = true;
        float step = 3 * Time.deltaTime;

        if (path.Count > 0)
        {

            float zIndex = path[0].transform.position.z;
            transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, step);
            transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);

            foreach (OverlayTile tile in rangeFinderTiles)
            {
                MapManager.Instance.map[tile.grid2DLocation].SetArrowSprite(ArrowDirection.None);
            }

            if (Vector2.Distance(transform.position, path[0].transform.position) < 0.0001f)
            {
                PositionOnTile(path[0]);
                path.RemoveAt(0);
            }
        }
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

    /* ------------------------------------------------------------------------
    * Function: ReturnClosestTileInRangeToTarget
    * Description: Gets the closest tile in range to the target.
    * ---------------------------------------------------------------------- */
    private OverlayTile ReturnClosestTileInRangeToTarget()
    {
        float minDistance = float.MaxValue;
        int minIndex = 0;
        for (int i = 0; i < rangeFinderTiles.Count; i++)
        {
            if (!rangeFinderTiles[i].isOccupied || rangeFinderTiles[i].characterOnTile == this)
            {
                float distance = GetManhattanDistance(target.activeTile.grid2DLocation, rangeFinderTiles[i].grid2DLocation);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }
        }
        return rangeFinderTiles[minIndex];
    }

    /* ------------------------------------------------------------------------
    * Function: GetManhattanDistance
    * Description: Takes in a starting node and a neighbour node. Calculates
    * the Manhattan Distance (Distance between nodes ignoring the diagonal)
    * ---------------------------------------------------------------------- */
    private float GetManhattanDistance(Vector2Int point1, Vector2Int point2)
    {
        return Mathf.Abs(point1.x - point2.x) + Mathf.Abs(point1.y - point2.y);
    }

    /* ------------------------------------------------------------------------
    * Function: PositionOnTile
    * Description: Handles the logic of placing the enemy on an active tile.
    * ---------------------------------------------------------------------- */
    private void PositionOnTile(OverlayTile tile)
    {
        activeTile.isOccupied = false;
        activeTile.characterOnTile = null;
        activeTile.isBlocked = false;
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        activeTile = tile;
        movedThisTurn = true;
        tile.characterOnTile = this;
        tile.isOccupied = true;
        tile.isBlocked = true;

    }
}
