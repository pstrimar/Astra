using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float MaxLaserAmount = 3f;
    public float CurrentLaserAmount
    {
        get { return _currentLaserAmount; }
        set { _currentLaserAmount = Mathf.Clamp(value, 0, MaxLaserAmount); }
    }

    public float LaserUseSpeed = 5f;
    public static event Action<float> onLaserUsed;

    [SerializeField] Player player;
    [SerializeField] int damage = 10;
    [SerializeField] LayerMask whatToHit;

    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform hitPrefab;
    [SerializeField] float effectSpawnRate = 10f;

    [SerializeField] string weaponShootSound = "LaserBeamSound";
    [SerializeField] string weaponRechargeSound = "LaserRecharge";

    private float _currentLaserAmount;

    private Transform firePoint;
    private float timeToSpawnEffect = 2f;
    [SerializeField] float maxLaserLength = 15f;

    private AudioManager audioManager;    

    private void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null) 
        {
            Debug.LogError("No firePoint");
        }

        CurrentLaserAmount = MaxLaserAmount;
    }

    private void Start() 
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null) 
        {
            Debug.LogError("No AudioManager found!");
        }
    }


    private void Update()
    {
        if (player.InputHandler.ShootInput && CurrentLaserAmount > Mathf.Epsilon)
        {
            Shoot();
        }
        else if (!player.InputHandler.ShootInput && CurrentLaserAmount != MaxLaserAmount)
        {
            audioManager.StopSound(weaponShootSound);
            audioManager.PlaySoundOnce(weaponRechargeSound);
            CurrentLaserAmount += LaserUseSpeed * Time.deltaTime;
            onLaserUsed?.Invoke(CurrentLaserAmount);
        }
    }

    private void Shoot()
    {
        audioManager.PlaySoundOnce(weaponShootSound);

        // Instantiate laser beam at firepoint position
        LineRenderer laserBeam = Instantiate(lineRenderer, firePoint.position, firePoint.rotation);
        laserBeam.SetPosition(0, firePoint.position);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, Vector2.right * player.FacingDirection, 100f, whatToHit);

        // Set endpoint to object we hit
        if (hit && hit.distance < maxLaserLength)
        {
            laserBeam.SetPosition(1, hit.point);
            
            if (Time.time >= timeToSpawnEffect)
            {
                SpawnImpactParticles(hit);

                // If we hit an enemy that is alive, we call damage on it
                if (hit.collider.GetComponentInParent<IDamageable>() != null && !hit.collider.GetComponentInParent<Enemy>().IsDead)
                {
                    hit.collider.GetComponentInParent<IDamageable>().Damage(damage);
                }
            }
        }
        // Set endpoint off screen
        else
        {
            laserBeam.SetPosition(1, new Vector2((maxLaserLength * player.FacingDirection) + firePoint.position.x, firePoint.position.y));
        }

        // Reduce laser amount left
        CurrentLaserAmount -= LaserUseSpeed * Time.deltaTime;

        // Broadcast laser amount left
        onLaserUsed?.Invoke(CurrentLaserAmount);

        // Destroy the line renderer
        Destroy(laserBeam.gameObject, .02f);
    }

    // Spawn impact particles normal to impact
    private void SpawnImpactParticles(RaycastHit2D hit)
    {
        Transform hitParticles = Instantiate(hitPrefab, hit.point, Quaternion.FromToRotation(Vector3.right, hit.normal));
        Destroy(hitParticles.gameObject, 1f);
        timeToSpawnEffect = Time.time + 1 / effectSpawnRate;    
    }
}
