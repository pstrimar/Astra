using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayerStats")]
public class PlayerStats : SingletonScriptableObject<PlayerStats>
{
    public int maxHealth = 100;

    public float maxFuelAmount = 1f;

    public int crystals;

    private int _currentHealth;

    public int currentHealth 
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    private float _curentFuelAmount;

    public float currentFuelAmount
    {
        get { return _curentFuelAmount; }
        set { _curentFuelAmount = Mathf.Clamp(value, 0, maxFuelAmount); }
    }

    public bool isDead
    {
        get { return currentHealth == 0; }
    }

    public float healthRegenRate = 2f;

    public float movementSpeed = 10f;

    public float climbSpeed = 6f;

    public float thrusterForce = 400f;
    public float thrusterFuelBurnSpeed = 1f;
    public float thrusterFuelRegenSpeed = 0.3f;    
}
