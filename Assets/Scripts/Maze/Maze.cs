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
        /// Плетеный ли лабиринт
        /// </summary>
        public bool isBraid { get; private set; }
        /// <summary>
        /// Размер прогружаемого лабиринта
        /// </summary>
        public Vector2Int runTimeMazeSize { get; private set; }
        /// <summary>
        /// Координаты в глобальном пространстве, в которых находится позиция (0, 0) лабиринта
        /// </summary>
        public Vector3 mazeStartGlobalPosition { get; private set; }
        /// <summary>
        /// Сгенерирован ли лабиринт в данный момент?
        /// </summary>
        public bool isGenerate { get; private set; }
        /// <summary>
        /// Первая клетка в отображаемом лабиринте
        /// </summary>
        public Cell runTimeStartCell { get; private set; }
        public Cell[,] mazeArray { get; private set; }
        /// <summary>
        /// Отображен ли в данный момент лабиринт в мире
        /// </summary>
        public Vector3 cellSize { get; private set; }
        //Render settings
        private CellPref prefab;
        public bool isRenderer { get; private set; }
        public CellPref[,] renderList { get; private set; }
        /// <summary>
        /// Parent object for cells
        /// </summary>
        private Transform _mazeParent;
        public Maze(GenerateData generateData, Vector3 mazeStartCoordinates, CellPref prefab, Transform mazeParent = null, bool isBraid = false) : base(generateData)
        {
            mazeStartGlobalPosition = mazeStartCoordinates;
            this.prefab = prefab;
            this.isBraid = isBraid;
            cellSize = prefab.gameObject.transform.localScale;
            isGenerate = false;
            _mazeParent = mazeParent;
        }
        /// <summary>
        /// Генерирует отображаемую область лабиринта
        /// </summary>
        /// <param name="runTimeMazeSize">Размер отображаемой области лабиринта</param>
        /// <param name="startLocalPosition">Локальные координаты начала отображаемой области</param>
        /// <returns></returns>
        public bool Generate(Vector2Int runTimeMazeSize, Vector2Int startLocalPosition)
        {
            Debug.Log("Maze is generating");
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
            //Циклы идут отдельно, т.к. в методе RecalculateCellValues нужна информация о всех клетках, а если мы делаем все в одном цикле,
            //то клетки, у которых позиция больше текущей, не будут созданы. На прозводительность это никак не влияет, т.к. действия те же
            for (int y = 0; y < runTimeMazeSize.y; y++)
            {
                for (int x = 0; x < runTimeMazeSize.x; x++)
                {
                    RecalculateCellValues(mazeArray[y, x]);
                }
            }
            return true;
        }//ДОБАВИТЬ ПАРАМЕТРЫ
        /// <summary>
        /// Смещает область отображаемого лабиринта на distance
        /// </summary>
        /// <param name="distance">Дистанция, на которую нужно сместить отображаемый лабиринт</param>
        public void TransformRunTimeMaze(Vector2Int distance)//Заменить transform на "сместить"
        {
            for (int y = 0; y < runTimeMazeSize.y; y++)
            {
                for (int x = 0; x < runTimeMazeSize.x; x++)
                {
                    Vector2Int lastPositionInMaze = mazeArray[y, x].positionInMaze;
                    mazeArray[y, x] = new Cell(lastPositionInMaze + distance, cellSize, this);//Переделать на переназначение клеток
                    RecalculateCellValues(mazeArray[y, x]);
                }
            }
            RecalculateMazeValues();
        }
        /// <summary>
        /// Возвращает ссылку на клетку с позицией position, если такая присутствует в отображаемом лабиринте
        /// </summary>
        /// <param name="position"></param>
        /// <param name="cell">Ссылка на клетку</param>
        /// <returns>Возвращает true, если такая клетка есть в отображаемом лабиринте и false если нет</returns>
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
        /// Обновляет все данные клетки исходя из ее positionInMaze
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="walls">Просчитываем ли мы необходимость активации верхних и боковых стен?</param>
        /// <param name="globalPosition">Просчитываем ли мы глобальную позицию клетки в лабиринте</param>
        private void RecalculateCellValues(Cell cell, bool walls = true)
        {
            if (walls)
            {
                if (RandomChoice(cell.positionInMaze, generateData.seed, GenBoolType.sideCellsDeleteCheck, runTimeMazeSize.x))
                    cell.leftWall = false;
                //Если мы уже сделали просчет с множеством данной клетки, то можно больше этого не делать
                if (cell.isCalculateTop)
                {
                    return;
                }
                //Ищем все левые и правые клетки множества
                List<Vector2Int> _cellsSet = new List<Vector2Int>();
                if (cell.leftWall == false)//Это для проверки того, что наша клетка не является левым краем множества
                {
                    Vector2Int thisCellPositionLeft = cell.positionInMaze;
                    //Do while потому что если у клетки есть левая стена, тело цикла должно выполниться все равно
                    do
                    {
                        thisCellPositionLeft.x -= 1;
                        _cellsSet.Add(thisCellPositionLeft);
                        //Проверяем 
                        Cell updateDataCell;
                        // Debug.Log(GetCellAtPositionInMaze(thisCellPositionLeft, out updateDataCell));
                        // Debug.Log(updateDataCell);
                        if (GetCellAtPositionInMaze(thisCellPositionLeft, out updateDataCell) && updateDataCell != null)
                        {
                            updateDataCell.isCalculateTop = true;
                        }
                    } while (RandomChoice(thisCellPositionLeft, generateData.seed, GenBoolType.sideCellsDeleteCheck, runTimeMazeSize.x));
                }
                //Разворачиваем множество, чтобы елементы были в правильном пордке
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
                    Debug.LogWarning("Генерация плетеного лабиринта пока не готова");
                }
            }
        }
        /// <summary>
        /// Обновляет все данные лабиринта: runTimeMazeSize, mazeStartGlobalPosition, isGenerate, runTimeStartCell, isRenderer
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
        /// <summary>Меняет область отображаемого лабиринта на область с позицией newPosition первой(самой маленькой по координатам) клетки
        /// </summary>
        /// <param name="newPosition">Позиция первой(самой маленькой по координатам) клетки новой области</param>
        public void EditRunTimeMazePosition(Vector2Int newPosition)//Заменить Change на "изменить"
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
        /// Изменяет размер отображаемого лабиринта так, что начальная клетка предыдущей области будет начальной клеткой новой
        /// </summary>
        /// <param name="newSize">Новый размер. Он должен быть > 0 по каждой координате</param>
        /// <returns>Возвращает False, если введенные данные некоректны</returns>
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
        /// Удаляет лабиринт
        /// </summary>
        public void RemoveMaze(bool deleteRender = true)
        {
            mazeArray = null;
            runTimeMazeSize = Vector2Int.zero;
            RecalculateMazeValues();
        }
        /// <summary>
        /// Обновляет отображение лабиринта в мире
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
                        newCell = MonoBehaviour.Instantiate(prefab, position, prefab.transform.localRotation, _mazeParent);
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
        /// Удаляет отображение лабиринта в мире
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
        /// Меняет начальные координаты лабиринта в глобальном пространстве
        /// </summary>
        /// <param name="newCoordinates">Новые координаты</param>
        public void TransformMazeStartGlobalPosition(Vector3 newCoordinates)//Изменить transform на "сместить"
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
        /// Возвращает позицию объекта в лабиринте по его глобальным координатам. Координату y не учитывает
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