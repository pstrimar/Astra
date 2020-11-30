using System.Collections;
using UnityEngine;

public class FixShip : MonoBehaviour, ISaveable
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
        DialogueManager.onDialogue += HandleDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.onDialogue -= HandleDialogue;
    }

    private IEnumerator FixTheShip(GameObject newShip)
    {
        Fader fader = FindObjectOfType<Fader>();

        yield return fader.FadeOut(fadeOutTime);
        oldShip.SetActive(false);

        // Hides "Kyp" on the other side of the ship from the beginning of the game
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

    [System.Serializable]
    struct SpaceshipSaveData
    {
        public bool oldShip;
        public bool newShipMedium;
        public bool newShipLarge;
        public bool smokeParticles;
        public bool alien;
    }

    public object CaptureState()
    {
        SpaceshipSaveData data = new SpaceshipSaveData();
        data.oldShip = oldShip.activeSelf;
        data.newShipMedium = newShipMedium.activeSelf;
        data.newShipLarge = newShipLarge.activeSelf;
        data.smokeParticles = smokeParticles.activeSelf;
        data.alien = alien.activeSelf;

        return data;
    }

    public void RestoreState(object state)
    {
        SpaceshipSaveData data = (SpaceshipSaveData)state;

        oldShip.SetActive(data.oldShip);
        newShipMedium.SetActive(data.newShipMedium);
        newShipLarge.SetActive(data.newShipLarge);
        smokeParticles.SetActive(data.smokeParticles);
        alien.SetActive(data.alien);
    }
}
