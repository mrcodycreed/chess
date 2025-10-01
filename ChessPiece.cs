using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a chess piece
/// </summary>
public abstract class ChessPiece : MonoBehaviour
{
    public Vector2Int Position { get; set; }
    public Color Color { get; set; }
    public bool HasMoved = false;

    /// <summary>
    /// Determines if the piece can move to the specified position on the chess board.
    /// </summary>
    /// <param name="x">The x-coordinate of the destination position on the board.</param>
    /// <param name="y">The y-coordinate of the destination position on the board.</param>
    /// <returns>True if the piece can move to the specified position, false otherwise.</returns>
    public abstract bool CanMoveTo(int x, int y);

    public string Name
    {
        get
        {
            string colourName = Color == Color.white ? "White" : "Black";
            return $"{name.Replace("(Clone)", "")}";
        }
    }

    /// <summary>
    /// Returns a list of all the possible moves that can be made by this chess piece.
    /// </summary>
    /// <returns>A list of Vector2Int representing the positions that this chess piece can move to.</returns>
    public virtual List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (CanMoveTo(x, y))
                {
                    possibleMoves.Add(new Vector2Int(x, y));
                }
            }
        }
        return possibleMoves;
    }

    void OnMouseDown()
    {
        Clicked();
    }

    /// <summary>
    /// Handles the click logic
    /// </summary>
    public void Clicked()
    {
        if (ChessBoard.Instance.GameFinished) return;

        if (GameManager.Instance.ComputerOpponent)
        {
            if (GameManager.Instance.PlayerTurn)
            {
                if (Color == GameManager.Instance.PlayerColour)
                {
                    GameObject.Find("Targeting Circle").GetComponent<SpriteRenderer>().enabled = true;
                    GameObject.Find("Targeting Circle").transform.position = new Vector3(transform.position.x, -0.49f, transform.position.z);
                    GameManager.Instance.SelectedPiece = this;
                    AudioManager.Instance.PlaySelect();
                }
                else
                {
                    if (GameManager.Instance.SelectedPiece != null)
                    {
                        bool canMoveTo = GameManager.Instance.SelectedPiece.CanMoveTo(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                        if (canMoveTo)
                        {
                            ChessBoard.Instance.MoveTo(GameManager.Instance.SelectedPiece, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z), true);
                            GameObject.Find("Targeting Circle").GetComponent<SpriteRenderer>().enabled = true;
                            GameObject.Find("Targeting Circle").transform.position = new Vector3(transform.position.x, -0.49f, transform.position.z);
                            AudioManager.Instance.PlayMove();
                        }
                    }
                }
            }
        }
        else
        {
            if ((Color == GameManager.Instance.PlayerColour && GameManager.Instance.PlayerTurn) || (Color != GameManager.Instance.PlayerColour && !GameManager.Instance.PlayerTurn))
            {
                GameObject.Find("Targeting Circle").GetComponent<SpriteRenderer>().enabled = true;
                GameObject.Find("Targeting Circle").transform.position = new Vector3(transform.position.x, -0.49f, transform.position.z);
                GameManager.Instance.SelectedPiece = this;
                AudioManager.Instance.PlaySelect();
            }
            else
            {
                if (GameManager.Instance.SelectedPiece != null)
                {
                    bool canMoveTo = GameManager.Instance.SelectedPiece.CanMoveTo(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                    if (canMoveTo)
                    {
                        ChessBoard.Instance.MoveTo(GameManager.Instance.SelectedPiece, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z), true);
                        GameObject.Find("Targeting Circle").GetComponent<SpriteRenderer>().enabled = true;
                        GameObject.Find("Targeting Circle").transform.position = new Vector3(transform.position.x, -0.49f, transform.position.z);
                        AudioManager.Instance.PlayMove();
                    }
                }
            }
        }
    }
}




