using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour, ISaveable
{
    private int fallBoundary = -20;

    [SerializeField] string deathSoundName = "DeathVoice";
    [SerializeField] string damageSoundName = "Grunt";
    public Transform deathParticles;

    public bool Invincible;

    private AudioManager audioManager;

    [SerializeField] StatusIndicator statusIndicator;

    [Header("Static Data")]
    public PlayerStats stats;
    public event Action<int> onHealthChanged;

    private void Awake()
    {
        stats = PlayerStats.Instance;
        stats.currentHealth = stats.maxHealth;
    }

    private void Start() 
    {
        if (statusIndicator == null)
        {
            Debug.LogError("No StatusIndicator referenced on Player");
        }
        else 
        {
            statusIndicator.SetMaxHealth(stats.maxHealth);
            statusIndicator.SetMaxFuel(stats.thrusterFuelAmount);
        }        

        audioManager = AudioManager.Instance;
        if (audioManager == null) 
        {
            Debug.LogError("No AudioManager found!");
        }

        InvokeRepeating("RegenHealth", 1f/stats.healthRegenRate, 1f/stats.healthRegenRate);
    }

    void Update() 
    {
        if (transform.position.y <= fallBoundary) 
        {
            DamagePlayer(999999);
        }
    }

    private void OnEnable() 
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onToggleMenu += HandleMenuToggle;
        }
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.onDialogue += HandleDialogue;
        }
        if (FindObjectOfType<Obstacle>() !=null)
        {
            foreach (Obstacle obstacle in FindObjectsOfType<Obstacle>())
            {
                obstacle.onHitPlayer += SetInvincibility;
            }
        }
    }    

    private void OnDisable() 
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onToggleMenu -= HandleMenuToggle;
        }
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.onDialogue -= HandleDialogue;
        }
        if (FindObjectOfType<Obstacle>() != null)
        {
            foreach (Obstacle obstacle in FindObjectsOfType<Obstacle>())
            {
                obstacle.onHitPlayer -= SetInvincibility;
            }
        }
    }

    void RegenHealth() 
    {
        stats.currentHealth += 1;
        if (onHealthChanged != null)
        {
            onHealthChanged(stats.currentHealth);
        }
    }

    void HandleMenuToggle(bool active)
    {
        GetComponent<PlayerController>().enabled = !active;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Weapon _weapon = GetComponentInChildren<Weapon>();

        if (_weapon != null)
        {
            _weapon.enabled = !active;
        }
    }

    private void HandleDialogue(bool enabled)
    {
        // Disables the PlayerController during dialogue and re-enables once complete
        GetComponent<PlayerController>().enabled = enabled;
    }

    private void SetInvincibility()
    {
        StartCoroutine(BecomeInvincible());
    }

    private IEnumerator BecomeInvincible()
    {
        Invincible = true;
        yield return new WaitForSeconds(2);
        Invincible = false;
    }

    public void DamagePlayer(int damage) 
    {
        stats.currentHealth -= damage;        

        if (stats.currentHealth <= 0) 
        {
            // Play death sound
        audioManager.PlaySound(deathSoundName);

        GameManager.KillPlayer(this);
        }
        else 
        {
            // Play damage sound
            audioManager.PlaySound(damageSoundName);
        }

        if (onHealthChanged != null)
        {
            onHealthChanged(stats.currentHealth);
        }
    }

    [System.Serializable]
    struct PlayerData
    {
        public int currentHealth;
        public int maxHealth;
        public float fuel;
        public float[] position;
    }

    public object CaptureState()
    {
        PlayerData data = new PlayerData();

        data.currentHealth = stats.currentHealth;
        data.maxHealth = stats.maxHealth;
        data.fuel = stats.thrusterFuelAmount;
        data.position = new float[2];
        data.position[0] = transform.position.x;
        data.position[1] = transform.position.y;

        return data;
    }

    public void RestoreState(object state)
    {
        PlayerData data = (PlayerData)state;

        stats.currentHealth = data.currentHealth;
        stats.maxHealth = data.maxHealth;
        stats.thrusterFuelAmount = data.fuel;
        transform.position = new Vector2(data.position[0], data.position[1]);        
    }
}
