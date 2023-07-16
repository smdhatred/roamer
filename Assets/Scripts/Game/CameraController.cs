using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed;
    private Vector3 cameraPosition;
    private void Awake()
    {
        cameraPosition = transform.position;
    }
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(0, 0, -10), speed);
    }
}
