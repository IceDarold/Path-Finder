using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generate.Random;

namespace Generate.MazeGen
{
    using static RandomValuesGenerator;
    public class Maze : Generator
    {
        /// <summary>
        /// �������� �� ��������
        /// </summary>
        public bool isBraid { get; private set; }
        /// <summary>
        /// ������ ������������� ���������
        /// </summary>
        public Vector2Int runTimeMazeSize { get; private set; }
        /// <summary>
        /// ���������� � ���������� ������������, � ������� ��������� ������� (0, 0) ���������
        /// </summary>
        public Vector3 mazeStartGlobalPosition { get; private set; }
        /// <summary>
        /// ������������ �� �������� � ������ ������?
        /// </summary>
        public bool isGenerate { get; private set; }
        /// <summary>
        /// ������ ������ � ������������ ���������
        /// </summary>
        public Cell runTimeStartCell { get; private set; }
        public Cell[,] mazeArray { get; private set; }
        /// <summary>
        /// ��������� �� � ������ ������ �������� � ����
        /// </summary>
        public Vector3 cellSize { get; private set; }
        //Render settings
        private CellPref prefab;
        public bool isRenderer { get; private set; }
        public CellPref[,] renderList { get; private set; }
        public Maze(GenerateData generateData, Vector3 mazeStartCoordinates, CellPref prefab, bool isBraid = false) : base(generateData)
        {
            mazeStartGlobalPosition = mazeStartCoordinates;
            this.prefab = prefab;
            this.isBraid = isBraid;
            cellSize = prefab.gameObject.transform.localScale;
            isGenerate = false;
        }
        /// <summary>
        /// ���������� ������������ ������� ���������
        /// </summary>
        /// <param name="runTimeMazeSize">������ ������������ ������� ���������</param>
        /// <param name="startLocalPosition">��������� ���������� ������ ������������ �������</param>
        /// <returns></returns>
        public bool Generate(Vector2Int runTimeMazeSize, Vector2Int startLocalPosition)
        {
            mazeArray = new Cell[runTimeMazeSize.y, runTimeMazeSize.x];
            if (isGenerate)
            {
                return false;
            }
            for (int y = 0; y < runTimeMazeSize.y; y++)
            {
                for (int x = 0; x < runTimeMazeSize.x; x++)
                {
                    Cell newCell = new Cell(new Vector2Int(x + startLocalPosition.x, y + startLocalPosition.y), cellSize, this);
                    mazeArray[y, x] = newCell;
                }
            }
            RecalculateMazeValues();
            //����� ���� ��������, �.�. � ������ RecalculateCellValues ����� ���������� � ���� �������, � ���� �� ������ ��� � ����� �����,
            //�� ������, � ������� ������� ������ �������, �� ����� �������. �� ����������������� ��� ����� �� ������, �.�. �������� �� ��
            for (int y = 0; y < runTimeMazeSize.y; y++)
            {
                for (int x = 0; x < runTimeMazeSize.x; x++)
                {
                    RecalculateCellValues(mazeArray[y, x]);
                }
            }
            return true;
        }//�������� ���������
        /// <summary>
        /// ������� ������� ������������� ��������� �� distance
        /// </summary>
        /// <param name="distance">���������, �� ������� ����� �������� ������������ ��������</param>
        public void TransformRunTimeMaze(Vector2Int distance)//�������� transform �� "��������"
        {
            for (int y = 0; y < runTimeMazeSize.y; y++)
            {
                for (int x = 0; x < runTimeMazeSize.x; x++)
                {
                    Vector2Int lastPositionInMaze = mazeArray[y, x].positionInMaze;
                    mazeArray[y, x] = new Cell(lastPositionInMaze + distance, cellSize, this);//���������� �� �������������� ������
                    RecalculateCellValues(mazeArray[y, x]);
                }
            }
            RecalculateMazeValues();
        }
        /// <summary>
        /// ���������� ������ �� ������ � �������� position, ���� ����� ������������ � ������������ ���������
        /// </summary>
        /// <param name="position"></param>
        /// <param name="cell">������ �� ������</param>
        /// <returns>���������� true, ���� ����� ������ ���� � ������������ ��������� � false ���� ���</returns>
        public bool GetCellAtPositionInMaze(Vector2Int position, out Cell cell)
        {
            bool isOk = true;
            try
            {
                cell = mazeArray[position.y - mazeArray[0, 0].positionInMaze.y, position.x - mazeArray[0, 0].positionInMaze.x];
               // return true;
            }
            catch (System.Exception)
            {
             //   Debug.Log("Return false");
                cell = null;
                isOk = false;
                //return false;
            }
            return isOk;
        }
        /// <summary>
        /// ��������� ��� ������ ������ ������ �� �� positionInMaze
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="walls">������������ �� �� ������������� ��������� ������� � ������� ����?</param>
        /// <param name="globalPosition">������������ �� �� ���������� ������� ������ � ���������</param>
        private void RecalculateCellValues(Cell cell, bool walls = true)
        {
            if (walls)
            {
                if (RandomChoice(cell.positionInMaze, generateData.seed, GenBoolType.sideCellsDeleteCheck, runTimeMazeSize.x))
                    cell.leftWall = false;
                //���� �� ��� ������� ������� � ���������� ������ ������, �� ����� ������ ����� �� ������
                if (cell.isCalculateTop)
                {
                    return;
                }
                //���� ��� ����� � ������ ������ ���������
                List<Vector2Int> _cellsSet = new List<Vector2Int>();
                if (cell.leftWall == false)//��� ��� �������� ����, ��� ���� ������ �� �������� ����� ����� ���������
                {
                    Vector2Int thisCellPositionLeft = cell.positionInMaze;
                    //Do while ������ ��� ���� � ������ ���� ����� �����, ���� ����� ������ ����������� ��� �����
                    do
                    {
                        thisCellPositionLeft.x -= 1;
                        _cellsSet.Add(thisCellPositionLeft);
                        //��������� 
                        Cell updateDataCell;
                        // Debug.Log(GetCellAtPositionInMaze(thisCellPositionLeft, out updateDataCell));
                        // Debug.Log(updateDataCell);
                        if (GetCellAtPositionInMaze(thisCellPositionLeft, out updateDataCell) && updateDataCell != null)
                        {
                            updateDataCell.isCalculateTop = true;
                        }
                    } while (RandomChoice(thisCellPositionLeft, generateData.seed, GenBoolType.sideCellsDeleteCheck, runTimeMazeSize.x));
                }
                //������������� ���������, ����� �������� ���� � ���������� ������
                _cellsSet.Reverse();
                _cellsSet.Add(cell.positionInMaze);
                cell.isCalculateTop = true;
                Vector2Int thisCellPositionRight = cell.positionInMaze + Vector2Int.right;
                while (RandomChoice(thisCellPositionRight, generateData.seed, GenBoolType.sideCellsDeleteCheck, runTimeMazeSize.x))
                {
                    _cellsSet.Add(thisCellPositionRight);
                    Cell updateDataCell;
                    if (GetCellAtPositionInMaze(thisCellPositionRight, out updateDataCell) && updateDataCell != null)
                    {
                        updateDataCell.isCalculateTop = true;
                    }
                    thisCellPositionRight.x += 1;
                }
                if (!isBraid)
                {
                    /* string positionList = "[ ";
                    foreach (var item in _cellsSet)
                    {
                        positionList += (item.ToString() + ", ");
                    }
                    positionList += "]";*/
                    Cell updateCell;
                    Vector2Int choicePosition = _cellsSet[RandomIndex(_cellsSet.ToArray(), generateData.seed, runTimeMazeSize.x)];
                    if (GetCellAtPositionInMaze(choicePosition, out updateCell) && updateCell != null)
                    {
                        updateCell.topWall = false;
                        updateCell.isCalculateTop = true;
                    }
                }
                else
                {
                    Debug.LogWarning("��������� ��������� ��������� ���� �� ������");
                }
            }
        }
        /// <summary>
        /// ��������� ��� ������ ���������: runTimeMazeSize, mazeStartGlobalPosition, isGenerate, runTimeStartCell, isRenderer
        /// </summary>
        private void RecalculateMazeValues()
        {
            if (mazeArray != null)
            {
                runTimeStartCell = mazeArray[0, 0];
                runTimeMazeSize = new Vector2Int(mazeArray.GetLength(1), mazeArray.GetLength(0));
                isGenerate = true;
            }
            else
            {
                isGenerate = false;
                runTimeStartCell = null;
                runTimeMazeSize = Vector2Int.zero;
            }
            if (renderList != null)
            {
                isRenderer = true;
            }
            else
            {
                isRenderer = false;
            }
        }
        /// <summary>������ ������� ������������� ��������� �� ������� � �������� newPosition ������(����� ��������� �� �����������) ������
        /// </summary>
        /// <param name="newPosition">������� ������(����� ��������� �� �����������) ������ ����� �������</param>
        public void EditRunTimeMazePosition(Vector2Int newPosition)//�������� Change �� "��������"
        {
            for (int y = 0; y < runTimeMazeSize.y; y++)
            {
                for (int x = 0; x < runTimeMazeSize.x; x++)
                {
                    mazeArray[y, x] = new Cell(newPosition + new Vector2Int(x, y), cellSize, this);
                    RecalculateCellValues(mazeArray[y, x]);
                }
            }
            RecalculateMazeValues();
        }
        /// <summary>
        /// �������� ������ ������������� ��������� ���, ��� ��������� ������ ���������� ������� ����� ��������� ������� �����
        /// </summary>
        /// <param name="newSize">����� ������. �� ������ ���� > 0 �� ������ ����������</param>
        /// <returns>���������� False, ���� ��������� ������ ����������</returns>
        public bool EditRunTimeMazeSize(Vector2Int newSize)
        {
            if (newSize.x <= 0 || newSize.y <= 0)
            {
                return false;
            }
            mazeArray = new Cell[newSize.y, newSize.x];
            runTimeMazeSize = newSize;
            for (int y = 0; y < newSize.y; y++)
            {
                for (int x = 0; x < newSize.x; x++)
                {
                    mazeArray[y, x] = new Cell(new Vector2Int(x, y), cellSize, this);
                    RecalculateCellValues(mazeArray[y, x]);
                }
            }
            RecalculateMazeValues();
            return true;
        }
        /// <summary>
        /// ������� ��������
        /// </summary>
        public void RemoveMaze(bool deleteRender = true)
        {
            mazeArray = null;
            runTimeMazeSize = Vector2Int.zero;
            RecalculateMazeValues();
        }
        /// <summary>
        /// ��������� ����������� ��������� � ����
        /// </summary>
        public void UpdateRender(bool topWallsOff = false, bool sideWallsOff = false)
        {
            if (renderList == null || renderList.GetLength(0) != runTimeMazeSize.y || renderList.GetLength(1) != runTimeMazeSize.x)
            {
                if (renderList != null)
                    DeleteRender();
                renderList = new CellPref[runTimeMazeSize.y, runTimeMazeSize.x];
            }
            for (int y = 0; y < runTimeMazeSize.y; y++)
            {
                for (int x = 0; x < runTimeMazeSize.x; x++)
                {
                    Vector3 position = mazeStartGlobalPosition + new Vector3(mazeArray[y, x].positionInMaze.x * prefab.transform.localScale.x, 0, mazeArray[y, x].positionInMaze.y * prefab.transform.localScale.z);
                    CellPref newCell;
                    if (renderList[y, x] == null)
                    {
                        newCell = MonoBehaviour.Instantiate(prefab, position, prefab.transform.localRotation);
                        renderList[y, x] = newCell;
                    }
                    else
                    {
                        newCell = renderList[y, x];
                        newCell.transform.position = position;
                    }
                    newCell._leftWall.SetActive(!sideWallsOff && mazeArray[y, x].leftWall);
                    newCell._topWall.SetActive(!topWallsOff && mazeArray[y, x].topWall);
                }
            }
            isRenderer = true;
        }
        /// <summary>
        /// ������� ����������� ��������� � ����
        /// </summary>
        public void DeleteRender()
        {
            if (!isRenderer) return;
            foreach (var item in renderList)
            {
                MonoBehaviour.Destroy(item.gameObject);
            }
            renderList = null;
            isRenderer = false;
        }
        /// <summary>
        /// ������ ��������� ���������� ��������� � ���������� ������������
        /// </summary>
        /// <param name="newCoordinates">����� ����������</param>
        public void TransformMazeStartGlobalPosition(Vector3 newCoordinates)//�������� transform �� "��������"
        {
            mazeStartGlobalPosition = newCoordinates;
            for (int y = 0; y < mazeArray.GetLength(0); y++)
            {
                for (int x = 0; x < mazeArray.GetLength(1); x++)
                {
                    RecalculateCellValues(mazeArray[y, x], false);
                }
            }
        }
        /// <summary>
        /// ���������� ������� ������� � ��������� �� ��� ���������� �����������. ���������� y �� ���������
        /// </summary>
        /// <param name="globalPosition"></param>
        /// <returns></returns>
        public Vector2Int GetPositionInMaze(Vector3 globalPosition)
        {
            return new Vector2Int(Mathf.FloorToInt((mazeStartGlobalPosition.x + globalPosition.x) / cellSize.x),
                Mathf.FloorToInt((mazeStartGlobalPosition.z + globalPosition.z) / cellSize.z));
        }
    }
}