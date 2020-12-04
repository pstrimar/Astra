using UnityEngine;

public class UIOverlay : MonoBehaviour
{
    private GameObject player;
    private CanvasGroup canvasGroup;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        // Hide UI Overlay if player is disabled, but not if we won the game
        if (!player.activeSelf && GameManager.RemainingLives > 0 && !GameManager.Instance.playerWonGame)
            canvasGroup.alpha = 0;
        else
            canvasGroup.alpha = 1;
    }
}
