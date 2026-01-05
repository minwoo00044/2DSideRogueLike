using UnityEngine;

public class DelayedFollowCamera : MonoBehaviour
{
    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TargetResetting();
    }
    public void TargetResetting()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;
        Vector3 offset = new Vector3(0, 0, -10);
        float smoothSpeed = 0.125f;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

    }
}
