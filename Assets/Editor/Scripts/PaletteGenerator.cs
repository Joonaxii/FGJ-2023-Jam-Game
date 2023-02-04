using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PaletteGenerator : EditorWindow
{
    [SerializeField] private string _name;
    [SerializeField] private Palette _main;
    [SerializeField] private Palette _alt;

    private SerializedObject _so;

    private SerializedProperty _propName;
    private SerializedProperty _propMain;
    private SerializedProperty _propAlt;

    private Texture2D _paletteTex;
    private static Color[] _colors = new Color[8 * 2];

    [MenuItem("Tools/Palette Generator")]
    static void Init()
    {
        PaletteGenerator window = GetWindow<PaletteGenerator>();
        window.Show();
    }

    private void OnEnable()
    {
        _so = new SerializedObject(this);
        _propName = _so.FindProperty("_name");
        _propMain = _so.FindProperty("_main");
        _propAlt = _so.FindProperty("_alt");

        if (_paletteTex == null)
        {
            _paletteTex = new Texture2D(8, 2, TextureFormat.RGB24, false);
            _paletteTex.filterMode = FilterMode.Point;
        }

        LoadFromDisk();
    }

    private void OnDestroy()
    {
        if (_paletteTex != null)
        {
            DestroyImmediate(_paletteTex, true);
        }
    }

    private void LoadFromDisk()
    {
        string path = $"{Application.dataPath}/Palettes/Configs/";
        Directory.CreateDirectory(path);
        path += "Palette Conf.cfg";

        if (File.Exists(path))
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            using(var br = new BinaryReader(fs))
            {
                _name = br.ReadString();
                _main.Read(fs);
                _alt.Read(fs);
            }
        }
    }

    private void SaveToDisk()
    {
        string path = $"{Application.dataPath}/Palettes/Configs/";
        Directory.CreateDirectory(path);
        path += "Palette Conf.cfg";

        using (FileStream fs = new FileStream(path, FileMode.Create))
        using (var bw = new BinaryWriter(fs))
        {
             bw.Write(_name);
            _main.Write(fs);
            _alt.Write(fs);
        }
    }

    private void OnGUI()
    {
        _so.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_propName, new GUIContent("Palette Name"), true);
        _main.DrawGuiLayout(_propMain, new GUIContent("Main Palette"));
        _alt.DrawGuiLayout(_propAlt, new GUIContent("Alternate Palette"));
        bool changed = EditorGUI.EndChangeCheck();
        _so.ApplyModifiedProperties();

        if (changed)
        {
            SaveToDisk();
        }

        GUI.enabled = !string.IsNullOrWhiteSpace(_name);
        if (GUILayout.Button("Generate Palette"))
        {
            for (int i = 0, j = 8; i < 8; i++, j++)
            {
                _colors[i] = _main.AsColor(i);
                _colors[j] = _alt.AsColor(i);
            }

            _paletteTex.SetPixels(_colors);
            _paletteTex.Apply(false, false);

            string path = $"{Application.dataPath}/Palettes/";
            Directory.CreateDirectory(path);
            File.WriteAllBytes($"{path}{_name}.png", _paletteTex.EncodeToPNG());
            AssetDatabase.Refresh();
        }

        GUI.enabled = true;
    
    }

    [Serializable]
    public unsafe struct Palette
    {
        public fixed float colors[8 * 4];

        public Color AsColor(int index) 
        { 
            fixed(float* ptr = colors)
            {
                return *(Color*)(ptr + (index * 4));
            }
        }

        public void DrawGuiLayout(SerializedProperty prop, GUIContent label)
        {
            prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, label, true);
            if (prop.isExpanded)
            {

                EditorGUI.indentLevel++;
                fixed (float* col = colors)
                {
                    Color* cPtr = (Color*)col;
                    for (int i = 0; i < 8; i++)
                    {
                        cPtr[i] = EditorGUILayout.ColorField(new GUIContent($"Color #{i}"), cPtr[i], true, false, true);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        public void Read(Stream stream)
        {
            fixed (float* col = colors)
            {
                Span<byte> buf = new Span<byte>(col, 8 * 4 * 4);
                stream.Read(buf);
            }
        }

        public void Write(Stream stream)
        {
            fixed (float* col = colors)
            {
                Span<byte> buf = new Span<byte>(col, 8 * 4 * 4);
                stream.Write(buf);
            }
        }
    }
}
