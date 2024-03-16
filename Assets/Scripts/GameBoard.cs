using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


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

        if (!IsBoardFull())
        {
            if (GameManager.Instance.IsAITurn)
            {
                AIMove(GameManager.Instance.AI);
                GameManager.Instance.IsAITurn = false;
            }
            else
            {
                AIMove(GameManager.Instance.Player);
                GameManager.Instance.IsAITurn = true;
            }
        }

    }

    void AIMove(eSign Sign)
    {
        int bestScore = int.MinValue;
        var move = (x: 0, y: 0);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j].IsAvailable)
                {
                    board[i, j].SetSign(Sign);
                    int score = MiniMax(Sign, board, 0, int.MinValue, int.MaxValue, false);
                    board[i, j].Unsign();
                    if (score > bestScore)
                    {
                        bestScore = score;
                        move = (x: i, y: j);
                    }
                }
            }
        }
        Debug.Log("Best score: " + bestScore);
        board[move.x, move.y].SetSign(Sign);
    }


    int MiniMax(eSign Sign, BoardCell[,] board, int depth, int alpha, int beta, bool isMax)
    {
        eSign result = WinnerSign();

        if (Sign == GameManager.Instance.AI)
        {
            if (result == GameManager.Instance.AI)
            {
                return 10 - depth; 
            }
            else if (result == GameManager.Instance.Player)
            {
                return depth - 10;
            }

            else if (IsBoardFull())
            {
                return 0;
            }
        }
        else if (Sign == GameManager.Instance.Player)
        {
            if (result == GameManager.Instance.AI)
            {
                return depth - 10;
            }
            else if (result == GameManager.Instance.Player)
            {
                return 10 - depth; 
            }
            else if (IsBoardFull())
            {
                return 0;
            }
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
                        board[i, j].SetSign(Sign);
                        int score = MiniMax(Sign, board, depth + 1, alpha, beta, false);
                        board[i, j].Unsign();
                        bestScore = Mathf.Max(score, bestScore);

                        alpha = Mathf.Max(alpha, score);
                        if (beta <= alpha)
                        {
                            break;
                        }
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

                        if (Sign == GameManager.Instance.AI)
                        {
                            board[i, j].SetSign(GameManager.Instance.Player);
                        }
                        else if (Sign == GameManager.Instance.Player)
                        {
                            board[i, j].SetSign(GameManager.Instance.AI);
                        }

                        int score = MiniMax(Sign, board, depth + 1, alpha, beta, true);
                        board[i, j].Unsign();
                        bestScore = Mathf.Min(score, bestScore);

                        beta = Mathf.Min(beta, score);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
            }
            return bestScore;
        }
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