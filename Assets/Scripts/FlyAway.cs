using System.Collections;
using UnityEngine;

public class FlyAway : MonoBehaviour
{
    [SerializeField] Transform spaceshipFlyAwayPoint;
    [SerializeField] Cinemachine.CinemachineVirtualCamera cinemachineCam;
    [SerializeField] Transform rocketTrailParticles;
    private float fadeOutTime = 1f;
    private float fadeInTime = 2f;
    private bool sceneStarted;
    private GameObject[] firePoints;

    void Update()
    {
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

        this.transform.position = spaceshipFlyAwayPoint.position;

        cinemachineCam.m_Follow = this.transform;

        firePoints = GameObject.FindGameObjectsWithTag("FirePoint");

        foreach (GameObject firePoint in firePoints)
        {
            Instantiate(rocketTrailParticles, firePoint.transform.position, Quaternion.identity).SetParent(transform);
        }
        fader.FadeIn(fadeInTime);
        GameManager.Instance.WinGame();
    }
}
