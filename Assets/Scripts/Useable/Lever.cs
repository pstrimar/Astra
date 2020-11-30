using System;
using UnityEngine;

public class Lever : MonoBehaviour, IUseable, ISaveable
{
    public static event Action onUseLever;
    protected Animator anim;
    protected bool leverPosition = true;
    protected LeverSaveData data;

    public virtual void Awake()
    {
        anim = GetComponent<Animator>();

        data = new LeverSaveData();
    }

    // Switches lever position
    public virtual void Use()
    {
        onUseLever?.Invoke();
        anim.SetBool("Switch", leverPosition);
        leverPosition = !leverPosition;
    }

    [System.Serializable]
    protected struct LeverSaveData
    {
        public bool leverPosition;
        public bool enemyDropped;
        public float[] startingPos;
        public float[] endingPos;
        public float[] moveableObjectPosition;
    }

    public virtual object CaptureState()
    {
        data.leverPosition = leverPosition;
        return data;
    }

    public virtual void RestoreState(object state)
    {
        LeverSaveData data = (LeverSaveData)state;
        leverPosition = data.leverPosition;
        GetComponent<Animator>().SetBool("Switch", !leverPosition);
    }
}
