using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents Knight
/// </summary>
public class Knight : ChessPiece
{
    /// <summary>
    /// Determines if the piece can move to the specified position on the chess board.
    /// </summary>
    /// <param name="x">The x-coordinate of the destination position on the board.</param>
    /// <param name="y">The y-coordinate of the destination position on the board.</param>
    /// <returns>True if the piece can move to the specified position, false otherwise.</returns>
    public override bool CanMoveTo(int x, int y)
    {
        ChessPiece targetPiece = ChessBoard.Instance.GetPiece(x, y);
        if (targetPiece != null && targetPiece.Color == this.Color) return false;
 
        // Calculate the movement distance in x and y direction
        int deltaX = Mathf.Abs(x - Position.x);
        int deltaY = Mathf.Abs(y - Position.y);

        // Check if the movement is valid (L-shape with length 2 and 1)
        if ((deltaX == 1 && deltaY == 2) || (deltaX == 2 && deltaY == 1))
        {
            return true;
        }
        else
        {
            // Invalid move
            return false;
        }
    }
}
