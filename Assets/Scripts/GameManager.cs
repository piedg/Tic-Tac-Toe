
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameBoard gameBoard;

    public static GameManager Instance;

    private bool isAITurn;
    public bool IsAITurn { get => isAITurn; set => isAITurn = value; }

    bool isGameOver = false;

    [SerializeField]
    float resetGameTime = 1f;
    float resetGameTimer;

    [SerializeField]
    private eSign player;
    public eSign Player { get => player; private set { } }

    [SerializeField]
    private eSign ai;
    public eSign AI { get => ai; private set { } }

    private eSign winner = eSign.Empty;
    public eSign Winner { get => winner; set => winner = value; }
    private int lastAIBestScore;
    public int LastAIBestScore { get => lastAIBestScore; set => lastAIBestScore = value; }
    private int aiMaxDepth;
    public int AiMaxDepth { get => aiMaxDepth; set => aiMaxDepth = value; }

    public event Action OnGameStart;
    public event Action OnGameOver;
    public event Action OnRestartGame;
    public event Action OnAIMove;
    public event Action<int> OnMaxDepthChanged;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gameBoard.OnGameOver += GameOver;
        gameBoard.OnAIMove += GameBoard_OnAIMove;

        resetGameTimer = resetGameTime;
    }

    int prevAiMaxDepth;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }

        if (isGameOver)
        {
            resetGameTimer -= Time.deltaTime;
            if (resetGameTimer <= 0)
            {
                RestartGame();

                OnRestartGame?.Invoke();
            }
        }

        if (prevAiMaxDepth != aiMaxDepth)
        {
            prevAiMaxDepth = aiMaxDepth;
            OnMaxDepthChanged?.Invoke(aiMaxDepth);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        resetGameTimer = resetGameTime;
        isGameOver = false;
        winner = eSign.Empty;
        InvertSigns();
        gameBoard.ResetBoard();
    }

    private void InvertSigns()
    {
        if (player == eSign.X)
        {
            player = eSign.O;
            ai = eSign.X;
        }
        else
        {
            player = eSign.X;
            ai = eSign.O;
        }
    }

    public void GameBoard_OnAIMove(int bestScore)
    {
        LastAIBestScore = bestScore;
        OnAIMove?.Invoke();
    }

    public void SwapTurn()
    {
        isAITurn = !isAITurn;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }
}
