using UnityEngine;

public class JumpSurprise : MonoBehaviour
{
    private SpriteMask spriteMask;

    // Start is called before the first frame update
    void Start()
    {
        spriteMask = FindObjectOfType<SpriteMask>();        
    }

    public void DisableSpriteMask()
    {
        spriteMask.enabled = false;
    }

    public void PlaySurpriseSound()
    {
        AudioManager.Instance.PlaySound("Surprise");
    }

    public void PlayBossBattleMusic()
    {
        AudioManager.Instance.PauseSound("CrystalCave");
        AudioManager.Instance.PlaySound("BossBattle");
    }

    public void ShowHealthBar()
    {
        GameManager.Instance.ShowBossHealthBar();
    }
}
