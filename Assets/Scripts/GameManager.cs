using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager Instance;

    [SerializeField] int maxLives = 3;

    private static int _remainingLives;

    public static int RemainingLives
    {
        get { return _remainingLives; }
    }

    [SerializeField] Transform playerPrefab;
    [SerializeField] float spawnDelay = 3.7f;
    [SerializeField] Transform spawnPrefab;
    [SerializeField] string respawnCountdownSoundName = "RespawnCountdown";
    [SerializeField] string spawnSoundName = "Spawn";
    [SerializeField] string gameOverSoundName = "GameOver";
    [SerializeField] CameraShake cameraShake;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject upgradeMenu;

    public event Action<bool> onToggleMenu;

    [SerializeField] int startingCrystals;
    public static int Crystals;
    private Transform spawnPoint;

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

        if (cameraShake == null) 
        {
            Debug.LogError("No camera shake referenced in GameMaster.");
        }
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.U) && !PlayerData.Instance.isDead)
        {
            ToggleUpgradeMenu();
        }
    }

    private void OnEnable()
    {
        if (gameOverUI.GetComponent<GameOverUI>() != null)
        {
            gameOverUI.GetComponent<GameOverUI>().onRetry += HandleRetry;
        }
    }    

    private void OnDisable()
    {
        if (gameOverUI.GetComponent<GameOverUI>() != null)
        {
            gameOverUI.GetComponent<GameOverUI>().onRetry -= HandleRetry;
        }
    }    

    private void ToggleUpgradeMenu()
    {
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        onToggleMenu?.Invoke(upgradeMenu.activeSelf);
    }

    private void HandleRetry()
    {
        _remainingLives = maxLives;

        Crystals = startingCrystals;
    }

    public void EndGame()
    {
        AudioManager.Instance.PlaySound(gameOverSoundName);
        Debug.Log("GAME OVER");
        gameOverUI.SetActive(true);
    }

    public IEnumerator _RespawnPlayer(GameObject player) 
    {
        AudioManager.Instance.PlaySound(respawnCountdownSoundName);
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
        Instantiate(player.deathParticles, player.transform.position, Quaternion.identity);
        player.gameObject.SetActive(false);
        _remainingLives --;
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

    private void _KillEnemy(Enemy _enemy) 
    {
        // Play sounds
        AudioManager.Instance.PlaySound(_enemy.deathSoundName);
        _enemy.isDead = true;

        // Add particles
        Transform _clone = Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity);
        Destroy(_clone.gameObject, 5f);

        // Camera shake
        //cameraShake.Shake(_enemy.shakeAmount, _enemy.shakeLength);
        //Destroy(_enemy.gameObject);
    }

    [System.Serializable]
    struct GameData
    {
        public int lives;
        public int crystals;
    }

    public object CaptureState()
    {
        GameData data = new GameData();
        data.lives = _remainingLives;
        data.crystals = Crystals;

        return data;
    }

    public void RestoreState(object state)
    {
        GameData data = (GameData)state;
        _remainingLives = data.lives;
        Crystals = data.crystals;
    }
}
