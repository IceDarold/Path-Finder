using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generate.Bioms;
using Generate.MazeGen;

namespace Generate
{
    public class GenerateManager : MonoBehaviour
    {
        [Header("Same settings")]
        [SerializeField] private GameObject player;
        [SerializeField] private GenerateData _generateData;
        #region MazeSettings
        [Header("Maze settings")]
        [SerializeField] private bool isGenerateMaze;
        [Tooltip("Глобальная позиция первой клетки в лабиринте")]
        [SerializeField] private Vector3 _firstWorldCoordinates;
        [Tooltip("Позиция первой сгенерированной клетки в локальной системе координат лабиринта")]
        [SerializeField] private Vector2Int _startPositionInMaze = new Vector2Int(0, 0);
        [Tooltip("Размер прогружаемого в реальном времени лабиринта")]
        [SerializeField] private Vector2Int _runTimeMazeSize;

        [Header("Render settings")]
        [Tooltip("Префаб клетки в лабиринте")]
        [SerializeField] private CellPref _prefab;
        [SerializeField] private bool _offTopWalls;
        [SerializeField] private bool _offSideWalls;
        private Maze maze;
        private GleidsManager _gleidsManager;
        private Vector3 lastUpdatePosition;

        [Header("UpdateSettings")]
        [Tooltip("Обновлять лабиринт?")]
        [SerializeField] private bool _isUpdate;
        [Tooltip("Расстояние между игроком и левым концом лабиринта, при котором конец удаляется. В клетках")]
        [SerializeField] private byte _allowableDistance = 10;
        [Tooltip("Происходит обновление лабиринта постоянно или после того, как игрок отдалится от предыдущего места на updateDistance")]
        [SerializeField] private bool _isConstUpdate;
        [Tooltip("Расстояние между игроком и предыдущим местом обновления, при котором происходит проверка на обновление лабиринта")]
        [SerializeField] private float _checkDistance;
        [Tooltip("Обновлять рендер после генерации?")]
        [SerializeField] private bool isUpdateRenderInRunTime;
        #endregion
        #region GleidsSettings
        [Header("GleidSettings")]
        [SerializeField] bool _isGenerateGleids;
        [SerializeField] GleidSettings _gleidSettings;//Сделать появление этого только если _isGenerateGleid равно True
        #endregion
        #region BiomSettings
        [SerializeField] private bool _isGenerateBioms;
        [SerializeField] private BiomGenerateSettings generateBiomSettings;
        #endregion
        #region Debug
        [Header("Develop settings. Debug/test")]
        [Space(10)]
        [Tooltip("Обновляет размер лабиринта на newSize")]
        [SerializeField] private bool updateSize;
        [SerializeField] private Vector2Int newSize;
        [Space(10)]
        [Tooltip("Удаляет отображение лабиринта")]
        [SerializeField] private bool deleteRender;
        private bool isRenderDeleted;
        [Space(10)]
        [Tooltip("Обновляет отображение лабиринта")]
        [SerializeField] private bool updateRender;
        [Space(10)]
        [Tooltip("Сгенерировать лабиринт")]
        [SerializeField] private bool generate;
        [Space(10)]
        [Tooltip("Меняет область отображаемого лабиринта на область с позицией newPosition первой(самой маленькой по координатам) клетки")]
        [SerializeField] private bool editRunTimeMazePosition;
        [SerializeField] private Vector2Int newPosition;
        [Space(10)]
        [Tooltip("Смещает область отображаемого лабиринта на distance")]
        [SerializeField] private bool transformRunTimeMaze;
        [SerializeField] private Vector2Int distance;
        [Space(10)]
        [Tooltip("Меняет начальные координаты лабиринта в глобальном пространстве на globalPosition")]
        [SerializeField] private bool transformMazeStartGlobalPosition;
        [SerializeField] private Vector3 globalPosition;
        [Space(10)]
        [Tooltip("Удаляет лабиринт")]
        [SerializeField] private bool deleteMaze;
        [Tooltip("Удалить вместе с лабиринтом его отображение?")]
        [SerializeField] private bool isDeleteRender;
        [Space(10)]
        [Tooltip("Возвращает позицию в лабиринте по глобальным координатам объекта")]
        [SerializeField] private bool getPositionInMazeToGloballPosition;
        [SerializeField] private Vector3 objectGlobalPosition;
        [SerializeField] private Vector2Int cellPosition;
        [SerializeField] private Cell objectCellData;
        [SerializeField] private CellPref objectCellLink;
        #endregion
        private void Update()
        {
            //Проверка на то, что лабиринт вообще сгенерирован
            if (isGenerateMaze)
            {
                if (maze == null)
                    maze = new Maze(_generateData, _firstWorldCoordinates, _prefab);
                if (!maze.isGenerate)
                {
                    maze.Generate(_runTimeMazeSize, _startPositionInMaze);
                    if (_isGenerateGleids)
                    {
                        _gleidsManager = new GleidsManager(_gleidSettings, _generateData, maze);
                        _gleidsManager.GenerateGleids();
                    }
                    if (_isUpdate)
                    {
                        lastUpdatePosition = player.transform.position;
                    }
                }
            }
            //Проверка на движение игрока
            if (maze != null && _isUpdate)
            {
                if (_isConstUpdate || (player.transform.position - lastUpdatePosition).magnitude >= _checkDistance)
                {
                    Vector2Int playerPositionInMaze = maze.GetPositionInMaze(player.transform.position);
                    Vector2Int difference = playerPositionInMaze - maze.mazeArray[maze.runTimeMazeSize.y / 2, 0].positionInMaze;
                    if (difference.x != _allowableDistance || difference.y != 0)
                    {
                        maze.TransformRunTimeMaze(difference - new Vector2Int(_allowableDistance, 0));
                        if (_isGenerateGleids)
                        {
                            _gleidsManager.GenerateGleids();
                        }
                    }
                    lastUpdatePosition = player.transform.position;
                }
            }
            //Обновление отображения
            if (maze != null && isUpdateRenderInRunTime)
            {
                maze.UpdateRender(_offTopWalls, _offSideWalls);
            }
            if (maze != null)
            {
                DevelopSettingsActivate();
            }
        }
        private void DevelopSettingsActivate()
        {
            if (updateSize)
            {
                updateSize = false;
                _runTimeMazeSize = newSize;
                maze.EditRunTimeMazeSize(newSize);
            }
            if (maze.runTimeMazeSize != _runTimeMazeSize)
            {
                maze.EditRunTimeMazeSize(_runTimeMazeSize);
            }
            if (deleteRender)
            { 
                deleteRender = false;
                isUpdateRenderInRunTime = true;
                if (maze != null)
                    maze.DeleteRender();
            }
            if (editRunTimeMazePosition)
            {
                editRunTimeMazePosition = false;
                Debug.Log("qwe1");
                if (maze != null)
                    maze.EditRunTimeMazePosition(newPosition);
            }
            if (updateRender)
            {
                updateRender = false;
                isRenderDeleted = false;
                if (maze != null)
                    maze.UpdateRender();
            }
            if (deleteMaze)
            {
                deleteMaze = false;
                isRenderDeleted = true;
                isGenerateMaze = false;
                maze.RemoveMaze(isDeleteRender);
                if (isUpdateRenderInRunTime)
                    maze.UpdateRender();
            }
            if (transformMazeStartGlobalPosition)
            {
                transformMazeStartGlobalPosition = false;
                if (maze != null)
                    maze.TransformMazeStartGlobalPosition(globalPosition);
            }
            if (transformRunTimeMaze)
            {
                transformRunTimeMaze = false;
                if (maze != null)
                    maze.TransformRunTimeMaze(distance);
            }
            if (generate)
            {
                generate = false;
                isGenerateMaze = true;
            }
            if (getPositionInMazeToGloballPosition)
            {
                getPositionInMazeToGloballPosition = false;
                cellPosition = maze.GetPositionInMaze(objectGlobalPosition);
            }
        }
    }
}