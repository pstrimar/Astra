using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LayerMask whatToHit;
    [SerializeField] Transform hitPrefab;
    [SerializeField] float effectSpawnRate = 10f;
    private Transform firePoint;
    private float timeToSpawnEffect = 2f;
    private int damage = 9001;

    private void Awake()
    {
        firePoint = transform.Find("FirePoint");
    }

    void Update()
    {
        lineRenderer.SetPosition(0, firePoint.position);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, Vector2.right, 100f, whatToHit);

        if (hit)
        {
            lineRenderer.SetPosition(1, hit.point);

            if (Time.time >= timeToSpawnEffect)
            {
                SpawnImpactParticles(hit);

                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    hit.collider.GetComponent<IDamageable>().Damage(damage);
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(1, new Vector2(30f + firePoint.position.x, firePoint.position.y));
        }
    }

    private void SpawnImpactParticles(RaycastHit2D hit)
    {
        Transform hitParticles = Instantiate(hitPrefab, hit.point, Quaternion.FromToRotation(Vector3.right, hit.normal));
        Destroy(hitParticles.gameObject, 1f);
        timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
    }
}
