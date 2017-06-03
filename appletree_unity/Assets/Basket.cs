using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour {

	public Rigidbody BasketObject;
	CameraPivot pivoter;

	// Use this for initialization
	void Start () {
		pivoter = CameraManager.instance.GetComponent<CameraPivot>();

		BasketObject.transform.LookAt(RotationPoint.position);
		BasketObject.transform.position = RotationPoint.position + Vector3.back * GameManager.instance.BasketDepth;
	}
	
	// Update is called once per frame
	void Update () {
		ControlBasket();
		RotateBasket();
	}

	public void FixedUpdate()
	{
		Vector3 targpos = BasketObject.transform.position;
		targpos += -BasketObject.transform.right * GameManager.instance.ControlVelocity;
		targpos.y = RotationPoint.position.y;
		Vector3 actual = (targpos - RotationPoint.position).normalized * GameManager.instance.BasketDepth + RotationPoint.position;

		Vector3 vel = (actual - BasketObject.transform.position).normalized;
		vel.y = 0.0F;
		BasketObject.velocity = vel * BasketSpeed;

	}
	public Transform RotationPoint;
	private float BasketOffset = 3.3F;
	private float BasketSpeed = 2.6F;
	private float BasketRot = 0.0F;
	private float CamAngle = 0.0F;
	private float CamSpeed = 0.0F, CamMaxSpeed = 2.6F, CamDecay = 0.9F;
	Quaternion CamRotation;
	bool camrotate;
	public void ControlBasket()
	{
		Vector3 lookpos = RotationPoint.position;
		lookpos.y = BasketObject.transform.position.y;

		Vector3 targ = pivoter.transform.position - BasketObject.transform.position;
		float angle = Vector3.Angle(targ, pivoter.transform.forward);
		if(angle > 30)
		{
			int sign = Vector3.Cross(targ, pivoter.transform.forward).y < 0 ? 1 : -1;
			angle *= sign;
			angle /= 30;

			CamSpeed += angle;
			if(CamSpeed > CamDecay) CamSpeed -= CamDecay;
				else if(CamSpeed < -CamDecay) CamSpeed += CamDecay;
				else CamSpeed = 0.0F;

			CamSpeed = Mathf.Clamp(CamSpeed, -CamMaxSpeed, CamMaxSpeed);

			pivoter.transform.rotation = pivoter.transform.rotation * Quaternion.Euler(0,CamSpeed, 0);
		}


		
	}

	public void RotateBasket()
	{
		Vector3 lookpos = RotationPoint.position;
		lookpos.y = BasketObject.transform.position.y;

		if(BasketRot > 0.05F) BasketRot -= 0.82F;
		else if(BasketRot < -0.05F) BasketRot += 0.82F;
		else BasketRot = 0.0F;
		BasketRot += GameManager.instance.ControlVelocity * 5.0F;
		BasketRot = Mathf.Clamp(BasketRot, -20F, 20F);

		Quaternion newrot = Quaternion.LookRotation(lookpos - BasketObject.transform.position);
		Vector3 newrot_euler = newrot.eulerAngles;
		newrot_euler.z = BasketRot;
		newrot = Quaternion.Euler(newrot_euler);
		BasketObject.transform.localRotation = Quaternion.Slerp(BasketObject.transform.localRotation, newrot, Time.deltaTime * 15);
	}
}
