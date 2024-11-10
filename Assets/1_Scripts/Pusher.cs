using UnityEngine;

public class Pusher : MonoBehaviour
{
    [SerializeField] private Vector2Int direction = Vector2Int.zero;

    public Vector2Int Direction { get { return direction; } }

    void Start()
    {
        if (direction == Vector2Int.zero)
            Debug.LogError("Push Direction Must Be Set");
        else if (direction.magnitude > 1)
            Debug.LogError("Push Direction Magnitude Too Large");
    }

    public void ActivatePusher()
    {
        print("Pusher Activated");
        
        // Play push animation
    }
}