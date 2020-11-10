using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField] Slider healthBarSlider;
    [SerializeField] Slider fuelBarSlider;
    [SerializeField] Slider laserBarSlider;

    private Player player;

    public static StatusIndicator Instance;

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
        Debug.Log("StatusIndicator enabled");
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.onFuelUsed += UpdateFuel;
            player.onHealthChanged += UpdateHealth;
            player.weapon.onLaserUsed += UpdateLaser;
        }
    }    

    private void OnDisable()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.onFuelUsed -= UpdateFuel;
            player.onHealthChanged -= UpdateHealth;
            player.weapon.onLaserUsed -= UpdateLaser;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            player.onFuelUsed += UpdateFuel;
            player.onHealthChanged += UpdateHealth;
            player.weapon.onLaserUsed += UpdateLaser;
        }
    }

    private void UpdateFuel(float fuel)
    {
        fuelBarSlider.value = fuel;
    }

    private void UpdateHealth(int currentHealth)
    {
        healthBarSlider.value = currentHealth;
    }

    private void UpdateLaser(float laserAmount)
    {
        laserBarSlider.value = laserAmount;
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

    public void SetMaxLaser(float maxLaser)
    {
        laserBarSlider.maxValue = maxLaser;
        laserBarSlider.value = maxLaser;
    }
}
