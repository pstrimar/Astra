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
        GameManager.onLifeLost += HandleLifeLost;
    }

    private void OnDisable()
    {
        GameManager.onLifeLost -= HandleLifeLost;
    }

    private void Start()
    {
        livesText.text = "LIVES: " + GameManager.RemainingLives;
    }

    // Update Lives display when life lost
    private void HandleLifeLost()
    {
        livesText.text = "LIVES: " + GameManager.RemainingLives;
    }
}
