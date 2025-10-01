using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a square on the chess board
/// </summary>
public class Square : MonoBehaviour
{

    /// <summary>
    /// Handles click logic
    /// </summary>
    void OnMouseDown()
    {
        if (ChessBoard.Instance.GameFinished) return;

        if (GameManager.Instance.ComputerOpponent)
        {
            if (GameManager.Instance.PlayerTurn && !ChessBoard.Instance.PieceMoving)
            {
                if (GameManager.Instance.SelectedPiece == null) return;

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
        else
        {
            if ( !ChessBoard.Instance.PieceMoving)
            {
                if (GameManager.Instance.SelectedPiece == null) return;

                bool canMoveTo = GameManager.Instance.SelectedPiece.CanMoveTo(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                if (canMoveTo)
                {
                    ChessBoard.Instance.MoveTo(GameManager.Instance.SelectedPiece, Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z), true);
                    GameObject.Find("Targeting Circle").GetComponent<SpriteRenderer>().enabled = true;
                    GameObject.Find("Targeting Circle").transform.position = new Vector3(transform.position.x, -0.49f, transform.position.z);
                    AudioManager.Instance.PlayMove();
                    GameManager.Instance.SelectedPiece = null;
                }
            }
        }
    }
}
