﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] Text healthText;

    [SerializeField] Text fuelText;

    [SerializeField] float healthMultiplier = 1.3f;

    [SerializeField] float fuelAmountMultiplier = 1.2f;

    [SerializeField] int upgradeCost = 50;

    private PlayerStats stats;

    private void OnEnable() 
    {
        stats = PlayerStats.Instance;
        UpdateValues();
    }

    void UpdateValues()
    {
        healthText.text = "Health: " + stats.maxHealth;
        fuelText.text = "Fuel: " + stats.thrusterFuelAmount;
    }

    public void UpgradeHealth() 
    {
        if (GameManager.Crystals < upgradeCost)
        {
            AudioManager.Instance.PlaySound("NoCrystals");
            return;
        }

        stats.maxHealth = (int)(stats.maxHealth * healthMultiplier);

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

        stats.thrusterFuelAmount *= fuelAmountMultiplier;

        GameManager.Crystals -= upgradeCost;
        AudioManager.Instance.PlaySound("Crystals");

        UpdateValues();
        Debug.Log("thruster fuel amount: " + stats.thrusterFuelAmount);
    }
}
