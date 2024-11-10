using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dot : MonoBehaviour
{
    [SerializeField] private float moveSeconds = 0.15f;
    [SerializeField] private float bonkSeconds = 0.06f;
    [SerializeField] private TileBase goalTile;
    [SerializeField] private Sprite autoSprite;
    [SerializeField] private Sprite selectedSprite;

    public bool IsMoving { get; private set; }
    public Vector2Int Position { get; private set; }
    public TileBase GoalTile { get { return goalTile; } }

    public Action ReachedGoal;

    private bool isSelected;
    private bool reachedGoal;

    private Vector3 startPos;

    private Pusher pusherScript;
    private Teleporter teleporterScript;
    
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        Position = Vector2Int.RoundToInt(transform.position);
        startPos = transform.position;
    }

    public void SetSelected(bool _isSelected)
    {
        isSelected = _isSelected;
        spriteRenderer.sprite = _isSelected ? selectedSprite : autoSprite;
    }
    
    public void MoveToStartPos()
    {
        Position = Vector2Int.RoundToInt(startPos);
        StartCoroutine(MoveDotAnimation((Vector3Int)Position));
    }

    private void TeleportDot(Vector2Int teleportPos)
    {
        Position = teleportPos;
        transform.position = new Vector3(Position.x, Position.y, 0);
    }
    
    //* This function will move the character without checking if the move is valid
    public void MoveDot(Vector2Int direction, TileBase nextTile, GameObject nextTileObject)
    {
        Position += direction;
        StartCoroutine(MoveDotAnimation((Vector3Int)Position));

        if (nextTile == goalTile)
        {
            reachedGoal = true;
            return;
        }

        if (nextTileObject == null)
            return;

        nextTileObject.TryGetComponent<Pusher>(out pusherScript);
        nextTileObject.TryGetComponent<Teleporter>(out teleporterScript);
    }

    public void BonkDot(Vector2Int direction)
    {
        StartCoroutine(WallBonkAnimation((Vector3Int)direction));
    }

    private IEnumerator MoveDotAnimation(Vector3Int movePos)
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

        if (pusherScript)
        {
            pusherScript.ActivatePusher();

            Position += pusherScript.Direction;
            StartCoroutine(MoveDotAnimation((Vector3Int)Position));
            
            pusherScript = null;
        }
        else if (teleporterScript)
        {
            teleporterScript.ActivateTeleporter();

            TeleportDot(teleporterScript.TeleportPosition);
            
            teleporterScript = null;
        }

        if (isSelected && reachedGoal)
        {
            reachedGoal = false;

            ReachedGoal?.Invoke();
        }

        IsMoving = false;
    }

    private IEnumerator WallBonkAnimation(Vector3Int moveDir)
    {        
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
