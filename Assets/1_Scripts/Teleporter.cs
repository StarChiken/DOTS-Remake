using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Teleporter linkedTeleporter;
    
    void Start()
    {
        if (!linkedTeleporter)
            Debug.LogError("No Linked Teleporter Set");
    }

    public void ActivateTeleporter()
    {
        print("Teleporter Activated");

        // Play teleport animation
    }

    public Vector2Int GetTeleportPosition()
    {
        return Vector2Int.RoundToInt(linkedTeleporter.transform.position);
    }
}
