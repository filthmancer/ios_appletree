using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraPivot : MonoBehaviour {

	[Range(0,360)]
	public float Rotation;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, Rotation, 0), Time.deltaTime * 15);
	}

	public void RotateBy(float r)
	{
		Rotation += r;
	}
}
