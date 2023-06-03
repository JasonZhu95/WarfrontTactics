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
        TurnManager.OnPlayerTurnChanged -= OnPlayerTurnChanged;
        OnCharacterDeath += FindCharacterTarget;
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
            AttackCharactersInRange();
        }
    }

    public void PerformAction()
    {
        FindCharacterTarget();
        ShowTilesInRange();
        PlaceArrowsOnPath();
    }

    /* ------------------------------------------------------------------------
    * Function: FindCharacterTarget
    * Description: Loops through all the tiles on the map and looks for
    * characters.  The closest character is then set as the target.
    * ---------------------------------------------------------------------- */
    private void FindCharacterTarget()
    {
        Debug.Log("Called");
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

    private void ShowTilesInRange()
    {
        rangeFinderTiles = rangeFinder.GetTilesInRange(new Vector2Int(activeTile.gridLocation.x, activeTile.gridLocation.y), range);
        foreach(var item in rangeFinderTiles)
        {
            item.ShowTile();
        }
    }

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

    private IEnumerator DealDamage()
    {
        yield return new WaitForSeconds(1f);

        foreach (OverlayTile tile in rangeFinderTiles)
        {
            tile.HideTile();
        }
        if (charactersInRange.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, charactersInRange.Count);
            charactersInRange[randomIndex].TakeDamage(attack);
            charactersInRange.Clear();
        }

        StartCoroutine(FinishTurn());
    }

    private IEnumerator FinishTurn()
    {
        yield return new WaitForSeconds(.5f);
        enemyManager.IncrementEnemyIndex();
        enemyManager.PerformNextAction();
    }

    private void PlaceArrowsOnPath()
    {
        path = pathFinder.FindPath(activeTile, ReturnClosestTileInRangeToTarget(), rangeFinderTiles);
        foreach (OverlayTile tile in rangeFinderTiles)
        {
            MapManager.Instance.map[tile.grid2DLocation].SetArrowSprite(ArrowDirection.None);
        }

        StartCoroutine(PlaceArrowAfterTime());
    }

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

    private void AttackCharactersInRange()
    {
        ShowTilesInAttackRange();
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

    private OverlayTile ReturnClosestTileInRangeToTarget()
    {
        float minDistance = float.MaxValue;
        int minIndex = 0;
        for (int i = 0; i < rangeFinderTiles.Count; i++)
        {
            float distance = Vector2Int.Distance(target.activeTile.grid2DLocation, rangeFinderTiles[i].grid2DLocation);
            if (distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }
        return rangeFinderTiles[minIndex];
    }

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
