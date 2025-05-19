using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    public int lvl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (lvl == 1)
            {
                SaveAndLoad.instance._lvl1Flag = checkpointID;
            }
            else if (lvl == 2)
            {
                SaveAndLoad.instance._lvl2Flag = checkpointID;
            }
            else if (lvl == 3)
            {
                SaveAndLoad.instance._lvl3Flag = checkpointID;
            }
        }
    }

}
