using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPickup : MonoBehaviour, ISaveable
{
    [SerializeField] int crystalAmount = 1;
    private bool pickedUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !pickedUp)
        {
            pickedUp = true;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GameManager.Crystals += crystalAmount;
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
