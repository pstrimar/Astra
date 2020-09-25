using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField] Slider healthBarSlider;
    [SerializeField] Slider fuelBarSlider;
    [SerializeField] Text healthText;

    public PlayerData stats;

    private void Awake()
    {
        stats = PlayerData.Instance;
    }

    private void OnEnable()
    {
        PlayerMotor motor = FindObjectOfType<PlayerMotor>();
        PlayerOld player = FindObjectOfType<PlayerOld>();
        if (motor != null)
        {
            motor.onFuelUsed += UpdateFuel;
        }
        if (player != null)
        {
            player.onHealthChanged += UpdateHealth;
        }
    }    

    private void OnDisable()
    {
        PlayerMotor motor = FindObjectOfType<PlayerMotor>();
        PlayerOld player = FindObjectOfType<PlayerOld>();
        if (motor != null)
        {
            motor.onFuelUsed -= UpdateFuel;
        }
        if (player != null)
        {
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

        healthText.text = currentHealth + "/" + stats.maxHealth + " HP";
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
