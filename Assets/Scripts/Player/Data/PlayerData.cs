using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : SingletonScriptableObject<PlayerData>
{
    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Thrust State")]
    public float thrusterForce = 400f;
    public float thrusterFuelBurnSpeed = 1f;
    public float thrusterFuelRegenSpeed = 0.3f;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.3f;
    public LayerMask whatIsGround;
    public float currentFuelAmount
    {
        get { return _curentFuelAmount; }
        set { _curentFuelAmount = Mathf.Clamp(value, 0, maxFuelAmount); }
    }

    private float _curentFuelAmount;

    public int maxHealth = 100;

    public float maxFuelAmount = 1f;

    public int crystals;

    private int _currentHealth;

    public int currentHealth 
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public bool isDead
    {
        get { return currentHealth == 0; }
    }

    public float healthRegenRate = 2f;

    

    public float climbSpeed = 6f;

     
}
