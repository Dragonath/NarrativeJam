using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine;

[Serializable]
class PlayerData
{
    public int currentLevelIndex;
}

public class SaveAndLoad : MonoBehaviour
{
    public static SaveAndLoad saveAndLoad;

    public int currentLevelIndex;  

    void Awake()
    {
        if (saveAndLoad == null)
        {
            DontDestroyOnLoad(gameObject);
            saveAndLoad = this;
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

        // Save the current level index
        data.currentLevelIndex = currentLevelIndex;

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

        }
    }
}
