using UnityEngine;

public class PortalFX : MonoBehaviour
{
    [SerializeField] Transform portalFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Turn player graphics off when entering portal
            collision.transform.Find("Graphics").gameObject.SetActive(false);

            // Create portal effects
            Instantiate(portalFX, this.transform.position, portalFX.rotation);
        }
    }
}
