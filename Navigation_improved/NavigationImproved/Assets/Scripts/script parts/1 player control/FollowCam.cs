using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // reference to our target transform
    public Transform target;

    // dampening factor for the camera movement
    [Range(0, 10)]
    public float damping = 1.0f;

    // lower dead zone limit nothing will move inside
    // this area
    [Range(0, 0.5f)]
    public float deadZoneLower = 0.1f;

    // upper dead zone limit, movement values will always
    // be on maximum beyond this point
    [Range(0.5f, 1.0f)]
    public float deadZoneUpper = 0.7f;

    // the actual movement in units that are possible
    [Range(0, 20)]
    public float movementRange = 10.0f;
    
    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }
    
	void LateUpdate ()
    {
        // get NDC space mouse position, unity viewport has a [0, 1] range it seems we want [-1, 1]
        Vector3 mouse = _cam.ScreenToViewportPoint(Input.mousePosition) * 2.0f - new Vector3(1.0f, 1.0f, 0.0f);

        // target position 
        Vector3 targetPos = target.position;
        targetPos += new Vector3(GetMovementValue(mouse.x), 0.0f, GetMovementValue(mouse.y)) * movementRange;
        targetPos.y = transform.position.y;

        // dampen the actual movement
        transform.position = Vector3.Lerp(transform.position, targetPos, damping * Time.deltaTime);
	}

    float GetMovementValue(float NDCpos)
    {
        float result = (Mathf.Abs(NDCpos) - deadZoneLower) / (deadZoneUpper - deadZoneLower);
        result = Mathf.Clamp(result, 0.0f, 1.0f);
        result *= (NDCpos > 0.0f) ? 1.0f : -1.0f;

        return result;
    }
}
