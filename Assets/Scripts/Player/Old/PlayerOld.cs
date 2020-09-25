using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class PlayerOld : MonoBehaviour, ISaveable
{
    private int fallBoundary = -10;

    [SerializeField] string deathSoundName = "DeathVoice";
    [SerializeField] string damageSoundName = "Grunt";
    public Transform deathParticles;

    public bool Invincible;

    private AudioManager audioManager;

    [SerializeField] StatusIndicator statusIndicator;

    [Header("Static Data")]
    public global::PlayerData stats;
    public event Action<int> onHealthChanged;

    private void Awake()
    {
        stats = global::PlayerData.Instance;
        stats.currentHealth = stats.maxHealth;
        stats.currentFuelAmount = stats.maxFuelAmount;
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
            statusIndicator.SetMaxFuel(stats.maxFuelAmount);
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
    }

    void RegenHealth() 
    {
        stats.currentHealth += 1;
        onHealthChanged?.Invoke(stats.currentHealth);
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

        //GameManager.KillPlayer(this);
        }
        else 
        {
            StartCoroutine(BecomeInvincible());

            // Play damage sound
            audioManager.PlaySound(damageSoundName);
        }

        onHealthChanged?.Invoke(stats.currentHealth);
    }

    public void AddKnockbackForce(float force, Vector2 direction)
    {
        StartCoroutine(DisableMovementAndApplyForce(force, direction));
        GetComponent<Flicker>().startBlinking = true;
    }

    private IEnumerator DisableMovementAndApplyForce(float force, Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        GetComponent<PlayerController>().enabled = false;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
        GetComponent<PlayerController>().enabled = true;
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
        data.fuel = stats.currentFuelAmount;
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
        stats.currentFuelAmount = data.fuel;
        transform.position = new Vector2(data.position[0], data.position[1]);        
    }
}
