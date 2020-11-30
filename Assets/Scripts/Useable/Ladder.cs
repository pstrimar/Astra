using System;
using UnityEngine;

public class Ladder : MonoBehaviour, IUseable
{
    public static event Action<bool> onUseLadder;

    public void Use()
    {
        // we need to start climbing
        onUseLadder?.Invoke(true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            onUseLadder?.Invoke(false);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), false);
        }
    }
}
