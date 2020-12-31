using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CrystalCounterUI : MonoBehaviour
{
    private Text crystalCount;

    void Awake()
    {
        crystalCount = GetComponent<Text>();
    }

    private void OnEnable()
    {
        CrystalPickup.onCrystalPickedUp += HandleCrystalPickup;
        GameOverUI.onReplay += HandleCrystalCount;
        GameOverUI.onRetry += HandleCrystalCount;
    }

    private void OnDisable()
    {
        CrystalPickup.onCrystalPickedUp -= HandleCrystalPickup;
        GameOverUI.onReplay -= HandleCrystalCount;
        GameOverUI.onRetry -= HandleCrystalCount;
    }

    private void Start()
    {
        crystalCount.text = "CRYSTALS: " + GameManager.Crystals;
    }

    // Update crystal text when crystal picked up
    private void HandleCrystalPickup(int obj)
    {
        crystalCount.text = "CRYSTALS: " + GameManager.Crystals;
    }

    private void HandleCrystalCount()
    {
        crystalCount.text = "CRYSTALS: " + GameManager.Crystals;
    }
}
