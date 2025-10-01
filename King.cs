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
        if (BoardState == null || !BoardState.IsWithinBounds(x, y)) return false;

        PieceData? targetPiece = BoardState.GetPiece(x, y);
        if (targetPiece.HasValue && targetPiece.Value.Color == PieceColour) return false;

        if (Mathf.Abs(x - Position.x) <= 1 && Mathf.Abs(y - Position.y) <= 1) return true;

        return CanCastle(x, y);
    }

    /// <summary>
    /// Determines whether the king and a rook can castle from their current positions to the specified position.
    /// </summary>
    /// <param name="x">The x-coordinate of the destination square.</param>
    /// <param name="y">The y-coordinate of the destination square.</param>
    /// <returns><c>true</c> if the king and rook can castle to the specified position, otherwise <c>false</c>.</returns>
    private bool CanCastle(int x, int y)
    {
        if (BoardState == null) return false;
        if (y != Position.y) return false;
        if (Mathf.Abs(x - Position.x) != 2) return false;
        if (BoardState.GetPiece(x, y).HasValue) return false;

        PieceData? kingData = CurrentPieceData;
        if (!kingData.HasValue || kingData.Value.HasMoved) return false;

        bool castleKingSide = x > Position.x;

        if (PieceColour == PieceColor.White)
        {
            if (castleKingSide && !BoardState.Castling.WhiteKingSide) return false;
            if (!castleKingSide && !BoardState.Castling.WhiteQueenSide) return false;
        }
        else
        {
            if (castleKingSide && !BoardState.Castling.BlackKingSide) return false;
            if (!castleKingSide && !BoardState.Castling.BlackQueenSide) return false;
        }

        int rookX = castleKingSide ? 7 : 0;
        PieceData? rookData = BoardState.GetPiece(rookX, Position.y);
        if (!rookData.HasValue || rookData.Value.Type != PieceType.Rook || rookData.Value.Color != PieceColour || rookData.Value.HasMoved)
            return false;

        for (int i = Mathf.Min(Position.x, x) + 1; i < Mathf.Max(Position.x, x); i++)
        {
            if (BoardState.GetPiece(i, Position.y).HasValue) return false;
        }

        return true;
    }
}
