using System;
using UnityEngine;

public class CrystalPickup : MonoBehaviour, ISaveable
{
    public static event Action<int> onCrystalPickedUp;
    [SerializeField] int crystalAmount = 1;
    private bool pickedUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.activeSelf && !pickedUp)
        {
            pickedUp = true;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;

            // Broadcast crystal pickup and amount
            onCrystalPickedUp?.Invoke(crystalAmount);
            AudioManager.Instance.PlaySound("Crystals");
        }
    }

    public object CaptureState()
    {
        return gameObject.GetComponentInChildren<SpriteRenderer>().enabled;
    }

    public void RestoreState(object state)
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = (bool)state;
        gameObject.GetComponent<CircleCollider2D>().enabled = (bool)state;
    }
}
