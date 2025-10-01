using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles gameplay states
/// </summary>
public class GameManager : MonoBehaviour
{
    #region SINGLETON

    public static GameManager Instance;
    private void Awake() => Instance = this;

    #endregion

    public ChessPiece SelectedPiece;
    public Color PlayerColour = Color.white;
    public bool PlayerTurn = true;
    public bool ComputerOpponent = true;

    /// <summary>
    /// Changes the game turn between player and computer
    /// </summary>
    public void ChangeTurn()
    {
        PlayerTurn = !PlayerTurn;
    }
}
