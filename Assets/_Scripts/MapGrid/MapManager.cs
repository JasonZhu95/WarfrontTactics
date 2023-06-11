using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ArrowTranslator;

/* ----------------------------------------------------------------------------
 * Class: MapManager
 * Description: Singleton class in charge of handling dynamic map generation
 * ---------------------------------------------------------------------------- */
public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }

    [SerializeField] private GameObject overlayTilePrefab;
    [SerializeField] private GameObject overlayContainer;
    [SerializeField] private BlockedTilesListSO blockedTiles;
    private HashSet<Sprite> blockedTileSet;

    public Dictionary<Vector2Int, OverlayTile> map;
    public bool ignoreBottomTiles;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        IOrderedEnumerable<Tilemap> tileMaps = gameObject.GetComponentsInChildren<Tilemap>().OrderByDescending(x => x.GetComponent<TilemapRenderer>().sortingOrder);
        map = new Dictionary<Vector2Int, OverlayTile>();
        blockedTileSet = new HashSet<Sprite>(blockedTiles.blockedTileSprites);

        foreach (var tm in tileMaps)
        {
            BoundsInt bounds = tm.cellBounds;

            // Loop through all tiles in the tilemap grid
            for(int z = bounds.max.z; z >= bounds.min.z; z--)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    for (int x = bounds.min.x; x < bounds.max.x; x++)
                    {
                        if (z == 0 && ignoreBottomTiles)
                        {
                            return;
                        }

                        if (tm.HasTile(new Vector3Int(x, y, z)))
                        {
                            if (!map.ContainsKey(new Vector2Int(x, y)))
                            {
                                GameObject overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                                Vector3 cellWorldPosition = tm.GetCellCenterWorld(new Vector3Int(x, y, z));
                                overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                                overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tm.GetComponent<TilemapRenderer>().sortingOrder;
                                overlayTile.gameObject.GetComponent<OverlayTile>().gridLocation = new Vector3Int(x, y, z);
                                overlayTile.gameObject.GetComponent<OverlayTile>().SetArrowSprite(ArrowDirection.None);

                                // Check if the tile needs to be blocked based off sprite data.
                                Sprite sprite = tm.GetSprite(new Vector3Int(x, y, z));
                                if (blockedTileSet.Contains(sprite))
                                {
                                    overlayTile.gameObject.GetComponent<OverlayTile>().isBlocked = true;
                                }

                                map.Add(new Vector2Int(x, y), overlayTile.gameObject.GetComponent<OverlayTile>());
                            }
                        }
                    }
                }
            }
        }
    }

    /* ------------------------------------------------------------------------
    * Function: GetSurroundingTiles
    * Description: Takes a Vector2Int Position of a tile and returns a list of
    * surrounding tiles.
    * ---------------------------------------------------------------------- */
    public List<OverlayTile> GetSurroundingTiles(Vector2Int originTile)
    {
        var surroundingTiles = new List<OverlayTile>();

        // Check Right Neighbour
        Vector2Int TileToCheck = new Vector2Int(originTile.x + 1, originTile.y);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1 && !map[TileToCheck].isBlocked)
                surroundingTiles.Add(map[TileToCheck]);
        }

        // Check Left Neighbour
        TileToCheck = new Vector2Int(originTile.x - 1, originTile.y);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1 && !map[TileToCheck].isBlocked)
                surroundingTiles.Add(map[TileToCheck]);
        }

        // Check Top Neighbour
        TileToCheck = new Vector2Int(originTile.x, originTile.y + 1);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1 && !map[TileToCheck].isBlocked)
                surroundingTiles.Add(map[TileToCheck]);
        }

        // Check Bottom Neighbour
        TileToCheck = new Vector2Int(originTile.x, originTile.y - 1);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1&& !map[TileToCheck].isBlocked)
                surroundingTiles.Add(map[TileToCheck]);
        }

        return surroundingTiles;
    }

    /* ------------------------------------------------------------------------
    * Function: GetSurroundingTilesForAttack
    * Description: Gets surrounding tiles including enemies
    * ---------------------------------------------------------------------- */
    public List<OverlayTile> GetSurroundingTilesForAttack(Vector2Int originTile)
    {
        var surroundingTiles = new List<OverlayTile>();

        // Check Right Neighbour
        Vector2Int TileToCheck = new Vector2Int(originTile.x + 1, originTile.y);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1 && (!map[TileToCheck].isBlocked || map[TileToCheck].characterOnTile != null))
                surroundingTiles.Add(map[TileToCheck]);
        }

        // Check Left Neighbour
        TileToCheck = new Vector2Int(originTile.x - 1, originTile.y);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1 && (!map[TileToCheck].isBlocked || map[TileToCheck].characterOnTile != null))
                surroundingTiles.Add(map[TileToCheck]);
        }

        // Check Top Neighbour
        TileToCheck = new Vector2Int(originTile.x, originTile.y + 1);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1 && (!map[TileToCheck].isBlocked || map[TileToCheck].characterOnTile != null))
                surroundingTiles.Add(map[TileToCheck]);
        }

        // Check Bottom Neighbour
        TileToCheck = new Vector2Int(originTile.x, originTile.y - 1);
        if (map.ContainsKey(TileToCheck))
        {
            if (Mathf.Abs(map[TileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1 && (!map[TileToCheck].isBlocked || map[TileToCheck].characterOnTile != null))
                surroundingTiles.Add(map[TileToCheck]);
        }

        return surroundingTiles;
    }
}
