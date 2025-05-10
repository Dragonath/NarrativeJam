using Unity.VisualScripting;
using UnityEngine;

public class Background : MonoBehaviour
{
    private float start , startY;
    private float length;
    public GameObject mainCam;
    public float parallaxFactor, parallaxFactorY;
    private float distance;
    private float distanceY;
    private float movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start = transform.position.x; // Store the initial position of the background
        startY = transform.position.y; // Store the initial Y position of the background
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Get the width of the background sprite
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update the background position based on camera movement
        distance = mainCam.transform.position.x * parallaxFactor;
        distanceY = mainCam.transform.position.y * parallaxFactorY;
        movement = mainCam.transform.position.x * (1 - parallaxFactor); 
        transform.position = new Vector3(start + distance, startY + distanceY, transform.position.z); 

        if (movement > start + length) // If the background has moved past its length
        {
            start += length; // Reset the start position to create a seamless loop
        }
        else if (movement < start - length) // If the background has moved past its length in the opposite direction
        {
            start -= length; // Reset the start position to create a seamless loop
        }
    }
}
