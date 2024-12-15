using System.Collections.Generic;
using UnityEngine;

public class MazeTile : MonoBehaviour
{
        public GameObject mazeTileObject;
        public int mazeTileNumber;
        private BoxCollider _mazeTileCollider;
        
        public void SetMazeTileNumber(int number)
        {
                mazeTileNumber = number;
        }

        public void SetMazeTile(GameObject mazeTo)
        {
                mazeTileObject = mazeTo;
        }
        
        public void SpawnTileObjects(GameObject enemyObject, List<GameObject> obstacles)
        {
                var floorObj = transform.GetChild(0).gameObject;
                _mazeTileCollider = floorObj.GetComponent<BoxCollider>();
                
                var colliderBounds = _mazeTileCollider.bounds;
                var objCenter = floorObj.transform.position;
                
                float[] ranges = {
                        objCenter.x - colliderBounds.extents.x,
                        objCenter.x + colliderBounds.extents.x,
                        objCenter.z - colliderBounds.extents.z,
                        objCenter.z + colliderBounds.extents.z,
                };
                
                var randomX = Random.Range(ranges[0], ranges[1]);
                var randomZ = Random.Range(ranges[2], ranges[3]);
                var randomPos = new Vector3(randomX, 0.0f, randomZ);
                
                Instantiate(enemyObject, randomPos, Quaternion.identity);
                
                MazeGenerator.Instance.enemiesSpawned += 1;

                if (obstacles.Count == 0) return;
                
                var obstacleCountToSpawn = Random.Range(0, 6);
                
                for (var i = 0; i < obstacleCountToSpawn; i++)
                {
                        var rX = Random.Range(ranges[0], ranges[1]);
                        var rZ = Random.Range(ranges[2], ranges[3]);
                        var rP = new Vector3(rX, 0.0f, rZ);
                        var obstacle = Instantiate(obstacles[Random.Range(0, obstacles.Count)], rP, Quaternion.identity);
                        obstacle.transform.Rotate(0,Random.Range(0, 360),0);
                }
        }
}