using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float damping = 0.5f;

    public Transform target;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        target = Player_Controller.instance.transform; 
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            targetPosition.z = transform.position.z; // Keep the camera's z position unchanged
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, damping);
        }
    }
}
