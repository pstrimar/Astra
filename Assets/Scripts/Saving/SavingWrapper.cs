using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingWrapper : MonoBehaviour
{
    const string defaultSaveFile = "save";
    [SerializeField] float fadeInTime = .2f;

    private void Awake()
    {
        StartCoroutine(LoadLastScene());
    }

    IEnumerator LoadLastScene()
    {
        yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
        Fader fader = FindObjectOfType<Fader>();
        fader.FadeOutImmediate();
        yield return fader.FadeIn(fadeInTime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadFromButton();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Delete();
        }
    }

    public void Load()
    {
        GetComponent<SavingSystem>().Load(defaultSaveFile);
    }

    public void LoadFromButton()
    {
        StartCoroutine(LoadLastScene());
    }

    public void Save()
    {
        GetComponent<SavingSystem>().Save(defaultSaveFile);
    }

    public void Delete()
    {
        GetComponent<SavingSystem>().Delete(defaultSaveFile);
    }
}
