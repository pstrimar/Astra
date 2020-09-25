using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    enum DestinationIdentifier
    {
        A, B, C, D, E
    }

    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destination;
    [SerializeField] float fadeOutTime = 2f;
    [SerializeField] float fadeInTime = 1f;
    [SerializeField] float fadeWaitTime = 0.5f;

    public event Action<int> onSceneLoaded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {            
            StartCoroutine(Transition());
        }
    }

    private IEnumerator Transition()
    {
        if (sceneToLoad < 0)
        {
            Debug.LogError("Scene to load not set.");
            yield break;
        }

        DontDestroyOnLoad(gameObject);

        Fader fader = FindObjectOfType<Fader>();
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();

        PlayerOld player = GameObject.FindWithTag("Player").GetComponent<PlayerOld>();
        PlayerController playerController = player.GetComponent<PlayerController>();
        player.enabled = false;
        player.GetComponent<Rigidbody2D>().isKinematic = true;
        playerController.enabled = false;

        yield return fader.FadeOut(fadeOutTime);

        wrapper.AutoSave();

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        newPlayerController.enabled = false;

        wrapper.AutoLoad();

        Portal otherPortal = GetOtherPortal();
        UpdatePlayer(otherPortal);

        wrapper.AutoSave();

        yield return new WaitForSeconds(fadeWaitTime);

        onSceneLoaded?.Invoke(sceneToLoad);

        fader.FadeIn(fadeInTime);

        newPlayerController.enabled = true;
        Destroy(gameObject);
    }

    private void UpdatePlayer(Portal otherPortal)
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = otherPortal.spawnPoint.position;
    }

    private Portal GetOtherPortal()
    {
        foreach (Portal portal in FindObjectsOfType<Portal>())
        {
            if (portal == this) continue;

            if (portal.destination != this.destination) continue;

            return portal;
        }

        return null;
    }
}
