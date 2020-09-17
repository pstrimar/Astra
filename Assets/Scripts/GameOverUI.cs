using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] string mouseHoverSoundName = "ButtonHover";
    [SerializeField] string buttonPressSoundName = "ButtonPress";

    public event Action onRetry;

    private void Start() 
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null) 
        {
            Debug.LogError("No AudioManager found!");
        }
    }

    public void Quit()
    {
        audioManager.PlaySound(buttonPressSoundName);

        Debug.Log("APPLICATION QUIT!");
        Application.Quit();
    }

    public void Retry()
    {
        audioManager.PlaySound(buttonPressSoundName);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (onRetry != null)
        {
            onRetry();
        }
        gameObject.SetActive(false);
    }

    public void OnMouseOver() 
    {
        audioManager.PlaySound(mouseHoverSoundName);
    }
}
