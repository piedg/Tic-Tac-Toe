using UnityEngine;


public class GameBoard : MonoBehaviour
{
    [SerializeField] BoardCell BoardCell;
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
                BoardCell boardCell = Instantiate(BoardCell, transform);
                boardCell.transform.position = new Vector3(x + offsetX, y - offsetY);
                boardCell.name = "x: " + x + " y: " + y;
                board[x, y] = boardCell;
                boardCell.Unsign();
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GameOver) return;

        if (WinnerSign() == GameManager.Instance.Player)
        {
            Debug.Log("Player Won");
            GameManager.Instance.GameOver = true;
            return;
        }
        else if (WinnerSign() == GameManager.Instance.AI)
        {
            Debug.Log("AI Won");
            GameManager.Instance.GameOver = true;
            return;
        }
        else if (IsBoardFull())
        {
            Debug.Log("Tie");
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
                if (hit.collider.TryGetComponent(out BoardCell boardCell))
                {
                    if (boardCell.IsAvailable)
                    {
                        boardCell.SetSign(GameManager.Instance.Player);
                        GameManager.Instance.IsAITurn = true;
                    }
                }
            }
        }

        if (GameManager.Instance.IsAITurn && !IsBoardFull())
        {
            BestMove();
        }
    }

    void BestMove()
    {
        int bestScore = int.MinValue;
        var move = (x: 0, y: 0);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j].IsAvailable)
                {
                    board[i, j].SetSign(GameManager.Instance.AI);
                    int score = MiniMax(board, 0, false);
                    board[i, j].Unsign();
                    if (score > bestScore)
                    {
                        bestScore = score;
                        move = (x: i, y: j);
                    }
                }
            }
        }
        board[move.x, move.y].SetSign(GameManager.Instance.AI);
        GameManager.Instance.IsAITurn = false;
    }


    int MiniMax(BoardCell[,] board, int depth, bool isMax)
    {
        eSign result = WinnerSign();

        if (result == GameManager.Instance.Player)
        {
            return -10;
        }
        else if (result == GameManager.Instance.AI)
        {
            return 10;
        }
        else if (IsBoardFull())
        {
            return 0;
        }

        if (isMax)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j].IsAvailable)
                    {
                        board[i, j].SetSign(GameManager.Instance.AI);
                        int score = MiniMax(board, depth + 1, false);
                        board[i, j].Unsign();
                        bestScore = Mathf.Max(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j].IsAvailable)
                    {
                        board[i, j].SetSign(GameManager.Instance.Player);
                        int score = MiniMax(board, depth + 1, true);
                        board[i, j].Unsign();
                        bestScore = Mathf.Min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }

    eSign WinnerSign()
    {
        eSign winnerSign = eSign.Empty;

        // Controlla le righe
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0].CurrentSign == board[i, 1].CurrentSign &&
                board[i, 1].CurrentSign == board[i, 2].CurrentSign &&
                board[i, 0].CurrentSign != eSign.Empty)
            {
                winnerSign = board[i, 0].CurrentSign;
            }
        }

        // Controlla le colonne
        for (int i = 0; i < 3; i++)
        {
            if (board[0, i].CurrentSign == board[1, i].CurrentSign &&
                board[1, i].CurrentSign == board[2, i].CurrentSign &&
                board[0, i].CurrentSign != eSign.Empty)
            {
                winnerSign = board[0, i].CurrentSign;
            }
        }

        // Controlla le diagonali
        if (board[0, 0].CurrentSign == board[1, 1].CurrentSign &&
            board[1, 1].CurrentSign == board[2, 2].CurrentSign &&
            board[0, 0].CurrentSign != eSign.Empty)
        {
            winnerSign = board[0, 0].CurrentSign;
        }

        if (board[0, 2].CurrentSign == board[1, 1].CurrentSign &&
            board[1, 1].CurrentSign == board[2, 0].CurrentSign &&
            board[0, 2].CurrentSign != eSign.Empty)
        {
            winnerSign = board[0, 2].CurrentSign;
        }

        return winnerSign;
    }

    bool IsBoardFull()
    {
        foreach (BoardCell cell in board)
            if (cell.CurrentSign == eSign.Empty)
                return false;
        return true;
    }
}