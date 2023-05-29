using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* ----------------------------------------------------------------------------
 * Class: RangeFinder
 * Description: Class in charge of determining the distance or range of a
 * selected character
 * ---------------------------------------------------------------------------- */
public class RangeFinder
{
    /* ------------------------------------------------------------------------
    * Function: GetTilesInRange
    * Description: Takes in the starting location and the maximum range of
    * the character.  Returns a list of available tiles in range.
    * ---------------------------------------------------------------------- */
    public List<OverlayTile> GetTilesInRange(Vector2Int location, int range)
    {
        OverlayTile startingTile = MapManager.Instance.map[location];
        List<OverlayTile> inRangeTiles = new List<OverlayTile>();
        int stepCount = 0;

        //Add initial starting Tile
        inRangeTiles.Add(startingTile);

        List<OverlayTile> tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);
        // Step through all of the surrounding tiles in range
        while (stepCount < range)
        {
            List<OverlayTile> surroundingTiles = new List<OverlayTile>();

            foreach (var item in tileForPreviousStep)
            {
                surroundingTiles.AddRange(MapManager.Instance.GetSurroundingTiles(new Vector2Int(item.gridLocation.x, item.gridLocation.y)));
            }

            inRangeTiles.AddRange(surroundingTiles);
            tileForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;
        }

        return inRangeTiles.Distinct().ToList();
    }
}
