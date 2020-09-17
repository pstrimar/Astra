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

    void Update()
    {
        crystalCount.text = "CRYSTALS: " + GameManager.Crystals;
    }
}
