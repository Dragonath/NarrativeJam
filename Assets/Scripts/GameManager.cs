using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance; // Singleton instance of GameManager


    void Awake()
    {
        // Check if an instance of GameManager already exists
        if (instance == null)
        {
            instance = this; // Assign this instance to the static instance
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy this object if another instance already exists
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
