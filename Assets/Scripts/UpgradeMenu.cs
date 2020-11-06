using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] Text healthText;

    [SerializeField] Text fuelText;

    [SerializeField] int healthUpgrade = 10;

    [SerializeField] float fuelAmountUpgrade = 0.2f;

    [SerializeField] int upgradeCost = 5;

    private PlayerData stats;

    private void OnEnable() 
    {
        stats = PlayerData.Instance;
        UpdateValues();
    }

    void UpdateValues()
    {
        healthText.text = "Health: " + stats.maxHealth;
        fuelText.text = "Fuel: " + stats.maxFuelAmount;
    }

    public void UpgradeHealth() 
    {
        if (GameManager.Crystals < upgradeCost)
        {
            AudioManager.Instance.PlaySound("NoCrystals");
            return;
        }

        stats.maxHealth += healthUpgrade;

        StatusIndicator.Instance.SetMaxHealth(stats.maxHealth);

        GameManager.Crystals -= upgradeCost;
        AudioManager.Instance.PlaySound("Crystals");

        UpdateValues();
    }

    public void UpgradeFuel() 
    {
        if (GameManager.Crystals < upgradeCost)
        {
            AudioManager.Instance.PlaySound("NoCrystals");
            return;
        }

        stats.maxFuelAmount += fuelAmountUpgrade;

        StatusIndicator.Instance.SetMaxFuel(stats.maxFuelAmount);

        GameManager.Crystals -= upgradeCost;
        AudioManager.Instance.PlaySound("Crystals");

        UpdateValues();
    }
}
