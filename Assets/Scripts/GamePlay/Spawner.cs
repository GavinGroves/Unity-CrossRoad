using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    [SerializeField]private int direction;
    [SerializeField]private List<GameObject> spawnObjects;

    private void Start()
    {
        InvokeRepeating(nameof(CreateSpawn),0.2f,Random.Range(5f,7f));
    }

    private void CreateSpawn()
    {
        int index = Random.Range(0, spawnObjects.Count);
        var target = Instantiate(spawnObjects[index], transform.position,Quaternion.identity,transform);
        target.GetComponent<MoveForward>().dir = direction;
    }
}
