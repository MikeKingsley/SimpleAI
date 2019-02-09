using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {

    [Range(0,200f)]
    public float wanderRadius = 40f;
    [Range(0,1000)]
    public int wanderPointsLimit = 1000;

    [Header("Stats")]
    public float teamCountBlue = 0;
    public float teamCountRed = 0;

    public Text playerCount_UI;

    public List<AIController> iControllers = new List<AIController>();
    public List<Vector3> wanderPoints = new List<Vector3>();

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        BuildWanderPoints(wanderPointsLimit);
    }

    public void AddToList(AIController controller)
    {
        SetTeamCount(controller.enemyStats.teamID, true);
        iControllers.Add(controller);
    }

    public void RemoveFromList(AIController controller)
    {
        SetTeamCount(controller.enemyStats.teamID, false);
        iControllers.Remove(controller);
    }

    void SetTeamCount(int team, bool add)
    {
        if (team == 1)
        {
            if (add) { teamCountBlue++; } else { teamCountBlue--; }
        } else if (team == 2)
        {
            if (add) { teamCountRed++; } else { teamCountRed--; }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        playerCount_UI.text = teamCountBlue.ToString() + " / " + teamCountRed.ToString();
    }

    void BuildWanderPoints(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            wanderPoints.Add(RandowWanderPoint());
        }
    }

    Vector3 RandowWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint, out navHit, wanderRadius, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

    /// <summary>Checks line of sight from this object to the passed argument transform.</summary>
    /// <param name="start">Transform to start from.</param>
    /// <param name="target">Vector3 target point.</param>
    /// <param name="rayCastStart">Vector3 ray cast position to start from.</param>
    /// <param name="range">Max distance range.</param>
    /// <param name="fov">Sight field of view.</param>
    /// <param name="mask">Layer to check for obstacles. Use -1 for all layers.</param>
    /// <returns>Bool: true if has line of sight, else false.</returns>
    public bool LineOfSight(Transform caller, Vector3 target, Vector3 rayCastStart, float range, float fov, int mask)
    {
        float dstToTarget = Vector3.Distance(caller.position, target);
        Vector3 dirToTarget = (target - caller.position).normalized;
        if (dstToTarget <= range && Vector3.Angle(caller.forward, dirToTarget) < fov / 2)
        {
            if (!Physics.Raycast(rayCastStart, dirToTarget, dstToTarget, mask))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Turns object to the face the in the direction of the target transform over time.</summary>
    /// <param name="start">Transform to turn</param>
    /// <param name="target">Vector3 to turn towards</param>
    /// <param name="speed">Rate of turn</param>
    public Quaternion TurnToFace(Transform start, Vector3 target, float speed)
    {
        Vector3 dirToTarget = (target - start.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dirToTarget.x, 0, dirToTarget.z));
        return Quaternion.Slerp(start.rotation, lookRotation, Time.deltaTime * speed);
    }


}
