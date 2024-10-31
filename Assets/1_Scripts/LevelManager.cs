using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [Header("Dots")]
    [SerializeField] private Dot[] dots;
    [Range(0, 1f)]
    [SerializeField] private float replayWaitTime;
    
    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase floorTile;
    
    private bool canMove = false;

    private int maxMoves = 0;
    private int currentRound = 0;
    private int currentMove = 0;
    
    private List<Vector3Int>[] dotReplayPositions;
    
    void Start()
    {
        canMove = true;
        
        dots[0].SetSelected(true);
        
        dotReplayPositions = new List<Vector3Int>[dots.Length];
        for (int i = 0; i < dotReplayPositions.Length; i++)
            dotReplayPositions[i] = new List<Vector3Int>();
    }

    private void MoveCurrentDot(Vector3Int moveDir)
    {
        if (!canMove)
            return;

        if (dots[currentRound].IsMoving)
            return;
        
        Vector3Int nextPos = (Vector3Int)dots[currentRound].Position + moveDir;

        bool validPos = tilemap.HasTile(nextPos) && (tilemap.GetTile(nextPos) == floorTile || tilemap.GetTile(nextPos) == dots[currentRound].GoalTile);

        if (validPos)
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
            
            if (tilemap.GetTile(nextPos) == dots[currentRound].GoalTile)
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

                    if (currentMove > maxMoves)
                        maxMoves = currentMove;
                    
                    currentMove = 0;
                    currentRound++;
                }
            }
        }
        else
        {
            dots[currentRound].TryMoveDot(false, (Vector2Int)moveDir);
        }

        foreach (var dot in dots)
        {
            if (dots[currentRound] == dot)
                continue;

            if (dots[currentRound].Position != dot.Position)
                continue;
            
            ResetLevel();
        }
    }

    private void ResetLevel()
    {
        print("RESET LEVEL");
        
        dots[currentRound].SetSelected(false);
        dots[0].SetSelected(true);
        
        maxMoves = 0;
        currentMove = 0;
        currentRound = 0;
        
        foreach (var positionList in dotReplayPositions)
            positionList.Clear();
        
        StartCoroutine(ResetDots());
    }

    private IEnumerator ResetDots()
    {
        canMove = false;
        yield return new WaitForSeconds(replayWaitTime);

        foreach (var dot in dots)
            dot.MoveToStartPos();
        
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
        
        for (int i = 0; i < maxMoves; i++)
        {
            for (int j = 0; j < dots.Length; j++)
            {
                if (i >= dotReplayPositions[j].Count)
                    continue;
                dots[j].TryMoveDot(true, (Vector2Int)dotReplayPositions[j][i]);
            }
            yield return new WaitForSeconds(replayWaitTime);
        }
        
        //TODO: Load Next Level
    }
    
    private void OnUp()
    {
        MoveCurrentDot(Vector3Int.up);
    }
    
    private void OnDown()
    {
        MoveCurrentDot(Vector3Int.down);
    }
    
    private void OnLeft()
    {
        MoveCurrentDot(Vector3Int.left);
    }
    
    private void OnRight()
    {
        MoveCurrentDot(Vector3Int.right);
    }
}
