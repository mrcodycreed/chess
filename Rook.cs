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
        // Check if the target position is within the bounds of the board
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
            return false;

        // Check if the destination is the same as the current position
        if (x == Position.x && y == Position.y)
        {
            return false;
        }

        // Check if the destination is on the same row or column as the rook
        if (x != Position.x && y != Position.y)
        {
            return false;
        }

        int dx = x - Position.x;
        int dy = y - Position.y;

        int signX = dx == 0 ? 0 : dx / Mathf.Abs(dx);
        int signY = dy == 0 ? 0 : dy / Mathf.Abs(dy);

        for (int i = 1; i < Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)); i++)
        {
            int checkX = Position.x + i * signX;
            int checkY = Position.y + i * signY;

            if (ChessBoard.Instance.GetPiece(checkX, checkY) != null)
            {
                return false;
            }
        }

        // Check if the destination is occupied by a piece of the opposite color, or empty
        ChessPiece pieceAtDestination = ChessBoard.Instance.GetPiece(x, y);
        return pieceAtDestination == null || pieceAtDestination.Color != Color;
    }
}