using System.Collections;
using System.Collections.Generic;
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

    IEnumerator LoadLastScene()
    {
        yield return GetComponent<SavingSystem>().LoadLastScene(playerSaveFile);
        Fader fader = FindObjectOfType<Fader>();
        fader.FadeOutImmediate();
        yield return fader.FadeIn(fadeInTime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerLoad();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerSave();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Delete();
        }
    }

    public void autoLoad()
    {
        GetComponent<SavingSystem>().Load(defaultSaveFile);
    }

    public void playerLoad()
    {
        StartCoroutine(LoadLastScene());
    }

    public void autoSave()
    {
        GetComponent<SavingSystem>().Save(defaultSaveFile);
    }

    public void playerSave()
    {
        GetComponent<SavingSystem>().playerSave(playerSaveFile);
    }

    public void Delete()
    {
        GetComponent<SavingSystem>().Delete(defaultSaveFile);
        GetComponent<SavingSystem>().Delete(playerSaveFile);
    }
}
