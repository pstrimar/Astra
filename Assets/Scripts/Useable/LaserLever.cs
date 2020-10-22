using System.Collections;
using UnityEngine;

public class LaserLever : Lever, IUseable, ISaveable
{
    private GameObject laser;

    public override void Awake()
    {
        base.Awake();
        laser = GameObject.FindGameObjectWithTag("LaserToBeTurnedOff");
    }

    public override void Use()
    {
        base.Use();

        StartCoroutine(TemporarilyDisableLaser());
    }

    private IEnumerator TemporarilyDisableLaser()
    {
        laser.SetActive(false);
        yield return new WaitForSeconds(3f);
        laser.SetActive(true);
    }
}
