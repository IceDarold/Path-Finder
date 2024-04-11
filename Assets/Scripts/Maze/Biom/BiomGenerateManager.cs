using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generate.MazeGen;
using System;

namespace Generate.Bioms
{
    public class BiomGenerateManager
    {
        private BiomGenerateSettings _biomGenerateSettings;
        public BiomGenerateManager(BiomGenerateSettings biomGenerateSettings)
        {
            _biomGenerateSettings = biomGenerateSettings;
        }
        /// <summary>
        /// Ищет позицию квадрата с размером gridSize
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Возвращает массив из позиций 4 точек искомого квадрата</returns>
        private Vector2Int[] FindNearestSquarePositions(Vector2Int position, Vector2Int gridSize)
        {
            Vector2Int[] positionList = new Vector2Int[4];
            //Левая нижняя
            positionList[0] = new Vector2Int(position.x / _biomGenerateSettings.gridSize.x, position.y / gridSize.y * gridSize.y);
            //Левая верхняя
            positionList[1] = new Vector2Int(position.x / gridSize.x * gridSize.x , (position.y / gridSize.y + 1) * gridSize.y);
            //Правая нижняя
            positionList[2] = new Vector2Int((position.x / gridSize.x + 1) * gridSize.x, position.y / gridSize.y * gridSize.y);
            //Правая верхняя
            positionList[3] = new Vector2Int((position.x / gridSize.x + 1) * gridSize.x, (position.y / gridSize.y + 1) * gridSize.y);
            return positionList;
        }
        public BiomType GetBiomType(Cell cell)
        {
            return BiomType.Forest;
        }
        public void RenderBiom(Cell cell)
        {
            switch (GetBiomType(cell))
            {
                case BiomType.Forest:
                    break;
                case BiomType.Sand:
                    break;
                default:
                    break;
            }
        }
        public void UpdateBiom(Maze maze)
        {
            foreach(var item in maze.mazeArray)
            {
                RenderBiom(item);
            }
        }
    }
}
