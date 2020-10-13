using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLever : MonoBehaviour, IUseable, ISaveable
{
    private bool leverPosition = true;
    private Laser laser;
    private LineRenderer lineRenderer;
    private Animator anim;
    private Player player;

    private void Awake()
    {
        laser = GameObject.FindGameObjectWithTag("LaserToBeTurnedOff").GetComponent<Laser>();
        lineRenderer = laser.GetComponent<LineRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void Use()
    {
        player.IsUsingLever = true;
        anim.SetBool("Switch", leverPosition);
        leverPosition = !leverPosition;

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

    public object CaptureState()
    {
        return leverPosition;
    }

    public void RestoreState(object state)
    {
        leverPosition = (bool)state;
        GetComponent<Animator>().SetBool("Switch", !leverPosition);
    }
}
