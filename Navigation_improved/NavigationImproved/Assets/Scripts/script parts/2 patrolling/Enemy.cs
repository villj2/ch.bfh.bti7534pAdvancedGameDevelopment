using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public enum State
    {
        Patrolling,
        Chasing,
        Searching
    }

    // reference to the enmy hearing gameobject
    public EnemyHearing hearing;

    // ratio of sound volume heard to alert level gained
    [Range(0, 10)]
    public float soundAlertLevelIncrease = 1.0f;

    // reference to the enemy sight gameobject
    public EnemySight sight;

    // speed to use for chasing the player
    [Range(1, 100)]
    public float chaseSpeed = 6.0f;

    [Range(0, 30)]
    public float _alertLevelDecreaseTime = 10.0f;

    // current state
    private State _state = State.Patrolling;

    // current alert level [0, 1] 
    private float _alertLevel = 0.0f;

    // reference to the player transform
    private Transform _player;

    // last sound source or player sighting position for this agent
    private Vector3 _lastPositionOfInterest;


    // reference to the patrol path to use
    public PatrolPath path;

    // speed to use for patrolling
    [Range(1, 100)]
    public float patrolSpeed = 3.5f;

    // distance threshold to decide if a waypoint is 'reached'
    public float waypointReachedDistance = 1.2f;

    // reference to the NavMeshAgent component
    private NavMeshAgent _agent;

    // set the necessary references in the awake function
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();

    }
    // connect the sight and hearing callbacks on enable
    void OnEnable()
    {
        sight.playerSpotted += PlayerSpotted;
        hearing.soundHeard += SoundHeard;
    }
    // disconnect the sight and hearing callbacks on disable
    void OnDisable()
    {
        sight.playerSpotted -= PlayerSpotted;
        hearing.soundHeard -= SoundHeard;
    }

    // called when ever this enemy spots a player
    void PlayerSpotted(Vector3 position)
    {
        // change state to chasing immediately 
        _state = State.Chasing;
        _lastPositionOfInterest = position;
    }

    // called when ever this enemy hears a sound
    void SoundHeard(Vector3 position, float volume)
    {
        // update the last position of interest
        _lastPositionOfInterest = position;

        // increase alert level relative to the volume of the sound
        _alertLevel += soundAlertLevelIncrease * volume;
        _alertLevel = Mathf.Clamp(_alertLevel, 0.0f, 1.0f);

        // search the area the sound came from if we're not chasing anyone
        // and our alert level is high enough
        if (_alertLevel > 0.5f && _state != State.Chasing)
            _state = State.Searching;
    }
    // call the state appropriate function
    void Update()
    {
        switch (_state)
        {
            case State.Patrolling:
                Patrol();
                break;

            case State.Chasing:
                Chase();
                break;

            case State.Searching:
                Search();
                break;
        }
    }


    void Patrol()
    {
        // update speed
        _agent.speed = patrolSpeed;

        // current goal position based on our path object
        Vector3 dest = path.currentWaypoint.transform.position;

        sight.LookForward();

        // update agent destination
        _agent.SetDestination(dest);

        if (Vector3.Distance(dest, transform.position) < waypointReachedDistance)
            path.Next();
    }
    void Chase()
    {
        // if player is not in sight and we arrived at the last position he was seen at
        // then switch to searching the area
        if (!sight.playerInSight && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _state = State.Searching;
            return;
        }

        // always look at the current player position while chasing
        // (cheating to not lose him around edges)
        // todo:    a more 'natural' approach would be to let the agent
        //          look around after turning edges.
        sight.LookAtPosition(_player.position);

        // update speed
        _agent.speed = chaseSpeed;

        // update agent destination
        _agent.SetDestination(_lastPositionOfInterest);

        // keep allert level on maximum
        _alertLevel = 1.0f;
    }

    void Search()
    {
        // set the speed to a lerp between chase and patrol speed based on alert level
        _agent.speed = patrolSpeed + _alertLevel * (chaseSpeed - patrolSpeed);

        // decrease alert level over time
        if (_alertLevelDecreaseTime > 0.0f)
            _alertLevel -= Time.deltaTime / _alertLevelDecreaseTime;
        else
            _alertLevel = 0.0f;

        if (_alertLevel <= 0.0f)
        {
            _alertLevel = 0.0f;
            _state = State.Patrolling;
        }

        // keep looking at the current target
        sight.LookAtPosition(_lastPositionOfInterest);

        // todo:    let the agent search the immediate area around '_lastPositionOfInterest'
        //          rather than just walking there.
        _agent.SetDestination(_lastPositionOfInterest);
    }

    void OnDrawGizmos()
    {
        if (sight == null)
            return;

        // project the world space head direction onto the Y plane for visualization
        Vector3 headDirProj = Vector3.ProjectOnPlane(sight.headLookWS, Vector3.up);

        // add a half view angle rotation to center the view cone
        Quaternion rot = Quaternion.AngleAxis(-0.5f * sight.viewAngle, Vector3.up);
        // lerp the color based on alert level
        Handles.color = Color.Lerp(new Color(0.0f, 1.0f, 0.0f, 0.3f), new Color(1.0f, 0.0f, 0.0f, 0.3f), _alertLevel);
        Handles.DrawSolidArc(transform.position, Vector3.up, rot * headDirProj, sight.viewAngle, sight.viewDistance);
    }
}
