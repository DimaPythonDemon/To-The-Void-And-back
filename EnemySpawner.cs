using UnityEngine;

namespace Assets.Scripts
{
    public class Direction
    {
        public Vector2 Position;
        public bool isAvailable = true;
    }

    public class MovePosition
    {
        public Direction Center = new Direction();
        public Direction Up = new Direction();
        public Direction Down = new Direction();
        public Direction Right = new Direction();
        public Direction Left = new Direction();
    }

    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private LevelGeneration levelGeneration;
        [SerializeField] private PlayerControl player;

        public void CreateEnemy(Tile tile)
        {
            var enemy = Instantiate(enemyPrefab, transform);
            enemy.transform.position = tile.Position;
            enemy.Init(levelGeneration.GetPositionsToMoveEnemies(tile), player);
        }
    }
}