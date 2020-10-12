using UnityEngine;

public class Ladder : MonoBehaviour, IUseable
{
    Player player;

    [SerializeField] Collider2D groundCollider;

    public void Use()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // we need to start climbing
        player.OnLadder = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            player.OnLadder = false;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), false);
        }
    }
}
