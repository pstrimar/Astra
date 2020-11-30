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

    public static event Action<int> onSceneLoaded;

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

        // Disable player
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        PlayerInputHandler playerInputHandler = player.GetComponent<PlayerInputHandler>();
        player.enabled = false;
        player.GetComponent<Rigidbody2D>().isKinematic = true;
        playerInputHandler.enabled = false;

        yield return fader.FadeOut(fadeOutTime);

        wrapper.AutoSave();

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        // Disable player movement of player in loaded scene
        PlayerInputHandler newPlayerInputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
        newPlayerInputHandler.enabled = false;

        wrapper.AutoLoad();

        // Find new portal in loaded scene
        Portal otherPortal = GetOtherPortal();
        
        wrapper.AutoSave();

        // Move player to spawn location in loaded scene
        UpdatePlayer(otherPortal);

        yield return new WaitForSeconds(fadeWaitTime);

        // Broadcast scene loaded
        onSceneLoaded?.Invoke(sceneToLoad);
        
        yield return fader.FadeIn(fadeInTime);

        // Enable player movement and destroy old portal
        newPlayerInputHandler.enabled = true;
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
