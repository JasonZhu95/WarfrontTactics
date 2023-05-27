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
    /* ------------------------------------------------------------------------
    * Function: FindPath
    * Description: Takes the Starting Node and Ending Node and Finds the 
    * shortest path using the A* algorithm
    * ---------------------------------------------------------------------- */
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closedList = new List<OverlayTile>();

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

            List<OverlayTile> neighbourTiles = GetNeighbourTiles(currentOverlayTile);

            foreach(var neighbour in neighbourTiles)
            {
                // Check if the neighbour is valid.
                // The MathF check is looking to see if the tile is too high
                if (neighbour.IsBlocked || closedList.Contains(neighbour) || Mathf.Abs(currentOverlayTile.gridLocation.z - neighbour.gridLocation.z) > 1)
                {
                    continue;
                }

                // Using Manhattan Distance to calculate shortest path

                neighbour.G = GetManhattanDistance(start, neighbour);
                neighbour.H = GetManhattanDistance(end, neighbour);

                neighbour.previousTile = currentOverlayTile;
                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
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
    * Function: GetNeighbourTiles
    * Description: Takes in a starting node. Returns a list of all neighbours
    * of the starting node.
    * ---------------------------------------------------------------------- */
    private List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile)
    {
        Dictionary<Vector2Int, OverlayTile> map = MapManager.Instance.map;

        List<OverlayTile> neighbours = new List<OverlayTile>();

        // Neighbour On Top
        Vector2Int locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1
            );
        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        // Neighbour On Bottom
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y - 1
            );
        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        // Neighbour On Right
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x + 1,
            currentOverlayTile.gridLocation.y
            );
        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        // Neighbour On Left
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x - 1,
            currentOverlayTile.gridLocation.y
            );
        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        return neighbours;
    }
}
