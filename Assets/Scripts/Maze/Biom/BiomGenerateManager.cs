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
        /// ���� ������� �������� � �������� gridSize
        /// </summary>
        /// <param name="position"></param>
        /// <returns>���������� ������ �� ������� 4 ����� �������� ��������</returns>
        private Vector2Int[] FindNearestSquarePositions(Vector2Int position, Vector2Int gridSize)
        {
            Vector2Int[] positionList = new Vector2Int[4];
            //����� ������
            positionList[0] = new Vector2Int(position.x / _biomGenerateSettings.gridSize.x, position.y / gridSize.y * gridSize.y);
            //����� �������
            positionList[1] = new Vector2Int(position.x / gridSize.x * gridSize.x , (position.y / gridSize.y + 1) * gridSize.y);
            //������ ������
            positionList[2] = new Vector2Int((position.x / gridSize.x + 1) * gridSize.x, position.y / gridSize.y * gridSize.y);
            //������ �������
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
