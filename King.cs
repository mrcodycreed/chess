using UnityEngine;

/// <summary>
/// Represents a King
/// </summary>
public class King : ChessPiece
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

        // Check if the target position is already occupied by a friendly piece
        ChessPiece targetPiece = ChessBoard.Instance.GetPiece(x, y);
        if (targetPiece != null && targetPiece.Color == this.Color) return false;

        // Check if the target position is one square away horizontally or vertically
        if (Mathf.Abs(x - Position.x) <= 1 && Mathf.Abs(y - Position.y) <= 1) return true;

        // Check if the target position is a valid castling move
        if (CanCastle(x, y)) return true;

        return false;
    }

    /// <summary>
    /// Determines whether the king and a rook can castle from their current positions to the specified position.
    /// </summary>
    /// <param name="x">The x-coordinate of the destination square.</param>
    /// <param name="y">The y-coordinate of the destination square.</param>
    /// <returns><c>true</c> if the king and rook can castle to the specified position, otherwise <c>false</c>.</returns>
    private bool CanCastle(int x, int y)
    {
        // Check if the king and rook involved in castling have not moved
        if (HasMoved || ChessBoard.Instance.GetPiece(x, y) != null) return false;

        // Check if the target position is two squares away horizontally
        if (Mathf.Abs(x - Position.x) != 2 || Mathf.Abs(y - Position.y) != 0) return false;

        // Check if there are no pieces between the king and rook
        int rookX = x > Position.x ? 7 : 0;
        ChessPiece rook = ChessBoard.Instance.GetPiece(rookX, Position.y);
        if (rook == null || rook.GetType() != typeof(Rook) || rook.Color != Color || rook.HasMoved) return false;

        for (int i = Mathf.Min(Position.x, x) + 1; i < Mathf.Max(Position.x, x); i++)
        {
            if (ChessBoard.Instance.GetPiece(i, Position.y) != null) return false;
        }

        return true;
    }
}