using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public bool spawnUp = true;
    public bool rotate;
    [SerializeField] Rigidbody2D obstaclePrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float startTime;
    [SerializeField] float repeatRate = 3f;
    [SerializeField] float speed = 12;
    [SerializeField] Transform impactParticles;
    [SerializeField] string impactSound = "Splash";

    void Start()
    {
        InvokeRepeating("SpawnObstacle", startTime, repeatRate);
    }

    private void SpawnObstacle()
    {
        Rigidbody2D obstacle = Instantiate(obstaclePrefab, spawnPoint.position, obstaclePrefab.transform.rotation);
        if (!rotate)
            obstacle.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else
            obstacle.AddTorque(Random.Range(-20f, 20f));

        if (spawnUp)
            obstacle.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        else
            obstacle.AddForce(Vector2.down * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            if (impactParticles != null)
            {
                Transform particles = Instantiate(impactParticles, 
                    new Vector2(collision.transform.position.x, collision.transform.position.y + .5f), Quaternion.identity);
                                              
                Destroy(particles.gameObject, 2f);
            }

            if (collision.gameObject.GetComponent<AudioSource>() != null)
            {
                collision.gameObject.GetComponent<AudioSource>().Play();
            }
            collision.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(collision.gameObject, 2f);
        }
    }
}
