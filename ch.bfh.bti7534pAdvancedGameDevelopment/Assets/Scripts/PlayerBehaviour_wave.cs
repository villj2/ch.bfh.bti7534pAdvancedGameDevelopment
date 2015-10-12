using UnityEngine;
using System.Collections;

public class PlayerBehaviour_wave : MonoBehaviour {
    
	private Animator _animator;

    // Use this for initialization
    void Start () {
		_animator = GetComponent<Animator>();
	}

    void Update()
    {
        if (Input.GetKey("up"))
            _animator.SetInteger("AnimParam", 1);
        else _animator.SetInteger("AnimParam", 0);
        
        if (Input.GetKey("space"))
        {
            _animator.SetLayerWeight(1, 1.0f);
        }
        else
        {
            _animator.SetLayerWeight(1, 0.0f);
        }
    }
}
