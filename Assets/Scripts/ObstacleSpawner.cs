using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] Rigidbody2D obstaclePrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float startTime;
    [SerializeField] float repeatRate = 3f;
    [SerializeField] float speed = 12;

    void Start()
    {
        InvokeRepeating("SpawnObstacle", startTime, repeatRate);
    }

    private void SpawnObstacle()
    {
        Rigidbody2D obstacle = Instantiate(obstaclePrefab, spawnPoint.position, obstaclePrefab.transform.rotation);
        obstacle.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        obstacle.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
            Destroy(collision.gameObject);
    }
}
