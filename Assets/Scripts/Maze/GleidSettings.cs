using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GleidSettings", menuName = "Generate/GleidSettings")]
public class GleidSettings : ScriptableObject
{
    [Min(2)] public Vector2Int minGleidSize = new Vector2Int(5, 5);
    [Space(15)]
    [Min(2)] public Vector2Int maxGleidSize = new Vector2Int(10, 10);
    [Space(15)]
    [Tooltip("Радиус генерации глейдов. Чем больше, тем меньше будет глейдов")]
    //6 -  должнл быть gleidSize
    [Min(1)] public Vector2Int gleidChange = new Vector2Int(124, 182);
    [Space(15)]
    [Tooltip("Диапазон рандомного добавления к позиции глейда")]
    [Min(0)] public Vector2Int addRange = new Vector2Int(10, 10);
    [Space(15)]
    [Tooltip("Удалять ли границу глейда?")]
    public bool isDeleteBroad;
    public bool isRandomAppear = true;
    public bool isRandomAdd = true;
    public bool isRandomGleidSize = false;
    public bool isSquareGleid = false;
    private void OnValidate()
    {
        if (maxGleidSize.x > gleidChange.x)
        {
            //EditorUtility.DisplayPopupMenu(new Rect(10, 10, 50, 50), "Блаблабла", new MenuCommand(new Object()));
            //EditorUtility.DisplayDialog("Ошибка", "Размер глейда не может быть больше радиуса генерации!", "Хорошо, исправлюсь");
            maxGleidSize.x = gleidChange.x;
        }
        if (maxGleidSize.y > gleidChange.y)
        {
            maxGleidSize.y = gleidChange.y;
        }
        if (minGleidSize.x > maxGleidSize.x)
        {
            minGleidSize.x = maxGleidSize.x;
        }
        if (minGleidSize.y > maxGleidSize.y)
        {
            minGleidSize.y = maxGleidSize.y;
        }
        if (maxGleidSize.x < minGleidSize.x)
        {
            maxGleidSize.x = minGleidSize.x;
        }
        if (maxGleidSize.y < minGleidSize.y)
        {
            maxGleidSize.y = minGleidSize.y;
        }
        if (gleidChange.x - (isRandomGleidSize ? maxGleidSize.x : minGleidSize.x) <= 0)
        {
            gleidChange.x = isRandomGleidSize ? maxGleidSize.x : minGleidSize.x;
        }
        if (gleidChange.y - (isRandomGleidSize ? maxGleidSize.y : minGleidSize.y) <= 0)
        {
            gleidChange.y = isRandomGleidSize ? maxGleidSize.y : minGleidSize.y;
        }
        if (addRange.x >= gleidChange.x - (isRandomGleidSize ? maxGleidSize.x : minGleidSize.x))
        {
            //Debug.LogError("Диапазон рандомного добавления к позиции глейда должен быть меньше радиуса генерации!");
            //EditorUtility.DisplayDialog("Ошибка", "Диапазон рандомного добавления к позиции глейда должен быть меньше радиуса генерации!", "Хорошо, исправлюсь");
            addRange.x = gleidChange.x - (isRandomGleidSize ? maxGleidSize.x : minGleidSize.x);
        }
        if (addRange.y >= gleidChange.y - (isRandomGleidSize ? maxGleidSize.y : minGleidSize.y))
        {
            addRange.y = gleidChange.y - (isRandomGleidSize ? maxGleidSize.y : minGleidSize.y);
        }
    }
}
