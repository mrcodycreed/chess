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
        if (BoardState == null || !BoardState.IsWithinBounds(x, y)) return false;

        PieceData? targetPiece = BoardState.GetPiece(x, y);
        if (targetPiece.HasValue && targetPiece.Value.Color == PieceColour) return false;

        PieceData? selfData = CurrentPieceData;
        if (!selfData.HasValue) return false;

        int deltaX = x - Position.x;
        int deltaY = y - Position.y;
        int forwardDirection = PieceColour == PieceColor.White ? 1 : -1;

        if (deltaX == 0 && deltaY == forwardDirection)
        {
            return !targetPiece.HasValue;
        }

        if (deltaX == 0 && deltaY == forwardDirection * 2 && !selfData.Value.HasMoved)
        {
            Vector2Int intermediate = new Vector2Int(x, y - forwardDirection);
            return !targetPiece.HasValue && !BoardState.GetPiece(intermediate).HasValue;
        }

        if (Mathf.Abs(deltaX) == 1 && deltaY == forwardDirection)
        {
            if (targetPiece.HasValue && targetPiece.Value.Color != PieceColour) return true;

            if (!targetPiece.HasValue && BoardState.EnPassantTarget.HasValue)
            {
                Vector2Int enPassant = BoardState.EnPassantTarget.Value;
                if (enPassant.x == x && enPassant.y == y) return true;
            }
        }

        return false;
    }
}

