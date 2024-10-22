using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI winnerText, aiBestScore;

    [SerializeField]
    TextMeshProUGUI aiMaxDepthText;

    [SerializeField]
    TextMeshProUGUI currentAiMaxDepthText;

    [SerializeField]
    GameObject startPanel;

    [SerializeField]
    Button aiFirstChoice, playerFirstChoice;

    [SerializeField]
    Button decreaseDepthBtn, incrementDepthBtn;

    [SerializeField]
    Button restartSceneBtn;

    private void Start()
    {
        GameManager.Instance.OnGameStart += ShowStartPanel;
        GameManager.Instance.OnGameOver += UpdateWinnerText;
        GameManager.Instance.OnRestartGame += ResetTexts;
        GameManager.Instance.OnAIMove += UpdateLastAIBestScore;

        aiFirstChoice.onClick.AddListener(() => AIFirstChoiceButton());
        playerFirstChoice.onClick.AddListener(() => PlayerFirstChoiceButton());

        decreaseDepthBtn.onClick.AddListener(() => DecreaseAIDepth());
        incrementDepthBtn.onClick.AddListener(() => IncrementAIDepth());

        restartSceneBtn.onClick.AddListener(() => RestartScene());

        ShowStartPanel();
        UpdateAIMaxDepthText();
    }

    private void RestartScene()
    {
        GameManager.Instance.RestartScene();
    }

    public void AIFirstChoiceButton()
    {
        GameManager.Instance.IsAITurn = true;
        HideStartPanel();
    }

    public void PlayerFirstChoiceButton()
    {
        GameManager.Instance.IsAITurn = false;
        HideStartPanel();
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        restartSceneBtn.gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void HideStartPanel()
    {
        startPanel.SetActive(false);
        restartSceneBtn.gameObject.SetActive(true);
        Time.timeScale = 1;
        UpdateCurrentAIMaxDepth();
    }

    private void UpdateWinnerText()
    {
        eSign winner = GameManager.Instance.Winner;

        if (winner == eSign.Empty)
        {
            winnerText.text = "Tie!";
        }
        else
        {
            winnerText.text = winner + " won!";
        }
    }

    private void UpdateLastAIBestScore()
    {
        aiBestScore.text = $"AI Best Score:\n {GameManager.Instance.LastAIBestScore}";
    }

    private void ResetTexts()
    {
        winnerText.text = "";
        aiBestScore.text = "";
    }

    private void UpdateAIMaxDepthText()
    {
        aiMaxDepthText.text = $"{GameManager.Instance.AiMaxDepth}";
    }

    private void IncrementAIDepth()
    {
        GameManager.Instance.AiMaxDepth = Mathf.Clamp(GameManager.Instance.AiMaxDepth + 1, -1, 99);
        UpdateAIMaxDepthText();
    }

    private void DecreaseAIDepth()
    {
        GameManager.Instance.AiMaxDepth = Mathf.Clamp(GameManager.Instance.AiMaxDepth - 1, -1, 99);
        UpdateAIMaxDepthText();
    }

    private void UpdateCurrentAIMaxDepth()
    {
        currentAiMaxDepthText.text = $"Current Max Depth: {GameManager.Instance.AiMaxDepth}";
    }
}
