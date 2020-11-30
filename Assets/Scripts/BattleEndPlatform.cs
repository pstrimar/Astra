using UnityEngine;

public class BattleEndPlatform : MonoBehaviour
{
    [SerializeField] Enemy boss;

    // Hide the platform
    void Start()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<BoxCollider2D>().enabled = false;
    }

    void Update()
    {
        // Reveal the platform out of the pit
        if (boss.IsDead)
        {
            transform.GetComponent<SpriteRenderer>().enabled = true;
            transform.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
