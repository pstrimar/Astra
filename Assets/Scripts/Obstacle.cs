using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] int damage = 20;
    [SerializeField] float knockbackStrength = 2f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponentInParent<IDamageable>() != null)
        {
            Player player = other.collider.GetComponent<Player>();
            if (player != null && player.isActiveAndEnabled && !player.Invincible)
            {
                Vector2 direction = other.transform.position - transform.position;
                player.AddKnockbackForce(knockbackStrength, direction);                
            }

            other.collider.GetComponentInParent<IDamageable>().Damage(damage);
        }        
    }
}
