using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [Header("Dots")]
    public Dot[] dots;
    
    [Header("Tilemap")]
    public Tilemap tilemap;
    public TileBase floorTile;
    public TileBase redGoal;
    public TileBase blueGoal;
    public TileBase yellowGoal;
    public TileBase greenGoal;

    private int currentDotIndex = 0;
    
    private Vector2Int[] goalPositions;
    
    private List<Vector3Int>[] dotReplayPositions;
    
    void Start()
    {
        dotReplayPositions = new List<Vector3Int>[dots.Length];
        goalPositions = new Vector2Int[dots.Length];
        SetGoalPositions();
    }
    
    private void SetGoalPositions()
    {
        BoundsInt cellBounds = tilemap.cellBounds;
        for (int i = cellBounds.size.y; i >= 0; i--)
        {
            for (int j = 0; j <  cellBounds.size.x; j++)
            {
                Vector2Int pos = new Vector2Int(j + cellBounds.position.x, i + cellBounds.position.y);

                if (!tilemap.HasTile((Vector3Int)pos))
                    continue;
                
                TileBase tile = tilemap.GetTile((Vector3Int)pos);
                if (tile == redGoal)
                {
                    goalPositions[0] = pos;
                }
                else if (tile == blueGoal)
                {
                    goalPositions[1] = pos;
                }
                else if (tile == yellowGoal)
                {
                    goalPositions[2] = pos;
                }
                else if (tile == greenGoal)
                {
                    goalPositions[3] = pos;
                }
            }
        }
    }

    void Update()
    {
        
    }

    private void MoveCurrentDot(Vector3Int moveDir)
    {
        Vector3Int nextPos = (Vector3Int)dots[currentDotIndex].Position + moveDir;
        dots[currentDotIndex].TryMoveDot(tilemap.HasTile(nextPos), (Vector2Int)moveDir);

        foreach (var dot in dots)
        {
            if (dots[currentDotIndex] == dot)
                continue;

            if (dots[currentDotIndex].Position == dot.Position)
            {
                // Reset Level
            }
        }
        
        if (dots[currentDotIndex].Position == goalPositions[currentDotIndex])
        {
            print("GOAL REACHED");
            if (currentDotIndex == dots.Length - 1)
            {
                print("END OF LEVEL");
                // Play End of Level Replay
                // Load Next Level
            }
            else
            {
                currentDotIndex++;
            }
        }
    }
    
    private void OnUp()
    {
        print("UP");
        MoveCurrentDot(Vector3Int.up);
    }
    
    private void OnDown()
    {
        print("DOWN");
        MoveCurrentDot(Vector3Int.down);
    }
    
    private void OnLeft()
    {
        print("LEFT");
        MoveCurrentDot(Vector3Int.left);
    }
    
    private void OnRight()
    {
        print("RIGHT");
        MoveCurrentDot(Vector3Int.right);
    }
}
