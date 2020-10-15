using UnityEngine;

public class Lever : MonoBehaviour, IUseable, ISaveable
{
    protected Player player;
    protected Animator anim;
    protected bool leverPosition = true;
    protected LeverSaveData data;

    public virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        anim = GetComponent<Animator>();

        data = new LeverSaveData();
    }

    public virtual void Use()
    {
        player.IsUsingLever = true;
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
