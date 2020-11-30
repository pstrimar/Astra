using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool horizontal;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LayerMask whatToHit;
    [SerializeField] Transform hitPrefab;
    [SerializeField] float effectSpawnRate = 10f;
    [SerializeField] string laserSparksName = "LaserSparks";
    private Transform firePoint;
    private Vector2 direction;
    private float timeToSpawnEffect = 2f;
    private int damage = 9001;

    private void Awake()
    {
        firePoint = transform.Find("FirePoint");
    }

    void Update()
    {
        // Set start position to firepoint
        lineRenderer.SetPosition(0, firePoint.position);

        if (horizontal)
            direction = Vector2.right;
        else
            direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, 100f, whatToHit);

        if (hit)
        {
            // Set end position to what we hit
            lineRenderer.SetPosition(1, hit.point);

            if (Time.time >= timeToSpawnEffect)
            {
                SpawnImpactParticles(hit);

                // Damage what we hit
                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    hit.collider.GetComponent<IDamageable>().Damage(damage);
                }
            }
        }
        // We hit nothing, so set end position off screen
        else
        {
            if (horizontal)
                lineRenderer.SetPosition(1, new Vector2(30f + firePoint.position.x, firePoint.position.y));
            else
                lineRenderer.SetPosition(1, new Vector2(firePoint.position.x, firePoint.position.y - 30f));
        }
    }

    // Spawn impact particles normal to what we hit
    private void SpawnImpactParticles(RaycastHit2D hit)
    {
        Transform hitParticles = Instantiate(hitPrefab, hit.point, Quaternion.FromToRotation(Vector3.right, hit.normal));
        Destroy(hitParticles.gameObject, 1f);
        timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
    }
}
