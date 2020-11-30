using System.Collections;
using UnityEngine;

public class FlyAway : MonoBehaviour
{
    [SerializeField] Transform spaceshipFlyAwayPoint;
    [SerializeField] Cinemachine.CinemachineVirtualCamera cinemachineCam;
    [SerializeField] Transform rocketTrailParticles;
    [SerializeField] GameObject[] firePoints;
    private float fadeOutTime = 1f;
    private float fadeInTime = 2f;
    private bool sceneStarted;

    void Update()
    {
        // Translate ship up after scene starts
        if (sceneStarted)
        {
            transform.Translate(Vector2.up * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.gameObject.SetActive(false);
            StartCoroutine(FlyAwayScene());
            sceneStarted = true;
        }
    }

    private IEnumerator FlyAwayScene()
    {
        Fader fader = FindObjectOfType<Fader>();

        yield return fader.FadeOut(fadeOutTime);

        // Sets the transform to the spaceship in the sky
        this.transform.position = spaceshipFlyAwayPoint.position;

        // Sets the camera to follow this transform
        cinemachineCam.m_Follow = this.transform;

        // Instantiates rocket trail particles at each engine
        foreach (GameObject firePoint in firePoints)
        {
            Instantiate(rocketTrailParticles, firePoint.transform.position, Quaternion.identity).SetParent(transform);
        }

        fader.FadeIn(fadeInTime);
        GameManager.Instance.WinGame();
    }
}
