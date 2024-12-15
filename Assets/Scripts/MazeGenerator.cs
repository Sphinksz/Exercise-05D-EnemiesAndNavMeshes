using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;

public class MazeGenerator : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject player;
    public List<MazeTile> mazeTiles;
    public GameObject enemyPrefab;
    public List<GameObject> obstacles;
    public TextMeshProUGUI enemyCount;
    
    const int N = 1;
    const int E = 2;
    const int S = 4;
    const int W = 8;

    Dictionary<Vector2, int> cell_walls = new();

    private const float TileSize = 10;
    public int width = 10;   // Width of map  
    public int height = 10;  // Height of map
    public Vector3 playerStartPos = new(2.91f, 1f, 4.6f);
    List<List<int>> map = new();
    public static MazeGenerator Instance;
    public int enemiesSpawned = 0;


    public TextMeshProUGUI GetEnemyCountText()
    {
        return enemyCount;
    }
    
    public int GetEnemyCount()
    {
        return enemiesSpawned;
    }

    public void SetEnemyCount(int count)
    {
        enemiesSpawned = count;
    }
    
    private void Start()
    {
        cell_walls[new Vector2(0, -1)] = N;
        cell_walls[new Vector2(1, 0)] = E;
        cell_walls[new Vector2(0, 1)] = S;
        cell_walls[new Vector2(-1, 0)] = W;

        MakeMaze();

        var p = Instantiate(player);
        p.transform.position = playerStartPos;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    private List<Vector2> CheckNeighbors(Vector2 cell, List<Vector2> unvisited)
    {
        // Returns a list of cell's unvisited neighbors

        return (from n in cell_walls.Keys where unvisited.IndexOf((cell + n)) != -1 select cell + n).ToList();
    }
    
    private void MakeMaze()
    {
        var unvisited = new List<Vector2>();
        var stack = new List<Vector2>();
        for (var i = 0; i < width; i++)
        {
            map.Add(new List<int>());
            for (var j = 0; j < height; j++)
            {
                map[i].Add(N | E | S | W);
                unvisited.Add(new Vector2(i, j));
            }
        }

        var current = new Vector2(0, 0);

        unvisited.Remove(current);

        while (unvisited.Count > 0) {
            var neighbors = CheckNeighbors(current, unvisited);

            if (neighbors.Count > 0)
            {
                
                var next = neighbors[Random.Range(0, neighbors.Count)];
                stack.Add(current);

                var dir = next - current;

                var currentWalls = map[(int)current.x][(int)current.y] - cell_walls[dir];

                var nextWalls = map[(int)next.x][(int)next.y] - cell_walls[-dir];

                map[(int)current.x][(int)current.y] = currentWalls;

                map[(int)next.x][(int)next.y] = nextWalls;

                current = next;
                unvisited.Remove(current);

            }
            else if (stack.Count > 0) { 
                current = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
            
            }
        }

        var currentTile = 0;
        for (var i = 0; i < width; i++)
        {
            
            for (var j = 0; j < height; j++)
            {
                var tile = Instantiate(tiles[map[i][j]], gameObject.transform, true);
                var spawnEnemy = Random.value < 0.5f;
                tile.transform.Translate(new Vector3 (j*TileSize, 0, i * TileSize));
                tile.name += " " + i + ' ' + j;
                tile.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
                var mazeT = tile.AddComponent<MazeTile>();
                mazeT.SetMazeTile(tile);
                mazeT.SetMazeTileNumber(currentTile);
                mazeTiles.Add(mazeT);
                if (currentTile != 0)
                {
                    if (spawnEnemy)
                    {
                        mazeT.SpawnTileObjects(enemyPrefab,obstacles);
                    }
                }
                currentTile++;
            }

        }
    }
}