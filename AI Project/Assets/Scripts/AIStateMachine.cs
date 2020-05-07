using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIStateMachine : MonoBehaviour
{
    #region Variables
    private NavMeshAgent agent;

    [Header("Player seeking")]
    public GameObject player;
    public float distanceToSeek = 10f;
    public float distanceToStopSeek = 20f;
    

    [Header("Waypoint variables")] 
    public Transform[] waypoints;

    private int currentWaypoint = 0;
    private float minDistanceToWaypoint = 3f;
   

    [Header("Wait Variables")] 
    public float waitTime = 2f;


    [Header("AI stat variables")]
    public float maxHealth = 100f;
    public float enemyhealth = 100f;
    public float damage = 20f;
    public float attackRange = 3f;
    float fleeHealth;

    #endregion
    #region States
    public enum State
    {
        Patrol,
        Seek,
        Wait,
        Flee,
    }

    public State state;
    #endregion
    #region Common Code
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        //FLEE WHEN HEALTH IS AT 25% OF MAXIMUM HEALTH
        fleeHealth = maxHealth / 4;

        //LEMME KNOW IF THE AGENT ISNT CONNECTED
        if (agent == null)
        {
            Debug.LogError("Agent not attached to MoveAI");
        }

        NextState();
    }
    void Update()
    {
        //DETECT WHETHER HEALTH IS <25% AND FLEE IF IT IS
        if (enemyhealth < fleeHealth && enemyhealth < PLAYER.playerHealth)
        {
            state = State.Flee;
            FleeState();
        }
    }
    public void Move(Transform destination)
    {
        //IF OBJECT ISNT AT DESTINATION, GET CLOSER
        if (destination != null)
        {
            if (agent.destination != destination.position)
            {
                agent.SetDestination(destination.position);
            }
        }
    }


    public void Stop()
    {
        //IF OBJECT IS AT DESTINATION, STOP
        if (agent.destination != agent.transform.position)
        {
            agent.SetDestination(agent.transform.position);
        }
    }

    void NextState()
    {
        //FIND NAME OF NEXT STATE
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                   System.Reflection.BindingFlags.NonPublic |
                                   System.Reflection.BindingFlags.Instance);
        //ENTER NEXT STATE
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }
    #endregion
    #region Patrol Code
    IEnumerator PatrolState()
    {
        Debug.Log("Patrol: Enter");
        while (state == State.Patrol)
        {
            //WHILE PATROLING, CHECK IF PLAYER IS NEARBY

            Patrol();

            CheckForSeek();



            yield return 0;
        }
        //NEXT ORDER IS ISSUED
        Debug.Log("Patrol: Exit");
        NextState();
    }

    IEnumerator WaitState()
    {
        Debug.Log("Wait: Enter");
        float waitStartTime = Time.time;
        while (state == State.Wait)
        {
            //STOP AND WAIT A FEW SECONDS
            Stop();

            if (Time.time > waitStartTime + waitTime)
            {
                state = State.Patrol;
            }

            CheckForSeek();

            yield return 0;
        }
        //NEXT ORDER IS ISSUED
        Debug.Log("Wait: Exit");
        NextState();
    }
    public void Patrol()
    {
        //WHEN WAYPOINT IS REACHED, WAIT, THEN LOCK ONTO NEXT WAYPOINT
        if (Vector3.Distance(agent.transform.position, waypoints[currentWaypoint].transform.position) < minDistanceToWaypoint)
        {
            state = State.Wait;


            currentWaypoint++;
        }
        //REPEAT PATROL ROUTE WHEN FINISHED
        if (currentWaypoint >= waypoints.Length)
        {
            
            currentWaypoint = 0;
        }
        //MOVE TO NEXT WAYPOINT
        Move(waypoints[currentWaypoint].transform);
    }
    #endregion
    #region Seek Code
    public void CheckForSeek()
    {
        //IF PLAYER IS NEAR...
        if(Vector3.Distance(player.transform.position, agent.transform.position) < distanceToSeek)
        {
            //...FOLLOW THE PLAYER
            state = State.Seek;
        }
    }
    IEnumerator SeekState()
    {
        //WHILE IN SEEK MODE
        Debug.Log("Seek: Enter");
        while (state == State.Seek)
        {
            //MOVE TOWARDS THE PLAYER
            Move(player.transform);

            if (Vector3.Distance(player.transform.position, agent.transform.position) > distanceToStopSeek)
            {
                //GO BACK TO PATROLLING IF PLAYER IS TOO FAR
                state = State.Patrol;
            }
            


            yield return 0;
        }
        //NEXT ORDERS
        Debug.Log("Seek: Exit");
        NextState();
    }
    #endregion
    #region Attack Code
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            //IF A BULLET HITS, TAKE AWAY HEALTH
            enemyhealth = enemyhealth - damage;
            Debug.Log("enemy health = " + enemyhealth);

        }
    }
    #endregion
    #region Flee Code
    public void Retreat(Transform destination)
    {
        //CHANGE DIRECTION OF MOVEMENT
        Vector3 newDir = transform.position - player.transform.position;

        Vector3 newPos = transform.position + newDir;

        //MOVE TO NEW LOCATION AWAY FROM THE PLAYER
        agent.SetDestination(newPos);


    }
    IEnumerator FleeState()
    {
        Debug.Log("Flee: Enter");
        while (state == State.Flee)
        {
            //ENTER RETREAT MODE
            Retreat(player.transform);

            
            
            yield return 0;
        }
        Debug.Log("Flee: Exit");
        //NEXT ORDERS
        NextState();
    }
    #endregion
}
