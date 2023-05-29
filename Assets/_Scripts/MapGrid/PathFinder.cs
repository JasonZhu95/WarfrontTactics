using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/* ----------------------------------------------------------------------------
 * Class: PathFinder
 * Description: Class responsible for managing A* Pathfinding
 * ------------------------------------------------------------------------- */
public class PathFinder
{
    private Dictionary<Vector2Int, OverlayTile> searchableTiles;

    /* ------------------------------------------------------------------------
    * Function: FindPath
    * Description: Takes the Starting Node and Ending Node and Finds the 
    * shortest path using the A* algorithm
    * ---------------------------------------------------------------------- */
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> inRangeTiles)
    {
        searchableTiles = new Dictionary<Vector2Int, OverlayTile>();

        List<OverlayTile> openList = new List<OverlayTile>();
        HashSet<OverlayTile> closedList = new HashSet<OverlayTile>();

        if (inRangeTiles.Count > 0)
        {
            foreach (OverlayTile tile in inRangeTiles)
            {
                searchableTiles.Add(tile.grid2DLocation, MapManager.Instance.map[tile.grid2DLocation]);
            }
        }
        else
        {
            searchableTiles = MapManager.Instance.map;
        }

        openList.Add(start);

        while (openList.Count > 0)
        {
            // Get the Tile with the Lowest F Cost in the list
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

            // Move the lowest f cost tile to the final path list
            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if (currentOverlayTile == end)
            {
                return GetFinishedList(start, end);
            }

            foreach(var tile in GetNeighbourOverlayTiles(currentOverlayTile))
            {
                // Check if the neighbour is valid.
                if (tile.isBlocked || closedList.Contains(tile) || Mathf.Abs(currentOverlayTile.transform.position.z - tile.transform.position.z) > 1)
                {
                    continue;
                }

                // Using Manhattan Distance to calculate shortest path

                tile.G = GetManhattanDistance(start, tile);
                tile.H = GetManhattanDistance(end, tile);

                tile.previousTile = currentOverlayTile;

                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                }
            }
        }
        return new List<OverlayTile>();
    }

    /* ------------------------------------------------------------------------
    * Function: GetFinishedList
    * Description: Backtrack from the end node to return a final path.  Each
    * overlayTile has knowledge of a previous Tile in the path. This function
    * loops through the path starting at end and returns a list of the path.
    * ---------------------------------------------------------------------- */
    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();
        OverlayTile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previousTile;
        }

        finishedList.Reverse();

        return finishedList;
    }

    /* ------------------------------------------------------------------------
    * Function: GetManhattanDistance
    * Description: Takes in a starting node and a neighbour node. Calculates
    * the Manhattan Distance (Distance between nodes ignoring the diagonal)
    * ---------------------------------------------------------------------- */
    private int GetManhattanDistance(OverlayTile start, OverlayTile neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) +
            Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

    /* ------------------------------------------------------------------------
    * Function: GetNeighbourOverlayTiles
    * Description: Takes in the current tile and returns a list of neighbors.
    * ---------------------------------------------------------------------- */
    private List<OverlayTile> GetNeighbourOverlayTiles(OverlayTile currentOverlayTile)
    {
        var map = MapManager.Instance.map;

        List<OverlayTile> neighbours = new List<OverlayTile>();

        // Check Right Neighbour
        Vector2Int locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x + 1,
            currentOverlayTile.gridLocation.y
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        // Check Left Neighbour
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x - 1,
            currentOverlayTile.gridLocation.y
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        // Check Top Neighbour
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        // Check Bottom Neighbour
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y - 1
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        return neighbours;
    }
}
