/*using UnityEngine;
using DataStructures;
using System.Collections.Generic;
using static Generate.Random;
using UnityEditor;

namespace Generate
{
    public class GenerateGleidManager : MonoBehaviour
    {
        [SerializeField] private GleidSettings gleidsSettings;
        public bool IsGleidCheck(Cell cell, Maze maze)
        {
            Vector2Int positionInGleid;
            cell.cellPref.GetComponent<MeshRenderer>().material.color = Color.white;
            Gleid gleid;
            if (!SearchNearestGleidStart(cell._positionInMaze, cell, maze, out positionInGleid, out gleid))
            {
                return false;
            }
            cell.cellType = Cell.CellType.GleidCell;
            RemoveWalls(cell, positionInGleid, gleid);
            return true;
        }
        public bool GenerateGleid(Cell cell, Maze maze)
        {
            Vector2Int positionInGleid;
            cell.cellPref.GetComponent<MeshRenderer>().material.color = Color.white;
            Vector2Int gleidSize;
            if (!SearchNearestGleidStart(cell._positionInMaze, cell, maze, out positionInGleid, out gleidSize))
            {
                return false;
            }
            cell.cellType = Cell.CellType.GleidCell;
            RemoveWalls(cell, positionInGleid, gleidSize);
            return true;
        }
        private bool SearchNearestGleidStart(Vector2Int _positionInMaze, Cell cell, Maze maze, out Vector2Int positionInGleid, out Gleid gleid)
        {
            //gleidFormule = _positionInMaze.x % gleidSize.x == gleidSize.x / 3 || _positionInMaze.y % gleidSize.y == gleidSize.x / 2 + 1;
            //gleidChange = (8, 8)
            //12 * * * * * - - - - - *  *  *  *  *
            //11 * * * * * - - - - - *  *  *  *  *
            //10 * * * * * - - - - - *  *  *  *  *
            //9  * * * * * - - - - - *  *  *  *  *
            //8  * * * * * - - - - - *  *  *  *  *
            //7  - - - - - - - - - - -  -  -  -  -
            //6  - - - - - - - - - - -  -  -  -  -
            //5  - - - - - - - - - - -  -  -  -  -
            //4  * * * * * - - - - - *  *  *  *  *
            //3  * * * * * - - - - - *  *  *  *  *
            //2  * * * * * - - - - - *  *  *  *  *
            //1  * * * * * - - - - - *  *  *  *  *
            //0  * * * * * - - - - - *  *  *  *  *
            //   0 1 2 3 4 5 6 7 8 9 10 11 12 13 14
            Vector2Int _nearestGleidPosition;
            bool isFits = CheckIsFits(Vector2Int.zero, _positionInMaze, new List<Vector2Int>(), maze, out positionInGleid, out gleid, 0);
            cell.cellPref.gleid = gleid;
            cell.cellPref.positionInGleid = positionInGleid;
            return isFits && gleidsSettings.isRandomAppear ? RandomChoice(gleid.StartPosition, maze.seed, GenType.checkGleid, maze._mazeSize.x) : isFits;
            //cell.cellPref.randomAdd = randomAdd;//Для теста
        }

        private void RemoveWalls(Cell cell, Vector2Int myPositionInGleid, Gleid gleid)
        {
            if (myPositionInGleid.x >= gleid.GleidSize.x || myPositionInGleid.x < 0 || myPositionInGleid.y >= gleid.GleidSize.y || myPositionInGleid.y < -1)
            {
                if (myPositionInGleid.y == -1)
                {
                }
                return;
            }
            Debug.LogWarning(myPositionInGleid);

            cell.cellPref.GetComponent<MeshRenderer>().material.color = Color.red;
            cell.cellPref._leftWall.SetActive(false);
            cell.cellPref._topWall.SetActive(false);
            //Если клетка самая левая
            if (myPositionInGleid.x == 0)
            {
                cell.cellPref._leftWall.SetActive(true);
                if (myPositionInGleid == Vector2Int.zero)
                {
                    cell.cellPref.GetComponent<MeshRenderer>().material.color = Color.green;
                }
            }
            //Если клетка самая правая
            if (myPositionInGleid.x == gleid.GleidSize.x)
            {
                cell.cellPref._leftWall.SetActive(true);
            }
            //Если клетка самая верхняя
            if (myPositionInGleid.y == gleid.GleidSize.y - 1)
            {
                cell.cellPref._topWall.SetActive(true);
            }
            //Если клетка самая нижняя
            if (myPositionInGleid.y == -1)
            {
                cell.cellPref.GetComponent<MeshRenderer>().material.color = Color.blue;
                cell.cellPref._topWall.SetActive(true);
                if (myPositionInGleid.x == 0)
                {
                    cell.cellPref._topWall.SetActive(false);
                }
            }
            return;
            if (myPositionInGleid.y == gleid.GleidSize.y)
            {
                cell.cellPref.GetComponent<MeshRenderer>().material.color = Color.gray;
            }
            if (myPositionInGleid.x == gleid.GleidSize.x)
            {
                cell.cellPref.GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
            if (myPositionInGleid == Vector2Int.zero)
            {
                cell.cellPref.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }

        private struct CheckedGleidPosition
        {
            public Gleid gleid;
            public Vector2Int position;
            public CheckedGleidPosition(Gleid gleid, Vector2Int position)
            {
                this.gleid = gleid;
                this.position = position;
            }
        }
        private bool isGleidFits(Vector2Int movementDirection, Vector2Int _positionInMaze, Maze maze, out Vector2Int positionInGleid, out Gleid gleid)
        {
            Vector2Int lastGleidPosition = SearchNearGleid(_positionInMaze);
            gleid = new Gleid(SearchNearGleid(_positionInMaze + new Vector2Int(lastGleidPosition.x + movementDirection.x, lastGleidPosition.y + movementDirection.y)), gleidsSettings, maze);
            //Если клетка находится в глейде
            if (_positionInMaze.x >= gleid.StartPosition.x && _positionInMaze.y >= gleid.StartPosition.y && _positionInMaze.x <= gleid.StartPosition.x + gleid.GleidSize.x && _positionInMaze.y <= nearestGleidPosition.y + gleidSize.y)
            {
                positionInGleid = new Vector2Int(_positionInMaze.x - gleid.StartPosition.x, _positionInMaze.y - gleid.StartPosition.y);
                return true;
            }
        }
        private bool CheckIsFits(Vector2Int movementDirection, Vector2Int _positionInMaze, List<Vector2Int> alreadyCheckList, Maze maze, out Vector2Int positionInGleid, out Gleid gleid, int count = 0)
        {
            if (count == 20)
            {
                Debug.LogError("Перебор!");
                Debug.Log($"{movementDirection}, {_positionInMaze}");
                alreadyCheckList.ForEach(item =>
                {
                    Vector2Int _comingPos = alreadyCheckList.IndexOf(item) == 0 ? Vector2Int.zero : alreadyCheckList[alreadyCheckList.IndexOf(item) - 1];
                    Debug.Log($"Шаг: {item} Позиция глейда: {new Vector2Int((_positionInMaze.x / gleidsSettings.gleidChange.x + _comingPos.x + movementDirection.x) * gleidsSettings.gleidChange.x, (_positionInMaze.y / gleidChange.y + _comingPos.x + movementDirection.y) * gleidChange.y)}"
                );
                });
            }
            count++;
            alreadyCheckList.Add((alreadyCheckList.Count == 0 ? Vector2Int.zero : alreadyCheckList[alreadyCheckList.Count - 1]) + movementDirection);
            Vector2Int _comingPosition = alreadyCheckList.Count == 1 ? Vector2Int.zero : alreadyCheckList[alreadyCheckList.Count - 2];

            gleid = new Gleid(SearchNearGleid(_positionInMaze + new Vector2Int(_comingPosition.x + movementDirection.x, _comingPosition.y + movementDirection.y)), gleidsSettings, maze);
            //Если клетка находится в глейде
            if (_positionInMaze.x >= gleid.StartPosition.x && _positionInMaze.y >= gleid.StartPosition.y && _positionInMaze.x <= gleid.StartPosition.x + gleid.GleidSize.x && _positionInMaze.y <= nearestGleidPosition.y + gleidSize.y)
            {
                positionInGleid = new Vector2Int(_positionInMaze.x - gleid.StartPosition.x, _positionInMaze.y - gleid.StartPosition.y);
                return true;
            }
            //Если клетка находится левее от начала глейда
            if (_positionInMaze.x < gleid.StartPosition.x && !alreadyCheckList.Contains(alreadyCheckList[alreadyCheckList.Count - 1] + Vector2Int.left))
            {
                return CheckIsFits(new Vector2Int(-1, 0), _positionInMaze, alreadyCheckList, maze, out positionInGleid, out gleid.StartPosition, out gleid.GleidSize, count);
            }
            //Если клетка находится точно под глейдом, по иксу она точно не меньше,тк есть верхний иф и не больше размера глейда
            if (_positionInMaze.y + 1 == gleid.StartPosition.y && _positionInMaze.x <= gleid.StartPosition.x + gleid.GleidSize.x)
            {
                positionInGleid = new Vector2Int(_positionInMaze.x - gleid.StartPosition.x, -1);
                return true;
            }
            Vector2Int nearTopGleidPosition = SearchNearGleid(_positionInMaze + Vector2Int.up);
            nearTopGleidPosition += RandomAdd(nearTopGleidPosition, maze.seed, maze._mazeSize.x, addRange);
            //Если клетка находится точно под верхним глейдом, по иксу она точно не меньше,тк есть верхний иф и не больше размера глейда
            if (_positionInMaze.y + 1 == nearTopGleidPosition.y && _positionInMaze.x <= nearTopGleidPosition.x + gleid.GleidSize.x && _positionInMaze.x >= gleid.StartPosition.x)
            {
                gleid.StartPosition = nearTopGleidPosition;
                positionInGleid = new Vector2Int(_positionInMaze.x - gleid.StartPosition.x, -1);
                return true;
            }
            //Если клетка находится ниже, но не ровно под глейдом
            if (_positionInMaze.y < gleid.StartPosition.y && !alreadyCheckList.Contains(alreadyCheckList[alreadyCheckList.Count - 1] - Vector2Int.up))
            {
                return CheckIsFits(new Vector2Int(0, -1), _positionInMaze, alreadyCheckList, maze, out positionInGleid, out gleid.StartPosition, out gleidSize, count);
            }
            //Если клетка находится сверху, но не левее начала глейда и глейд сверху мы уже проверяли
            if (gleid.StartPosition.y + gleid.GleidSize.y < _positionInMaze.y && !alreadyCheckList.Contains(alreadyCheckList[alreadyCheckList.Count - 1] + Vector2Int.up))
            {
                return CheckIsFits(Vector2Int.up, _positionInMaze, alreadyCheckList, maze, out positionInGleid, out gleid.StartPosition, out gleidSize, count);
            }
            //Если клетка находится справа, но не ниже начала глейда и глейд справа мы еще не проверяли
            if (gleid.StartPosition.x + gleid.GleidSize.x < _positionInMaze.x && !alreadyCheckList.Contains(alreadyCheckList[alreadyCheckList.Count - 1] + new Vector2Int(1, 0)))
            {
                return CheckIsFits(new Vector2Int(1, 0), _positionInMaze, alreadyCheckList, maze, out positionInGleid, out gleid, count);
            }
            positionInGleid = new Vector2Int(_positionInMaze.x - gleid.StartPosition.x, _positionInMaze.y - gleid.StartPosition.y);
            return false;
        }
        public Vector2Int SearchNearGleid(Vector2Int _positionInMaze) => new Vector2Int((_positionInMaze.x / gleidsSettings.gleidChange.x) * gleidsSettings.gleidChange.x, (_positionInMaze.y / gleidsSettings.gleidChange.y) * gleidsSettings.gleidChange.y);

    }
}*/