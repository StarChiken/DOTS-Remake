using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public Sprite autoSprite;
    public Sprite selectedSprite;

    public Vector2Int Position { get; private set; }

    public bool IsMoving { get; private set; }

    void Start()
    {
        Position = Vector2Int.RoundToInt(transform.position);
    }

    void Update()
    {
        
    }

    //* This function will move the character without checking if the move is valid
    public void TryMoveDot(bool isValidMove, Vector2Int direction)
    {
        // Move Character
        // If this is the Player Character, Store Position of Character to Replay Positions
        if (IsMoving)
            return;
        
        if (isValidMove)
        {
            Position += direction;
            //transform.position = new Vector3(Position.x, Position.y, 0);
            print("Can move to: " + Position);
            // Play Move Animation
            StartCoroutine(MoveDotAnimation((Vector3Int)Position));
        }
        else
        {
            print("Cannot move to: " + (Position + direction));
            // Play Bonk Animation
            StartCoroutine(WallHitEnum((Vector3Int)direction));
        }
    }

    IEnumerator MoveDotAnimation(Vector3Int movePos)
    {
        const float moveLerpDuration = 0.15f;

        IsMoving = true;
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        while (timeElapsed < moveLerpDuration)
        {
            transform.position = Vector3.Lerp(startPos, movePos, timeElapsed / moveLerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = movePos;
        IsMoving = false;
    }
    
    IEnumerator WallHitEnum(Vector3Int moveDir)
    {
        const float wallHitLerpDuration = 0.06f;
        
        IsMoving = true;
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        Vector3 hitPos = startPos + (Vector3)moveDir * 0.1f;
        while (timeElapsed < wallHitLerpDuration)
        {
            transform.position = Vector3.Lerp(startPos, hitPos, timeElapsed / wallHitLerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        timeElapsed = 0;
        
        while (timeElapsed < wallHitLerpDuration)
        {
            transform.position = Vector3.Lerp(hitPos, startPos, timeElapsed / wallHitLerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;
        IsMoving = false;
    }
}
