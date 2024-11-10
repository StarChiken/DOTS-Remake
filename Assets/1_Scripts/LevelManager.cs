using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [Header("Dots")]
    [SerializeField] private Dot[] dots;
    [Range(0, 1f)]
    [SerializeField] private float replayWaitSeconds;
    
    [Header("Assignment")]
    [SerializeField] private Tilemap tilemap;
    
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
        for (int i = 0; i < dots.Length; i++)
        {
            dotReplayPositions[i] = new List<Vector3Int>();
            dots[i].ReachedGoal += HandleReachedGoalEvent;
        }
    }

    private void MoveAllDots(Vector3Int moveDir)
    {
        if (!canMove)
            return;

        if (dots[currentRound].IsMoving)
            return;

        Vector3Int nextPos = (Vector3Int)dots[currentRound].Position + moveDir;
        
        if (tilemap.HasTile(nextPos))
        {
            MoveCurrentDot(moveDir, nextPos);

            for (int i = 0; i < currentRound; i++)
            {
                if (currentMove >= dotReplayPositions[i].Count)
                    continue;

                MoveAutoDot(i);
            }

            currentMove++;
        }
        else
        {
            dots[currentRound].BonkDot((Vector2Int)moveDir);
        }

        //* Checks if the player moved dot into another dot & resets level if they did
        foreach (var dot in dots)
        {
            if (dots[currentRound] == dot)
                continue;

            if (dots[currentRound].Position != dot.Position)
                continue;
            
            ResetLevel();
        }
    }

    private void MoveCurrentDot(Vector3Int moveDir, Vector3Int nextPos)
    {
        TileBase nextTile = tilemap.GetTile(nextPos);
        GameObject nextTileObject = tilemap.GetInstantiatedObject(nextPos);

        dots[currentRound].MoveDot((Vector2Int)moveDir, nextTile, nextTileObject);
        dotReplayPositions[currentRound].Add(moveDir);
    }

    private void MoveAutoDot(int i)
    {
        Vector3Int autoMoveDir = dotReplayPositions[i][currentMove];
        Vector3Int autoNextPos = (Vector3Int)dots[i].Position + autoMoveDir;

        TileBase autoNextTile = tilemap.GetTile(autoNextPos);
        GameObject autoNextTileObject = tilemap.GetInstantiatedObject(autoNextPos);

        dots[i].MoveDot((Vector2Int)autoMoveDir, autoNextTile, autoNextTileObject);
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
        yield return new WaitForSeconds(replayWaitSeconds);

        foreach (var dot in dots)
            dot.MoveToStartPos();
        
        yield return new WaitForSeconds(replayWaitSeconds);
        
        canMove = true;
    }
    
    private IEnumerator ReplayMoves()
    {
        dots[currentRound].SetSelected(false);
     
        yield return new WaitForSeconds(replayWaitSeconds);

        foreach (var dot in dots)
        {
            dot.MoveToStartPos();
            dot.SetSelected(false);
        }
        
        yield return new WaitForSeconds(replayWaitSeconds);
        
        for (int i = 0; i < maxMoves; i++)
        {
            for (int j = 0; j < dots.Length; j++)
            {
                if (i >= dotReplayPositions[j].Count)
                    continue;

                Vector3Int autoMoveDir = dotReplayPositions[j][i];
                Vector3Int autoNextPos = (Vector3Int)dots[j].Position + autoMoveDir;

                TileBase autoNextTile = tilemap.GetTile(autoNextPos);
                GameObject autoNextTileObject = tilemap.GetInstantiatedObject(autoNextPos);

                dots[j].MoveDot((Vector2Int)autoMoveDir, autoNextTile, autoNextTileObject);
            }
            yield return new WaitForSeconds(replayWaitSeconds);
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Scene nextScene = SceneManager.GetSceneByBuildIndex(nextSceneIndex);
        if (nextScene.IsValid())
        {
            print($"Loading Scene: {nextScene.name}");
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    private void HandleReachedGoalEvent()
    {
        print("GOAL REACHED");
        if (currentRound == dots.Length - 1)
        {
            print("END OF LEVEL");

            canMove = false;

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

    #region UNITY_INPUT_SYSTEM_EVENTS
    private void OnUp()
    {
        MoveAllDots(Vector3Int.up);
    }
    
    private void OnDown()
    {
        MoveAllDots(Vector3Int.down);
    }
    
    private void OnLeft()
    {
        MoveAllDots(Vector3Int.left);
    }
    
    private void OnRight()
    {
        MoveAllDots(Vector3Int.right);
    }
    #endregion
}
