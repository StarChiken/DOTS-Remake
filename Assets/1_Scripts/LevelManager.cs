using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [Header("Dots")]
    public Dot[] dots;
    [Range(0, 1f)]
    public float replayWaitTime;
    
    [Header("Tilemap")]
    public Tilemap tilemap;
    public TileBase floorTile;
    public TileBase redGoal;
    public TileBase blueGoal;
    public TileBase yellowGoal;
    public TileBase greenGoal;
    
    private bool canMove = false;
    
    private int currentRound = 0;
    private int currentMove = 0;
    
    private Vector2Int[] goalPositions;
    
    private List<Vector3Int>[] dotReplayPositions;
    
    void Start()
    {
        canMove = true;
        
        dots[0].SetSelected(true);
        
        dotReplayPositions = new List<Vector3Int>[dots.Length];
        for (int i = 0; i < dotReplayPositions.Length; i++)
            dotReplayPositions[i] = new List<Vector3Int>();
        
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

    private void MoveCurrentDot(Vector3Int moveDir)
    {
        if (!canMove)
            return;

        if (dots[currentRound].IsMoving)
            return;
        
        Vector3Int nextPos = (Vector3Int)dots[currentRound].Position + moveDir;
        if (tilemap.HasTile(nextPos))
        {
            dots[currentRound].TryMoveDot(true, (Vector2Int)moveDir);
            dotReplayPositions[currentRound].Add(moveDir);

            for (int i = 0; i < currentRound; i++)
            {
                if (currentMove >= dotReplayPositions[i].Count)
                    continue;
                dots[i].TryMoveDot(true, (Vector2Int)dotReplayPositions[i][currentMove]);
            }
            
            currentMove++;
        }
        else
        {
            dots[currentRound].TryMoveDot(false, (Vector2Int)moveDir);
        }

        foreach (var dot in dots)
        {
            if (dots[currentRound] == dot)
                continue;

            if (dots[currentRound].Position == dot.Position)
            {
                // Reset Level
                print("RESET LEVEL");
                ResetLevel();
            }
        }
        
        if (dots[currentRound].Position == goalPositions[currentRound])
        {
            print("GOAL REACHED");
            if (currentRound == dots.Length - 1)
            {
                print("END OF LEVEL");
                canMove = false;
                // Play End of Level Replay
                StartCoroutine(ReplayMoves());
            }
            else
            {
                print("NEXT ROUND");
                
                StartCoroutine(ResetDots());
                
                dots[currentRound].SetSelected(false);
                dots[currentRound + 1].SetSelected(true);
                
                currentMove = 0;
                currentRound++;
            }
        }
    }

    private void ResetLevel()
    {
        dotReplayPositions = new List<Vector3Int>[dots.Length];
        for (int i = 0; i < dotReplayPositions.Length; i++)
        {
            dotReplayPositions[i] = new List<Vector3Int>();
            dots[i].SetSelected(false);
        }
        
        currentMove = 0;
        currentRound = 0;
        
        dots[0].SetSelected(true);
        
        StartCoroutine(ResetDots());
    }

    private IEnumerator ResetDots()
    {
        canMove = false;
        yield return new WaitForSeconds(replayWaitTime);
        
        for (int i = 0; i < currentRound; i++)
            dots[i].MoveToStartPos();
        
        yield return new WaitForSeconds(replayWaitTime);
        
        canMove = true;
    }
    
    private IEnumerator ReplayMoves()
    {
        dots[currentRound].SetSelected(false);
     
        yield return new WaitForSeconds(replayWaitTime);

        foreach (var dot in dots)
        {
            dot.MoveToStartPos();
            dot.SetSelected(false);
        }
        
        yield return new WaitForSeconds(replayWaitTime);
        
        for (int i = 0; i < currentMove; i++)
        {
            for (int j = 0; j < dots.Length; j++)
            {
                if (i >= dotReplayPositions[j].Count)
                    continue;
                dots[j].TryMoveDot(true, (Vector2Int)dotReplayPositions[j][i]);
            }
            yield return new WaitForSeconds(replayWaitTime);
        }
        
        //Load Next Level
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
