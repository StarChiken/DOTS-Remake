using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dot : MonoBehaviour
{
    [SerializeField] private TileBase goalTile;
    [SerializeField] private Sprite autoSprite;
    [SerializeField] private Sprite selectedSprite;

    public bool IsMoving { get; private set; }
    public Vector2Int Position { get; private set; }
    public TileBase GoalTile { get { return goalTile; } }

    private Vector3 startPos;

    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        Position = Vector2Int.RoundToInt(transform.position);
        startPos = transform.position;
    }

    public void SetSelected(bool isSelected)
    {
        spriteRenderer.sprite = isSelected ? selectedSprite : autoSprite;
    }
    
    public void MoveToStartPos()
    {
        Position = Vector2Int.RoundToInt(startPos);
        StartCoroutine(MoveDotAnimation((Vector3Int)Position, 0.15f));
    }
    
    //* This function will move the character without checking if the move is valid
    public void TryMoveDot(bool isValidMove, Vector2Int direction)
    {
        if (isValidMove)
        {
            Position += direction;
            StartCoroutine(MoveDotAnimation((Vector3Int)Position, 0.15f));
        }
        else
        {
            StartCoroutine(WallBonkAnimation((Vector3Int)direction));
        }
    }

    IEnumerator MoveDotAnimation(Vector3Int movePos, float moveSeconds)
    {
        IsMoving = true;
        float timeElapsed = 0;
        Vector3 beginPos = transform.position;
        while (timeElapsed < moveSeconds)
        {
            transform.position = Vector3.Lerp(beginPos, movePos, timeElapsed / moveSeconds);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = movePos;
        IsMoving = false;
    }
    
    IEnumerator WallBonkAnimation(Vector3Int moveDir)
    {
        const float bonkSeconds = 0.06f;
        
        IsMoving = true;
        float timeElapsed = 0;
        
        Vector3 beginPos = transform.position;
        Vector3 bonkPos = beginPos + (Vector3)moveDir * 0.1f;
        
        while (timeElapsed < bonkSeconds)
        {
            transform.position = Vector3.Lerp(beginPos, bonkPos, timeElapsed / bonkSeconds);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        timeElapsed = 0;
        
        while (timeElapsed < bonkSeconds)
        {
            transform.position = Vector3.Lerp(bonkPos, beginPos, timeElapsed / bonkSeconds);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = beginPos;
        IsMoving = false;
    }
}
