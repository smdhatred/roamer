using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeController : MonoBehaviour
    {
        [SerializeField] private Escape escapePrefab;

        [SerializeField] private GridSpawner gridSpawner;

        [SerializeField] private MazeGenerator mazeGenerator;

        private CustomGrid gridXY;

        private void Start()
        {
            gridXY = gridSpawner.grid;

            SpawnEscape();
            //SpawnMaze();
        }
        private void SpawnMaze()
        {
            mazeGenerator.SpawnMaze();
        }

        private void SpawnEscape()
        {
            Vector3 escapePosition = gridXY.GetEscapeWorldPosition(Random.Range(1, 10), Random.Range(1, 10));

            Instantiate(escapePrefab, escapePosition, Quaternion.identity, transform);
        }
    }
}