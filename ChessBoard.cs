using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// The ChessBoard class is responsible for representing the state of the chess game board, as well as for moving the pieces on it.
/// </summary>
public class ChessBoard : MonoBehaviour
{
    #region SINGLETON

    public static ChessBoard Instance;
    private void Awake() => Instance = this;

    #endregion

    public ChessPiece[,] board;

    [SerializeField] private GameObject whitePawnPrefab;
    [SerializeField] private GameObject blackPawnPrefab;
    [SerializeField] private GameObject whiteRookPrefab;
    [SerializeField] private GameObject blackRookPrefab;
    [SerializeField] private GameObject whiteKnightPrefab;
    [SerializeField] private GameObject blackKnightPrefab;
    [SerializeField] private GameObject whiteBishopPrefab;
    [SerializeField] private GameObject blackBishopPrefab;
    [SerializeField] private GameObject whiteQueenPrefab;
    [SerializeField] private GameObject blackQueenPrefab;
    [SerializeField] private GameObject whiteKingPrefab;
    [SerializeField] private GameObject blackKingPrefab;

    private float moveTime = 0.5f;
    private ChessPiece pieceToTake = null;
    public bool PieceMoving = false;
    public bool GameFinished = false;

    [SerializeField] private bool animateY = true;

    /// <summary>
    /// Initializes the scene
    /// </summary>
    private void Start()
    {
        SetUpBoard();
    }

    /// <summary>
    /// Spawns the pieces and puts them in their initial positions
    /// </summary>
    private void SetUpBoard() {
        board = new ChessPiece[8, 8];

        // Set up white pieces
        board[0, 0] = Instantiate(whiteRookPrefab).GetComponent<ChessPiece>();
        board[1, 0] = Instantiate(whiteKnightPrefab).GetComponent<ChessPiece>();
        board[2, 0] = Instantiate(whiteBishopPrefab).GetComponent<ChessPiece>();
        board[3, 0] = Instantiate(whiteQueenPrefab).GetComponent<ChessPiece>();
        board[4, 0] = Instantiate(whiteKingPrefab).GetComponent<ChessPiece>();
        board[5, 0] = Instantiate(whiteBishopPrefab).GetComponent<ChessPiece>();
        board[6, 0] = Instantiate(whiteKnightPrefab).GetComponent<ChessPiece>();
        board[7, 0] = Instantiate(whiteRookPrefab).GetComponent<ChessPiece>();

        for (int i = 0; i < 8; i++) {
            board[i, 1] = Instantiate(whitePawnPrefab).GetComponent<ChessPiece>();
        }

        // Set up black pieces
        board[0, 7] = Instantiate(blackRookPrefab).GetComponent<ChessPiece>();
        board[1, 7] = Instantiate(blackKnightPrefab).GetComponent<ChessPiece>();
        board[2, 7] = Instantiate(blackBishopPrefab).GetComponent<ChessPiece>();
        board[3, 7] = Instantiate(blackQueenPrefab).GetComponent<ChessPiece>();
        board[4, 7] = Instantiate(blackKingPrefab).GetComponent<ChessPiece>();
        board[5, 7] = Instantiate(blackBishopPrefab).GetComponent<ChessPiece>();
        board[6, 7] = Instantiate(blackKnightPrefab).GetComponent<ChessPiece>();
        board[7, 7] = Instantiate(blackRookPrefab).GetComponent<ChessPiece>();

        for (int i = 0; i < 8; i++) {
            board[i, 6] = Instantiate(blackPawnPrefab).GetComponent<ChessPiece>();
        }

        // Set the initial position and name of each piece
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null)
                {
                    board[i, j].Position = new Vector2Int(i, j);
                    board[i, j].Color = (j < 2) ? Color.white : Color.black;
                    string colourName = (j < 2) ? "White" : "Black";
                    PositionOnBoard(board[i, j]);
                    board[i, j].name = $"{board[i, j].GetType().Name} {colourName} {i},{j}"; // Set the name
                }
            }
        }
    }

    /// <summary>
    /// Visually places the pieces in position
    /// </summary>
    private void PositionAllPiecesOnBoard()
    {
        for(int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                PositionOnBoard(board[x, y]);
            }
        }
    }

    /// <summary>
    /// Takes a piece from the board
    /// </summary>
    public void TakePiece()
    {
        if (pieceToTake == null) return;

        print($"Taking {pieceToTake.Name}");
        UIManager.Instance.AddMove($"Taking {pieceToTake.Name}");
        Destroy(pieceToTake.gameObject);
        pieceToTake = null;
        AudioManager.Instance.PlayTake();
    }

    /// <summary>
    /// Attempts to move the specified ChessPiece to the given (x,y) position on the ChessBoard.
    /// </summary>
    /// <param name="piece">The ChessPiece to move.</param>
    /// <param name="x">The x-coordinate of the target position.</param>
    /// <param name="y">The y-coordinate of the target position.</param>
    /// <param name="positionOnBoard">Whether to visually update the board or not</param>
    /// <returns>True if the move was successful, false otherwise.</returns>
    public bool MoveTo(ChessPiece piece, int x, int y, bool positionOnBoard)
    {
        if (PieceMoving ||  GameFinished) return false;

        bool checkBeforeMove = CheckForCheck();

        // Store the current position of the piece
        Vector2Int pieceStartPos = piece.Position;

        board[piece.Position.x, piece.Position.y] = null;

        if(board[x,y] != null)
        {
            pieceToTake = board[x, y];
            Invoke("TakePiece", moveTime - (moveTime / 4));
        }

        board[x, y] = piece;
        piece.Position = new Vector2Int(x, y);
        piece.HasMoved = true;

        bool checkAfterMove = CheckForCheck();

        if (checkBeforeMove && checkAfterMove)
        {
            UIManager.Instance.AddMove($"That move will not stop check");

            // Revert the move
            board[x, y] = null;
            board[pieceStartPos.x, pieceStartPos.y] = piece;
            piece.Position = pieceStartPos;

            return false;
        }

        if (positionOnBoard)
        {
            StartCoroutine(MovePieceCoroutine(piece, piece.Position, moveTime));
            if(animateY) StartCoroutine(MoveY(piece, moveTime, 0.5f));
        }

        print($"{piece.Name} to ({x},{y})");
        UIManager.Instance.AddMove($"{piece.Name} to ({x},{y})");

        // Check for win condition
        bool win = CheckForWin();
        if (win)
        {
            // Handle win state
            HandleWinState(piece.Color);
            return true;
        }

        return true;
    }

    /// <summary>
    /// Checks for the checkmate win state
    /// </summary>
    private bool CheckForWin()
    {
        // Check if the current player's opponent is in checkmate
        Color currentPlayerColor = GameManager.Instance.PlayerTurn ? Color.white : Color.black;
        List<ChessPiece> opponentPieces = GetPieces(currentPlayerColor == Color.white ? Color.black : Color.white);
        foreach (ChessPiece opponentPiece in opponentPieces)
        {
            if (opponentPiece is King)
            {         
                if (IsInCheckmate(opponentPiece))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Activates the win state
    /// </summary>
    /// <param name="winnerColor">The winning side</param>
    private void HandleWinState(Color winnerColor)
    {
        GameFinished = true;
        string winnerName = winnerColor == Color.white ? "White" : "Black";
        UIManager.Instance.Win(winnerName);
    }

    /// <summary>
    /// Coroutine to move a ChessPiece object up on the y-axis and then back down to its original position
    /// over a certain amount of time.
    /// </summary>
    /// <param name="piece">The ChessPiece object to move.</param>
    /// <param name="moveTime">The amount of time it should take for the piece to complete the movement.</param>
    /// <param name="moveDistance">The distance the piece should move on the y-axis.</param>
    /// <returns>An IEnumerator object to allow the method to be used as a coroutine.</returns>
    IEnumerator MoveY(ChessPiece piece, float moveTime, float moveDistance)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = new Vector3(piece.transform.localPosition.x,0, piece.transform.localPosition.z);
        Vector3 targetPosition = startPosition + new Vector3(0f, moveDistance, 0f);

        while (elapsedTime < moveTime && piece != null)
        {
            piece.transform.localPosition = Vector3.Lerp(new Vector3(piece.transform.localPosition.x, startPosition.y, piece.transform.localPosition.z), new Vector3( piece.transform.localPosition.x, targetPosition.y, piece.transform.localPosition.z), (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(piece != null) piece.transform.localPosition = new Vector3(piece.transform.localPosition.x, targetPosition.y, piece.transform.localPosition.z);

        yield return new WaitForSeconds(0.01f);

        elapsedTime = 0f;
        while (elapsedTime < moveTime && piece != null)
        {
            piece.transform.localPosition = Vector3.Lerp(new Vector3(piece.transform.localPosition.x, targetPosition.y, piece.transform.localPosition.z), new Vector3(piece.transform.localPosition.x, startPosition.y, piece.transform.localPosition.z), (elapsedTime / moveTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (piece != null) piece.transform.localPosition = new Vector3(piece.transform.localPosition.x, startPosition.y, piece.transform.localPosition.z);
    }

    /// <summary>
    /// Coroutine that moves a chess piece smoothly to a target position on the board over a specified amount of time.
    /// </summary>
    /// <param name="piece">The chess piece to move.</param>
    /// <param name="targetPosition">The target position on the board where the chess piece should move to.</param>
    /// <param name="moveTime">The time it should take to move the chess piece to the target position.</param>
    /// <returns>An IEnumerator that can be used to start the coroutine.</returns>
    IEnumerator MovePieceCoroutine(ChessPiece piece, Vector2Int targetPosition, float moveTime)
    {
        if (piece == null)
        {
            Debug.LogWarning("Invalid piece!");
            yield break;
        }

        PieceMoving = true;

        Vector3 startPosition = piece.transform.position;
        Vector3 targetPositionWorld = new Vector3(targetPosition.x, 0, targetPosition.y);

        float elapsedTime = 0f;
        while (elapsedTime < moveTime)
        {
            piece.transform.position = Vector3.Lerp(startPosition, targetPositionWorld, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        piece.Position = targetPosition;

        PositionAllPiecesOnBoard();

        GameManager.Instance.ChangeTurn();

        if (GameManager.Instance.ComputerOpponent)
        {
            if (!GameManager.Instance.PlayerTurn)
            {
                GameObject.Find("Targeting Circle").GetComponent<SpriteRenderer>().enabled = false;
                Invoke("AIMove", UnityEngine.Random.Range(0.5f, 2f));
            }

        }
        PieceMoving = false;

        yield return null;
    }

    /// <summary>
    /// Returns the ChessPiece object at the specified (x, y) position on the ChessBoard.
    /// </summary>
    /// <param name="x">The x-coordinate of the position to check.</param>
    /// <param name="y">The y-coordinate of the position to check.</param>
    /// <returns>The ChessPiece object at the specified (x, y) position. If no ChessPiece is found at that position, returns null.</returns>
    public ChessPiece GetPiece(int x, int y)
    {
        if (x < 0 || x >= board.GetLength(0) || y < 0 || y >= board.GetLength(1)) return null;
        return board[x, y];
    }

    /// <summary>
    /// Gets a list of all ChessPieces of a given color currently on the board.
    /// </summary>
    /// <param name="colour">The color of the pieces to get.</param>
    /// <returns>A list of ChessPieces of the given color.</returns>
    public List<ChessPiece> GetPieces(Color colour)
    {
        List<ChessPiece> pieces = new List<ChessPiece>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null)
                {
                    if (board[x, y].Color == colour)
                        pieces.Add(board[x, y]);
                }
            }
        }

        return pieces;
    }

    /// <summary>
    /// Checks if the given ChessPiece is on the ChessBoard and updates its position accordingly.
    /// </summary>
    /// <param name="piece">The ChessPiece to check and update position for.</param>
    private void PositionOnBoard(ChessPiece piece)
    {
        if (piece == null) return;
        Vector3 position = new Vector3(piece.Position.x, 0f, piece.Position.y);
        piece.transform.position = position;
    }

    #region AI

    /// <summary>
    /// Starts an AI turn
    /// </summary>
    public void AIMove()
    {
        GameObject.Find("Targeting Circle").GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(CalculateBestMove(Color.black));
    }

    private ChessPiece lastAIPieceMoved;

    /// <summary>
    /// Calculates the best move for the given color within the maximum thinking time.
    /// </summary>
    /// <param name="colourToCalculate">The color of the player to calculate the move for.</param>
    /// <returns>An IEnumerator that yields when the calculation is complete.</returns>
    private IEnumerator CalculateBestMove(Color colourToCalculate)
    {
        DateTime startTime = DateTime.Now;

        // Get a list of all pieces on the board belonging to the AI
        List<ChessPiece> pieces = GetPieces(colourToCalculate);

        // Initialize variables to hold the best move and its resulting score
        ChessPiece bestPiece = null;
        Vector2Int bestMove = Vector2Int.zero;

        int bestScore = colourToCalculate == Color.white ? int.MinValue : int.MaxValue;

        ChessPiece[,] savedBoard = board.Clone() as ChessPiece[,];

        ChessPiece kingPiece = pieces.Find(piece => piece is King);

        bool isKingInCheck = IsInCheck(kingPiece);

        // Loop through all possible moves for each piece
        foreach (ChessPiece piece in pieces)
        {
            List<Vector2Int> possibleMoves = piece.GetPossibleMoves();

            foreach (Vector2Int move in possibleMoves)
            {
                if (move != piece.Position)
                {
                    board = savedBoard.Clone() as ChessPiece[,];

                    if (move.x > 0 && move.x < 8 && move.y > 0 && move.y < 8)
                    {
                        if (piece.CanMoveTo(move.x, move.y))
                        {
                            board[piece.Position.x, piece.Position.y] = null;
                            board[move.x, move.y] = piece;

                            if (isKingInCheck && WouldMovePutKingInCheck(kingPiece, kingPiece.Position)) // If king was in check and this move doesn't resolve check, discard move
                                continue;

                            yield return EvaluateBoard(piece.Color);
                            int score = lastEvaluatedScore;

                            if (piece == lastAIPieceMoved)
                            {
                                if (colourToCalculate == Color.white)
                                    score -= 50;
                                else
                                    score += 50;
                            }

                            if (piece is King)
                            {
                                if (WouldMovePutKingInCheck(piece, move))
                                {
                                    if (colourToCalculate == Color.white)
                                        score -= 9999;
                                    else
                                        score += 9999;
                                }
                            }

                            // Update the best move if the new score is higher
                            if (colourToCalculate == Color.white && score > bestScore || colourToCalculate == Color.black && score < bestScore)
                            {
                                bestScore = score;
                                bestPiece = piece;
                                bestMove = move;
                            }
                        }
                    }
                }
            }
        }

        board = savedBoard.Clone() as ChessPiece[,];

        // Move the piece to the best possible position on the board
        if (bestPiece != null)
        {
            lastAIPieceMoved = bestPiece;
            MoveTo(bestPiece, bestMove.x, bestMove.y, false);
            AudioManager.Instance.PlayMove();
            StartCoroutine(MovePieceCoroutine(bestPiece, bestMove, moveTime));
            if (animateY) StartCoroutine(MoveY(bestPiece, moveTime, 0.5f));
            yield return null;
        }

        yield return null;
    }

    #endregion

    #region SCORING

    private PawnTable pawnTable = new PawnTable();
    private KnightTable knightTable = new KnightTable();
    private BishopTable bishopTable = new BishopTable();
    private RookTable rookTable = new RookTable();
    private QueenTable queenTable = new QueenTable();
    private KingTable kingTable = new KingTable();
    private int lastEvaluatedScore = 0;

    /// <summary>
    /// Evaluates the current state of the board for the specified color.
    /// </summary>
    /// <param name="colour">The colour to evaluate the board for.</param>
    /// <returns>An integer value representing the evaluation score of the board for the specified color.</returns>
    public int EvaluateBoard(Color colour)
    {
        int score = 0;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                ChessPiece piece = board[x, y];

                // Evaluate material score
                if (piece != null)
                {
                    if (piece.Color == colour)
                    {
                        if (piece is Pawn)
                        {
                            score += 100;
                        }
                        else if (piece is Knight)
                        {
                            score += 320;
                        }
                        else if (piece is Bishop)
                        {
                            score += 330;
                        }
                        else if (piece is Rook)
                        {
                            score += 500;
                        }
                        else if (piece is Queen)
                        {
                            score += 900;
                        }
                        else if (piece is King)
                        {
                            score += 20000;
                        }
                    }

                    if (piece is Pawn)
                    {
                        score += (piece.Color == Color.white) ? 100 + pawnTable.GetScore(x, y) : -100 - pawnTable.GetScore(7 - x, 7 - y);
                    }
                    else if (piece is Knight)
                    {
                        score += (piece.Color == Color.white) ? 320 + knightTable.GetScore(x, y) : -320 - knightTable.GetScore(7 - x, 7 - y);
                    }
                    else if (piece is Bishop)
                    {
                        score += (piece.Color == Color.white) ? 330 + bishopTable.GetScore(x, y) : -330 - bishopTable.GetScore(7 - x, 7 - y);
                    }
                    else if (piece is Rook)
                    {
                        score += (piece.Color == Color.white) ? 500 + rookTable.GetScore(x, y) : -500 - rookTable.GetScore(7 - x, 7 - y);
                    }
                    else if (piece is Queen)
                    {
                        score += (piece.Color == Color.white) ? 900 + queenTable.GetScore(x, y) : -900 - queenTable.GetScore(7 - x, 7 - y);
                    }
                    else if (piece is King)
                    {
                        score += (piece.Color == Color.white) ? 20000 + kingTable.GetScore(x, y) : -20000 - kingTable.GetScore(7 - x, 7 - y);
                    }

                    // Add a bonus for each opposing piece that can be captured by this piece
                    foreach (ChessPiece opponent in GetPieces(piece.Color == Color.white ? Color.black : Color.white))
                    {
                        if (piece.CanMoveTo(opponent.Position.x, opponent.Position.y))
                        {
                            score += (piece.Color == Color.white ? 1 : -1) * 2;
                        }
                    }
                }
            }
        }

        lastEvaluatedScore = score;
        return score;
    }

    /// <summary>
    /// Checks for a Check condition
    /// </summary>
    private bool CheckForCheck()
    {
        bool inCheck = false;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if(board[x, y] != null && IsInCheck(board[x,y]))
                {
                    print($"{board[x, y].Name} is in check");
                    UIManager.Instance.AddMove($"{board[x, y].Name} is in check");
                    inCheck = true;
                }
            }
        }

        return inCheck;
    }

    /// <summary>
    /// Determines whether the given ChessPiece is in check or not.
    /// </summary>
    /// <param name="piece">The ChessPiece to check for check.</param>
    /// <returns>True if the given ChessPiece is in check, otherwise false.</returns>
    private bool IsInCheck(ChessPiece piece)
    {
        // Find the position of the king
        if (piece is King king)
        {
            Vector2Int kingPosition = king.Position;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    ChessPiece opposingPiece = board[x, y];
                    if (opposingPiece != null && opposingPiece.Color != king.Color)
                    {
                        if (opposingPiece.CanMoveTo(kingPosition.x, kingPosition.y))
                        {
                            // The king is in check
                            return true;
                        }
                    }
                }
            }
        }

        // The piece is not a king or is not in check
        return false;
    }

    /// <summary>
    /// Checks if the King of the given color is in check
    /// </summary>
    /// <param name="kingPosition">The position of the King</param>
    /// <param name="colour">The color of the King to check</param>
    /// <returns>True if the King is in check, false otherwise</returns>
    public bool IsInCheck(Vector2Int kingPosition, Color colour)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                ChessPiece opposingPiece = board[x, y];
                if (opposingPiece != null && opposingPiece.Color != colour)
                {
                    if (!(opposingPiece is King) && opposingPiece.CanMoveTo(kingPosition.x, kingPosition.y))
                    {
                        // The king is in check
                        return true;
                    }
                }
            }
        }
        
        // The piece is not a king or is not in check
        return false;
    }

    /// <summary>
    /// Determines if a given move for a given piece would put the same colored king in check.
    /// </summary>
    /// <param name="piece">The piece to check the move for.</param>
    /// <param name="move">The move to check.</param>
    /// <returns>Returns true if the move would put the same colored king in check, false otherwise.</returns>
    public bool WouldMovePutKingInCheck(ChessPiece piece, Vector2Int move)
    {
        ChessPiece[,] savedBoard = board.Clone() as ChessPiece[,];

        bool retVal = false;

        board[piece.Position.x, piece.Position.y] = null;
        board[move.x, move.y] = piece;

        if (IsInCheckmate(piece))
            retVal = true;

        board = savedBoard.Clone() as ChessPiece[,];

        return retVal;
    }

    /// <summary>
    /// Checks if a ChessPiece passed to it as a parameter is in checkmate.
    /// A ChessPiece is in checkmate if it is in check and cannot move out of the way, capture the attacking piece, or have another piece block the attack.
    /// </summary>
    /// <param name="piece">The ChessPiece to check for checkmate.</param>
    /// <returns>True if the piece is in checkmate, false otherwise.</returns>
    public bool IsInCheckmate(ChessPiece piece)
    {
        if (!IsInCheck(piece))
        {
            return false;
        }

        // Check if the king can move out of check
        foreach (Vector2Int move in piece.GetPossibleMoves())
        {
            int newX = piece.Position.x + move.x;
            int newY = piece.Position.y + move.y;

            if (piece.CanMoveTo(newX,newY))
            {
                return false;
            }
        }

        // Check if any other piece can block the attack
        List<ChessPiece> attackingPieces = GetAttackingPieces(piece);
        foreach (ChessPiece attackingPiece in attackingPieces)
        {
            if (attackingPiece != piece)
            {
                List<Vector2Int> lineOfAttack = GetLineOfAttack(attackingPiece, piece);
                foreach (Vector2Int blockPosition in lineOfAttack)
                {
                    if (CanBlockAttack(piece, attackingPiece, blockPosition))
                    {
                        return false;
                    }
                }
            }
        }

        UIManager.Instance.AddMove($"{piece.Name} is in checkmate");

        return true;
    }

    /// <summary>
    /// Generates a list of board positions representing the line of attack from the attacking piece to the defending piece.
    /// </summary>
    /// <param name="attacker">The ChessPiece that is initiating the attack.</param>
    /// <param name="defender">The ChessPiece that is being attacked.</param>
    /// <returns>A list of Vector2Int objects representing the line of attack from the attacker to the defender on the chess board.</returns>
    private List<Vector2Int> GetLineOfAttack(ChessPiece attacker, ChessPiece defender)
    {
        List<Vector2Int> lineOfAttack = new List<Vector2Int>();

        int dx = defender.Position.x - attacker.Position.x;
        int dy = defender.Position.y - attacker.Position.y;

        int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
        int stepX = (dx != 0) ? dx / Math.Abs(dx) : 0;
        int stepY = (dy != 0) ? dy / Math.Abs(dy) : 0;

        // Add positions from attacker to defender to the line of attack
        for (int i = 1; i <= steps; i++)
        {
            int x = attacker.Position.x + i * stepX;
            int y = attacker.Position.y + i * stepY;
            lineOfAttack.Add(new Vector2Int(x, y));
        }

        return lineOfAttack;
    }

    /// <summary>
    /// Determines whether a given block position can block an attack from an attacking piece on the king
    /// </summary>
    /// <param name="king">The ChessPiece representing the king</param>
    /// <param name="attackingPiece">The ChessPiece representing the attacking piece</param>
    /// <param name="blockPosition">The position to block the attack</param>
    /// <returns>True if the attack can be blocked, false otherwise</returns>
    private bool CanBlockAttack(ChessPiece king, ChessPiece attackingPiece, Vector2Int blockPosition)
    {
        List<ChessPiece> friendlyPieces = GetPieces(king.Color);
        foreach (ChessPiece friendlyPiece in friendlyPieces)
        {
            if (friendlyPiece != king && friendlyPiece != attackingPiece)
            {
                if (friendlyPiece.CanMoveTo(blockPosition.x, blockPosition.y))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Returns a list of all the ChessPieces that are attacking the specified piece
    /// </summary>
    /// <param name="piece">The ChessPiece being attacked</param>
    /// <returns>List of attacking ChessPieces</returns>
    private List<ChessPiece> GetAttackingPieces(ChessPiece piece)
    {
        List<ChessPiece> attackingPieces = new List<ChessPiece>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null)
                {
                    ChessPiece p = board[x, y];

                    if (!(p.Color == piece.Color) && p.CanMoveTo(piece.Position.x, piece.Position.y))
                    {
                        attackingPieces.Add(p);
                    }
                }
            }
        }

        return attackingPieces;
    }

    #endregion

}

/// <summary>
/// A static class that provides a table of scores for each position on the board for the pawn pieces
/// </summary>
public class PawnTable
{
    private readonly int[,] table;

    /// <summary>
    /// A two-dimensional array representing the scores for each position on the board for pawns
    /// </summary>
    public PawnTable()
    {
        table = new int[8, 8] {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            { 5,  5, 10, 25, 25, 10,  5,  5 },
            { 0,  0,  0, 20, 20,  0,  0,  0 },
            { 5, -5,-10,  0,  0,-10, -5,  5 },
            { 5, 10, 10,-20,-20, 10, 10,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0 }
        };
    }

    /// <summary>
    /// Gets the score value of the piece at the given position on the board.
    /// </summary>
    /// <param name="x">The x coordinate of the piece's position.</param>
    /// <param name="y">The y coordinate of the piece's position.</param>
    /// <returns>The score value of the piece.</returns>
    public int GetScore(int x, int y)
    {
        return table[x, y];
    }
}

/// <summary>
/// A static class that provides a table of scores for each position on the board for the piece
/// </summary>
public class KnightTable
{
    private readonly int[,] table;

    /// <summary>
    /// A two-dimensional array representing the scores for each position on the board for the piece
    /// </summary>
    public KnightTable()
    {
        table = new int[8, 8]
        {
            {-50,-40,-30,-30,-30,-30,-40,-50},
            {-40,-20,  0,  5,  5,  0,-20,-40},
            {-30,  5, 10, 15, 15, 10,  5,-30},
            {-30,  0, 15, 20, 20, 15,  0,-30},
            {-30,  5, 15, 20, 20, 15,  5,-30},
            {-30,  0, 10, 15, 15, 10,  0,-30},
            {-40,-20,  0,  0,  0,  0,-20,-40},
            {-50,-40,-30,-30,-30,-30,-40,-50}
        };
    }

    /// <summary>
    /// Gets the score value of the piece at the given position on the board.
    /// </summary>
    /// <param name="x">The x coordinate of the piece's position.</param>
    /// <param name="y">The y coordinate of the piece's position.</param>
    /// <returns>The score value of the piece.</returns>
    public int GetScore(int x, int y)
    {
        return table[x, y];
    }
}


/// <summary>
/// A static class that provides a table of scores for each position on the board for the piece
/// </summary>
public class BishopTable
{
    private readonly int[,] table;

    /// <summary>
    /// A two-dimensional array representing the scores for each position on the board for the piece
    /// </summary>
    public BishopTable()
    {
        table = new int[8, 8]
        {
            {-20,-10,-10,-10,-10,-10,-10,-20},
            {-10,  5,  0,  0,  0,  0,  5,-10},
            {-10, 10, 10, 10, 10, 10, 10,-10},
            {-10,  0, 10, 10, 10, 10,  0,-10},
            {-10,  5,  5, 10, 10,  5,  5,-10},
            {-10,  0,  5, 10, 10,  5,  0,-10},
            {-10,  0,  0,  0,  0,  0,  0,-10},
            {-20,-10,-10,-10,-10,-10,-10,-20}
        };
    }

    /// <summary>
    /// Gets the score value of the piece at the given position on the board.
    /// </summary>
    /// <param name="x">The x coordinate of the piece's position.</param>
    /// <param name="y">The y coordinate of the piece's position.</param>
    /// <returns>The score value of the piece.</returns>
    public int GetScore(int x, int y)
    {
        return table[x, y];
    }
}

/// <summary>
/// A static class that provides a table of scores for each position on the board for the piece
/// </summary>
public class RookTable
{
    private readonly int[,] table;

    /// <summary>
    /// A two-dimensional array representing the scores for each position on the board for the piece
    /// </summary>
    public RookTable()
    {
        table = new int[8, 8]
        {

            {-5, -5, -5, -5, -5, -5, -5, -5},
            {-3,  0,  0,  0,  0,  0,  0, -3},
            {-3,  0,  5,  5,  5,  5,  0, -3},
            {-3,  0,  5, 10, 10,  5,  0, -3},
            {-3,  0,  5, 10, 10,  5,  0, -3},
            {-3,  0,  5,  5,  5,  5,  0, -3},
            {-3,  0,  0,  0,  0,  0,  0, -3},
            {-5, -5, -5, -5, -5, -5, -5, -5}
        };
    }

    /// <summary>
    /// Gets the score value of the piece at the given position on the board.
    /// </summary>
    /// <param name="x">The x coordinate of the piece's position.</param>
    /// <param name="y">The y coordinate of the piece's position.</param>
    /// <returns>The score value of the piece.</returns>
    public int GetScore(int x, int y)
    {
        return table[x, y];
    }
}

/// <summary>
/// A static class that provides a table of scores for each position on the board for the piece
/// </summary>
public class QueenTable
{
    private readonly int[,] table;

    /// <summary>
    /// A two-dimensional array representing the scores for each position on the board for the piece
    /// </summary>
    public QueenTable()
    {
        table = new int[8, 8]
        {
            { -20,-10,-10, -5, -5,-10,-10,-20},
            { -10,  0,  0,  0,  0,  0,  0,-10},
            { -10,  0,  5,  5,  5,  5,  0,-10},
            {  -5,  0,  5,  5,  5,  5,  0, -5},
            {   0,  0,  5,  5,  5,  5,  0, -5},
            { -10,  5,  5,  5,  5,  5,  0,-10},
            { -10,  0,  5,  0,  0,  0,  0,-10},
            { -20,-10,-10, -5, -5,-10,-10,-20}
        };
    }

    /// <summary>
    /// Gets the score value of the piece at the given position on the board.
    /// </summary>
    /// <param name="x">The x coordinate of the piece's position.</param>
    /// <param name="y">The y coordinate of the piece's position.</param>
    /// <returns>The score value of the piece.</returns>
    public int GetScore(int x, int y)
    {
        return table[x, y];
    }
}

/// <summary>
/// A static class that provides a table of scores for each position on the board for the piece
/// </summary>
public class KingTable
{
    private readonly int[,] table;

    /// <summary>
    /// A two-dimensional array representing the scores for each position on the board for the piece
    /// </summary>
    public KingTable()
    {
        table = new int[8, 8]
        {
            { -30,-40,-40,-50,-50,-40,-40,-30},
            { -30,-40,-40,-50,-50,-40,-40,-30},
            { -30,-40,-40,-50,-50,-40,-40,-30},
            { -30,-40,-40,-50,-50,-40,-40,-30},
            { -20,-30,-30,-40,-40,-30,-30,-20},
            { -10,-20,-20,-20,-20,-20,-20,-10},
            {  20, 20,  0,  0,  0,  0, 20, 20},
            {  20, 30, 10,  0,  0, 10, 30, 20}
        };
    }

    /// <summary>
    /// Gets the score value of the piece at the given position on the board.
    /// </summary>
    /// <param name="x">The x coordinate of the piece's position.</param>
    /// <param name="y">The y coordinate of the piece's position.</param>
    /// <returns>The score value of the piece.</returns>
    public int GetScore(int x, int y)
    {
        return table[x, y];
    }
}


