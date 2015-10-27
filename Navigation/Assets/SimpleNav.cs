using UnityEngine;
using System.Collections;

public class SimpleNav : MonoBehaviour {

    public Transform target;
    public NavMeshAgent agent;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        agent.SetDestination(target.position);
	}
}
