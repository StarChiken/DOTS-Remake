using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase floorTile;
    public TileBase redGoal;
    public TileBase blueGoal;
    public TileBase yellowGoal;
    public TileBase greenGoal;

    private Vector2Int redGoalPos;
    private Vector2Int blueGoalPos;
    private Vector2Int yellowGoalPos;
    private Vector2Int greenGoalPos;
    
    void Start()
    {
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
                    redGoalPos = pos;
                }
                else if (tile == blueGoal)
                {
                    blueGoalPos = pos;
                }
                else if (tile == yellowGoal)
                {
                    yellowGoalPos = pos;
                }
                else if (tile == greenGoal)
                {
                    greenGoalPos = pos;
                }
            }
        }
    }

    void Update()
    {
        
    }

    private void OnUp()
    {
        print("UP");
    }
    
    private void OnDown()
    {
        print("DOWN");
    }
    
    private void OnLeft()
    {
        print("LEFT");
    }
    
    private void OnRight()
    {
        print("RIGHT");
    }
}
