using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalFX : MonoBehaviour
{
    [SerializeField] Transform portalFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().FindChildObject("Graphics").SetActive(false);

            Instantiate(portalFX, this.transform.position, portalFX.rotation);
        }
    }
}
