using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float maxLaserAmount = 3f;
    public float currentLaserAmount
    {
        get { return _currentLaserAmount; }
        set { _currentLaserAmount = Mathf.Clamp(value, 0, maxLaserAmount); }
    }

    public float laserUseSpeed = 5f;
    public event Action<float> onLaserUsed;

    [SerializeField] int damage = 10;
    [SerializeField] LayerMask whatToHit;

    [SerializeField] Transform bulletTrailPrefab;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform hitPrefab;
    [SerializeField] float effectSpawnRate = 10f;

    // Handle camera shaking
    [SerializeField] float camShakeAmount = 0.05f;
    [SerializeField] float camShakeLength = 0.1f;
    //private CameraShake camShake;

    [SerializeField] string weaponShootSound = "DefaultShot";

    private float _currentLaserAmount;

    private Transform firePoint;
    private float timeToSpawnEffect = 2f;

    private AudioManager audioManager;
    private Player player;

    private void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null) 
        {
            Debug.LogError("No firePoint");
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        currentLaserAmount = maxLaserAmount;
    }

    private void Start() 
    {
        //camShake = GameManager.Instance.GetComponent<CameraShake>();
        //if (camShake == null)
        //{
        //    Debug.LogError("No CameraShake script found on GM object");
        //}

        DisableLaser();

        audioManager = AudioManager.Instance;
        if (audioManager == null) 
        {
            Debug.LogError("No AudioManager found!");
        }
    }


    private void Update()
    {
        if (player.InputHandler.ShootInput && currentLaserAmount > Mathf.Epsilon)
        {
            Shoot();
        }
        else if (player.InputHandler.ShootInput && currentLaserAmount <= Mathf.Epsilon)
        {
            DisableLaser();
        }
        else if (!player.InputHandler.ShootInput && currentLaserAmount != maxLaserAmount)
        {
            DisableLaser();
            currentLaserAmount += laserUseSpeed * Time.deltaTime;
            onLaserUsed?.Invoke(currentLaserAmount);
        }
    }

    private void Shoot()
    {
        EnableLaser();

        lineRenderer.SetPosition(0, firePoint.position);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, Vector2.right * player.FacingDirection, 100f, whatToHit);

        if (hit)
        {
            lineRenderer.SetPosition(1, hit.point);
            
            if (Time.time >= timeToSpawnEffect)
            {
                SpawnImpactParticles(hit);

                if (hit.collider.GetComponent<IDamageable>() != null && !hit.collider.GetComponent<Enemy>().isDead)
                {
                    hit.collider.GetComponent<IDamageable>().Damage(damage);
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(1, new Vector2((30f * player.FacingDirection) + firePoint.position.x, firePoint.position.y));
        }

        currentLaserAmount -= laserUseSpeed * Time.deltaTime;
        onLaserUsed?.Invoke(currentLaserAmount);        
    }

    private void DisableLaser()
    {
        lineRenderer.enabled = false;
    }

    private void EnableLaser()
    {
        lineRenderer.enabled = true;
    }

    private void SpawnImpactParticles(RaycastHit2D hit)
    {
        Transform hitParticles = Instantiate(hitPrefab, hit.point, Quaternion.FromToRotation(Vector3.right, hit.normal));
        Destroy(hitParticles.gameObject, 1f);
        timeToSpawnEffect = Time.time + 1 / effectSpawnRate;    
    }
}
