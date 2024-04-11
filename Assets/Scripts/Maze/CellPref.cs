using UnityEngine;

namespace Generate
{
    public class CellPref : MonoBehaviour
    {
        public GameObject _leftWall;
        public GameObject _topWall;
        public CellPref _topNeighbor;
        public CellPref _rightNeighbor;
        public CellPref _bottomNeighbor;
        public CellPref _leftNeighbor;
        public Vector2Int positionInMaze;
        [Header("Debug variables")]
        public Vector2Int positionInGleid;
        public Vector2Int nearestGleidPosition;
    }
}
