using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, ISaveable
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
    public bool isDead;
    public bool invulnerable;

    //What to chase
    public Transform target;
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform backCheck;
    public Transform fallCheck;
    [SerializeField] bool searchingForPlayer = false;
    public PhysicsMaterial2D fullFriction;

    public bool isFlipped = false;
    public float shakeAmount = 0.1f;
    public float shakeLength = 0.1f;

    public string deathSoundName = "EnemyDeath";
    public string hurtSoundName = "EnemyHurt";

    public int facingDirection = 1;
    private float hurtTimer = 0f;

    private void Awake() 
    {
        stats.Init();       

        if (deathParticles == null)
        {
            Debug.LogError("No deathParticles referenced on Enemy.");
        }
    }

    private void Start()
    {
        GameManager.Instance.onToggleMenu += OnUpgradeMenuToggle;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"));

        target = GameObject.FindWithTag("Player").transform;
    }

    private void OnDisable() 
    {
        GameManager.Instance.onToggleMenu -= OnUpgradeMenuToggle;
    }

    void OnUpgradeMenuToggle(bool active)
    {
        GetComponent<Animator>().enabled = !active;
    }

    public void Damage(int damage) 
    {
        if (!invulnerable)
        {
            stats.currentHealth -= damage;

            if (Time.time - hurtTimer > 1f)
            {
                AudioManager.Instance.PlaySound(hurtSoundName);
                GetComponent<Animator>().SetTrigger("hurt");
            }

            if (stats.currentHealth <= 0 && !isDead)
            {
                GameManager.KillEnemy(this);
                GetComponent<Animator>().SetBool("dead", true);
            }

            hurtTimer = Time.time;
        }        
    }

    public void Attack()
    {
        Player player = target.GetComponent<Player>();

        if (player.isActiveAndEnabled && !player.Invincible && GetComponentInChildren<CircleCollider2D>().IsTouching(target.GetComponent<CapsuleCollider2D>()))
        {
            Vector2 direction = player.transform.position - transform.position;
            player.AddKnockbackForce(stats.knockbackStrength, direction);
            player.Damage(stats.damage);
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

    [System.Serializable]
    struct EnemySaveData
    {
        public int currentHealth;
        public bool isDead;
        public float[] position;
    }

    public object CaptureState()
    {
        EnemySaveData data = new EnemySaveData();
        data.currentHealth = stats.currentHealth;
        data.isDead = isDead;
        data.position = new float[2];
        data.position[0] = transform.position.x;
        data.position[1] = transform.position.y;

        return data;
    }

    public void RestoreState(object state)
    {
        EnemySaveData data = (EnemySaveData)state;
        stats.currentHealth = data.currentHealth;
        isDead = data.isDead;
        transform.position = new Vector2(data.position[0], data.position[1]);

        if (isDead)
        {
            GetComponent<Animator>().SetBool("dead", true);
            GetComponent<Rigidbody2D>().sharedMaterial = fullFriction;
        }
    }
}
