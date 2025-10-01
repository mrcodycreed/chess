using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A clickable Piece 
/// </summary>
public class Piece : MonoBehaviour
{
    [SerializeField] private ChessPiece pieceParent;

    /// <summary>
    /// Click logic
    /// </summary>
    void OnMouseDown()
    {
        if (pieceParent == null)
            transform.parent.GetComponent<ChessPiece>().Clicked();
        else
            pieceParent.Clicked();
    }
}
