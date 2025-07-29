using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelGenerator levelGen = (LevelGenerator)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Level"))
        {
            levelGen.GenerateLevel();
        }

        if (GUILayout.Button("Print Level Grid"))
        {
            //levelGen.PrintGrid();
        }

        if(GUILayout.Button("Instantiate Level"))
        {
            //Debug.Log(Tile.TILE_PREFABS.Count);
            GameManager gm = GameManager.GetGameManager();
            gm.levelGen = (LevelGenerator)target;
            gm.Start();
        }
    }
}
