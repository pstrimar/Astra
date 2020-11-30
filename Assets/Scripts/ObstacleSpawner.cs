using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public bool spawnUp = true;
    public bool rotate;
    [SerializeField] Rigidbody2D obstaclePrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform impactParticles;
    [SerializeField] float startTime;
    [SerializeField] float repeatRate = 3f;
    [SerializeField] float speed = 12;
    [SerializeField] bool hideBeforeDestroy = true;    
    [SerializeField] string impactSound = "Splash";

    void Start()
    {
        if (repeatRate > 0)
            InvokeRepeating("SpawnObstacle", startTime, repeatRate);
    }

    public void SpawnObstacle()
    {
        Rigidbody2D obstacle = Instantiate(obstaclePrefab, spawnPoint.position, obstaclePrefab.transform.rotation);
        // Freeze rotation
        if (!rotate)
            obstacle.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else
            obstacle.AddTorque(Random.Range(-20f, 20f));

        // Add force in up direction
        if (spawnUp)
            obstacle.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        // Add force in down direction
        else
            obstacle.AddForce(Vector2.down * speed, ForceMode2D.Impulse);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Ground"));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            // Splash particles for rocks falling in river
            if (impactParticles != null)
            {
                Transform particles = Instantiate(impactParticles, 
                    new Vector2(collision.transform.position.x, collision.transform.position.y + .5f), Quaternion.identity);
                                              
                Destroy(particles.gameObject, 2f);
            }

            // Splash sounds for rocks falling in river
            if (collision.gameObject.GetComponent<AudioSource>() != null)
            {
                collision.gameObject.GetComponent<AudioSource>().Play();
            }
            if (hideBeforeDestroy)
                collision.GetComponent<SpriteRenderer>().enabled = false;

            Destroy(collision.gameObject, 2f);
        }
    }
}
