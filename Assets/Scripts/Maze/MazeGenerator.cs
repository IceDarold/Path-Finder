using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generate;

namespace Generate 
{
    /*public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] MazeSettings mazeSettings;
        [SerializeField] GameObject player;
        private byte _allowableDistance;
        [Header("Debug variables")]
        [SerializeField] Ob ob;
        private Vector3 lastCellPosition;
        private Maze maze;

        public void Generate()
        {
            maze = new Maze(mazeSettings);
            ob.maze = maze;
            _allowableDistance = Convert.ToByte(mazeSettings.runTimeGleidSize.x / 2 + 1);
            lastCellPosition = player.transform.position;
            if (mazeSettings.isGenerateMaze)
                StartGeneration();
        }
        void StartGeneration()
        {
            List<Cell> _freeCellList = new List<Cell>();
            for (int i = 0; i < mazeSettings.runTimeGleidSize.x * mazeSettings.runTimeGleidSize.y; i++)
            {
                _freeCellList.Add(new Cell(Instantiate(mazeSettings.prefab, transform), null));
                _freeCellList[_freeCellList.Count - 1]._id = i;
            }
            for (int i = 0; i < mazeSettings.runTimeGleidSize.y; i++)
            {
                Maze.StringType stringType = new Maze.StringType(Maze.StringType.PositionType.None, Maze.StringType.StartType.JustStart);
                if (i == 0)
                {
                    stringType.startType = Maze.StringType.StartType.StartBottomString;//√оворим, что эта строка нижн¤¤ и первый раз объ¤вл¤ем еЄ
                }
                else if (i == mazeSettings.runTimeGleidSize.y - 1)//≈сли сейчас последн¤¤ строка, то строка будет верхней
                {
                    stringType.startType = Maze.StringType.StartType.FinishTopString;//√оворим, что эта строка верхн¤¤ и первый раз объ¤вл¤ем еЄ
                }
                else if (i == mazeSettings.runTimeGleidSize.y / 2)//≈сли сейчас центральна¤ строка, то строка будет центральной, то есть перва¤ и последние клетки в ней будут центральнами дл¤ боковых строк
                {
                    stringType.startType = Maze.StringType.StartType.MiddleString;
                }
                maze.StartLineGenerate(new Maze.LineSettings(maze, CellsString.StringType.Horizontal, runTimeGleidSize.x,
                    mazeSettings.firstWorldCoordinates + new Vector3(0, 0, mazeSettings.prefab.transform.localScale.z * i), new Vector2Int(0, i),
                    mazeSettings.prefab.transform.localScale, stringType), i, _freeCellList);
            }
        }
        private void Update()
        {
            if (mazeSettings.isGenerateMaze && mazeSettings.isUpdate)
            {
                //≈сли 
                if (mazeSettings.isConstUpdate || Math.Abs(player.transform.localPosition.x - lastCellPosition.x) >= mazeSettings.prefab.transform.localScale.x
                    || Math.Abs(player.transform.localPosition.z - lastCellPosition.z) >= mazeSettings.prefab.transform.localScale.z)
                {
                    CheckDistance();
                    lastCellPosition = player.transform.position;
                }
            }
        }
        private void CheckDistance()
        {
            //≈сли игрок находитс¤ на рассто¤ние больше _allowableDistance от центральной клетки верхней строки
            if (maze._topMiddleCell.cellPref.transform.position.z - player.transform.position.z >= _allowableDistance * mazeSettings.prefab.transform.localScale.z)
            {
                maze.StringDelete(Maze.StringType.PositionType.TopString);
            }
            //≈сли игрок находитс¤ на рассто¤ние больше _allowableDistance от центральной клетки нижней строки
            else if (player.transform.position.z - maze._bottomMiddleCell.cellPref.transform.position.z >= _allowableDistance * mazeSettings.prefab.transform.localScale.z)
            {
                maze.StringDelete(Maze.StringType.PositionType.BottomString);
            }
            //≈сли игрок находитс¤ на рассто¤ние больше _allowableDistance от центральной клетки правой строки
            if (maze._rightMiddleCell.cellPref.transform.position.x - player.transform.position.x >= _allowableDistance * mazeSettings.prefab.transform.localScale.x)
            {
                maze.StringDelete(Maze.StringType.PositionType.RightString);
            }
            //≈сли игрок находитс¤ на рассто¤ние больше _allowableDistance от центральной клетки левой строки
            else if ((player.transform.position.x - maze._leftMiddleCell.cellPref.transform.position.x) >= _allowableDistance * mazeSettings.prefab.transform.localScale.x)
            {
                maze.StringDelete(Maze.StringType.PositionType.LeftString);
            }
        }
    }*/
}