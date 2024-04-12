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
        /// Список начальных позиций глейдов в которых находятся клетки отображаемого лабиринта
        /// </summary>
        public Vector2Int[] gleidStartPositions { get => startPositions; private set => startPositions = value; }
        private Vector2Int[] startPositions;
        /// <summary>
        /// Список размеров глейдов в которых находятся клетки отображаемого лабиринта
        /// </summary>
        public Vector2Int[] gleidsSizes { get => _gleidsSizes; private set => _gleidsSizes = value; }
        private Vector2Int[] _gleidsSizes;
        /// <summary>
        /// Генерирует глейды в лабиринте maze. Вызывать только в самом начале, когда глейды еще не сгенерированы. После обновления лабиринта вызывать UpdateGleids для более высокой
        /// производительности
        /// </summary>
        /// <param name="maze">Лабиринт</param>
        public void GenerateGleids()
        {
            GetGleidsStartInfo(maze.mazeArray, out startPositions, out _gleidsSizes);
            startPositions.ToList().ForEach(pos => Debug.Log(pos));
            for (int i = 0; i < startPositions.Length; i++)
            {
                //-1 потому что для того, чтобы создать нижнюю стену глейда нужно поставить стену у всех клеток, находящихся под глейдом,
                //т.к. у клетки есть только topWall и leftWall
                //+1 для создания свободного прохода вокруг глейда
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
        /// Обновляет глейды в лабиринте
        /// </summary>
        /// <param name="transformDistance">Расстояние на которые сместился лабиринт относительно предыдущего обновления</param>
        public void UpdateGleids(Vector2Int transformDistance)//Сместить
        {

        }
        /// <summary>
        /// Возвращает позиции начал и размеры глейдов у которых хотя бы одна клетка находится в отображаемом лабиринте
        /// </summary>
        /// <param name="mazeArray"></param>
        /// <param name="startPositions">Массив позиций начал глейдов</param>
        /// <param name="sizes">Массив размеров глейдов</param>
        private void GetGleidsStartInfo(Cell[,] mazeArray, out Vector2Int[] startPositions, out Vector2Int[] sizes)
        {
            List<Vector2Int> positionsList = new List<Vector2Int>();
            List<Vector2Int> sizesList = new List<Vector2Int>();
            //Получаем либо 1, либо -1
            //int multiplerX = mazeArray[0, mazeArray.GetLength(1) - 1].positionInMaze.x / Mathf.Abs(mazeArray[0, mazeArray.GetLength(1) - 1].positionInMaze.x);
           // int multiplerY = mazeArray[mazeArray.GetLength(0) - 1, 0].positionInMaze.y / Mathf.Abs(mazeArray[mazeArray.GetLength(0) - 1, 0].positionInMaze.y);
            //Находим позицию начала глейда, ближайшего к началу лабиринта с наименьшими координатами

            Vector2Int startPositionGleidChangesCount = new Vector2Int(mazeArray[0, 0].positionInMaze.x / gleidSettings.gleidChange.x
           // Vector2Int startPosition = new Vector2Int(mazeArray[0, 0].positionInMaze.x / gleidSettings.gleidChange.x
                * gleidSettings.gleidChange.x, mazeArray[0, 0].positionInMaze.y / gleidSettings.gleidChange.y
                * gleidSettings.gleidChange.y);
            //Если начальная позиция оказалась < 0, то стартовой позиции не будет хватать одного gleidChange.x/.y. Возьмем например x = -13. 
            //-13 / 10(gleidChange.x) = -1, а должно быть -2
            //startPosition.x += gleidSettings.gleidChange.x * (mazeArray[0, 0].positionInMaze.x < 0 ? -1 : 0);
            //startPosition.y += gleidSettings.gleidChange.y * (mazeArray[0, 0].positionInMaze.y < 0 ? -1 : 0);
            //Позиция, которую мы изменяем и проверяем
            //Ищем начала и размеры всех глейдов в отображаемой области лабиринта
            //Пока искомая позиция не вышла за пределы отображаемого лабиринта по y
            /*while (nearestPosition.y <= mazeArray[mazeArray.GetLength(0) - 1, 0].positionInMaze.y)
            {
                //Пока искомая позиция не вышла за пределы отображаемого лабиринта по x
                while (nearestPosition.x <= mazeArray[0, mazeArray.GetLength(1) - 1].positionInMaze.x)
                {
                    //Если мы генерируем каждый глейд или рандом победил добавляем в списки информацию о новом глейде
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
                    //Если мы генерируем каждый глейд или рандом победил добавляем в списки информацию о новом глейде
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
        /// Удаляет клетки относительно их позиции в лабиринте
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="positionInGleid">Позиция клетки в лабиринте</param>
        /// <param name="gleidSize">Размер глейда</param>
        private void RemoveWalls(Cell cell, Vector2Int positionInGleid, Vector2Int gleidSize)
        {
            //Если клетка самая левая в глейде или правее его и при этом находится в пределах глейда по y, включаем левую стену
            if ((positionInGleid.x == 0 || positionInGleid.x == gleidSize.x) && positionInGleid.y >= 0 &&
                positionInGleid.y <= gleidSize.y - 1)
            {
                cell.leftWall = true;
            }
            //Если клетка под глейдом или самая правая в глейде и при этом находится в пределах глейда по x, включаем верхнюю стену
            if ((positionInGleid.y == -1 || positionInGleid.y == gleidSize.y - 1) && positionInGleid.x >= 0 && positionInGleid.x <= gleidSize.x - 1)
            {
                cell.topWall = true;
            }
            //Если клетка находится внутри глейда, но ниже самой верхней строчки, удаляем верхнюю стену
            if (positionInGleid.x >= 0 && positionInGleid.x <= gleidSize.x - 1 && positionInGleid.y >= 0 && positionInGleid.y <= gleidSize.y - 2)
            {
                cell.topWall = false;
            }
            //Если клетка находится в глейде, но выше, самой левой строчки, удаляем левую стену
            if (positionInGleid.x >= 1 && positionInGleid.x <= gleidSize.x - 1 && positionInGleid.y >= 0 && positionInGleid.y <= gleidSize.y - 1)
            {
                cell.leftWall = false;
            }
            if (gleidSettings.isDeleteBroad)
            {
                //Если клетка слева или справа от глейда и при этом находится в пределах глейда по y, удаляем верхнюю стену
                if ((positionInGleid.x == -1 || positionInGleid.x == gleidSize.x) && positionInGleid.y >= -1 && positionInGleid.y <= gleidSize.y - 1)
                {
                    cell.topWall = false;
                }
                //Если клетка снизу или сверху от глейда и при этом находится в пределах глейда по x, удаляем левую стену
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