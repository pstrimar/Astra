using System;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    public static StatusIndicator Instance;
    public static event Action onStatusIndicatorEnabled;

    [SerializeField] Slider healthBarSlider;
    [SerializeField] Slider fuelBarSlider;
    [SerializeField] Slider laserBarSlider;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        // Broadcast status indicator enabled
        onStatusIndicatorEnabled?.Invoke();

        // Register for events
        Player.onFuelUsed += UpdateFuel;
        Player.onHealthChanged += UpdateHealth;
        Weapon.onLaserUsed += UpdateLaser;

    }

    private void OnDisable()
    {
        // Unregister for events
        Player.onFuelUsed -= UpdateFuel;
        Player.onHealthChanged -= UpdateHealth;
        Weapon.onLaserUsed -= UpdateLaser;

    }

    // Set fuelbar slider to current fuel value
    private void UpdateFuel(float fuel)
    {
        fuelBarSlider.value = fuel;
    }

    // Set healthbar slider to current health value
    private void UpdateHealth(int currentHealth)
    {
        healthBarSlider.value = currentHealth;
    }

    // Set laser slider to current laser value
    private void UpdateLaser(float laserAmount)
    {
        laserBarSlider.value = laserAmount;
    }

    // Set slider to max value
    public void SetMaxHealth(int maxHealth)
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = maxHealth;
    }

    // Set slider to max value
    public void SetMaxFuel(float maxFuel)
    {
        fuelBarSlider.maxValue = maxFuel;
        fuelBarSlider.value = maxFuel;
    }

    // Set slider to max value
    public void SetMaxLaser(float maxLaser)
    {
        laserBarSlider.maxValue = maxLaser;
        laserBarSlider.value = maxLaser;
    }
}
