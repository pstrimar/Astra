using System.Collections;
using UnityEngine;

public class SavingWrapper : MonoBehaviour
{
    const string defaultSaveFile = "autoSave";
    const string playerSaveFile = "playerSave";
    [SerializeField] float fadeInTime = .2f;

    private void Awake()
    {
        StartCoroutine(LoadLastScene());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Delete();
        }
    }

    public void AutoLoad()
    {
        SavingSystem.Instance.Load(defaultSaveFile);
    }

    public void PlayerLoad()
    {
        StartCoroutine(LoadLastScene());
    }

    public void AutoSave()
    {
        SavingSystem.Instance.Save(defaultSaveFile);
    }

    public void PlayerSave()
    {
        SavingSystem.Instance.playerSave(playerSaveFile);
    }

    public void Delete()
    {
        SavingSystem.Instance.Delete(defaultSaveFile);
        SavingSystem.Instance.Delete(playerSaveFile);
    }

    IEnumerator LoadLastScene()
    {
        yield return SavingSystem.Instance.LoadLastScene(playerSaveFile);
        Fader fader = FindObjectOfType<Fader>();
        fader.FadeOutImmediate();
        yield return fader.FadeIn(fadeInTime);
    }
}
