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
        // Check if the move is diagonal
        if (Mathf.Abs(x - Position.x) == Mathf.Abs(y - Position.y))
        {
            // Check if the path is clear
            int dx = (x > Position.x) ? 1 : -1;
            int dy = (y > Position.y) ? 1 : -1;
            int steps = Mathf.Abs(x - Position.x);
            for (int i = 1; i < steps; i++)
            {
                int checkX = Position.x + i * dx;
                int checkY = Position.y + i * dy;
                if (ChessBoard.Instance.GetPiece(checkX, checkY) != null)
                {
                    return false;
                }
            }

            // Check if the destination is empty or has an opposing piece
            ChessPiece targetPiece = ChessBoard.Instance.GetPiece(x, y);
            return targetPiece == null || targetPiece.Color != Color;
        }
        // Check if the move is vertical
        else if (x == Position.x || y == Position.y)
        {
            // Check if the path is clear
            int dx = (x > Position.x) ? 1 : (x < Position.x) ? -1 : 0;
            int dy = (y > Position.y) ? 1 : (y < Position.y) ? -1 : 0;
            int steps = Mathf.Max(Mathf.Abs(x - Position.x), Mathf.Abs(y - Position.y));
            for (int i = 1; i < steps; i++)
            {
                int checkX = Position.x + i * dx;
                int checkY = Position.y + i * dy;
                if (ChessBoard.Instance.GetPiece(checkX, checkY) != null)
                {
                    return false;
                }
            }

            // Check if the destination is empty or has an opposing piece
            ChessPiece targetPiece = ChessBoard.Instance.GetPiece(x, y);
            return targetPiece == null || targetPiece.Color != Color;
        }

        return false;
    }


}