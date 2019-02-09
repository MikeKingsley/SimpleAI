using UnityEngine;
using UnityEngine.AI;

public class AISpawner : MonoBehaviour {

    public GameObject[] objs;
    [Range(0,200)] public float spawnRadius=50;
    [Range(0, 15)] public float waitTimer=5;
    [Range(1,1000)] public int spawnLimit=5;
    public bool autoSpawn;

    GameManager gameManager;
    float timeElapsed;
    int spawnCount;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager script not found");
        }
    }

    void Update () {
        if (autoSpawn && spawnCount <= spawnLimit && CheckWaitTimer(waitTimer))
        {
            timeElapsed = 0;
            InitSpawn();
        }
	}

    public void InitSpawn()
    {
        spawnCount++;
        SpawnObject(FindSpawnPoint(), PickRandomObject());
    }

    public void ResetSpawner()
    {
        spawnCount = 0;
        timeElapsed = 0;
    }

    bool CheckWaitTimer(float time)
    {
        timeElapsed += Time.deltaTime;
        return (timeElapsed >= time);
    }

    void SpawnObject(Vector3 point, GameObject obj)
    {
        GameObject spawned = Instantiate(obj, point, transform.rotation);
        gameManager.AddToList(spawned.GetComponent<AIController>());
    }

    Vector3 FindSpawnPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * spawnRadius) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint, out navHit, spawnRadius, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }

    GameObject PickRandomObject()
    {
        if (objs != null)
        {
            return objs[Mathf.RoundToInt(Random.Range(0, objs.Length))].gameObject;
        }

        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

}
