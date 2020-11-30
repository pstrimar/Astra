using UnityEngine;

public class JumpSurprise : MonoBehaviour
{
    public SpriteMask spriteMask;

    // Event called by animation
    public void DisableSpriteMask()
    {
        spriteMask.enabled = false;
    }

    // Event called by animation
    public void PlaySurpriseSound()
    {
        AudioManager.Instance.PlaySound("Surprise");
    }

    // Event called by animation
    public void PlayBossBattleMusic()
    {
        AudioManager.Instance.PauseSound("CrystalCave");
        AudioManager.Instance.PlaySound("BossBattle");
    }

    // Event called by animation
    public void ShowHealthBar()
    {
        GameManager.Instance.ShowBossHealthBar();
    }
}
