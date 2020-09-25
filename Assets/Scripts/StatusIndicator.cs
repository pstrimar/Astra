using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField] Slider healthBarSlider;
    [SerializeField] Slider fuelBarSlider;
    [SerializeField] Text healthText;

    private PlayerData playerData;

    private void Awake()
    {
        playerData = PlayerData.Instance;
    }

    private void OnEnable()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.onFuelUsed += UpdateFuel;
            player.onHealthChanged += UpdateHealth;
        }
    }    

    private void OnDisable()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.onFuelUsed -= UpdateFuel;
            player.onHealthChanged -= UpdateHealth;
        }
    }

    private void UpdateFuel(float fuel)
    {
        fuelBarSlider.value = fuel;
    }

    private void UpdateHealth(int currentHealth)
    {
        healthBarSlider.value = currentHealth;

        healthText.text = currentHealth + "/" + playerData.maxHealth + " HP";
    }

    public void SetMaxHealth(int maxHealth)
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = maxHealth;
    }

    public void SetMaxFuel(float maxFuel)
    {
        fuelBarSlider.maxValue = maxFuel;
        fuelBarSlider.value = maxFuel;
    }
}
