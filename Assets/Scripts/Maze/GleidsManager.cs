using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generate;
using Generate.MazeGen;
using Generate.Random;
using System.Linq;

namespace Generate
{
    public class GleidsManager : Generator
    {
        public GleidSettings gleidSettings { get; private set; }
        private Maze maze;
        public GleidsManager(GleidSettings gleidSettings, GenerateData generateData, Maze maze) : base(generateData)
        {
            this.gleidSettings = gleidSettings;
            this.maze = maze;
        }
        /// <summary>
        /// ������ ��������� ������� ������� � ������� ��������� ������ ������������� ���������
        /// </summary>
        public Vector2Int[] gleidStartPositions { get => startPositions; private set => startPositions = value; }
        private Vector2Int[] startPositions;
        /// <summary>
        /// ������ �������� ������� � ������� ��������� ������ ������������� ���������
        /// </summary>
        public Vector2Int[] gleidsSizes { get => _gleidsSizes; private set => _gleidsSizes = value; }
        private Vector2Int[] _gleidsSizes;
        /// <summary>
        /// ���������� ������ � ��������� maze. �������� ������ � ����� ������, ����� ������ ��� �� �������������. ����� ���������� ��������� �������� UpdateGleids ��� ����� �������
        /// ������������������
        /// </summary>
        /// <param name="maze">��������</param>
        public void GenerateGleids()
        {
            GetGleidsStartInfo(maze.mazeArray, out startPositions, out _gleidsSizes);
            startPositions.ToList().ForEach(pos => Debug.Log(pos));
            for (int i = 0; i < startPositions.Length; i++)
            {
                //-1 ������ ��� ��� ����, ����� ������� ������ ����� ������ ����� ��������� ����� � ���� ������, ����������� ��� �������,
                //�.�. � ������ ���� ������ topWall � leftWall
                //+1 ��� �������� ���������� ������� ������ ������
                for (int y = -1; y < gleidsSizes[i].y + 1; y++)
                {
                    for (int x = -1; x < gleidsSizes[i].x + 1; x++)
                    {
                        Cell cell;
                        if (maze.GetCellAtPositionInMaze(startPositions[i] + new Vector2Int(x, y), out cell))
                        {
                            RemoveWalls(cell, new Vector2Int(x, y), gleidsSizes[i]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// ��������� ������ � ���������
        /// </summary>
        /// <param name="transformDistance">���������� �� ������� ��������� �������� ������������ ����������� ����������</param>
        public void UpdateGleids(Vector2Int transformDistance)//��������
        {

        }
        /// <summary>
        /// ���������� ������� ����� � ������� ������� � ������� ���� �� ���� ������ ��������� � ������������ ���������
        /// </summary>
        /// <param name="mazeArray"></param>
        /// <param name="startPositions">������ ������� ����� �������</param>
        /// <param name="sizes">������ �������� �������</param>
        private void GetGleidsStartInfo(Cell[,] mazeArray, out Vector2Int[] startPositions, out Vector2Int[] sizes)
        {
            List<Vector2Int> positionsList = new List<Vector2Int>();
            List<Vector2Int> sizesList = new List<Vector2Int>();
            //�������� ���� 1, ���� -1
            //int multiplerX = mazeArray[0, mazeArray.GetLength(1) - 1].positionInMaze.x / Mathf.Abs(mazeArray[0, mazeArray.GetLength(1) - 1].positionInMaze.x);
           // int multiplerY = mazeArray[mazeArray.GetLength(0) - 1, 0].positionInMaze.y / Mathf.Abs(mazeArray[mazeArray.GetLength(0) - 1, 0].positionInMaze.y);
            //������� ������� ������ ������, ���������� � ������ ��������� � ����������� ������������

            Vector2Int startPositionGleidChangesCount = new Vector2Int(mazeArray[0, 0].positionInMaze.x / gleidSettings.gleidChange.x
           // Vector2Int startPosition = new Vector2Int(mazeArray[0, 0].positionInMaze.x / gleidSettings.gleidChange.x
                * gleidSettings.gleidChange.x, mazeArray[0, 0].positionInMaze.y / gleidSettings.gleidChange.y
                * gleidSettings.gleidChange.y);
            //���� ��������� ������� ��������� < 0, �� ��������� ������� �� ����� ������� ������ gleidChange.x/.y. ������� �������� x = -13. 
            //-13 / 10(gleidChange.x) = -1, � ������ ���� -2
            //startPosition.x += gleidSettings.gleidChange.x * (mazeArray[0, 0].positionInMaze.x < 0 ? -1 : 0);
            //startPosition.y += gleidSettings.gleidChange.y * (mazeArray[0, 0].positionInMaze.y < 0 ? -1 : 0);
            //�������, ������� �� �������� � ���������
            //���� ������ � ������� ���� ������� � ������������ ������� ���������
            //���� ������� ������� �� ����� �� ������� ������������� ��������� �� y
            /*while (nearestPosition.y <= mazeArray[mazeArray.GetLength(0) - 1, 0].positionInMaze.y)
            {
                //���� ������� ������� �� ����� �� ������� ������������� ��������� �� x
                while (nearestPosition.x <= mazeArray[0, mazeArray.GetLength(1) - 1].positionInMaze.x)
                {
                    //���� �� ���������� ������ ����� ��� ������ ������� ��������� � ������ ���������� � ����� ������
                    if (!gleidSettings.isRandomAppear || RandomValuesGenerator.RandomChoice(nearestPosition, generateData.seed, RandomValuesGenerator.GenBoolType.gleidExistence, mazeArray.GetLength(1)))
                    {
                        sizesList.Add(RandomValuesGenerator.RandomVector2Int(nearestPosition, gleidSettings.minGleidSize, gleidSettings.maxGleidSize, generateData,
                            RandomValuesGenerator.GenVectorType.GleidSize));
                        positionsList.Add(nearestPosition + (gleidSettings.isRandomAdd ? RandomValuesGenerator.RandomVector2Int(nearestPosition, Vector2Int.zero, gleidSettings.addRange, generateData, RandomValuesGenerator.GenVectorType.GleidStartPositionAdd) : Vector2Int.zero));
                    }
                    nearestPosition.x += gleidSettings.gleidChange.x;
                }
                nearestPosition.y += gleidSettings.gleidChange.y;
                nearestPosition.x = startPosition.x;
            }*/
            int maxY = Mathf.Abs(mazeArray[mazeArray.GetLength(0) - 1, 0].positionInMaze.y) / gleidSettings.gleidChange.y + 1;
            int maxX = Mathf.Abs(mazeArray[0, mazeArray.GetLength(1) - 1].positionInMaze.x) / gleidSettings.gleidChange.x + 1;
            maxY -= startPositionGleidChangesCount.y;
            maxX -= startPositionGleidChangesCount.x;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    Vector2Int gleidPos = /*startPosition + */new Vector2Int(x * gleidSettings.gleidChange.x, y * gleidSettings.gleidChange.y/* * multiplerY*/);
                    //���� �� ���������� ������ ����� ��� ������ ������� ��������� � ������ ���������� � ����� ������
                    if (!gleidSettings.isRandomAppear || RandomValuesGenerator.RandomChoice(gleidPos, generateData.seed, RandomValuesGenerator.GenBoolType.gleidExistence, mazeArray.GetLength(1)))
                    {
                        sizesList.Add(RandomValuesGenerator.RandomVector2Int(gleidPos, gleidSettings.minGleidSize, gleidSettings.maxGleidSize, generateData,
                            RandomValuesGenerator.GenVectorType.GleidSize));
                        positionsList.Add(gleidPos + (gleidSettings.isRandomAdd ? RandomValuesGenerator.RandomVector2Int(gleidPos, Vector2Int.zero, gleidSettings.addRange, generateData, RandomValuesGenerator.GenVectorType.GleidStartPositionAdd) : Vector2Int.zero));
                    }
                }
            }
            startPositions = positionsList.ToArray();
            sizes = sizesList.ToArray();
        }
        /// <summary>
        /// ������� ������ ������������ �� ������� � ���������
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="positionInGleid">������� ������ � ���������</param>
        /// <param name="gleidSize">������ ������</param>
        private void RemoveWalls(Cell cell, Vector2Int positionInGleid, Vector2Int gleidSize)
        {
            //���� ������ ����� ����� � ������ ��� ������ ��� � ��� ���� ��������� � �������� ������ �� y, �������� ����� �����
            if ((positionInGleid.x == 0 || positionInGleid.x == gleidSize.x) && positionInGleid.y >= 0 &&
                positionInGleid.y <= gleidSize.y - 1)
            {
                cell.leftWall = true;
            }
            //���� ������ ��� ������� ��� ����� ������ � ������ � ��� ���� ��������� � �������� ������ �� x, �������� ������� �����
            if ((positionInGleid.y == -1 || positionInGleid.y == gleidSize.y - 1) && positionInGleid.x >= 0 && positionInGleid.x <= gleidSize.x - 1)
            {
                cell.topWall = true;
            }
            //���� ������ ��������� ������ ������, �� ���� ����� ������� �������, ������� ������� �����
            if (positionInGleid.x >= 0 && positionInGleid.x <= gleidSize.x - 1 && positionInGleid.y >= 0 && positionInGleid.y <= gleidSize.y - 2)
            {
                cell.topWall = false;
            }
            //���� ������ ��������� � ������, �� ����, ����� ����� �������, ������� ����� �����
            if (positionInGleid.x >= 1 && positionInGleid.x <= gleidSize.x - 1 && positionInGleid.y >= 0 && positionInGleid.y <= gleidSize.y - 1)
            {
                cell.leftWall = false;
            }
            if (gleidSettings.isDeleteBroad)
            {
                //���� ������ ����� ��� ������ �� ������ � ��� ���� ��������� � �������� ������ �� y, ������� ������� �����
                if ((positionInGleid.x == -1 || positionInGleid.x == gleidSize.x) && positionInGleid.y >= -1 && positionInGleid.y <= gleidSize.y - 1)
                {
                    cell.topWall = false;
                }
                //���� ������ ����� ��� ������ �� ������ � ��� ���� ��������� � �������� ������ �� x, ������� ����� �����
                if ((positionInGleid.y == -1 || positionInGleid.y == gleidSize.y) && positionInGleid.x >= 0 && positionInGleid.x <= gleidSize.x)
                {
                    cell.leftWall = false;
                }
            }
        }
        private void MakeEntry()
        {

        }
    }
}