using UnityEngine;

public class Lever : MonoBehaviour, IUseable
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
    

    // Start is called before the first frame update
    void Start()
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
}


