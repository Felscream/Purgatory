using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField]
    private Transform target;
    [Range(0,1)]
    public float lockSpeed = 0.3f;
    [SerializeField]
    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, lockSpeed);
        transform.position = smoothedPosition;
    }
}
