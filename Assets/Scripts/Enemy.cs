using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats 
    {
        [SerializeField] int maxHealth = 100;
        private int _currentHealth;
        public int damage = 40;
        public float walkSpeed = 3f;
        public float runSpeed = 15f;
        public float aggroRange = 15f;
        public float attackRange = 2f;
        public float knockbackStrength = 2f;

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

    //What to chase
    public Transform target;
    public Transform groundCheck;
    public Transform wallCheck;
    [SerializeField] bool searchingForPlayer = false;

    public bool isFlipped = false;
    public float shakeAmount = 0.1f;
    public float shakeLength = 0.1f;

    public string deathSoundName = "Explosion";

    public int facingDirection = 1;

    [Header("Optional: ")]
    [SerializeField] StatusIndicator statusIndicator;    

    private void Awake() 
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

    private void Start()
    {
        GameManager.Instance.onToggleMenu += OnUpgradeMenuToggle;

        target = GameObject.FindWithTag("Player").transform;
    }



    private void OnDisable() 
    {
        GameManager.Instance.onToggleMenu -= OnUpgradeMenuToggle;
    }

    void OnUpgradeMenuToggle(bool active)
    {
        // TODO: disable enemy behaviour
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

    public void Attack()
    {
        Player player = target.GetComponent<Player>();
        Debug.Log(Vector2.Distance(transform.position, target.position));
        float playerWidth = .2f;

        if (player.isActiveAndEnabled && !player.Invincible && Vector2.Distance(transform.position, target.position) - playerWidth <= stats.attackRange)
        {
            Vector2 direction = player.transform.position - transform.position;
            player.AddKnockbackForce(stats.knockbackStrength, direction);
            player.DamagePlayer(stats.damage);
        }
    }

    public void LookAtPlayer()
    {
        if (target == null) return;

        Vector2 flipped = transform.localScale;
        flipped.x *= -1;

        if (transform.position.x > target.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            isFlipped = true;
            facingDirection *= -1;
        }
        else if (transform.position.x < target.position.x && isFlipped)
        {
            transform.localScale = flipped;
            isFlipped = false;
            facingDirection *= -1;
        }
    }

    public void Flip()
    {
        facingDirection *= -1;
        isFlipped = !isFlipped;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
