using System.Collections;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    [SerializeField] private Vector2Int direction = Vector2Int.zero;
    [SerializeField] private float pushAnimationSeconds;

    public Vector2Int Direction { get { return direction; } }

    private Transform pusherArrow;
    
    void Start()
    {
        if (direction == Vector2Int.zero)
            Debug.LogError("Push Direction Must Be Set");
        else if (direction.magnitude > 1)
            Debug.LogError("Push Direction Magnitude Too Large");

        pusherArrow = GetComponentInChildren<Transform>();
    }

    public void ActivatePusher()
    {
        print("Pusher Activated");
        
        // Play push animation
        StartCoroutine(PusherAnimation());
    }

    IEnumerator PusherAnimation()
    {
        float timeElapsed = 0;

        float animationSeconds = pushAnimationSeconds / 2;
        Vector3 startPos = pusherArrow.position;
        Vector3 endPos = startPos + (Vector3)(Vector2)direction * 0.1f;
        
        while (timeElapsed < animationSeconds)
        {
            pusherArrow.position = Vector3.Lerp(startPos, endPos, timeElapsed / animationSeconds);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        timeElapsed = 0;
        
        while (timeElapsed < animationSeconds)
        {
            pusherArrow.position = Vector3.Lerp(endPos, startPos, timeElapsed / animationSeconds);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        pusherArrow.position = startPos;
    }
}