using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 initialPosition;
    //float originalY;
    public float speed = 1f;
    public float floatStrength = .1f;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        //initialPosition = transform.position;
        initialPosition = rb.position;
        //this.originalY = this.transform.position.y;
    }

    void FixedUpdate()
    {
        //transform.position = new Vector2(transform.position.x, originalY + ((float)Mathf.Sin(Time.time * speed) * floatStrength));

        float newY = Mathf.Sin(Time.time * speed) * floatStrength;
        Vector2 position = new Vector2(0, newY) + initialPosition;
        rb.MovePosition(position);
    }

}
