using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public enum TileType
    {
        Floor, Wall
    }

    public class Index : MonoBehaviour
    {
        public int x;
        public int y;
    }

    public class Tile : MonoBehaviour
    {
        public TileType Type;
        public Vector3 Position;
        public (int x, int y) Index;
    }

    public class LevelGeneration : MonoBehaviour
    {
        [SerializeField] private int columns = 12;
        [SerializeField] private int rows = 8;
        [SerializeField] private GameObject[] floorTiles;
        [SerializeField] private GameObject[] wallTiles;
        [SerializeField] private Radiation radiationPrefab;

        public Action EndGeneration;

        private Vector2 tileSize;
        private GameObject currentLevel;
        private Tile[,] grid;

        public int Level { get; set; }

        public void GenerateMap(int barrels, int columns, int rows)
        {
            if (currentLevel != null)
            {
                Destroy(currentLevel);
            }

            currentLevel = new GameObject("Level");
            currentLevel.transform.SetParent(transform);

            grid = new Tile[columns + 1, rows + 1];
            
            BoardSetup(columns, rows);
            MakeAnExit();
            
            for (int i = 0; i < barrels; i++)
            {
                SpawnBarrel();
            }
            
            CenterMap();
            var floorTrfm = floorTiles[0]?.transform;
            if (floorTrfm != null) tileSize = (Vector2)floorTrfm.localScale;
        }

        private void BoardSetup(int _columns, int _rows)
        {
            for (int x = 0; x < _columns + 1; x++)
            {
                for (int y = 0; y < _rows + 1; y++)
                {
                    var tileType = TileType.Floor;
                    GameObject toInstantiate = floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)];
                    
                    if (x == 0 || x == _columns || y == 0 || y == _rows)
                    {
                        tileType = TileType.Wall;
                        toInstantiate = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
                    }

                    GameObject instance = Instantiate(toInstantiate, currentLevel.transform);
                    instance.transform.position = new Vector3(x, y, 0f);
                    
                    var tile = instance.AddComponent<Tile>();
                    tile.Index = (x, y);
                    tile.Type = tileType;
                    tile.Position = instance.transform.position;

                    var i = instance.AddComponent<Index>();
                    i.x = x;
                    i.y = y;

                    grid[x, y] = tile;
                }
            }
        }

        private void CenterMap()
        {
            var shiftX = columns / 2f;
            var shiftY = rows / 2f;
            currentLevel.transform.position -= new Vector3(shiftX, shiftY, 0f);
            for (int x = 0; x < columns + 1; x++)
            {
                for (int y = 0; y < rows + 1; y++)
                {
                    grid[x, y].Position -= new Vector3(shiftX, shiftY, 0f);
                }
            }

            EndGeneration?.Invoke();
        }

        private Tile GetRandomTile(TileType type)
        {
            var list = new List<Tile>();
            for (int i = 0; i < columns + 1; i++)
            {
                for (int j = 0; j < rows + 1; j++)
                {
                    var tile = grid[i, j];
                    if (tile != null && tile.Type == type)
                    {
                        list.Add(tile);
                    }
                }
            }

            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        private Tile GetCurrentTile(Vector2 position)
        {
            for (int i = 0; i < columns + 1; i++)
            {
                for (int j = 0; j < rows + 1; j++)
                {
                    var tile = grid[i, j];
                    var tilePos = (Vector2)tile.Position;
                    var difX = Mathf.Abs(position.x - tilePos.x);
                    var difY = Mathf.Abs(position.y - tilePos.y);
                    
                    if (difX <= tileSize.x && difY <= tileSize.y)
                    {
                        return tile;
                    }
                }
            }

            return null;
        }

        public Tile GetEnemyTile()
        {
            return grid[columns - 1, rows - 1];
        }

        public MovePosition GetPositionsToMoveEnemies(Tile attachedTile)
        {
            var movePosition = new MovePosition();
            movePosition.Center.Position = attachedTile.Position;

            if (attachedTile.Index.x + 1 < columns)
            {
                movePosition.Right.Position = grid[attachedTile.Index.x + 1, attachedTile.Index.y].Position;
            }
            else
            {
                movePosition.Right.isAvailable = false;
            }

            if (attachedTile.Index.x - 1 > 0)
            {
                movePosition.Left.Position = grid[attachedTile.Index.x - 1, attachedTile.Index.y].Position;
            }
            else
            {
                movePosition.Left.isAvailable = false;
            }

            if (attachedTile.Index.y + 1 < rows)
            {
                movePosition.Up.Position = grid[attachedTile.Index.x, attachedTile.Index.y + 1].Position;
            }
            else
            {
                movePosition.Up.isAvailable = false;
            }

            if (attachedTile.Index.y - 1 > 0)
            {
                movePosition.Down.Position = grid[attachedTile.Index.x, attachedTile.Index.y - 1].Position;
            }
            else
            {
                movePosition.Down.isAvailable = false;
            }

            return movePosition;
        }

        public Vector2 GetNeighboringTilePosition(Vector2 position, Vector2 direction)
        {
            //print($"pos: {position}, dir {direction}");
            var currentTile = GetCurrentTile(position);
            if (currentTile)
            {
                currentTile.GetComponent<SpriteRenderer>().color = Color.red;
                var index = currentTile.Index;
                var intX = direction.x > 0f ? 1 : -1;
                var intY = direction.y > 0f ? 1 : -1;
                var x = index.x + intX;
                var y = index.y + intY;

                print($"{x},{y}");

                if (x < columns && x > 0 && y < rows && y > 0)
                {
                    print($"gPos: {grid[x, y].Position}");
                    //grid[x, y].GetComponent<SpriteRenderer>().color = Color.red;
                    return grid[x, y].Position;
                }
            }

            return position;
        }

        private void MakeAnExit()
        {
            var tile = GetRandomTile(TileType.Wall);
            int i = 0;

            while (tile.Index == (0,0) ||
                   tile.Index == (0, rows) ||
                   tile.Index == (columns, 0) ||
                   tile.Index == (columns, rows)
                   && ++i < 100)
            {
                tile = GetRandomTile(TileType.Wall);
            }
            var exitLocation = tile.Index;
            print(exitLocation);

            tile.gameObject.tag = "Door";
            
            var col = tile.GetComponent<BoxCollider2D>();
            if (col) col.isTrigger = true;

            var sr = tile.gameObject.GetComponent<SpriteRenderer>();
            if (sr) sr.enabled = false;
        }

        //Задание 1
        private void SpawnBarrel()
        {
            var barrel = Instantiate(radiationPrefab, currentLevel.transform);
            var barrelLocation = (0, 0);
            var check = GetRandomTile(TileType.Floor);
            while (check.Index == (columns / 2 - 1, rows / 2 - 1) || 
                check.Index == (columns / 2, rows / 2 - 1) ||
                check.Index == (columns / 2 + 1, rows / 2 - 1) ||
                check.Index == (columns / 2 - 1, rows / 2) ||
                check.Index == (columns / 2, rows / 2) ||
                check.Index == (columns / 2 + 1, rows / 2) ||
                check.Index == (columns / 2 - 1, rows / 2 + 1) ||
                check.Index == (columns / 2, rows / 2 + 1) ||
                check.Index == (columns / 2 + 1, rows / 2 + 1) ||
                check.Index == (columns - 1, rows - 1) ||
                check.Index == (columns - 2, rows - 1) ||
                check.Index == (columns - 1, rows - 2) ||
                check.Index == (columns - 2, rows - 2))
            {
                check = GetRandomTile(TileType.Floor);
            }
            barrel.transform.position = check.Position;
        }
    }
}
