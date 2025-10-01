using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents Pawn
/// </summary>
public class Pawn : ChessPiece
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
        int deltaX = x - Position.x;
        int deltaY = y - Position.y;

        // Pawns can only move forward
        int forwardDirection = (Color == Color.white) ? 1 : -1;
        if (deltaX == 0 && deltaY == forwardDirection)
        {
            // Check if the target square is empty
            return (ChessBoard.Instance.GetPiece(x, y) == null);
        }
        else if (deltaX == 0 && deltaY == forwardDirection * 2 && !HasMoved)
        {
            // Check if the pawn has not moved and the two squares in front are empty
            return (ChessBoard.Instance.GetPiece(x, y) == null && ChessBoard.Instance.GetPiece(x, y - forwardDirection) == null);
        }
        else if (Mathf.Abs(deltaX) == 1 && deltaY == forwardDirection)
        {
            // Check if the target square is occupied by an enemy piece
            return (targetPiece != null && targetPiece.Color != Color);
        }
        else
        {
            // Invalid move
            return false;
        }
    }
}

