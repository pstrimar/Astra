using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] Slider healthBarSlider;

    private Enemy boss;

    private void OnEnable()
    {
        boss = GameObject.Find("Crystal Crawler Boss").GetComponent<Enemy>();
        boss.onHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        boss.onHealthChanged -= UpdateHealth;
    }
    private void Update()
    {
        // Hide boss health bar if player has lost the game
        if (GameManager.RemainingLives == 0)
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void UpdateHealth(int currentHealth)
    {
        healthBarSlider.value = currentHealth;

        if (currentHealth <= 0 || GameManager.RemainingLives == 0)
        {
            transform.gameObject.SetActive(false);
            AudioManager.Instance.StopSound("BossBattle");
            AudioManager.Instance.PlaySound("CrystalCave");
        }
    }
}
