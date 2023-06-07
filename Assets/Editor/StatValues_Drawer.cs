using UnityEngine;

using UnityEditor;
using static BaseCombatEntity;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(StatValues))]
public class StatValues_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // List of strings, each representing a key to a stat
        List<string> keys = new List<string>(){
            "level",

            "maxHp",
            "maxMp",

            "atk_P",
            "atk_M",
            "def_P",
            "def_M",

            "speed",

            "maxXp"
        };

        //How many pixes should be between elements (horizontally)
        int bigSpace = 10;
        int smallSpace = 2;

        List<int> spacesAfterDrawingStat = new List<int>(){
            bigSpace, // "level",

            smallSpace, // "maxHp",
            bigSpace, // "maxMp",

            smallSpace,// "atk_P",
            smallSpace,// "atk_M",
            smallSpace,// "def_P",
            bigSpace,// "def_M",

            bigSpace,// "speed",

            0// "maxXp"
        };

        //Load stat values from data
        List<SerializedProperty> values = new List<SerializedProperty>();
        for (int i = 0; i < keys.Count; i++)
        {
            values.Add(property.FindPropertyRelative(keys[i]));
        }

        //Draw Labels and Values
        Rect rect = new Rect(150, position.y, 40, position.height);
        for (int i = 0; i < values.Count; i++)
        {
            string currLabel = keys[i] + ": "; //Something like "maxHp:"

            float prevWidth = rect.width;
            float textWidth = GUI.skin.label.CalcSize(new GUIContent(currLabel)).x;

            rect.width = textWidth;

            // EditorGUI.PropertyField(rect, keys[i], GUIContent.none);
            EditorGUI.LabelField(rect, currLabel);

            rect.x += textWidth;
            rect.width = prevWidth;

            EditorGUI.PropertyField(rect, values[i], GUIContent.none);
            rect.x += 50 + spacesAfterDrawingStat[i];
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
