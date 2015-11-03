using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Soundwave : MonoBehaviour {
    
    public float speed;    
    public float distance;    
    private float _radius = 0.0f;
    private SphereCollider _trigger;

    // normalized sound volume in range [0, 1]
    public float normalizedVolume
    {
        get { return 1.0f - _radius / distance; }
    }
    
    void Start()
    {
        _trigger = GetComponent<SphereCollider>();
        _trigger.isTrigger = true;
        _trigger.radius = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        _radius += Time.deltaTime * speed;        
        _trigger.radius = _radius;
        
        if (_radius > distance)
            Destroy(gameObject);
	}

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.1f);
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
