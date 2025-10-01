using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents Queen
/// </summary>
public class Queen : ChessPiece
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

        bool isDiagonal = Mathf.Abs(deltaX) == Mathf.Abs(deltaY);
        bool isStraight = deltaX == 0 || deltaY == 0;

        if (!isDiagonal && !isStraight) return false;

        int stepX = 0;
        int stepY = 0;
        int steps = 0;

        if (isDiagonal)
        {
            stepX = deltaX > 0 ? 1 : -1;
            stepY = deltaY > 0 ? 1 : -1;
            steps = Mathf.Abs(deltaX);
        }
        else
        {
            stepX = deltaX == 0 ? 0 : (deltaX > 0 ? 1 : -1);
            stepY = deltaY == 0 ? 0 : (deltaY > 0 ? 1 : -1);
            steps = Mathf.Max(Mathf.Abs(deltaX), Mathf.Abs(deltaY));
        }

        for (int i = 1; i < steps; i++)
        {
            int checkX = Position.x + i * stepX;
            int checkY = Position.y + i * stepY;
            if (BoardState.GetPiece(checkX, checkY).HasValue)
            {
                return false;
            }
        }

        PieceData? targetPiece = BoardState.GetPiece(x, y);
        return !targetPiece.HasValue || targetPiece.Value.Color != PieceColour;
    }


}
