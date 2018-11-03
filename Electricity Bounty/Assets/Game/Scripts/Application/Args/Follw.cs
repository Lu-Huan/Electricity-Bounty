using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follw : MonoBehaviour {
    public Transform MainChar;
    private Transform Transform;
	// Use this for initialization
	void Start () {
        MainChar = MainChar.GetComponent<Transform>();
        Transform= GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        float x = MainChar.position.x;
        float z = MainChar.position.z;

        Transform.position = new Vector3(x, Transform.position.y, z-5);
    }
}
