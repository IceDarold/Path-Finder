using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerateData", menuName = "Generate/GenerateData")]
public class GenerateData : ScriptableObject
{
    [SerializeField] private int _seed;
    public int seed { get => _seed; }
}
