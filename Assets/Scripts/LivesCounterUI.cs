using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LivesCounterUI : MonoBehaviour
{
    private Text livesText;

    void Awake()
    {
        livesText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        GameManager.onLifeLost += HandleLifeCount;
        GameOverUI.onReplay += HandleLifeCount;
        GameOverUI.onRetry += HandleLifeCount;
    }

    private void OnDisable()
    {
        GameManager.onLifeLost -= HandleLifeCount;
        GameOverUI.onReplay -= HandleLifeCount;
        GameOverUI.onRetry -= HandleLifeCount;
    }

    private void Start()
    {
        HandleLifeCount();
    }

    // Update Lives display when life lost or retry/replay
    private void HandleLifeCount()
    {
        livesText.text = "LIVES: " + GameManager.RemainingLives;
    }
}
