using UnityEngine;
using System.Collections;

public class PlayerBehaviour_smoothwave : MonoBehaviour
{

    public float waveTransitionTime = 0.3f;
    private Animator _animator;
    private float _waveWeight = 0.0f;
    private float _transitionFactor;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _transitionFactor = 1.0f / waveTransitionTime;
    }

    void Update()
    {
        if (Input.GetKey("up"))
            _animator.SetInteger("AnimParam", 1);
        else _animator.SetInteger("AnimParam", 0);

        if (Input.GetKey("space"))
        {
            if (_waveWeight < 1.0f)
                _waveWeight = Mathf.Clamp(_waveWeight + _transitionFactor * Time.deltaTime, 0.0f, 1.0f);

            _animator.SetLayerWeight(1, _waveWeight);
        }
        else if (_waveWeight > 0.0f)
        {
            _waveWeight = Mathf.Clamp(_waveWeight - _transitionFactor * Time.deltaTime, 0.0f, 1.0f);
            
            _animator.SetLayerWeight(1, _waveWeight);
        }
    }
}
