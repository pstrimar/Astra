using UnityEngine;

public class SecretDoorLever : MonoBehaviour, IUseable, ISaveable
{
    [SerializeField] Transform moveableObject;
    [SerializeField] float speed = .5f;
    private Player player;
    private Vector3 startingPos;
    private Vector3 endingPos;
    private float targetDistance = 1f;
    private Animator anim;
    private bool leverPosition = true;
    private string secretDoorSound = "SecretDoor";

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        anim = GetComponent<Animator>();
        startingPos = moveableObject.position;
        endingPos = startingPos + Vector3.right * targetDistance;
    }

    private void Update()
    {
        if (!leverPosition && endingPos.x - moveableObject.position.x >= 0)
        {
            Vector2 dir = (endingPos - startingPos);
            moveableObject.transform.Translate(dir * speed * Time.deltaTime);
        }
        else if (leverPosition && moveableObject.position.x - startingPos.x >= 0)
        {
            Vector2 dir = (startingPos - endingPos);
            moveableObject.transform.Translate(dir * speed * Time.deltaTime);
        }
    }

    public void Use()
    {
        player.IsUsingLever = true;
        anim.SetBool("Switch", leverPosition);
        leverPosition = !leverPosition;
        AudioManager.Instance.PlaySound(secretDoorSound);
    }

    [System.Serializable]
    struct LeverSaveData
    {
        public bool leverPosition;
        public float[] startingPos;
        public float[] endingPos;
        public float[] moveableObjectPosition;
    }

    public object CaptureState()
    {
        LeverSaveData data = new LeverSaveData();
        data.leverPosition = leverPosition;

        data.startingPos = new float[2];
        data.startingPos[0] = startingPos.x;
        data.startingPos[1] = startingPos.y;

        data.endingPos = new float[2];
        data.endingPos[0] = endingPos.x;
        data.endingPos[1] = endingPos.y;

        data.moveableObjectPosition = new float[2];
        data.moveableObjectPosition[0] = moveableObject.position.x;
        data.moveableObjectPosition[1] = moveableObject.position.y;

        return data;
    }
    
    public void RestoreState(object state)
    {
        LeverSaveData data = (LeverSaveData)state;

        leverPosition = data.leverPosition;
        GetComponent<Animator>().SetBool("Switch", !leverPosition);

        moveableObject.position = new Vector2(data.moveableObjectPosition[0], data.moveableObjectPosition[1]);

        startingPos = new Vector2(data.startingPos[0], data.startingPos[1]);
        endingPos = new Vector2(data.endingPos[0], data.endingPos[1]);
    }
}


