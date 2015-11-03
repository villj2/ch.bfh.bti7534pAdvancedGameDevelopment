using UnityEngine;
using UnityEditor;


[RequireComponent(typeof(SphereCollider))]
public class EnemySight : MonoBehaviour
{
    public delegate void PlayerSpottedDelegate(Vector3 position);
    public event PlayerSpottedDelegate playerSpotted;
    
    [Range(0, 360)]
    public float viewAngle = 70;

    [Range(0, 100)]
    public float viewDistance = 10.0f;
    
    // reference to the sphere collider trigger
    private SphereCollider _sightTrigger;

    // current head look vector, the agent can look 360° independent of his body
    // todo:    add a 'head look range' property to limit head rotation relative to body
    //          and show it in a similar way to the view angle
    private Vector3 _headLook = Vector3.forward;

    // player in sight flag
    private bool _playerInSight = false;
    public bool playerInSight
    {
        get { return _playerInSight; }
    }

    // last world position the player was seen at
    private Vector3 _lastPlayerSighting;
    public Vector3 lastPlayerSighting
    {
        get { return _lastPlayerSighting; }
    }

    // current look at direction in world space
    public Vector3 headLookWS
    {
        get { return transform.rotation * _headLook; }
    }
    
    void Awake()
    {
        _sightTrigger = GetComponent<SphereCollider>();
        _sightTrigger.isTrigger = true;
        _sightTrigger.radius = viewDistance; // TODO: update radius if the setting changes!
    }

    public void LookAtPosition(Vector3 position)
    {
        // since _headLook is in local space all we have to do to look at 
        // a given world space position is:
        // 1. transform to local space; 2. normalize.
        _headLook = transform.InverseTransformPoint(position).normalized;
    }

    public void LookForward()
    {
        _headLook = Vector3.forward;
    }

    // check if the player is in our sight range
    void OnTriggerStay(Collider other)
    {
        // early out for everything other than the player object
        if (other.gameObject.tag != "Player")
            return;
        
        _playerInSight = false;

        // calculate the angle between the agent's head look vector
        // and the player's position
        Vector3 direction = other.transform.position - transform.position;
        float angle = Vector3.Angle(direction, headLookWS);

        // return if the player is outside of our view angle
        if (angle > (0.5f * viewAngle))
            return;

        // the player is inside our view cone, test for occlusion by shooting a ray
        RaycastHit hit;        
        if (Physics.Raycast(transform.position, direction.normalized, out hit, _sightTrigger.radius))
        {
            if (hit.collider.gameObject.tag != "Player")
            {
                // visualize the occluded ray
                Debug.DrawLine(transform.position, hit.point, Color.red, 5.0f, false);
                return;
            }

            // visualize the ray that hit the player
            Debug.DrawLine(transform.position, hit.point, Color.green, 5.0f, false);
            
            _lastPlayerSighting = other.transform.position;
            _playerInSight = true;

            // invoke the callback if it has subscribers
            if (playerSpotted != null)
                playerSpotted(other.transform.position);
        }
    }

    // reset the player in sight flag on collider exit
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        _playerInSight = false;
    }
}
