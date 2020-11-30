using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] string hoverOverSound = "ButtonHover";
    [SerializeField] string pressButtonSound = "ButtonPress";
    [SerializeField] float fadeOutTime = 2f;
    [SerializeField] float fadeInTime = 2f;
    [SerializeField] float fadeWaitTime = 1f;

    // Called when player presses play button
    public void StartGame()
    {
        AudioManager.Instance.PlaySound(pressButtonSound);

        StartCoroutine(LoadFirstLevel());        
    }

    private IEnumerator LoadFirstLevel()
    {
        DontDestroyOnLoad(gameObject);

        Fader fader = FindObjectOfType<Fader>();

        // Fades scene to black over fade out duration
        yield return fader.FadeOut(fadeOutTime);

        // Loads first level
        SceneManager.LoadScene(1);

        // Waits before fading in
        yield return new WaitForSeconds(fadeWaitTime);

        // Finds new fader in scene
        fader = FindObjectOfType<Fader>();

        // Fades in over fade in duration
        fader.FadeIn(fadeInTime);

        // Plays first level music
        AudioManager.Instance.PlaySound("AboveGround");

        // bool to determine if opening cinematic plays
        GameManager.Instance.playerHasCrashed = false;

        Destroy(gameObject);
    }

    public void QuitGame() 
    {
        AudioManager.Instance.PlaySound(pressButtonSound);
        Application.Quit();
    }

    public void OnMouseOver() 
    {
        AudioManager.Instance.PlaySound(hoverOverSound);
    }
}
