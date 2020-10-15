using UnityEngine;

public class EnemyDropLever : Lever, IUseable, ISaveable
{
    [SerializeField] Rigidbody2D enemyRB;
    private bool enemyDropped = false;

    private void Start()
    {
        enemyRB.gravityScale = 0f;
    }

    public override void Use()
    {
        base.Use();
        if (!enemyDropped)
        {
            enemyRB.gravityScale = 1f;
            enemyDropped = true;
        }        
    }

    public override object CaptureState()
    {
        data.enemyDropped = enemyDropped;
        base.CaptureState();
        return data;
    }

    public override void RestoreState(object state)
    {
        LeverSaveData data = (LeverSaveData)state;
        enemyDropped = data.enemyDropped;

        base.RestoreState(state);
    }
}
