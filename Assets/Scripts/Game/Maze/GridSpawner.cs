using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class GridSpawner : MonoBehaviour
    {
        public int width;
        public int height;
        public int cellsize;
        public Vector3 gridSpawnPOS;
        [HideInInspector] public CustomGrid grid;

        private void Awake()
        {
            grid = new CustomGrid(width, height, cellsize, gridSpawnPOS);
        }
    }
}