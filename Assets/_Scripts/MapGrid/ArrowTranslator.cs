using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ----------------------------------------------------------------------------
 * Class: ArrowTranslator
 * Description: Class in charge of determining which arrow to display when
 * following a path on mouse hover.
 * ---------------------------------------------------------------------------- */
public class ArrowTranslator
{
    public enum ArrowDirection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        TopRight = 5,
        BottomRight = 6,
        TopLeft = 7,
        BottomLeft = 8,
        UpFinished = 9,
        DownFinished = 10,
        LeftFinished = 11,
        RightFinished = 12
    }

    /* ------------------------------------------------------------------------
    * Function: TranslateDirection
    * Description: Given the path and intended direction of the tiles
    * Finds the corresponding arrow direction to return using an enum
    * ---------------------------------------------------------------------- */
    public ArrowDirection TranslateDirection(OverlayTile previousTile, OverlayTile currentTile, OverlayTile futureTile)
    {
        bool isFinal = futureTile == null;
        Vector2Int pastDirection = previousTile != null ? currentTile.grid2DLocation - previousTile.grid2DLocation : new Vector2Int(0, 0);
        Vector2Int futureDirection = futureTile != null ? futureTile.grid2DLocation - currentTile.grid2DLocation : new Vector2Int(0, 0);
        Vector2Int direction = pastDirection != futureDirection ? pastDirection + futureDirection : futureDirection;

        if (direction == new Vector2Int(0, 1) && !isFinal)
        {
            return ArrowDirection.Up;
        }
        if (direction == new Vector2Int(0, -1) && !isFinal)
        {
            return ArrowDirection.Down;
        }
        if (direction == new Vector2Int(1, 0) && !isFinal)
        {
            return ArrowDirection.Right;
        }
        if (direction == new Vector2Int(-1, 0) && !isFinal)
        {
            return ArrowDirection.Left;
        }
        if (direction == new Vector2Int(1, 1))
        {
            if (pastDirection.y < futureDirection.y)
            {
                return ArrowDirection.BottomLeft;
            }
            else
            {
                return ArrowDirection.TopRight;
            }
        }
        if (direction == new Vector2Int(-1, 1))
        {
            if (pastDirection.y < futureDirection.y)
            {
                return ArrowDirection.BottomRight;
            }
            else
            {
                return ArrowDirection.TopLeft;
            }
        }
        if (direction == new Vector2Int(1, -1))
        {
            if (pastDirection.y > futureDirection.y)
            {
                return ArrowDirection.TopLeft;
            }
            else
            {
                return ArrowDirection.BottomRight;
            }
        }
        if (direction == new Vector2Int(-1, -1))
        {
            if (pastDirection.y > futureDirection.y)
            {
                return ArrowDirection.TopRight;
            }
            else
            {
                return ArrowDirection.BottomLeft;
            }
        }
        if (direction == new Vector2Int(0, 1) && isFinal)
        {
            return ArrowDirection.UpFinished;
        }
        if (direction == new Vector2Int(0, -1) && isFinal)
        {
            return ArrowDirection.DownFinished;
        }
        if (direction == new Vector2Int(1, 0) && isFinal)
        {
            return ArrowDirection.RightFinished;
        }
        if (direction == new Vector2Int(-1, 0) && isFinal)
        {
            return ArrowDirection.LeftFinished;
        }

        return ArrowDirection.None;
    }
}
