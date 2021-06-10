using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverButton : MonoBehaviour
{
    public void RestartGame()
    {
        GameObject.Find("Chessboard").GetComponent<ChessboardManager>().RestartGame();
        GameObject.Find("GameOverPanel").SetActive(false);
    }
}
