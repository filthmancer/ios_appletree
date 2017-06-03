using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {
	public float lifetime = 0.0F;
	public GameObject Model;
	protected Rigidbody _rigidbody;
	protected Material _material;
	protected bool Collected;
	// Use this for initialization
	void Start () {
		lifetime = 0.0F;
		_rigidbody = Model.GetComponent<Rigidbody>();
		_material = Model.GetComponent<Renderer>().material;
	}

	public virtual void Spawn()
	{
		Collected = false;
		lifetime = 0.0F;
	}
	
	// Update is called once per frame
	public virtual void Update () {
		lifetime += Time.deltaTime;
	}

	public void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Destruct") 
		{
			Destroy();
		}
		else if(c.tag == "Crate" && !Collected)
		{
			Collected = true;
			GameManager.instance.Collect(this);
		}
	}

	public void OnTriggerExit(Collider c)
	{
		if(c.tag == "Crate" && Collected)
		{
			Collected = false;
			GameManager.instance.UnCollect(this);
		}
	}

	public void OnCollisionEnter(Collision c)
	{
		if(c.transform.tag == "Collect")
		{
			//transform.SetParent(c.transform);
			FixedJoint j = this.gameObject.AddComponent<FixedJoint>();
			j.connectedBody = c.transform.GetComponent<Rigidbody>();
			j.breakForce = 3;
			j.breakTorque = 20;
		}
	}

	public virtual void Tap()
	{

	}

	public void Destroy()
	{
		ObjectPoolRef r = this.GetComponent<ObjectPoolRef>();
		if(r != null) r.Pool.Unspawn(this.gameObject);
		else Destroy(this.gameObject);
	}
}
