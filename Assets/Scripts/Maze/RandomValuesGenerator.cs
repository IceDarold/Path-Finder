using System;
using UnityEngine;
using Generate;
namespace Generate.Random
{
    public class RandomValuesGenerator : MonoBehaviour
    {
        public enum GenBoolType
        {
            sideCellsDeleteCheck,
            topCellsDeleteCheck,
            gleidExistence
        }
        public enum GenVectorType
        {
            GleidSize,
            GleidStartPositionAdd
        }
        //Reserved seed additions
        //0 - will a side cell wall be active
        //1 - will a top cells walls be active
        //2 - will a glide be generated
        //3 - generate random addition to gleid position
        //4 - 6 - generate random gleidSize
        //0 - 20 - GenBoolType
        //20 - 40 - GenVectorType
        public static int RandomIndex(Vector2Int[] cellsPositions, int seed, int mazeSizeX)
        {
            Vector2Int sumVector = new Vector2Int();
            foreach (var item in cellsPositions)
            {
                sumVector += item;
            }
            sumVector /= cellsPositions.Length;
            return NumGenerator.Gen(sumVector.x, sumVector.y, seed, mazeSizeX) % cellsPositions.Length;
        }
        /// <summary>
        /// Вычисляет уникальный идентификатор для любого цело-комплексного числа или проще говоря для любого Vector2Int
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static int CalculateUniqueIDFor2DVector(Vector2Int vector)
        {
            //Это пока просто балванка. Оно не работает как надо. Нужно доделать
            return 20 * vector.y + vector.x;
        }
        public static Vector2Int RandomVector2Int(Vector2Int positionInMaze, Vector2Int minValue, Vector2Int maxValue, GenerateData generateData, GenVectorType genVectorType)
        {
            Vector2Int returnVector = new Vector2Int();
            int generateNum = NumGenerator.Gen(positionInMaze.x, positionInMaze.y, generateData.seed + 20 + ((int)genVectorType),
                CalculateUniqueIDFor2DVector(positionInMaze));
            int generateNumForRangeX = generateNum / 10;
            int generateNumForRangeY = generateNum / 100;
            Vector2Int addRange = new Vector2Int(minValue.x != maxValue.x ? generateNumForRangeX % (maxValue - minValue).x : 0,
                minValue.y != maxValue.y ? generateNumForRangeY % (maxValue - minValue).y : 0);
            returnVector.x = addRange.x == 0 ? minValue.x : minValue.x + generateNum % (addRange.x + 1);
            returnVector.y = addRange.y == 0 ? minValue.y : minValue.y + (generateNum / 1000) % (addRange.y + 1);
            return returnVector;
        }
        public static bool RandomChoice(Vector2Int positionInMaze, int seed, GenBoolType genType, int mazeSizeX)
        {
            if (NumGenerator.Gen(positionInMaze.x, positionInMaze.y, seed + ((int)genType), mazeSizeX) % 2 == 0)
                return true;
            return false;
        }
    }
}
