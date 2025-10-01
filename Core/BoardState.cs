using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data model that tracks the state of the chess board without relying on MonoBehaviours.
/// </summary>
public class BoardState
{
    public const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private readonly PieceData?[,] squares = new PieceData?[8, 8];

    public PieceColor SideToMove { get; set; } = PieceColor.White;
    public CastlingRights Castling { get; private set; } = CastlingRights.All;
    public Vector2Int? EnPassantTarget { get; private set; }
    public int HalfmoveClock { get; private set; }
    public int FullmoveNumber { get; private set; } = 1;

    public List<MoveRecord> MoveHistory { get; } = new List<MoveRecord>();

    public BoardState()
    {
    }

    private BoardState(BoardState other)
    {
        SideToMove = other.SideToMove;
        Castling = other.Castling;
        EnPassantTarget = other.EnPassantTarget;
        HalfmoveClock = other.HalfmoveClock;
        FullmoveNumber = other.FullmoveNumber;
        MoveHistory.AddRange(other.MoveHistory);

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                squares[x, y] = other.squares[x, y];
            }
        }
    }

    public static BoardState CreateInitial()
    {
        var state = new BoardState();
        state.LoadFromFen(StartingFen);
        return state;
    }

    public BoardState Clone() => new BoardState(this);

    public PieceData? GetPiece(int x, int y)
    {
        if (!IsWithinBounds(x, y)) return null;
        return squares[x, y];
    }

    public PieceData? GetPiece(Vector2Int position) => GetPiece(position.x, position.y);

    public void SetPiece(int x, int y, PieceData? piece)
    {
        if (!IsWithinBounds(x, y)) return;
        squares[x, y] = piece;
    }

    public void SetPiece(Vector2Int position, PieceData? piece) => SetPiece(position.x, position.y, piece);

    public bool IsWithinBounds(int x, int y) => x >= 0 && x < 8 && y >= 0 && y < 8;

    public MoveRecord ApplyMove(Vector2Int from, Vector2Int to, PieceType? promotion = null)
    {
        PieceData? movingPiece = GetPiece(from);
        if (!movingPiece.HasValue)
            throw new InvalidOperationException("No piece exists on the origin square.");

        PieceData movingData = movingPiece.Value;
        MoveRecord record = new MoveRecord
        {
            From = from,
            To = to,
            MovingPiece = movingData,
            Promotion = promotion
        };

        PieceData? capturedPiece = GetPiece(to);
        Vector2Int? capturedPosition = null;

        bool isEnPassantCapture = false;
        if (movingData.Type == PieceType.Pawn && !capturedPiece.HasValue && EnPassantTarget.HasValue && EnPassantTarget.Value == to)
        {
            int direction = movingData.Color == PieceColor.White ? -1 : 1;
            Vector2Int pawnToCapture = new Vector2Int(to.x, to.y + direction);
            PieceData? enPassantPawn = GetPiece(pawnToCapture);
            if (enPassantPawn.HasValue && enPassantPawn.Value.Type == PieceType.Pawn && enPassantPawn.Value.Color != movingData.Color)
            {
                capturedPiece = enPassantPawn;
                capturedPosition = pawnToCapture;
                SetPiece(pawnToCapture, null);
                isEnPassantCapture = true;
            }
        }

        if (capturedPiece.HasValue && !capturedPosition.HasValue)
        {
            capturedPosition = to;
            SetPiece(to, null);
        }

        record.CapturedPiece = capturedPiece;
        record.CapturedPosition = capturedPosition;
        record.IsEnPassantCapture = isEnPassantCapture;

        bool isCastling = movingData.Type == PieceType.King && Mathf.Abs(to.x - from.x) == 2;
        if (isCastling)
        {
            int rookFromX = to.x > from.x ? 7 : 0;
            int rookToX = to.x > from.x ? to.x - 1 : to.x + 1;
            Vector2Int rookFrom = new Vector2Int(rookFromX, from.y);
            Vector2Int rookTo = new Vector2Int(rookToX, from.y);

            PieceData? rookData = GetPiece(rookFrom);
            if (rookData.HasValue)
            {
                SetPiece(rookFrom, null);
                PieceData movedRook = rookData.Value;
                movedRook.HasMoved = true;
                SetPiece(rookTo, movedRook);
                record.IsCastling = true;
                record.RookFrom = rookFrom;
                record.RookTo = rookTo;
            }
        }

        SetPiece(from, null);

        PieceData updatedPiece = movingData;
        updatedPiece.HasMoved = true;
        if (promotion.HasValue)
        {
            updatedPiece.Type = promotion.Value;
        }
        SetPiece(to, updatedPiece);

        UpdateCastlingRights(from, to, movingData, capturedPiece);
        UpdateEnPassantTarget(from, to, movingData);
        UpdateMoveCounters(movingData, capturedPiece.HasValue);

        SideToMove = movingData.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;

        MoveHistory.Add(record);
        return record;
    }

    private void UpdateCastlingRights(Vector2Int from, Vector2Int to, PieceData movingPiece, PieceData? capturedPiece)
    {
        if (movingPiece.Type == PieceType.King)
        {
            if (movingPiece.Color == PieceColor.White)
            {
                Castling = Castling.WithWhiteKingSide(false).WithWhiteQueenSide(false);
            }
            else
            {
                Castling = Castling.WithBlackKingSide(false).WithBlackQueenSide(false);
            }
        }
        else if (movingPiece.Type == PieceType.Rook)
        {
            if (movingPiece.Color == PieceColor.White)
            {
                if (from.x == 0 && from.y == 0) Castling = Castling.WithWhiteQueenSide(false);
                if (from.x == 7 && from.y == 0) Castling = Castling.WithWhiteKingSide(false);
            }
            else
            {
                if (from.x == 0 && from.y == 7) Castling = Castling.WithBlackQueenSide(false);
                if (from.x == 7 && from.y == 7) Castling = Castling.WithBlackKingSide(false);
            }
        }

        if (capturedPiece.HasValue && capturedPiece.Value.Type == PieceType.Rook)
        {
            if (capturedPiece.Value.Color == PieceColor.White)
            {
                if (to.x == 0 && to.y == 0) Castling = Castling.WithWhiteQueenSide(false);
                if (to.x == 7 && to.y == 0) Castling = Castling.WithWhiteKingSide(false);
            }
            else
            {
                if (to.x == 0 && to.y == 7) Castling = Castling.WithBlackQueenSide(false);
                if (to.x == 7 && to.y == 7) Castling = Castling.WithBlackKingSide(false);
            }
        }
    }

    private void UpdateEnPassantTarget(Vector2Int from, Vector2Int to, PieceData movingPiece)
    {
        if (movingPiece.Type == PieceType.Pawn && Mathf.Abs(to.y - from.y) == 2)
        {
            EnPassantTarget = new Vector2Int(from.x, (from.y + to.y) / 2);
        }
        else
        {
            EnPassantTarget = null;
        }
    }

    private void UpdateMoveCounters(PieceData movingPiece, bool isCapture)
    {
        if (movingPiece.Type == PieceType.Pawn || isCapture)
        {
            HalfmoveClock = 0;
        }
        else
        {
            HalfmoveClock++;
        }

        if (movingPiece.Color == PieceColor.Black)
        {
            FullmoveNumber++;
        }
    }

    public void LoadFromFen(string fen)
    {
        if (string.IsNullOrWhiteSpace(fen))
            throw new ArgumentException("FEN string cannot be null or empty", nameof(fen));

        string[] parts = fen.Split(' ');
        if (parts.Length < 4)
            throw new ArgumentException("FEN string must contain at least four parts", nameof(fen));

        Clear();

        string[] ranks = parts[0].Split('/');
        if (ranks.Length != 8)
            throw new ArgumentException("FEN placement data must have eight ranks", nameof(fen));

        for (int rankIndex = 0; rankIndex < 8; rankIndex++)
        {
            string rank = ranks[rankIndex];
            int file = 0;
            foreach (char c in rank)
            {
                if (char.IsDigit(c))
                {
                    file += c - '0';
                }
                else
                {
                    if (file >= 8)
                        throw new ArgumentException("FEN placement data is invalid", nameof(fen));

                    PieceData piece = PieceData.FromFenChar(c);
                    int y = 7 - rankIndex;
                    SetPiece(file, y, piece);
                    file++;
                }
            }

            if (file != 8)
                throw new ArgumentException("FEN rank does not contain eight squares", nameof(fen));
        }

        SideToMove = parts[1] == "w" ? PieceColor.White : PieceColor.Black;
        Castling = CastlingRights.FromFen(parts[2]);
        EnPassantTarget = parts[3] == "-" ? (Vector2Int?)null : ParseSquare(parts[3]);
        HalfmoveClock = parts.Length > 4 ? int.Parse(parts[4]) : 0;
        FullmoveNumber = parts.Length > 5 ? int.Parse(parts[5]) : 1;

        InitializeHasMovedFlags();
    }

    private void Clear()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                squares[x, y] = null;
            }
        }
        MoveHistory.Clear();
        Castling = CastlingRights.All;
        SideToMove = PieceColor.White;
        EnPassantTarget = null;
        HalfmoveClock = 0;
        FullmoveNumber = 1;
    }

    private void InitializeHasMovedFlags()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (!squares[x, y].HasValue) continue;

                PieceData piece = squares[x, y].Value;
                switch (piece.Type)
                {
                    case PieceType.Pawn:
                        int startRank = piece.Color == PieceColor.White ? 1 : 6;
                        piece.HasMoved = y != startRank;
                        break;
                    case PieceType.Rook:
                        piece.HasMoved = DetermineRookHasMoved(x, y, piece.Color);
                        break;
                    case PieceType.King:
                        piece.HasMoved = DetermineKingHasMoved(piece.Color);
                        break;
                    default:
                        piece.HasMoved = false;
                        break;
                }

                squares[x, y] = piece;
            }
        }
    }

    private bool DetermineRookHasMoved(int x, int y, PieceColor colour)
    {
        if (colour == PieceColor.White)
        {
            if (x == 0 && y == 0) return !Castling.WhiteQueenSide;
            if (x == 7 && y == 0) return !Castling.WhiteKingSide;
        }
        else
        {
            if (x == 0 && y == 7) return !Castling.BlackQueenSide;
            if (x == 7 && y == 7) return !Castling.BlackKingSide;
        }

        return true;
    }

    private bool DetermineKingHasMoved(PieceColor colour)
    {
        if (colour == PieceColor.White)
        {
            return !(Castling.WhiteKingSide || Castling.WhiteQueenSide);
        }
        return !(Castling.BlackKingSide || Castling.BlackQueenSide);
    }

    public string ToFen()
    {
        List<string> ranks = new List<string>();
        for (int rank = 7; rank >= 0; rank--)
        {
            int emptyCount = 0;
            string rankString = string.Empty;
            for (int file = 0; file < 8; file++)
            {
                PieceData? piece = squares[file, rank];
                if (piece.HasValue)
                {
                    if (emptyCount > 0)
                    {
                        rankString += emptyCount.ToString();
                        emptyCount = 0;
                    }
                    rankString += piece.Value.ToFenChar();
                }
                else
                {
                    emptyCount++;
                }
            }

            if (emptyCount > 0)
            {
                rankString += emptyCount.ToString();
            }

            ranks.Add(rankString);
        }

        string placement = string.Join("/", ranks);
        string side = SideToMove == PieceColor.White ? "w" : "b";
        string castling = Castling.ToFen();
        string enPassant = EnPassantTarget.HasValue ? FormatSquare(EnPassantTarget.Value) : "-";

        return $"{placement} {side} {castling} {enPassant} {HalfmoveClock} {FullmoveNumber}";
    }

    private static Vector2Int ParseSquare(string square)
    {
        if (square.Length != 2)
            throw new ArgumentException("Invalid en-passant square", nameof(square));

        int file = square[0] - 'a';
        int rank = square[1] - '1';
        if (file < 0 || file > 7 || rank < 0 || rank > 7)
            throw new ArgumentException("Invalid en-passant square", nameof(square));

        return new Vector2Int(file, rank);
    }

    private static string FormatSquare(Vector2Int square)
    {
        char file = (char)('a' + square.x);
        char rank = (char)('1' + square.y);
        return $"{file}{rank}";
    }
}

public enum PieceColor
{
    White,
    Black
}

public enum PieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public struct PieceData
{
    public PieceType Type;
    public PieceColor Color;
    public bool HasMoved;

    public static PieceData FromFenChar(char c)
    {
        PieceColor colour = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
        PieceType type = char.ToLowerInvariant(c) switch
        {
            'p' => PieceType.Pawn,
            'n' => PieceType.Knight,
            'b' => PieceType.Bishop,
            'r' => PieceType.Rook,
            'q' => PieceType.Queen,
            'k' => PieceType.King,
            _ => throw new ArgumentException($"Invalid FEN character '{c}'")
        };

        return new PieceData
        {
            Type = type,
            Color = colour,
            HasMoved = false
        };
    }

    public char ToFenChar()
    {
        char c = Type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Rook => 'r',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => ' '
        };

        return Color == PieceColor.White ? char.ToUpperInvariant(c) : c;
    }
}

public struct CastlingRights
{
    public bool WhiteKingSide;
    public bool WhiteQueenSide;
    public bool BlackKingSide;
    public bool BlackQueenSide;

    public static CastlingRights All => new CastlingRights
    {
        WhiteKingSide = true,
        WhiteQueenSide = true,
        BlackKingSide = true,
        BlackQueenSide = true
    };

    public static CastlingRights FromFen(string fen)
    {
        if (fen == "-")
            return new CastlingRights();

        CastlingRights rights = new CastlingRights();
        foreach (char c in fen)
        {
            switch (c)
            {
                case 'K': rights.WhiteKingSide = true; break;
                case 'Q': rights.WhiteQueenSide = true; break;
                case 'k': rights.BlackKingSide = true; break;
                case 'q': rights.BlackQueenSide = true; break;
                default: throw new ArgumentException($"Invalid castling rights character '{c}'");
            }
        }

        return rights;
    }

    public CastlingRights WithWhiteKingSide(bool value)
    {
        WhiteKingSide = value;
        return this;
    }

    public CastlingRights WithWhiteQueenSide(bool value)
    {
        WhiteQueenSide = value;
        return this;
    }

    public CastlingRights WithBlackKingSide(bool value)
    {
        BlackKingSide = value;
        return this;
    }

    public CastlingRights WithBlackQueenSide(bool value)
    {
        BlackQueenSide = value;
        return this;
    }

    public string ToFen()
    {
        string fen = string.Empty;
        if (WhiteKingSide) fen += "K";
        if (WhiteQueenSide) fen += "Q";
        if (BlackKingSide) fen += "k";
        if (BlackQueenSide) fen += "q";
        return string.IsNullOrEmpty(fen) ? "-" : fen;
    }
}

public struct MoveRecord
{
    public Vector2Int From;
    public Vector2Int To;
    public PieceData MovingPiece;
    public PieceData? CapturedPiece;
    public Vector2Int? CapturedPosition;
    public PieceType? Promotion;
    public bool IsCastling;
    public Vector2Int? RookFrom;
    public Vector2Int? RookTo;
    public bool IsEnPassantCapture;
}
