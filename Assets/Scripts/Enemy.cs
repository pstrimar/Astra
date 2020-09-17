using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats 
    {
        public int maxHealth = 100;
        private int _currentHealth;
        public int damage = 40;

        public int currentHealth
        {
            get {return _currentHealth;}
            set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init() 
        {
            currentHealth = maxHealth;
        }
    }

    public EnemyStats stats = new EnemyStats();

    public Transform deathParticles;
    public float shakeAmount = 0.1f;
    public float shakeLength = 0.1f;

    public string deathSoundName = "Explosion";

    public int moneyDrop = 10;

    [Header("Optional: ")]
    [SerializeField] StatusIndicator statusIndicator;    

    private void Start() 
    {
        stats.Init();
        
        if (statusIndicator != null)
        {
            //statusIndicator.UpdateHealth(stats.currentHealth, stats.maxHealth);
        }        

        if (deathParticles == null)
        {
            Debug.LogError("No deathParticles referenced on Enemy.");
        }
    }

    private void OnEnable() 
    {
        GameManager.Instance.onToggleMenu += OnUpgradeMenuToggle;
    }

    private void OnDisable() 
    {
        GameManager.Instance.onToggleMenu -= OnUpgradeMenuToggle;
    }

    void OnUpgradeMenuToggle(bool active)
    {
        GetComponent<EnemyAI>().enabled = !active;        
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void DamageEnemy(int damage) 
    {
        stats.currentHealth -= damage;
        if (stats.currentHealth <= 0) 
        {
            GameManager.KillEnemy(this);
        }
        if (statusIndicator != null)
        {
            //statusIndicator.UpdateHealth(stats.currentHealth, stats.maxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        Player player = other.collider.GetComponent<Player>();
        if (player != null) 
        {
            player.DamagePlayer(stats.damage);
            DamageEnemy(999999);
        }
    }
}
