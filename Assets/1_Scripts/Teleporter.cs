using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Teleporter linkedTeleporter;
    
    public Vector2Int TeleportPosition { get { return Vector2Int.RoundToInt(linkedTeleporter.transform.position); } }

    void Start()
    {
        if (!linkedTeleporter)
            Debug.LogError("Linked Teleporter Must Be Set");
    }

    public void ActivateTeleporter()
    {
        print("Teleporter Activated");

        // Play teleport animation
    }
}
