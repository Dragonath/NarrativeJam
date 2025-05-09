using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine;

[Serializable]
class PlayerData
{
    public int currentLevelIndex;
    public bool dashUnlocked;
    public bool runUnlocked;
    public bool jumpUnlocked;
    public bool walkUnlocked;
    public Vector3 playerPosition;
    public int playerHealth;
    public int playerMaxHealth;
    public int maxJumpCount;
}

public class SaveAndLoad : MonoBehaviour
{
    public static SaveAndLoad instance;

    public int currentLevelIndex;


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

        Load();
    }

    public void ResetStats()
    {
        currentLevelIndex = 0;
    }

    public void Save()
    {
        // Create a new BinaryFormatter and FileStream to save the data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        PlayerData data = new PlayerData();

        // Save the current data
        data.currentLevelIndex = currentLevelIndex;
        data.dashUnlocked = Player_Controller.instance.dashUnlocked;
        data.jumpUnlocked = Player_Controller.instance.jumpUnlocked;
        data.runUnlocked = Player_Controller.instance.runUnlocked;
        data.walkUnlocked = Player_Controller.instance.walkUnlocked;
        data.playerPosition = Player_Controller.instance.playerRB.position;
        data.playerHealth = Player_Controller.instance.currentHealth;
        data.playerMaxHealth = Player_Controller.instance.maxHealth;
        data.maxJumpCount = Player_Controller.instance.maxJumpCount;

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

            currentLevelIndex = data.currentLevelIndex;
            Player_Controller.instance.dashUnlocked = data.dashUnlocked;
            Player_Controller.instance.runUnlocked = data.runUnlocked;
            Player_Controller.instance.jumpUnlocked = data.jumpUnlocked;
            Player_Controller.instance.walkUnlocked = data.walkUnlocked;
            Player_Controller.instance.currentHealth = data.playerHealth;
            Player_Controller.instance.maxHealth = data.playerMaxHealth;
            Player_Controller.instance.maxJumpCount = data.maxJumpCount;

            GameManager.instance.LoadScene(currentLevelIndex, data.playerPosition);
        }
    }
}
