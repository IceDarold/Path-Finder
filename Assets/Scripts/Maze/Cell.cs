using System;
using UnityEngine;

namespace Generate.MazeGen
{
    public class Cell
    {
        public Vector2Int positionInMaze { get; set; }
        public Vector3 globalPosition
        {
            get
            {
                return maze.mazeStartGlobalPosition + new Vector3(positionInMaze.x * size.x, size.y, positionInMaze.y * size.z);
            }
        }
        public bool isGleidCell;
        /// <summary>
        /// Есть ли у клетки верхняя стена
        /// </summary>
        public bool topWall { get; set; }
        /// <summary>
        /// Есть ли у клетки левая стена
        /// </summary>
        public bool leftWall { get; set; }
        public bool isCalculateTop { get; set; }
        /// <summary>
        /// Размер клетки
        /// </summary>
        public Vector3 size { get; private set; }
        /// <summary>
        /// Лабиринт, в котором находится клетка
        /// </summary>
        public Maze maze { get; private set; }
        public Cell(Vector2Int positionInMaze, Vector3 size, Maze maze)
        {
            this.positionInMaze = positionInMaze;
            topWall = true;
            leftWall = true;
            isCalculateTop = false;
            this.size = size;
            this.maze = maze;
        }
    }
}
