using UnityEngine;

/* ----------------------------------------------------------------------------
 * Class: OverlayTile
 * Description: Manages the selected tile gameobject.
 * ---------------------------------------------------------------------------- */
public class OverlayTile : MonoBehaviour
{

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
     * Function: ShowTile()
     * Description: Changes the alpha of the sprite to 1 making it visible.
     * --------------------------------------------------------------------- */
    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    /* ------------------------------------------------------------------------
    * Function: HideTile()
    * Description: Changes the alpha of the sprite to 0 making it invisible.
    * --------------------------------------------------------------------- */
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }


}
