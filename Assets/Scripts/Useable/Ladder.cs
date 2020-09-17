using UnityEngine;
using UnityStandardAssets._2D;

public class Ladder : MonoBehaviour, IUseable
{
    PlayerMotor player;

    [SerializeField] Collider2D platformCollider;

    public void Use()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
                
        // we need to start climbing
        UseLadder(true);
        Physics2D.IgnoreCollision(player.GetComponent<CircleCollider2D>(), platformCollider, true);
        Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), platformCollider, true);

    }

    private void UseLadder(bool onLadder)
    {
        player.onLadder = onLadder;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();

        if (collision.tag == "Player")
        {
            UseLadder(false);
            Physics2D.IgnoreCollision(player.GetComponent<CircleCollider2D>(), platformCollider, false);
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), platformCollider, false);
        }
    }
}
