using UnityEngine;

public class Ladder : MonoBehaviour, IUseable
{
    Player player;

    [SerializeField] Collider2D platformCollider;

    public void Use()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // we need to start climbing
        UseLadder(true);
        Physics2D.IgnoreCollision(player.GetComponent<CircleCollider2D>(), platformCollider, true);
        Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), platformCollider, true);
    }

    private void UseLadder(bool onLadder)
    {
        player.OnLadder = onLadder;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            UseLadder(false);
            Physics2D.IgnoreCollision(player.GetComponent<CircleCollider2D>(), platformCollider, false);
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), platformCollider, false);
        }
    }
}
