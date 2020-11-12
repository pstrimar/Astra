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

    public void StartGame()
    {
        AudioManager.Instance.PlaySound(pressButtonSound);

        StartCoroutine(LoadFirstLevel());        
    }

    private IEnumerator LoadFirstLevel()
    {
        DontDestroyOnLoad(gameObject);

        Fader fader = FindObjectOfType<Fader>();

        yield return fader.FadeOut(fadeOutTime);

        SceneManager.LoadScene(1);

        yield return new WaitForSeconds(fadeWaitTime);

        fader = FindObjectOfType<Fader>();
        fader.FadeIn(fadeInTime);
        AudioManager.Instance.PlaySound("AboveGround");
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
