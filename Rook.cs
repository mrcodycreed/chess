using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Represents Rook
/// </summary>
public class Rook : ChessPiece
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

        if (x == Position.x && y == Position.y) return false;
        if (x != Position.x && y != Position.y) return false;

        int dx = x - Position.x;
        int dy = y - Position.y;

        int stepX = dx == 0 ? 0 : dx / Mathf.Abs(dx);
        int stepY = dy == 0 ? 0 : dy / Mathf.Abs(dy);

        int distance = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
        for (int i = 1; i < distance; i++)
        {
            int checkX = Position.x + i * stepX;
            int checkY = Position.y + i * stepY;

            if (BoardState.GetPiece(checkX, checkY).HasValue)
            {
                return false;
            }
        }

        PieceData? destination = BoardState.GetPiece(x, y);
        return !destination.HasValue || destination.Value.Color != PieceColour;
    }
}
