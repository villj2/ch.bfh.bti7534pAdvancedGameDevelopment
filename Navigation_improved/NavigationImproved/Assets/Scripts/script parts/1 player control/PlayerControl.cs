using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public Soundwave soundwave;

    [Range(0.5f, 1000f)]
    public float acceleration = 40.0f;
    [Range(1, 1000)]
    public float maxVelocityRangeStart = 1.0f;
    [Range(1, 1000)]
    public float maxVelocityRangeEnd = 10.0f;
    [Range(0, 1000)]
    public float friction = 1.0f;

    // current max velocity
    private float _maxVelocity;

    // increment steps of max velocity
    private float _maxVelInc = 1.0f;

    // current velocity
    private Vector3 _velocity;

    private float _footstepInterval = 0.5f;
    private float _footstepTimer = 0.0f;


    void Update()
    {
        // get input variables
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // update current max velocity based on mousewheele input
        if (scroll > 0)
            _maxVelocity += _maxVelInc;
        else if (scroll < 0)
            _maxVelocity -= _maxVelInc;

        _maxVelocity = Mathf.Clamp(_maxVelocity, maxVelocityRangeStart, maxVelocityRangeEnd);

        // move the player
        Vector3 accelDir = new Vector3(h, 0.0f, v);
        accelDir.Normalize();
        Move(accelDir);

        // emit a soundwave every _footstepInterval
        _footstepTimer += Time.deltaTime;
        if (_footstepTimer > _footstepInterval && _velocity.sqrMagnitude > 0.0f)
        {
            _footstepTimer = 0.0f;
            EmitSoundwave();
        }

    }

    void Move(Vector3 accelDir)
    {
        // apply friction
        float speed = _velocity.magnitude;
        if (speed != 0)
        {
            float drop = speed * friction * Time.deltaTime;
            _velocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        // accelerate
        float deltaVel = acceleration * Time.deltaTime;
        float projVel = Vector3.Dot(_velocity, accelDir);

        // see if the projected velocity exceeds our max velocity
        if (projVel + deltaVel > _maxVelocity)
            deltaVel = _maxVelocity - projVel;

        // increase velocity
        _velocity += accelDir * deltaVel;

        // update transform
        transform.position += _velocity * Time.deltaTime;
    }

    void EmitSoundwave()
    {
        Soundwave sound = Instantiate(soundwave, transform.position + Vector3.up * 0.1f, Quaternion.identity) as Soundwave;
        sound.distance = 10 * (_velocity.magnitude / maxVelocityRangeEnd);
    }
}
