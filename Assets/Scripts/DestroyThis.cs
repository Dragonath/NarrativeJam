using UnityEngine;

public class DestroyThis : MonoBehaviour
{
    public float destroyTime = 2f; // Time in seconds before the object is destroyed
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

}
