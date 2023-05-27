using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* ----------------------------------------------------------------------------
 * Class: MapManager
 * Description: Singleton class in charge of handling dynamic map generation
 * ---------------------------------------------------------------------------- */
public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance { get { return instance; } }
    [SerializeField] private OverlayTile overlayTilePrefab;
    [SerializeField] private GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;

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
        Tilemap tileMap = gameObject.GetComponentInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds;
        // Loop through all tiles in the tilemap grid
        for(int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    Vector3Int tileLocation = new Vector3Int(x, y, z);
                    Vector2Int tileKey = new Vector2Int(x, y);

                    // Generates an overlay tile for each tile location in grid.
                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        // Convert tile location coordinates to world space coordinates
                        OverlayTile overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        Vector3 cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        // Set overlay tile 1 higher on the z-axis sorting layer to appear above other tiles
                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }
}
