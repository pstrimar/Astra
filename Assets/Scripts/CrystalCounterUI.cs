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
        CrystalPickup.onCrystalPickedUp += HandleCrystalCount;
        GameOverUI.onReplay += HandleCrystalCountReset;
        GameOverUI.onRetry += HandleCrystalCountReset;
    }

    private void OnDisable()
    {
        CrystalPickup.onCrystalPickedUp -= HandleCrystalCount;
        GameOverUI.onReplay -= HandleCrystalCountReset;
        GameOverUI.onRetry -= HandleCrystalCountReset;
    }

    private void Start()
    {
        HandleCrystalCountReset();
    }

    // Update crystal text when crystal picked up
    private void HandleCrystalCount(int obj)
    {
        crystalCount.text = "CRYSTALS: " + GameManager.Crystals;
    }

    private void HandleCrystalCountReset()
    {
        crystalCount.text = "CRYSTALS: " + GameManager.Crystals;
    }
}
