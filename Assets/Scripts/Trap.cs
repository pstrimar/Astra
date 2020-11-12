using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] Enemy boss;
    [SerializeField] Collider2D bodyCollider;
    [SerializeField] Collider2D groundCollider;

    void Start()
    {
        boss.transform.Find("b crawler").gameObject.SetActive(false);
        boss.GetComponent<Animator>().enabled = false;
        boss.GetComponent<Rigidbody2D>().gravityScale = 0f;
        boss.GetComponent<Enemy>().enabled = false;
        boss.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            boss.GetComponent<Animator>().enabled = true;
            boss.GetComponent<Rigidbody2D>().gravityScale = 1f;
            boss.transform.Find("b crawler").gameObject.SetActive(true);
            Physics2D.IgnoreCollision(bodyCollider, groundCollider);
            boss.gameObject.layer = LayerMask.NameToLayer("Enemy");
            boss.GetComponent<Enemy>().enabled = true;
            boss.GetComponent<Animator>().Play("JumpSurprise");

            Destroy(this.gameObject);
        }
    }
}
