using UnityEngine;

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

    public bool IsBlocked { get; set; }
    public Vector3Int gridLocation { get; set; }
    public OverlayTile previousTile { get; set; }

    private void Start()
    {
        
    }

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
    * Function: HideTile
    * Description: Changes the alpha of the sprite to 0 making it invisible.
    * --------------------------------------------------------------------- */
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }


}
