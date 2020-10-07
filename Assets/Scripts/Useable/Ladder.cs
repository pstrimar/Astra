using UnityEngine;

public class Ladder : MonoBehaviour, IUseable
{
    Player player;

    [SerializeField] Collider2D ladderCollider;

    public void Use()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // we need to start climbing
        player.OnLadder = true;
        Physics2D.IgnoreCollision(player.GetComponent<CircleCollider2D>(), ladderCollider, true);
        Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), ladderCollider, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            player.OnLadder = false;
            Physics2D.IgnoreCollision(player.GetComponent<CircleCollider2D>(), ladderCollider, false);
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), ladderCollider, false);
        }
    }
}
