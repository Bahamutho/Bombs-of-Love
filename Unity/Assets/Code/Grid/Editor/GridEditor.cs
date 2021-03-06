﻿using UnityEngine;
using UnityEditor;
using NUnit.Framework;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    private Grid grid;

    public void Awake()
    {
        //Debug.Log("Awake");
        grid = Selection.activeGameObject.GetComponent<Grid>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update Layers"))
            grid.CreateUpdateLayerContainers();
        if (GUILayout.Button("Instantiate All"))
            grid.InstantiateAll();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete ALL"))
            grid.DeleteAll();
        if (GUILayout.Button("Delete Runtime"))
            grid.DeleteRuntime();
        GUILayout.EndHorizontal();
        

        grid.SelectedGridPrefab = grid.GridLevelData.PrefabList.SelectedGridPrefab;
        if (grid.SelectedGridPrefab != null)
            grid.ObjectToSpawn = grid.SelectedGridPrefab.Prefab;
    }

    public void OnSceneGUI()
    {
        if (grid == null)
            grid = Selection.activeGameObject.GetComponent<Grid>();
        //Debug.Log("OnSceneGUI");
        // Refresh faster
        if (Event.current.type == EventType.MouseMove)
        {
            SceneView.RepaintAll();
            if(grid != null)
                grid.SelectedGridCoord = grid.GetSceneMouseGridCoord();
        }

        if(Event.current.keyCode == grid.SpawnObjectKeyCode)
        {
            //Debug.Log("KeyCode: " + Event.current.keyCode);
            grid.SpawnObject();
            SceneView.RepaintAll();
        }
        if (Event.current.keyCode == grid.DeleteAllKeyCode)
        {
            //Debug.Log("KeyCode: " + Event.current.keyCode);
            grid.ClearAllOnSelectedGridCoord();
            SceneView.RepaintAll();
        }
        if (Event.current.keyCode == grid.DeleteLayerKeyCode)
        {
            //Debug.Log("KeyCode: " + Event.current.keyCode);
            grid.ClearOnSelectedLayerAndGridCoord();
            SceneView.RepaintAll();
        }

        //GridPrefabListEditor e = Editor.CreateEditor(grid.LevelData.PrefabList) as GridPrefabListEditor;
        //Debug.Log(e.SelectedGridPrefab);


        // Keep this object selected
        int controlID = GUIUtility.GetControlID(FocusType.Native);
        if (Event.current.type == EventType.layout)
            HandleUtility.AddDefaultControl(controlID);

        //// Spawn selecte object on mouseclick
        if (Event.current.type == EventType.MouseUp)
        {
            grid.SpawnObject();
            SceneView.RepaintAll();
        }
    }

    
}
