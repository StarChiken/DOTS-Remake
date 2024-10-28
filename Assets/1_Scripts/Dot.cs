using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public Sprite autoSprite;
    public Sprite selectedSprite;
    
    private Vector2Int position;

    private List<Vector3Int> replayPositions;
    
    void Start()
    {
        replayPositions = new List<Vector3Int>();
        replayPositions.Add(Vector3Int.RoundToInt(transform.position));
    }

    void Update()
    {
        
    }

    //* This function will move the character without checking if the move is valid
    public void TryMoveDot(bool isValidMove)
    {
        // Move Character
        // If this is the Player Character, Store Position of Character to Replay Positions
    }

    IEnumerator MoveDotAnimation()
    {
        yield return null;
    }
}
