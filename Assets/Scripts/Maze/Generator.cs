using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ����� ��� ���� �����������
/// </summary>
public class Generator
{
    public GenerateData generateData { get; private set; }
    public Generator(GenerateData generateData)
    {
        this.generateData = generateData;
    }
}
