using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float floatStrength = 1f;
    [Range(0, 1)]
    [SerializeField] float floatSpeed = 1f;

    private float originalY;

    void Start()
    {
        originalY = transform.position.y;
    }

    void Update()
    {
        // Move platform up and down with sine function
        transform.position = new Vector2(transform.position.x, originalY + (Mathf.Sin(Time.time * floatSpeed) * floatStrength));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Make player a child of platform for smooth moving with player
        if (collision.collider.tag == "Player")
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.collider.transform.SetParent(null);
        }
    }
}
