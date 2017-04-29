using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : Collectable {
	private float scaleoverride = 0.37F;
	public AnimationCurve GrowCurve, RotCurve;
	public float GrowTime = 12.0f, RipeTime = 10.0F, RotTime = 8.0F;
	public bool Picked = false;
	public float GrowRatio
	{
		get{
			if(Picked) return GrowRatio_final;
			else if(lifetime < GrowTime) return lifetime/GrowTime;
			else return (lifetime - GrowTime) / RotTime;
		}
	}

	public bool Growing
	{
		get{return lifetime < GrowTime;}
	}

	private float GrowRatio_final;

	public Color startColor, ripeColor, rottenColor;

	private float ripeMass = 300;
	public float Ripeness
	{
		get{

			float d = lifetime/GrowTime;
			if(d < 1.0f) return d;

			d = 1.0F - (lifetime-GrowTime)/RotTime;

			return d;
		}
	}
	
	public override void Spawn()
	{
		base.Spawn();
		Picked = false;
		transform.localScale = Vector3.zero;
		if(!_rigidbody) return;
		_rigidbody.isKinematic = true;
		_rigidbody.useGravity = false;	
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();

		if(Picked) 
		{
			return;
		}
		
		//if(Collected) _rigidbody.AddForce(Vector3.down * 1.4F);
		if(Growing) 
		{
			transform.localScale = Vector3.one * GrowCurve.Evaluate(GrowRatio) * scaleoverride;
			_material.color = Color.Lerp(startColor, ripeColor, GrowRatio);
			_rigidbody.mass = ripeMass * GrowRatio;
		}
		else
		{
			transform.localScale = Vector3.one * RotCurve.Evaluate(GrowRatio) * scaleoverride;
			_material.color = Color.Lerp(ripeColor, rottenColor, GrowRatio);
		}
	}

	float bounceforce = 0.5F;
	public override void Tap()
	{
		GrowRatio_final = GrowRatio;
		Picked = true;
		
		_rigidbody.isKinematic = false;
		_rigidbody.useGravity = true;

		Vector3 rand = new Vector3(Random.Range(-0.3F, 0.3F), 1.0F, Random.Range(-0.3F, 0.3F));
		rand.Normalize();
		_rigidbody.AddForce(rand * ripeMass * 200);
	}
}
