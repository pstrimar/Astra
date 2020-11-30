using UnityEngine;

public class SecretDoorLever : Lever, IUseable, ISaveable
{
    [SerializeField] Transform moveableObject;
    [SerializeField] float speed = .5f;
    [SerializeField] float targetDistance = 1f;
    private Vector3 startingPos;
    private Vector3 endingPos;    
    private string secretDoorSound = "SecretDoor";

    public override void Awake()
    {
        base.Awake();

        startingPos = moveableObject.position;
        endingPos = startingPos + Vector3.right * targetDistance;
    }

    private void Update()
    {
        // If lever is switched, move object towards ending position
        if (!leverPosition && endingPos.x - moveableObject.position.x >= 0)
        {
            Vector2 dir = (endingPos - startingPos);
            moveableObject.transform.Translate(dir * speed * Time.deltaTime);
        }
        // If lever is switched, move object towards starting position
        else if (leverPosition && moveableObject.position.x - startingPos.x >= 0)
        {
            Vector2 dir = (startingPos - endingPos);
            moveableObject.transform.Translate(dir * speed * Time.deltaTime);
        }
    }

    public override void Use()
    {
        base.Use();
        AudioManager.Instance.PlaySound(secretDoorSound);
    }

    public override object CaptureState()
    {
        data.startingPos = new float[2];
        data.startingPos[0] = startingPos.x;
        data.startingPos[1] = startingPos.y;

        data.endingPos = new float[2];
        data.endingPos[0] = endingPos.x;
        data.endingPos[1] = endingPos.y;

        data.moveableObjectPosition = new float[2];
        data.moveableObjectPosition[0] = moveableObject.position.x;
        data.moveableObjectPosition[1] = moveableObject.position.y;

        base.CaptureState();
        return data;        
    }
    
    public override void RestoreState(object state)
    {
        LeverSaveData data = (LeverSaveData)state;
        moveableObject.position = new Vector2(data.moveableObjectPosition[0], data.moveableObjectPosition[1]);

        startingPos = new Vector2(data.startingPos[0], data.startingPos[1]);
        endingPos = new Vector2(data.endingPos[0], data.endingPos[1]);
        base.RestoreState(state);
    }
}


