using System;
using System.Runtime.Serialization;
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
        public float rollSpeed = 5f;
        public float aggroRange = 15f;
        public float attackRange = 2f;
        public float knockbackStrength = 2f;

        public int currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            currentHealth = maxHealth;
        }
    }

    public EnemyStats stats = new EnemyStats();

    public Transform DeathParticles;
    public bool IsDead;
    public bool Invulnerable;
    public event Action<int> onHealthChanged;
    public static event Action onHit;

    //What to chase
    public Transform Target;
    public Transform GroundCheck;
    public Transform WallCheck;
    public Transform BackCheck;
    public Transform FallCheck;
    public PhysicsMaterial2D FullFriction;
    public CircleCollider2D LeftHitCheck;           // Used for boss only
    public CircleCollider2D RightHitCheck;          // Used for boss only

    public bool IsFlipped = false;

    public string DeathSoundName = "EnemyDeath";
    public string HurtSoundName = "EnemyHurt";
    public string AttackSoundName = "EnemyAttack";
    [OptionalField]
    [SerializeField] string landingSoundName = "LandingFootsteps";

    [OptionalField]
    [SerializeField] Transform landingParticles;
    public int FacingDirection = 1;
    private float hurtTimer = 0f;

    private void Awake()
    {
        stats.Init();

        if (DeathParticles == null)
        {
            Debug.LogError("No deathParticles referenced on Enemy.");
        }
    }

    private void Start()
    {
        Target = GameObject.FindWithTag("Player").transform;
    }

    private void OnEnable()
    {
        GameManager.onToggleMenu += OnUpgradeMenuToggle;
    }

    private void OnDisable()
    {
        GameManager.onToggleMenu -= OnUpgradeMenuToggle;
    }

    // Disable animation while menu is active
    void OnUpgradeMenuToggle(bool active)
    {
        GetComponent<Animator>().enabled = !active;
    }

    public void Damage(int damage)
    {
        if (!Invulnerable)
        {
            stats.currentHealth -= damage;

            // Broadcast hit
            onHit?.Invoke();

            // Show hurt animation every second while being damaged
            if (Time.time - hurtTimer > 1f)
            {
                AudioManager.Instance.PlaySound(HurtSoundName);
                GetComponent<Animator>().SetTrigger("hurt");
            }

            if (stats.currentHealth <= 0 && !IsDead)
            {
                GameManager.KillEnemy(this);
                GetComponent<Animator>().SetBool("dead", true);
            }

            hurtTimer = Time.time;
        }

        // Broadcast health after damage
        onHealthChanged?.Invoke(stats.currentHealth);
    }

    public void Attack()
    {
        AudioManager.Instance.PlaySound(AttackSoundName);
        Player player = Target.GetComponent<Player>();

        // If player is active, not invincible and within our slash radius
        if (player.isActiveAndEnabled && !player.Invincible && GetComponentInChildren<CircleCollider2D>().IsTouching(Target.GetComponent<CapsuleCollider2D>()))
        {
            // Direction to knock player back
            Vector2 direction = player.transform.position - transform.position;
            player.AddKnockbackForce(stats.knockbackStrength, direction);
            player.Damage(stats.damage);
        }
    }

    // For boss only, make an attack with left arm
    public void AttackLeft()
    {
        AudioManager.Instance.PlaySound(AttackSoundName);
        Player player = Target.GetComponent<Player>();

        if (player.isActiveAndEnabled && !player.Invincible && LeftHitCheck.IsTouching(Target.GetComponent<CapsuleCollider2D>()))
        {
            Vector2 direction = player.transform.position - transform.position;
            player.AddKnockbackForce(stats.knockbackStrength, direction);
            player.Damage(stats.damage);
        }
    }

    // For boss only, make an attack with right arm
    public void AttackRight()
    {
        AudioManager.Instance.PlaySound(AttackSoundName);
        Player player = Target.GetComponent<Player>();

        if (player.isActiveAndEnabled && !player.Invincible && RightHitCheck.IsTouching(Target.GetComponent<CapsuleCollider2D>()))
        {
            Vector2 direction = player.transform.position - transform.position;
            player.AddKnockbackForce(stats.knockbackStrength, direction);
            player.Damage(stats.damage);
        }
    }

    public void Land()
    {
        AudioManager.Instance.PlaySound(landingSoundName);
        Transform dustParticles = Instantiate(landingParticles, FallCheck.position, Quaternion.identity);
        Destroy(dustParticles.gameObject, 1f);
    }

    public void Flip()
    {
        FacingDirection *= -1;
        IsFlipped = !IsFlipped;

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
        data.isDead = IsDead;
        data.position = new float[2];
        data.position[0] = transform.position.x;
        data.position[1] = transform.position.y;

        return data;
    }

    public void RestoreState(object state)
    {
        EnemySaveData data = (EnemySaveData)state;
        stats.currentHealth = data.currentHealth;
        IsDead = data.isDead;
        transform.position = new Vector2(data.position[0], data.position[1]);

        if (IsDead)
        {
            GetComponent<Animator>().SetBool("dead", true);
            GetComponent<Rigidbody2D>().sharedMaterial = FullFriction;
        }
    }
}
