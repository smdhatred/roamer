using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maze
{
    static class Extensions
    {
        public static IEnumerable<(int, T)> Enumerate<T>(
        this IEnumerable<T> input,
        int start = 0
        )
        {
            int i = start;
            foreach (var t in input)
                yield return (i++, t);
        }
    }

    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] GridSpawner gridSpawner;

        [Range(0, 100)]
        [SerializeField] int R_WallSpawnPercentage = 0;

        [Range(0, 100)]
        [SerializeField] int B_WallSpawnPercentage;

        [SerializeField] GameObject wall;
        [SerializeField] int wall_yvalue;

        private CustomGrid gridXY;
        private int max_set_val = 1;

        private void Start()
        {
            gridXY = gridSpawner.grid;

            StartCoroutine(SpawnMaze(30f));
        }

        private IEnumerator SpawnMaze(float respawnTime)
        {
            SpawnEllersMaze();
            yield return new WaitForSeconds(respawnTime);
            DestroyMaze();
            StartCoroutine(SpawnMaze(respawnTime));

        }

        public void DestroyMaze()
        {
            foreach (Transform child in this.gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void SpawnMaze()
        {
            SpawnEllersMaze();
        }

        private void SpawnEllersMaze()
        {
            List<(int[], int)> first_row = new List<(int[], int)>();

            for (int x = 0; x < gridSpawner.width; x++)
            {
                int[] coords = new int[] { x, 0 };
                var cellTup = (coords, max_set_val);

                first_row.Add(cellTup);

                max_set_val += 1;
            }

            PlaceWalls_SameDir(first_row, new Vector3(0, 0, 90));

            List<(int[], int)> row = first_row;
            for (int y = 0; y < gridSpawner.height; y++)
            {
                if (y == gridSpawner.height - 1)
                {
                    PlaceWalls_SameDir(IncreaseAxisRow(1, row), new Vector3(0, 0, 90));
                    FinalRow(row);
                    break;
                }

                var joined_row = JoinRow_VerticalWalls(row);

                BottomWalls(joined_row, out List<int[]> b_wall_loc);

                row = IncreaseAxisRow(1, EmptyCells_NewRow(joined_row, b_wall_loc));
            }
        }

        private List<(int[], int)> JoinRow_VerticalWalls(List<(int[], int)> row)
        {
            var row_copy = new List<(int[], int)>(row);

            PlaceWall_XY(Vector3.zero, row_copy[0].Item1[0], row_copy[0].Item1[1]);

            for (int i = 0; i < row_copy.Count; i++)
            {
                if (i + 1 < row_copy.Count)
                {
                    var curr_cell = row_copy[i];
                    var next_cell = row_copy[i + 1];
                    bool join = false;
                    bool sameSet = false;

                    if (curr_cell.Item2 == next_cell.Item2)
                    {
                        join = false;
                        sameSet = true;
                    }
                    else
                    {
                        int rand_num = UnityEngine.Random.Range(0, 101);

                        if (inRange_Inclusive(rand_num, 0, R_WallSpawnPercentage))
                        {
                            join = false;
                        }
                        else
                        {
                            join = true;
                        }
                    }

                    if (join == false)
                    {
                        var coord = next_cell.Item1;

                        PlaceWall_XY(Vector3.zero, coord[0], coord[1]);
                    }
                    else
                    {
                        var newCellTup = (next_cell.Item1, curr_cell.Item2);
                        row_copy[i + 1] = newCellTup;
                    }
                }
            }

            PlaceWall_XY(Vector3.zero, row_copy[row_copy.Count - 1].Item1[0] + 1, row_copy[row_copy.Count - 1].Item1[1]);

            return row_copy;
        }

        private void BottomWalls(List<(int[], int)> row, out List<int[]> bottomWallLocations)
        {
            var row_copy = new List<(int[], int)>(row);
            var bwall_cells = new List<(int[], int)>();
            var no_bwall_cells = new List<(int[], int)>();

            foreach (var set_list in RowSortedBySet(row_copy))
            {
                var shuffledSets = set_list.OrderBy(a => rng.Next()).ToList();

                foreach (var (i, cell) in shuffledSets.Enumerate())
                {
                    if (i == 0)
                    {
                        no_bwall_cells.Add(cell);
                        continue;
                    }

                    int rand_num = UnityEngine.Random.Range(0, 101);

                    if (inRange_Inclusive(rand_num, 0, B_WallSpawnPercentage))
                    {
                        bwall_cells.Add(cell);
                    }
                    else
                    {
                        no_bwall_cells.Add(cell);
                    }
                }
            }

            List<int[]> bWalls = new List<int[]>();
            foreach (var cell in bwall_cells)
            {
                bWalls.Add(cell.Item1);
            }
            bottomWallLocations = bWalls;


            foreach (var cellTup in IncreaseAxisRow(1, bwall_cells))
            {
                var coord = cellTup.Item1;
                PlaceWall_XY(new Vector3(0, 0, 90), coord[0], coord[1]);
            }
        }


        private List<(int[], int)> EmptyCells_NewRow(List<(int[], int)> row, List<int[]> empty_cells)
        {
            var new_row = new List<(int[], int)>(row);
            foreach (var (i, cell) in row.Enumerate())
            {
                int cellX = cell.Item1[0];
                int cellY = cell.Item1[1];

                foreach (var coord in empty_cells)
                {
                    int eCellX = coord[0];
                    int eCellY = coord[1];

                    if (cellX == eCellX && cellY == eCellY)
                    {
                        var new_cell_tup = (cell.Item1, max_set_val);
                        new_row[i] = new_cell_tup;
                        max_set_val += 1;
                    }
                }
            }

            return new_row;
        }

        private void FinalRow(List<(int[], int)> row)
        {
            PlaceWall_XY(Vector3.zero, row[0].Item1[0], row[0].Item1[1]);
            PlaceWall_XY(Vector3.zero, row[row.Count - 1].Item1[0] + 1, row[row.Count - 1].Item1[1]);
        }

        private List<(int[], int)> IncreaseAxisRow(int add_amount, List<(int[], int)> row)
        {
            var new_row = new List<(int[], int)>();

            foreach (var celltup in row)
            {
                var coords = celltup.Item1;
                var set = celltup.Item2;

                int x = coords[0];
                int y = coords[1] + add_amount;

                int[] new_coords = { x, y };

                var new_cell_tup = (new_coords, set);

                new_row.Add(new_cell_tup);
            }

            return new_row;
        }


        private static System.Random rng = new System.Random();

        private List<List<(int[], int)>> RowSortedBySet(List<(int[], int)> row)
        {
            return row
                .Select((x) => new { Value = x })
                .GroupBy(x => x.Value.Item2)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        private void PlaceWalls_SameDir(List<(int[], int)> row, Vector3 dir)
        {
            foreach (var cellTup in row)
            {
                var coords = cellTup.Item1;

                PlaceWall_XY(dir, coords[0], coords[1]);
            }
        }

        private void PlaceWall_XY(Vector3 dir, int x, int y)
        {
            Vector3 position = gridXY.GetWorldPosition(x, y);
            position.z = 0;

            GameObject.Instantiate(wall, position, Quaternion.Euler(dir), transform);
        }

        private bool inRange_Inclusive(int num, int minRange, int maxRange)
        {
            if (minRange <= num && num <= maxRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}