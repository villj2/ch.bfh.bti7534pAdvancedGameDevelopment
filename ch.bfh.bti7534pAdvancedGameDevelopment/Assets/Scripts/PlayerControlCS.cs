using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerControlCS : MonoBehaviour
{
    private Animator _animator;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("Forward", Input.GetAxis("Vertical"));
        _animator.SetFloat("Turn", Input.GetAxis("Horizontal"));
    }
}