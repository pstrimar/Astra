using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] int damage = 20;
    [SerializeField] float knockbackStrength = 2f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        Player player = other.collider.GetComponent<Player>();
        if (player != null && !player.Invincible)
        {
            player.Damage(damage);

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = other.transform.position - transform.position;
                player.AddKnockbackForce(knockbackStrength, direction);
            }            
        }
    }
}
