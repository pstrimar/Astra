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
        GameManager.onToggleMenu += HandleMenuToggle;
        stats = PlayerData.Instance;
        UpdateValues();
    }

    private void OnDisable()
    {
        GameManager.onToggleMenu -= HandleMenuToggle;
    }    

    // Show our max health and fuel values
    void UpdateValues()
    {
        healthText.text = "Health: " + stats.maxHealth;
        fuelText.text = "Fuel: " + stats.maxFuelAmount;
    }

    public void UpgradeHealth() 
    {
        // Return if we don't have enough crystals to upgrade
        if (GameManager.Crystals < upgradeCost)
        {
            AudioManager.Instance.PlaySound("NoCrystals");
            return;
        }

        // Update player stats
        stats.maxHealth += healthUpgrade;

        // Update gauge with new health
        StatusIndicator.Instance.SetMaxHealth(stats.maxHealth);

        GameManager.Crystals -= upgradeCost;
        AudioManager.Instance.PlaySound("Crystals");

        UpdateValues();
    }

    public void UpgradeFuel() 
    {
        // Return if we don't have enough crystals to upgrade
        if (GameManager.Crystals < upgradeCost)
        {
            AudioManager.Instance.PlaySound("NoCrystals");
            return;
        }

        // Update player stats
        stats.maxFuelAmount += fuelAmountUpgrade;

        // Update gauge with new fuel
        StatusIndicator.Instance.SetMaxFuel(stats.maxFuelAmount);

        GameManager.Crystals -= upgradeCost;
        AudioManager.Instance.PlaySound("Crystals");

        UpdateValues();
    }

    private void HandleMenuToggle(bool shouldShowMenu)
    {
        if (shouldShowMenu)
        {
            ShowCanvasGroup();
        }
        else
        {
            HideCanvasGroup();
        }
    }

    private void ShowCanvasGroup()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideCanvasGroup()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
