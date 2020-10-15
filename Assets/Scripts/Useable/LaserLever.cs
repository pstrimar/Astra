using System.Collections;
using UnityEngine;

public class LaserLever : Lever, IUseable, ISaveable
{
    private Laser laser;
    private LineRenderer lineRenderer;

    public override void Awake()
    {
        base.Awake();
        laser = GameObject.FindGameObjectWithTag("LaserToBeTurnedOff").GetComponent<Laser>();
        lineRenderer = laser.GetComponent<LineRenderer>();
    }

    public override void Use()
    {
        base.Use();

        StartCoroutine(TemporarilyDisableLaser());
    }

    private IEnumerator TemporarilyDisableLaser()
    {
        laser.enabled = false;
        lineRenderer.enabled = false;
        yield return new WaitForSeconds(3f);
        laser.enabled = true;
        lineRenderer.enabled = true;
    }
}
