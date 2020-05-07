using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class AIStateMachine1 : MonoBehaviour
{
    #region Variables


    [Header("Player seeking")]
    public GameObject AIEnemy;
    public GameObject player;
    public float distanceToSeek = 10f;
    public float distanceToStopSeek = 20f;
    

    [Header("Waypoint variables")] 
    public Transform[] waypoints;

    private int currentWaypoint = 0;
    private float minDistanceToWaypoint = 3f;
    private Transform Target;
   

    [Header("Wait Variables")] 
    public float waitTime = 2f;


    [Header("AI stat variables")]
    public float maxHealth = 100f;
    public float enemyHealth = 100f;
    public float damage = 20f;
    public float attackRange = 3f;
    float fleeHealth;
    public float enemySpeed = 1f;

    [Header("Health Displays")]
    public Text playerHealthDisplay;
    public Text enemyHealthDisplay;

    #endregion
    #region States
    public enum State
    {
        Patrol,
        Seek,
        Flee,
    }

    public State state;
    #endregion
    #region Common Code
    void Start()
    {
        Target = waypoints[currentWaypoint].transform;

        //FLEE WHEN HEALTH IS AT 25% OF MAXIMUM HEALTH
        fleeHealth = maxHealth / 4;

        NextState();
    }
    void Update()
    {
        enemyHealthDisplay.text = "Enemy HP: " + enemyHealth;
        playerHealthDisplay.text = "Player HP: " + PLAYER.playerHealth;

        //DETECT WHETHER HEALTH IS <25% AND FLEE IF IT IS
        if (enemyHealth < fleeHealth && enemyHealth < PLAYER.playerHealth)
        {
            state = State.Flee;
            FleeState();
        }
    }
    #region Move
    public void Move()
    {
        Debug.Log("Moving");
        float step = enemySpeed * Time.deltaTime;
        //IF OBJECT ISNT AT DESTINATION, GET CLOSER
        
        
            if (AIEnemy.transform.position != Target.position)
            {
                AIEnemy.transform.position = Vector3.MoveTowards(AIEnemy.transform.position, Target.position, step);
            }
            if (Vector3.Distance(AIEnemy.transform.position, Target.position) < minDistanceToWaypoint)
            {
                currentWaypoint++;
            }
    }

    #endregion


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

            //CheckForSeek();
            CheckForSeek();


            yield return 0;
        }
        //NEXT ORDER IS ISSUED
        Debug.Log("Patrol: Exit");
        NextState();
    }

   
    public void Patrol()
    {
        
        Debug.Log("Patrol Enter");
        //WHEN WAYPOINT IS REACHED, WAIT, THEN LOCK ONTO NEXT WAYPOINT
        float step = enemySpeed * Time.deltaTime;
        Target = waypoints[currentWaypoint].transform;

        AIEnemy.transform.position = Vector3.MoveTowards(AIEnemy.transform.position, Target.position, step);
        if (AIEnemy.transform.position == Target.position)
        {
            currentWaypoint++;
        }
        
        
        //REPEAT PATROL ROUTE WHEN FINISHED
        if (currentWaypoint >= waypoints.Length)
        {
            
            currentWaypoint = 0;
        }
        //MOVE TO NEXT WAYPOINT
        
    }
    #endregion
    #region Seek Code
    public void CheckForSeek()
    {
        //IF PLAYER IS NEAR...
        if(Vector3.Distance(player.transform.position, AIEnemy.transform.position) < distanceToSeek)
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
            Move();

            float step = enemySpeed * Time.deltaTime;
            Target = player.transform;

            AIEnemy.transform.position = Vector3.MoveTowards(AIEnemy.transform.position, Target.position, step);
            if (AIEnemy.transform.position == Target.position)
            {
                currentWaypoint++;
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
            enemyHealth = enemyHealth - damage;
            Debug.Log("enemy health = " + enemyHealth);

        }
    }
    #endregion
    #region Flee Code
    IEnumerator FleeState()
    {
        Debug.Log("Flee: Enter");
        while (state == State.Flee)
        {
            //ENTER RETREAT MODE
            Retreat();



            yield return 0;
        }
        Debug.Log("Flee: Exit");
        //NEXT ORDERS
        NextState();
    }
    public void Retreat()
    {


        //MOVE TO NEW DIRECTION AWAY FROM THE PLAYER
        
        float step = enemySpeed * Time.deltaTime * -2;
        Target = player.transform;

        AIEnemy.transform.position = Vector3.MoveTowards(AIEnemy.transform.position, Target.position, step);
        if (AIEnemy.transform.position == Target.position)
        {
            currentWaypoint++;
        }

    }
    
    #endregion
}
