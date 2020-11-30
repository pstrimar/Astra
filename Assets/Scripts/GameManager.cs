using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager Instance;

    [SerializeField] int maxLives = 3;
    [SerializeField] int startingHealth = 100;
    [SerializeField] float startingFuel = 1f;

    private static int _remainingLives;

    public static int RemainingLives
    {
        get { return _remainingLives; }
    }

    public CameraShake CamShake;
    [SerializeField] float spawnDelay = 2f;
    [SerializeField] Transform spawnPrefab;
    [SerializeField] string spawnSoundName = "Spawn";
    [SerializeField] string gameOverSoundName = "GameOver";
    [SerializeField] string endingCreditsSoundName = "EndingCredits";
    [SerializeField] GameObject bossHealthBar;

    public static event Action<bool> onToggleMenu;
    public static event Action onLifeLost;
    public static event Action onGameOver;
    public static event Action onWinGame;

    [SerializeField] int startingCrystals;
    public static int Crystals;
    public bool playerHasCrashed;
    private Transform spawnPoint;
    private bool upgradeMenuVisible;

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

    private void Start() 
    {
        _remainingLives = maxLives;

        Crystals = startingCrystals;

        if (CamShake == null) 
        {
            Debug.LogError("No camera shake referenced in GameManager.");
        }
    }    

    private void OnEnable()
    {
        GameOverUI.onRetry += HandleRetry;
        GameOverUI.onReplay += HandleReplay;
        CrystalPickup.onCrystalPickedUp += HandleCrystalPickup;
    }    

    private void OnDisable()
    {
        GameOverUI.onRetry -= HandleRetry;
        GameOverUI.onReplay -= HandleReplay;
        CrystalPickup.onCrystalPickedUp -= HandleCrystalPickup;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) && !PlayerData.Instance.isDead)
        {
            ToggleUpgradeMenu();
        }
    }

    private void ToggleUpgradeMenu()
    {
        upgradeMenuVisible = !upgradeMenuVisible;
        onToggleMenu?.Invoke(upgradeMenuVisible);
    }

    private void HandleRetry()
    {
        _remainingLives = maxLives;

        Crystals = startingCrystals;

        // Do not show the opening cinematic again
        playerHasCrashed = true;

        PlayerData.Instance.maxFuelAmount = startingFuel;
        PlayerData.Instance.maxHealth = startingHealth;
    }

    private void HandleReplay()
    {
        _remainingLives = maxLives;

        Crystals = startingCrystals;

        // Will trigger the opening cinematic
        playerHasCrashed = false;

        PlayerData.Instance.maxFuelAmount = startingFuel;
        PlayerData.Instance.maxHealth = startingHealth;
    }

    private void HandleCrystalPickup(int crystalAmount)
    {
        Crystals += crystalAmount;
    }

    public void EndGame()
    {
        PlayerData.Instance.maxFuelAmount = startingFuel;
        PlayerData.Instance.maxHealth = startingHealth;
        playerHasCrashed = false;

        AudioManager.Instance.StopAllSounds();
        AudioManager.Instance.PlaySound(gameOverSoundName);
        onGameOver?.Invoke();
    }

    public void WinGame()
    {
        PlayerData.Instance.maxFuelAmount = startingFuel;
        PlayerData.Instance.maxHealth = startingHealth;
        playerHasCrashed = false;

        AudioManager.Instance.StopAllSounds();
        AudioManager.Instance.PlaySound(endingCreditsSoundName);
        onWinGame?.Invoke();
    }

    // Moves player to spawn point
    public IEnumerator _RespawnPlayer(GameObject player) 
    {
        yield return new WaitForSeconds(spawnDelay);

        AudioManager.Instance.PlaySound(spawnSoundName);
        spawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
        player.GetComponent<PlayerInput>().enabled = true;
        player.SetActive(true);
        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(clone.gameObject, 3f);
    }

    public static void KillPlayer(Player player) 
    {        
        Transform deathParticles = Instantiate(player.DeathParticles, player.transform.position, Quaternion.identity);
        Destroy(deathParticles.gameObject, 5f);

        player.gameObject.SetActive(false);
        _remainingLives--;

        // Broadcast life lost
        onLifeLost?.Invoke();

        // Game over
        if (_remainingLives <= 0)
        {
            Instance.EndGame();
        }
        else 
        {
            Instance.StartCoroutine(Instance._RespawnPlayer(player.gameObject));
        }        
    }

    public static void KillEnemy(Enemy enemy) 
    {
        Instance._KillEnemy(enemy);
    }

    public void ShowBossHealthBar()
    {
        bossHealthBar.SetActive(true);
    }

    private void _KillEnemy(Enemy _enemy) 
    {
        // Play sounds
        AudioManager.Instance.StopSound(_enemy.HurtSoundName);
        AudioManager.Instance.PlaySound(_enemy.DeathSoundName);

        _enemy.IsDead = true;
        _enemy.GetComponent<Rigidbody2D>().sharedMaterial = _enemy.FullFriction;

        // Add particles
        Transform deathParticles = Instantiate(_enemy.DeathParticles, _enemy.transform.position, Quaternion.identity);
        Destroy(deathParticles.gameObject, 5f);
    }

    [System.Serializable]
    struct GameData
    {
        public int lives;
        public int crystals;
        public bool playerHasCrashed;
    }

    public object CaptureState()
    {
        GameData data = new GameData();
        data.lives = _remainingLives;
        data.crystals = Crystals;
        data.playerHasCrashed = playerHasCrashed;

        return data;
    }

    public void RestoreState(object state)
    {
        GameData data = (GameData)state;
        _remainingLives = data.lives;
        Crystals = data.crystals;
        playerHasCrashed = data.playerHasCrashed;
    }
}
