using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generate.Bioms
{
    public class BiomGenerateSettings : ScriptableObject
    {
        [Header("Biom grid settings")]
        public Vector2Int gridSize;
    }
}