using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public GameObject Player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        var pos = new Vector3(Player.transform.position.x, Player.transform.position.y);

        this.transform.position = pos;
    }
}
