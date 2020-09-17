using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] string hoverOverSound = "ButtonHover";
    [SerializeField] string pressButtonSound = "ButtonPress";

    AudioManager audioManager;

    private void Start() 
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null) 
        {
            Debug.LogError("No AudioManager found!");
        }
    }

    public void StartGame()
    {
        audioManager.PlaySound(pressButtonSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() 
    {
        audioManager.PlaySound(pressButtonSound);
        Debug.Log("WE QUIT THE GAME!");
        Application.Quit();
    }

    public void OnMouseOver() 
    {
        audioManager.PlaySound(hoverOverSound);
    }
}
