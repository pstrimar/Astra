using UnityEngine;

public class CrashLanded : MonoBehaviour
{
    public void ShipCrashed()
    {
        GameManager.Instance.playerHasCrashed = true;
    }
}
