using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[Serializable]
class PlayerData
{
    public int currentLevelIndex;
    public bool dashUnlocked;
    public bool runUnlocked;
    public bool jumpUnlocked;
    public bool walkUnlocked;
    public float playerPositionX;
    public float playerPositionY;
    public int playerHealth;
    public int playerMaxHealth;
    public int maxJumpCount;
    public int lvl1Flag;
    public int lvl2Flag;
    public int lvl3Flag;
}

public class SaveAndLoad : MonoBehaviour
{
    public static SaveAndLoad instance;

    private bool updateCD = false; 
    public int updateCDTime = 3; // Time in seconds to update the player stats

    public int _currentLevelIndex;
    public bool _dashUnlocked;
    public bool _runUnlocked;
    public bool _jumpUnlocked;
    public bool _walkUnlocked;
    public Vector2 _playerPosition;
    public int _playerHealth;
    public int _playerMaxHealth;
    public int _maxJumpCount;
    public int _lvl1Flag;
    public int _lvl2Flag;
    public int _lvl3Flag;
    public Checkpoint lastCheckpoint; 


    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.paused && !updateCD && Player_Controller.instance != null)
        {
            updateCD = true;
            StartCoroutine(UpdateCD());
            _currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

            // Update the player stats
            Player_Controller.instance.dashUnlocked = _dashUnlocked;
            Player_Controller.instance.runUnlocked = _runUnlocked;
            Player_Controller.instance.jumpUnlocked = _jumpUnlocked;
            Player_Controller.instance.walkUnlocked = _walkUnlocked;
            Player_Controller.instance.maxJumps = _maxJumpCount;
            Player_Controller.instance.maxHealth = _playerMaxHealth;
            _playerHealth = Player_Controller.instance.currentHealth;
            _playerPosition = Player_Controller.instance.rb.position;
        }
    }

    public void ResetStats()
    {
        _currentLevelIndex = 0;
        File.Delete(Application.persistentDataPath + "/playerInfo.dat");
    }

    public void Save()
    {
        // Create a new BinaryFormatter and FileStream to save the data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        PlayerData data = new PlayerData();

        // Save the current data
        data.currentLevelIndex = _currentLevelIndex;
        data.dashUnlocked = _dashUnlocked;
        data.jumpUnlocked = _jumpUnlocked;
        data.runUnlocked = _runUnlocked;
        data.walkUnlocked = _walkUnlocked;
        data.playerPositionX = _playerPosition.x;
        data.playerPositionY = _playerPosition.y;
        data.playerHealth = _playerHealth;
        data.playerMaxHealth = _playerMaxHealth;
        data.maxJumpCount = _maxJumpCount;
        data.lvl1Flag = _lvl1Flag;
        data.lvl2Flag = _lvl2Flag;
        data.lvl3Flag = _lvl3Flag;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            _currentLevelIndex = data.currentLevelIndex;
            _dashUnlocked = data.dashUnlocked;
            _runUnlocked = data.runUnlocked;
            _jumpUnlocked = data.jumpUnlocked;
            _walkUnlocked = data.walkUnlocked;
            _playerHealth = data.playerHealth;
            _playerMaxHealth = data.playerMaxHealth;
            _maxJumpCount = data.maxJumpCount;
            _lvl1Flag = data.lvl1Flag;
            _lvl2Flag = data.lvl2Flag;
            _lvl3Flag = data.lvl3Flag;
            _playerPosition = new Vector2(data.playerPositionX, data.playerPositionY);
            GameManager.instance.LoadScene(_currentLevelIndex);
        }
        else 
        { 
            _dashUnlocked = false;
            _runUnlocked = false;
            _jumpUnlocked = false;
            _walkUnlocked = false;
            _playerHealth = 10;
            _playerMaxHealth = 10;
            _maxJumpCount = 2;
            _lvl1Flag = 0;
            _lvl2Flag = 0;
            _lvl3Flag = 0;
        }
    }

    IEnumerator UpdateCD()
    {
        yield return new WaitForSeconds(updateCDTime);
        updateCD = false;
    }

    public void RespawnPlayer()
    {
        Player_Controller.instance.currentHealth = _playerMaxHealth;
    }
}
