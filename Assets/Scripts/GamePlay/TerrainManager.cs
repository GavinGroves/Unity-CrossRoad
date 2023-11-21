using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainManager : MonoBehaviour
{
    public float offsetY;
    [SerializeField]private List<GameObject> terrainObjects;
    private GameObject spawnObjects;
    private int lastIndex;

    // private void Start()
    // {
    //     CheckPosition();
    // }

    private void OnEnable()
    {
        EventHandler.GetPointEvent += OnGetPointEvent;
    }

    private void OnDisable()
    {
        EventHandler.GetPointEvent -= OnGetPointEvent;
    }
    
    private void OnGetPointEvent(int obj)
    {
        CheckPosition();
    }

    public void CheckPosition()
    {
        if (transform.position.y - Camera.main.transform.position.y < offsetY / 2)
        {
            transform.position = new Vector3(0, Camera.main.transform.position.y + offsetY, 0);
            SpawnTerrain();
        }
    }
    
    private void SpawnTerrain()
    {
        int randomIndex = Random.Range(0, terrainObjects.Count);
        while (lastIndex == randomIndex)
        {
            randomIndex = Random.Range(0, terrainObjects.Count);
        }
        lastIndex = randomIndex;
        spawnObjects = terrainObjects[randomIndex];
        Instantiate(spawnObjects, transform.position, Quaternion.identity);
    }
}
