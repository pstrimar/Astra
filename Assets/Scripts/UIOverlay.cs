using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour
{
    private GameObject statusIndicators;
    private GameObject upgradeMenuText;
    private GameObject playerInfo;
    private GameObject player;

    void Start()
    {
        statusIndicators = GameObject.Find("StatusIndicators");
        upgradeMenuText = GameObject.Find("PressUForUpgradeMenu");
        playerInfo = GameObject.Find("PlayerInfo");
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        statusIndicators.SetActive(player.activeSelf);
        upgradeMenuText.SetActive(player.activeSelf);
        playerInfo.SetActive(player.activeSelf);
    }
}
