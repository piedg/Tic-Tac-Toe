using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] BoardCellVisual BoardCellVisual;
    public BoardCell[,] board = new BoardCell[3, 3];

    private void Start()
    {
        CreateBoard();
    }

    void CreateBoard()
    {
        float offsetX = -1f;
        float offsetY = 1f;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                BoardCell boardCell = new BoardCell();
                board[x, y] = boardCell;

                BoardCellVisual boardCellVis = Instantiate(BoardCellVisual, transform);
                boardCellVis.transform.position = new Vector3(x + offsetX, y - offsetY);
                boardCellVis.name = "x: " + x + " y: " + y;
                boardCellVis.ClearCell();
                boardCellVis.BoardCell = boardCell;
            }
        }
    }

    private void Update()
    {
        Debug.Log("AI Turn " + GameManager.Instance.IsAITurn);

        if (GameManager.Instance.GameOver) return;

        if (WinnerSign() == GameManager.Instance.Player)
        {
            Debug.Log("Player Won");
            GameManager.Instance.WinText.text = GameManager.Instance.Player + " Won!";
            GameManager.Instance.GameOver = true;
            return;
        }
        else if (WinnerSign() == GameManager.Instance.AI)
        {
            Debug.Log("AI Won");
            GameManager.Instance.WinText.text = GameManager.Instance.AI + " Won!";
            GameManager.Instance.GameOver = true;
            return;
        }
        else if (IsBoardFull())
        {
            Debug.Log("Tie");
            GameManager.Instance.WinText.text = "Tie!";
            GameManager.Instance.GameOver = true;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(mousePosition, Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out BoardCellVisual boardCellVis))
                {
                    if (boardCellVis.BoardCell.IsAvailable)
                    {
                        boardCellVis.BoardCell.SetSign(GameManager.Instance.Player);
                        GameManager.Instance.IsAITurn = true;
                    }
                }
            }
        }

        if (GameManager.Instance.IsAITurn && !IsBoardFull())
        {
            AIMove();
            GameManager.Instance.IsAITurn = false;
        }
    }

    void AIMove()
    {
        int bestScore = int.MaxValue;
        var move = (x: -1, y: -1);

        foreach (var possibleMove in GetNextStates(board, GameManager.Instance.AI))
        {
            int score = MiniMax(possibleMove.Item1, 0, true);
            if (score <= bestScore)
            {
                bestScore = score;
                move = (x: possibleMove.Item2, y: possibleMove.Item3);
            }
        }

        if (move.x != -1 && move.y != -1)
        {
            board[move.x, move.y].SetSign(GameManager.Instance.AI);
        }

        Debug.Log("Best score: " + bestScore);
    }


    int MiniMax(BoardCell[,] board, int depth, bool isMax)
    {
        eSign result = WinnerSign();

        if (result == GameManager.Instance.AI)
        {
            return 10 - depth; // Punteggio maggiore nel minor tempo possibile
        }
        else if (result == GameManager.Instance.Player)
        {
            return depth - 10; // Punteggio minore nel maggior tempo possibile
        }
        else if (IsBoardFull())
        {
            return 0;
        }

        int bestScore = isMax ? int.MinValue : int.MaxValue;
        eSign sign = isMax ? GameManager.Instance.AI : GameManager.Instance.Player;

        foreach (var possibleState in GetNextStates(board, sign))
        {
            bestScore = isMax ?
                Mathf.Max(bestScore, MiniMax(possibleState.Item1, depth + 1, false))
                : Mathf.Min(bestScore, MiniMax(possibleState.Item1, depth + 1, true));
        }

        return bestScore;
    }

    private List<(BoardCell[,], int, int)> GetNextStates(BoardCell[,] previousBoard, eSign sign)
    {
        var result = new List<(BoardCell[,], int, int)>();

        for (var x = 0; x < 3; x++)
        {
            for (var y = 0; y < 3; y++)
            {
                if (!previousBoard[x, y].IsAvailable) continue;

                BoardCell[,] possibleState = (BoardCell[,])previousBoard.Clone();
                possibleState[x, y].SetSign(sign);
                result.Add(new(possibleState, x, y));
            }
        }

        return result;
    }


    eSign WinnerSign()
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0].CurrentSign != eSign.Empty &&
                board[i, 0].CurrentSign == board[i, 1].CurrentSign &&
                board[i, 1].CurrentSign == board[i, 2].CurrentSign)
            {
                return board[i, 0].CurrentSign;
            }
        }

        // Controlla le colonne
        for (int i = 0; i < 3; i++)
        {
            if (board[0, i].CurrentSign != eSign.Empty &&
                board[0, i].CurrentSign == board[1, i].CurrentSign &&
                board[1, i].CurrentSign == board[2, i].CurrentSign)
            {
                return board[0, i].CurrentSign;
            }
        }

        // Controlla le diagonali
        if (board[0, 0].CurrentSign != eSign.Empty &&
            board[0, 0].CurrentSign == board[1, 1].CurrentSign &&
            board[1, 1].CurrentSign == board[2, 2].CurrentSign)
        {
            return board[0, 0].CurrentSign;
        }

        if (board[0, 2].CurrentSign != eSign.Empty &&
            board[0, 2].CurrentSign == board[1, 1].CurrentSign &&
            board[1, 1].CurrentSign == board[2, 0].CurrentSign)
        {
            return board[0, 2].CurrentSign;
        }

        return eSign.Empty;
    }

    bool IsBoardFull()
    {
        foreach (BoardCell cell in board)
            if (cell.CurrentSign == eSign.Empty)
                return false;
        return true;
    }
}