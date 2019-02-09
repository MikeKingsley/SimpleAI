using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    public float waitTimer = 2f;
    public int spawnLimit = 0;
    public bool checkSpawnClear = true;
    public LayerMask layerMask;
    public GameObject[] prefab;

    float timeElapsed;
    SphereCollider sphereCollider;
    int spawnCount = 0;

    void Start()
    {
        Spawn();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {        
        if (CheckSpawnCount() && CheckWaitTimer() && SpawnAreaClear())
        {
            Spawn();
        }
    }

    bool CheckSpawnCount()
    {
        if (spawnLimit > 0 && spawnCount <= spawnLimit)
        {
            return true;
        } else if (spawnLimit == 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    bool CheckWaitTimer()
    {
        timeElapsed += Time.deltaTime;
        return (timeElapsed >= waitTimer);
    }

    void Spawn()
    {
        Instantiate(PickRandomPrefab(), transform.position, transform.rotation);
        timeElapsed = 0;
        spawnCount++;
    }

    bool SpawnAreaClear()
    {
        if (checkSpawnClear)
        {
            Vector3 center = sphereCollider.transform.position + sphereCollider.center;
            float radius = sphereCollider.radius;

            Collider[] allOverlappingColliders = Physics.OverlapSphere(center, radius, layerMask);

            return (allOverlappingColliders.Length == 0);
        } else
        {
            return true;
        }
    }

    public GameObject PickRandomPrefab()
    {
        if (prefab != null)
        {
            return prefab[Mathf.RoundToInt(Random.Range(0, prefab.Length))].gameObject;
        }
        else
        {
            Debug.LogWarning("No prefabs defined in " + transform.name);
            return null;
        }
    }

}
