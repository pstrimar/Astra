using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float fireRate = 0f;
    [SerializeField] int damage = 10;
    [SerializeField] LayerMask whatToHit;

    [SerializeField] Transform bulletTrailPrefab;
    [SerializeField] Transform muzzleFlashPrefab;
    [SerializeField] Transform hitPrefab;
    [SerializeField] float effectSpawnRate = 10f;

    // Handle camera shaking
    [SerializeField] float camShakeAmount = 0.05f;
    [SerializeField] float camShakeLength = 0.1f;
    //private CameraShake camShake;

    [SerializeField] string weaponShootSound = "DefaultShot";

    private float timeToFire = 0f;
    private float timeToSpawnEffect = 0f;

    private Transform firePoint;

    private AudioManager audioManager;
    private Player player;
    
    void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null) 
        {
            Debug.LogError("No firePoint");
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Start() 
    {
        //camShake = GameManager.Instance.GetComponent<CameraShake>();
        //if (camShake == null)
        //{
        //    Debug.LogError("No CameraShake script found on GM object");
        //}

        audioManager = AudioManager.Instance;
        if (audioManager == null) 
        {
            Debug.LogError("No AudioManager found!");
        }
    }

    
    void Update()
    {
        if (fireRate == 0 && player.InputHandler.ShootInput)
        {
            Shoot();
        }
        else if (player.InputHandler.ShootInput && Time.time > timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, Vector2.right * player.FacingDirection, 100f, whatToHit);

        if (hit.collider.GetComponent<IDamageable>() != null) 
        {
            hit.collider.GetComponent<IDamageable>().Damage(damage);
        }

        if (Time.time >= timeToSpawnEffect) 
        {
            Vector3 hitPos;
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPos = firePoint.position * 30f;
                hitNormal = new Vector3(9999, 9999, 9999);
            }
            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }

            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1/effectSpawnRate;
        }
    }

    void Effect(Vector3 hitPos, Vector3 hitNormal)
    {
        Transform trail = Instantiate(bulletTrailPrefab, firePoint.position, firePoint.rotation);
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if (lr != null)
        {
            // Set positions
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }
        Destroy(trail.gameObject, 0.03f);

        if (hitNormal != new Vector3(9999, 9999, 9999))
        {
            Transform hitParticles = Instantiate(hitPrefab, hitPos, Quaternion.FromToRotation(Vector3.right, hitNormal));
            Destroy(hitParticles.gameObject, 1f);
        }

        Transform clone = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
        clone.parent = firePoint;
        float size = UnityEngine.Random.Range(0.6f, 0.9f);
        clone.localScale = new Vector3(size, size, 1f);
        Destroy(clone.gameObject, 0.02f);

        // Shake the camera
        //camShake.Shake(camShakeAmount, camShakeLength);

        // Play shoot sound
        audioManager.PlaySound(weaponShootSound);
    }
}
