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
        ChessPiece targetPiece = ChessBoard.Instance.GetPiece(x, y);
        if (targetPiece != null && targetPiece.Color == this.Color) return false;

        // Calculate the movement distance in x and y direction
        int deltaX = x - Position.x;
        int deltaY = y - Position.y;

        // Check if the movement is diagonal
        if (Mathf.Abs(deltaX) == Mathf.Abs(deltaY))
        {
            // Check if the diagonal path is clear
            int xDir = (deltaX > 0) ? 1 : -1;
            int yDir = (deltaY > 0) ? 1 : -1;
            int xTest = Position.x + xDir;
            int yTest = Position.y + yDir;
            while (xTest != x && yTest != y)
            {
                if (ChessBoard.Instance.GetPiece(xTest, yTest) != null)
                {
                    return false;
                }
                xTest += xDir;
                yTest += yDir;
            }

            return true;
        }
        else
        {
            // Invalid move
            return false;
        }
    }
   
}
