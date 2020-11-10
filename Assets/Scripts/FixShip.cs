using System.Collections;
using UnityEngine;

public class FixShip : MonoBehaviour
{
    [SerializeField] GameObject oldShip;
    [SerializeField] GameObject newShipMedium;
    [SerializeField] GameObject newShipLarge;
    [SerializeField] GameObject smokeParticles;
    [SerializeField] GameObject alien;
    private float fadeOutTime = 1f;
    private float fadeInTime = 2f;

    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.onDialogue += HandleDialogue;
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.onDialogue -= HandleDialogue;
        }
    }

    private IEnumerator FixTheShip(GameObject newShip)
    {
        Fader fader = FindObjectOfType<Fader>();

        yield return fader.FadeOut(fadeOutTime);
        oldShip.SetActive(false);
        alien.SetActive(false);
        smokeParticles.SetActive(false);
        newShip.SetActive(true);
        GameManager.Crystals = 0;

        fader.FadeIn(fadeInTime);
    }

    private void HandleDialogue(bool dialogue)
    {
        if (!dialogue && GameManager.Crystals > 4)
        {
            if (GameManager.Crystals <= 18)
            {
                StartCoroutine(FixTheShip(oldShip));
                return;
            }
            else if (GameManager.Crystals <= 36)
            {
                StartCoroutine(FixTheShip(newShipMedium));
                return;
            } 
            else if (GameManager.Crystals <= 55)
            {
                StartCoroutine(FixTheShip(newShipLarge));
            }
        }
    }
}
