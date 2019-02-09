using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    public bool resetPath = false;
    public float destinationReached = 1f;
    public Transform handSlot;
    public Transform chest;
    public Transform eyes;
    public GameObject weapon;
    public LayerMask layerMask;
    public LayerMask obstacleMask;
    public AIStats enemyStats;

    [Header("Ragdoll")]
    public bool ragdoll;
    public Rigidbody[] ragdollParts;

    public bool DrawGizmos;

    [HideInInspector] public bool alive = true;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public AIGun gun;
    GameManager gameManager;
    NavMeshAgent agent;
    Animator anim;
    float timeElapsed;
    float velocity;
    bool ragdollPrevState = false;
    public AIController currentTarget = null;
    Vector3 wanderPoint;
    bool takingHit;


    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager script not found");
        }

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = enemyStats.walkSpeed;

        currentHealth = enemyStats.Health;
        wanderPoint = GetWanderPoint();

        SpawnWeapon();
    }

    void Update()
    {
        if (alive)
        {
            Wander();
            AttackTarget();

            velocity = agent.velocity.magnitude;
            anim.SetFloat("speed", velocity);

            if (resetPath)
            {
                ResetPatrol();
            }
        } else if (CheckWaitTimer(2))
        {
            DestoryAI();
        }

        if (ragdoll != ragdollPrevState)
        {
            ToggleRagdoll();
            ragdollPrevState = ragdoll;
        }
    }

    void DestoryAI()
    {
        gameManager.RemoveFromList(this);
        gameObject.layer = 0;
        Destroy(GetComponent<AIController>());
        Destroy(anim);
        Destroy(agent);
        Destroy(gameObject, 15f);
    }

    bool CheckWaitTimer(float time)
    {
        timeElapsed += Time.deltaTime;
        return (timeElapsed >= Random.Range((time / 2), (enemyStats.attackRate * 2)));
    }

    void SpawnWeapon()
    {
        GameObject weap = Instantiate(weapon, handSlot.transform.position, handSlot.transform.rotation, handSlot);
        gun = weap.GetComponent<AIGun>();
    }

    void Wander()
    {
        if (!agent.pathPending && agent.remainingDistance < destinationReached)
        {
            wanderPoint = GetWanderPoint();
            agent.destination = wanderPoint;
        }
    }

    Vector3 GetWanderPoint()
    {
        return gameManager.wanderPoints[Random.Range(0, gameManager.wanderPointsLimit)];
    }

    void AttackTarget()
    {
        if (currentTarget == null)
            currentTarget = FindClosestTarget();

        if (currentTarget != null && currentTarget.alive && LineOfSight(currentTarget.transform))
        {
            StopPatrol();
            TurnToFace(currentTarget.transform);
            AimAtTarget(currentTarget);
            anim.SetBool("shooting", true);

            if (CheckWaitTimer(enemyStats.attackRate))
            {
                timeElapsed = 0;
                gun.Shoot(this);
            }
        } else
        {
            anim.SetBool("shooting", false);
            currentTarget = null;
        }

    }

    public void TakeDamage(float damage, AIController controller)
    {
        if (currentTarget == null)
        {
            currentTarget = controller;
        }
        currentHealth -= damage;
        StopPatrol();
        CheckAlive();
        
    }

    void CheckAlive()
    {
        if (currentHealth <= 0)
        {
            alive = false;
            ragdoll = true;

            if (Random.Range(1, 100) == 50)
            {
                DropWeapon();
            }
        }
    }

    void DropWeapon()
    {
        if (gun != null)
        {
            gun.transform.parent = null;
            Rigidbody gunrb = gun.GetComponent<Rigidbody>();
            gunrb.useGravity = !gunrb.useGravity;
            gunrb.isKinematic = !gunrb.isKinematic;
            Destroy(gun.gameObject, 30f);
        }
    }

    /// <summary>
    /// Clears the current path
    /// </summary>
    void StopPatrol()
    {
        agent.ResetPath();
    }

    void ResetPatrol()
    {
        agent.ResetPath();
        resetPath = false;
        anim.SetBool("shooting", false);
    }

    void AimAtTarget(AIController controller)
    {
        if (controller != null)
        {
            handSlot.LookAt(controller.chest);
        }
    }

    /// <summary>
    /// Loops through GameManagers list of AI controllers to find the closest target.
    /// </summary>
    /// <returns>Controller of target</returns>
    AIController FindClosestTarget()
    {
        AIController closest = null;
        float distance = Mathf.Infinity;

        for (int i = 0; i < gameManager.iControllers.Count; i++)
        {
            if (enemyStats.teamID != gameManager.iControllers[i].enemyStats.teamID)
            {
                float dstToTarget = Vector3.Distance(transform.position, gameManager.iControllers[i].transform.position);

                if (LineOfSight(gameManager.iControllers[i].transform) && dstToTarget < distance)
                {
                    distance = dstToTarget;
                    closest = gameManager.iControllers[i];
                }
            }

        }

        return closest;
    }

    /// <summary>
    /// Checks line of sight from this object to the passed argument transform
    /// </summary>
    /// <param name="target">Transform to check</param>
    /// <returns>Bool: true if has line of sight, else false.</returns>
    bool LineOfSight(Transform target)
    {
        float dstToTarget = Vector3.Distance(transform.position, target.position);
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (dstToTarget <= enemyStats.lookRange && Vector3.Angle(transform.forward, dirToTarget) < enemyStats.fov / 2)
        {
            if (!Physics.Raycast(gun.firePoint.position, dirToTarget, dstToTarget, obstacleMask))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Turns object to the face the in the direction of the target transform over time.
    /// </summary>
    /// <param name="target">Transform to turn towards</param>
    void TurnToFace(Transform target)
    {
        if (target != null)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dirToTarget.x, 0, dirToTarget.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void ToggleRagdoll()
    {
        if (ragdollParts.Length > 0)
        {
            anim.enabled = !anim.enabled;
            foreach (Rigidbody rb in ragdollParts)
            {
                rb.useGravity = !rb.useGravity;
                rb.isKinematic = !rb.isKinematic;
            }
        }
    }

    Vector3 DirFromAngle(float angleInDeg, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDeg += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDeg * Mathf.Deg2Rad));
    }

    void OnDrawGizmosSelected()
    {
        Vector3 leftFovLine = DirFromAngle(-enemyStats.fov / 2, false);
        Vector3 rightFovLine = DirFromAngle(enemyStats.fov / 2, false);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyStats.lookRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wanderPoint, 1f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + leftFovLine * enemyStats.lookRange);
        Gizmos.DrawLine(transform.position, transform.position + rightFovLine * enemyStats.lookRange);
    }
}
