using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Roll = LootSystem.Roll;

[CustomEditor(typeof(LootDrop))]
public class LootDropEditor : Editor
{
    private readonly List<Roll> _rolls = new List<Roll>();
    private int _dropAmount = 1;
    private LootDrop LootDrop => (LootDrop)target;
    private readonly int _maxRollSlider = 20;
    private readonly int _rollSliderWidth = 150;
    private readonly int _buttonHeight = 30;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space(10);
        DrawLine(Color.white, MakeLine(1));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Roll loot drop", GUILayout.Height(_buttonHeight)))
        {
            RollLootDrop();
        }

        _dropAmount = EditorGUILayout.IntSlider("", _dropAmount, 1, _maxRollSlider, GUILayout.Width(_rollSliderWidth), GUILayout.Height(_buttonHeight));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Clear roll", GUILayout.Height(_buttonHeight)))
        {
            _rolls.Clear();
        }

        EditorGUILayout.Space(10);
        if (_rolls.Count > 0)
        {
            DrawTotal();
        }
        
        for (int i = _rolls.Count - 1; i >= 0; i--)
        {
            ShowRoll(i);
        }
    }
    void ShowRoll(int i)
    {
        var currentRoll = _rolls[i];
        EditorGUILayout.LabelField($"Roll{(i + 1)}:");
        DrawLine(Color.white, MakeLine(1));
        EditorGUILayout.LabelField("Guaranteed items:", EditorStyles.boldLabel);
        CheckStringDuplicate(CollectNames(currentRoll.guaranteedItems.ToArray()));
        EditorGUILayout.LabelField("Chance items:", EditorStyles.boldLabel);
        CheckStringDuplicate(CollectNames(currentRoll.chanceItems.ToArray()));
        EditorGUILayout.Space(5);
        if (GUILayout.Button("delete", GUILayout.Height(30)))
        {
            _rolls.RemoveAt(i);
        }

        DrawLine(Color.white, MakeLine(1));
        EditorGUILayout.Space(20);
    }

    static GUIStyle MakeLine(int height)
    {
        GUIStyle horizontalLine = new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.whiteTexture
            },
            margin = new RectOffset(0, 0, 5, 4),
            fixedHeight = height
        };
        return horizontalLine;
    }

    static void DrawLine(Color color, GUIStyle horizontalLine)
    {
        var c = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = c;
    }

    private void DrawTotal()
    {
        EditorGUILayout.LabelField("All drops:", EditorStyles.boldLabel);
        var guaranteedItems = new List<GameObject>();
        var chanceItems = new List<GameObject>();
        foreach (var roll in _rolls)
        {
            guaranteedItems.AddList(roll.guaranteedItems);
            chanceItems.AddList(roll.chanceItems);
        }

        EditorGUILayout.LabelField("Guaranteed drops:", EditorStyles.boldLabel);
        CheckStringDuplicate(CollectNames(guaranteedItems.ToArray()));
        EditorGUILayout.LabelField("Chance drops:", EditorStyles.boldLabel);
        CheckStringDuplicate(CollectNames(chanceItems.ToArray()));
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField($"Total rolls:{_rolls.Count}", EditorStyles.boldLabel);
    }

    private string[] CollectNames(GameObject[] gameObjects)
    {
        var strings = new List<string>();
        foreach (var gameObject in gameObjects)
        {
            strings.Add(gameObject.name);
        }

        return strings.ToArray();
    }
    static void CheckStringDuplicate(string[] currentStrings)
    {
        var items = new List<string>();
        foreach (var item in currentStrings)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                var amount = 0;
                foreach (var otherItem in currentStrings)
                {
                    if (item == otherItem)
                    {
                        amount++;
                    }
                }
                EditorGUILayout.LabelField($"{amount}x {item}");
            } 
        }
    }

    private void RollLootDrop()
    {
        for (int i = 0; i < _dropAmount; i++)
        {
            var roll = LootSystem.Instance.RollLoot(LootDrop.lootTable, LootDrop.lootForce, LootDrop.transform);
            _rolls.Add(roll);
        }
    }

}