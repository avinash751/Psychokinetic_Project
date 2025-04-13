using Buffers;
using CustomInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class UiGridSpawner : MonoBehaviour,IResettable
{
    [HorizontalLine("References",2,FixedColor.BabyBlue)]
    [SerializeField] protected GameObject uiPrefabToSpawn;
    [SerializeField] protected RectTransform spawnStartPosition;

    [HorizontalLine("UI Spawn Settings", 2, FixedColor.BabyBlue)]
    [SerializeField] protected Vector3 cellSize;
    [SerializeField] protected  int maxCellsPerRow;
    [SerializeField] protected float XOffset;
    [SerializeField] protected float YOffset;

    [HorizontalLine("Debug", 2, FixedColor.BabyBlue)]  
    [ReadOnly][SerializeField] int rowIndex = 0;
    [ReadOnly][SerializeField] int columnIndex = 0;
    [Resettable][ReadOnly][SerializeField] protected List<GameObject> spawnedUI = new List<GameObject>();


    protected void SpawnUiInBulk(GameObject _uiToSpawn, int _amountToSpawn)
    {
        for (int i = 0; i < _amountToSpawn; i++)
        {
            SpawnUiIndividually(_uiToSpawn);
        }
    }
    protected virtual void SpawnUiIndividually(GameObject _uiToSpawn)
    {
        if (uiPrefabToSpawn == null || spawnStartPosition == null) return;
        if (rowIndex >= maxCellsPerRow) { rowIndex = 0; columnIndex++; }

        Vector3 cellOffset = new Vector3(XOffset * rowIndex, YOffset * columnIndex, 0);
        Vector3 cellSpawnPosition = spawnStartPosition.position + cellOffset;

        GameObject _newSpawnedUi = Instantiate(_uiToSpawn, cellSpawnPosition, Quaternion.identity);
        _newSpawnedUi.transform.SetParent(spawnStartPosition.transform);
        _newSpawnedUi.GetComponent<RectTransform>().localScale = cellSize;
        spawnedUI.Add(_newSpawnedUi);
        UpdateUIOnSpawn();

        rowIndex++;
    }

    /// <summary>
    /// If ui has certain elements that need to be updated after just being spawned 
    /// Then this function should be overridden in the child class
    /// </summary>

    protected virtual void UpdateUIOnSpawn()
    {
    }  
}