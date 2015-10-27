using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class Patrolling : MonoBehaviour
{
    public float ViewDistance = 10.0f;

    // list of our waypoints to traverse
    public List<Transform> _waypoints;

    // connect our path at the start and end point
    public bool closedPath;

    // reference to the NavMeshAgent component
    private NavMeshAgent _agent;

    private SphereCollider _sightTrigger;
	
	// distance threshold to decide if a waypoint is 'reached'
	public float waypointReachedDistance = 1.2f;

    // the direction we traverse the waypoints array in
    private bool _goingRight;

    // the index of the next waypoint to visit
    private int _nextWaypointIndex = 0;

    private Vector3 _lastPlayerPosition;
    private bool _playerInsight = false;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _sightTrigger = GetComponent<SphereCollider>();
        _sightTrigger.radius = ViewDistance;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        _lastPlayerPosition = other.transform.position;
        _playerInsight = true;
    }

    void Update()
    {
        if (!_playerInsight)
        {
            Vector3 dest = _waypoints[_nextWaypointIndex].position;

            // Normal waypoint traversal (patrolling) starts here
            _agent.SetDestination(dest);

            // check if we've reached our current destination 
            // waypoint comparing the distance against a threshold
            if (Vector3.Distance(dest, transform.position) < waypointReachedDistance)
            {
                // reverse direction if our path isn't closed
                if (!closedPath &&
                    (_goingRight && _nextWaypointIndex == _waypoints.Count - 1
                    || !_goingRight && _nextWaypointIndex == 0))
                {
                    _goingRight = !_goingRight;
                }

                if (_goingRight) _nextWaypointIndex++;
                else _nextWaypointIndex--;

                while (_nextWaypointIndex < 0)
                    _nextWaypointIndex += _waypoints.Count;

                _nextWaypointIndex %= _waypoints.Count;
            }
        }
        else
        {
            _agent.SetDestination(_lastPlayerPosition);
        }

        
    }

    // very simple visualization of waypoints
    void OnDrawGizmos()
    {
        // waypoints
        bool first = true;
        Vector3 prevPos = Vector3.zero;
        foreach (var wp in _waypoints)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(wp.position, 0.2f);

            if (!first)
                Gizmos.DrawLine(prevPos, wp.position);

            prevPos = wp.position;
            first = false;
        }

        if (closedPath && _waypoints.Count > 1)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_waypoints[0].position,
                            _waypoints[_waypoints.Count - 1].position);
        }
    }
}
