using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] string mouseHoverSoundName = "ButtonHover";
    [SerializeField] string buttonPressSoundName = "ButtonPress";

    public static event Action onRetry;
    public static event Action onReplay;
    public SavingWrapper savingWrapper;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("No AudioManager found!");
        }
    }

    private void OnEnable()
    {
        GameManager.onGameOver += HandleGameOver;
        GameManager.onWinGame += HandleWinGame;
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= HandleGameOver;
        GameManager.onWinGame -= HandleWinGame;
    }

    public void Quit()
    {
        AudioManager.Instance.PlaySound(buttonPressSoundName);
        savingWrapper.Delete();
        Application.Quit();
    }

    public void Retry()
    {
        AudioManager.Instance.PlaySound(buttonPressSoundName);
        savingWrapper.Delete();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        onRetry?.Invoke();
        HideCanvasGroup();
    }

    public void Replay()
    {
        AudioManager.Instance.PlaySound(buttonPressSoundName);
        AudioManager.Instance.PlaySound("AboveGround");
        savingWrapper.Delete();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        onReplay?.Invoke();
        HideCanvasGroup();
    }

    public void OnMouseOver()
    {
        AudioManager.Instance.PlaySound(mouseHoverSoundName);
    }

    private void HandleGameOver()
    {
        if (gameObject.tag == "GameOverScreen")
        {
            ShowCanvasGroup();
        }
    }

    private void HandleWinGame()
    {
        if (gameObject.tag == "WinScreen")
        {
            ShowCanvasGroup();
        }
    }

    // Show canvasgroup and make interactable
    private void ShowCanvasGroup()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    // Hide canvasgroup and make it not interactable
    private void HideCanvasGroup()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
