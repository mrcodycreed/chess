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
        if (BoardState == null || !BoardState.IsWithinBounds(x, y)) return false;

        PieceData? targetPiece = BoardState.GetPiece(x, y);
        if (targetPiece.HasValue && targetPiece.Value.Color == PieceColour) return false;

        int deltaX = Mathf.Abs(x - Position.x);
        int deltaY = Mathf.Abs(y - Position.y);

        return (deltaX == 1 && deltaY == 2) || (deltaX == 2 && deltaY == 1);
    }
}
