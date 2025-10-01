using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Bishop
/// </summary>
public class Bishop : ChessPiece
{

    /// <summary>
    /// Determines if the piece can move to the specified position on the chess board.
    /// </summary>
    /// <param name="x">The x-coordinate of the destination position on the board.</param>
    /// <param name="y">The y-coordinate of the destination position on the board.</param>
    /// <returns>True if the piece can move to the specified position, false otherwise.</returns>
    public override bool CanMoveTo(int x, int y)
    {
        if (BoardState == null || !BoardState.IsWithinBounds(x, y)) return false;

        int deltaX = x - Position.x;
        int deltaY = y - Position.y;

        if (deltaX == 0 && deltaY == 0) return false;
        if (Mathf.Abs(deltaX) != Mathf.Abs(deltaY)) return false;

        int stepX = deltaX > 0 ? 1 : -1;
        int stepY = deltaY > 0 ? 1 : -1;

        int xTest = Position.x + stepX;
        int yTest = Position.y + stepY;
        while (xTest != x && yTest != y)
        {
            if (BoardState.GetPiece(xTest, yTest).HasValue)
            {
                return false;
            }
            xTest += stepX;
            yTest += stepY;
        }

        PieceData? targetPiece = BoardState.GetPiece(x, y);
        return !targetPiece.HasValue || targetPiece.Value.Color != PieceColour;
    }
   
}
