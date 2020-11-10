using UnityEngine;

public class BoundaryDeath : MonoBehaviour
{
    [SerializeField] int damage = 9001;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamageable>() != null)
        {
            collision.GetComponent<IDamageable>().Damage(damage);
        }
    }
}
