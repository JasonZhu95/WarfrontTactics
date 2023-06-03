using System.Collections.Generic;
using UnityEngine;
using static ArrowTranslator;

/* ----------------------------------------------------------------------------
 * Class: OverlayTile
 * Description: Manages the data for each individual Tile in the map or grid.
 * ---------------------------------------------------------------------------- */
public class OverlayTile : MonoBehaviour
{
    /* A* Pathfinding Variables
     * G = Distance from Starting Node
     * H = Distance from End Node
     * F = G + H */
    public int G { get; set; }
    public int H { get; set; }
    public int F { get => G + H; }

    public bool isBlocked = false;
    public bool isOccupied = false;
    public CharacterData characterOnTile;

    public OverlayTile previousTile { get; set; }
    public Vector3Int gridLocation { get; set; }
    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public List<Sprite> arrows;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTile();
        }
    }

    /* ------------------------------------------------------------------------
     * Function: ShowTile
     * Description: Changes the alpha of the sprite to 1 making it visible.
     * --------------------------------------------------------------------- */
    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    /* ------------------------------------------------------------------------
     * Function: ShowTileRed
     * Description: Changes the alpha of the sprite to 1 making it visible.
     * --------------------------------------------------------------------- */
    public void ShowTileRed()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
    }

    /* ------------------------------------------------------------------------
    * Function: HideTile
    * Description: Changes the alpha of the sprite to 0 making it invisible.
    * --------------------------------------------------------------------- */
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetArrowSprite(ArrowDirection.None);
    }

    /* ------------------------------------------------------------------------
    * Function: SetArrowSprite
    * Description: Takes the direction of the arrow sprite and sets the alpha
    * of the sprite to show from the enum ArrowDirection.
    * --------------------------------------------------------------------- */
    public void SetArrowSprite(ArrowDirection d)
    {
        SpriteRenderer arrow = GetComponentsInChildren<SpriteRenderer>()[1];
        if (d == ArrowDirection.None)
        {
            arrow.color = new Color(1, 1, 1, 0);
        }
        else
        {
            arrow.color = new Color(1, 1, 1, 1);
            arrow.sprite = arrows[(int)d];
            arrow.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }
}
