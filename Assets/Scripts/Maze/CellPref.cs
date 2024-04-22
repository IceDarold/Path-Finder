using UnityEngine;

namespace Generate
{
    public class CellPref : MonoBehaviour
    {
        public GameObject _leftWall;
        public GameObject _topWall;
        [Header("Debug variables")]
        public CellPref _topNeighbor;
        public CellPref _rightNeighbor;
        public CellPref _bottomNeighbor;
        public CellPref _leftNeighbor;
        public Vector2Int positionInMaze;
        public Vector2Int positionInGleid;
        public Vector2Int nearestGleidPosition;
    }
}
