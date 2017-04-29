using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	void Awake() {instance = this;}

	public float BasketDepth = 2.4F;
	public float BasketLength = 0.3F;
	public Rigidbody Basket;
	public Transform SpawnPoint;
	CameraPivot pivoter;
	public float ControlSpeed = 0.3F, ControlAcc = 0.1F, ControlDecay = 0.004F;
	public float ControlVelocity;
	public LevelPools Level_Collects;

	public TextMesh Score;
	public int RipeScore = 0, RottenScore = 0;
	// Use this for initialization
	void Start () {
		pivoter = CameraManager.instance.GetComponent<CameraPivot>();

		Level_Collects.Init(this.transform);
		StartCoroutine(SpawnFruit());

		Basket.transform.LookAt(RotationPoint.position);
		Basket.transform.position = RotationPoint.position + Vector3.back * BasketDepth;
	}
	
	// Update is called once per frame
	void Update () {
		if(ControlVelocity > 0.05F) ControlVelocity -= ControlDecay;
		else if(ControlVelocity < -0.05F) ControlVelocity += ControlDecay;
		else ControlVelocity = 0.0F;

		if(Input.GetKey(KeyCode.A)) ControlVelocity += ControlAcc;
		if(Input.GetKey(KeyCode.D)) ControlVelocity -= ControlAcc;
	

		ControlVelocity = Mathf.Clamp(ControlVelocity, -1.0F, 1.0F);
		ControlBasket();
		TapInput();

		Score.text = "RIPE: " + RipeScore + " ROTTEN: " + RottenScore;
	}

	public float InputRadius;
	public GameObject DebugSphere;
	public void TapInput()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray inputray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit [] hit;

			hit = Physics.SphereCastAll(inputray.origin, InputRadius, inputray.direction);
			for(int i = 0; i < hit.Length; i++)
			{
				if(hit[i].transform.tag == "Collect")
				{
					hit[i].transform.GetComponent<Collectable>().Tap();
				}
			}
		}

	
	}

	public IEnumerator SpawnFruit()
	{
		float waittime = 0.0F;
		Vector2 waitradial = new Vector2(0.4F, 1.1F);
		
		while(true)
		{
			waittime = Random.Range(waitradial.x, waitradial.y);
			Collectable a = CreateCollectable(CollectType.Fruit);
			if(a) a.Spawn();
			if(a) a.transform.position = RandomTreeRadial();
			yield return new WaitForSeconds(waittime);
		}
	}

	public Collectable CreateCollectable(CollectType t)
	{
		GameObject pref = null;
		switch(t)
		{
			case CollectType.Fruit:
			pref = (Level_Collects.Fruit.Get() as GameObject);
			break;
			case CollectType.FruitBad:
			pref = (Level_Collects.FruitBad.Get() as GameObject);
			break;
			case CollectType.Bomb:
			pref = (Level_Collects.Bomb.Get() as GameObject);
			break;
			case CollectType.Coin:
			pref = (Level_Collects.Coin.Get() as GameObject);
			break;
		}

		if(pref != null) return pref.GetComponent<Collectable>();
		return null;
	}

	public Vector3 RandomTreeRadial( float height = 0.7F)
	{
		Vector3 fin = SpawnPoint.position;
		Vector3 vel = new Vector3(Random.Range(-1.0F,1.0F), 0.0F, Random.Range(-1.0F, 1.0F));
		vel.Normalize();
		fin += vel * BasketDepth;
		fin.z += Random.Range(-BasketLength, BasketLength);
		fin.y += Random.Range(-height, height);
		return fin;
	}

	public void FixedUpdate()
	{
		Vector3 targpos = Basket.transform.position;
		targpos += -Basket.transform.right * ControlVelocity;
		targpos.y = RotationPoint.position.y;
		Vector3 actual = (targpos - RotationPoint.position).normalized * BasketDepth + RotationPoint.position;

		Vector3 vel = (actual - Basket.transform.position).normalized;
		vel.y = 0.0F;
		Basket.velocity = vel * BasketSpeed;

	}
	public Transform RotationPoint;
	private float BasketOffset = 3.3F;
	private float BasketSpeed = 2.6F;
	private float BasketRot = 0.0F;
	private float CamAngle = 0.0F;
	private float CamSpeed = 0.0F, CamMaxSpeed = 1.2F, CamDecay = 0.9F;
	Quaternion CamRotation;
	bool camrotate;
	public void ControlBasket()
	{
		Vector3 lookpos = RotationPoint.position;
		lookpos.y = Basket.transform.position.y;
		Basket.transform.LookAt(lookpos);

		//Basket.transform.RotateAround(Vector3.zero, Vector3.up, ControlVelocity * BasketSpeed);
		if(BasketRot > 0.05F) BasketRot -= 0.72F;
		else if(BasketRot < -0.05F) BasketRot += 0.72F;
		else BasketRot = 0.0F;
		BasketRot += ControlVelocity * 0.8F;
		BasketRot = Mathf.Clamp(BasketRot, -25F, 25F);

		//Quaternion newrot = Basket.transform.localRotation * Quaternion.LookRotation(RotationPoint.position);
		//Vector3 newrot_euler = newrot.eulerAngles;
		//newrot_euler.z = BasketRot;
		//newrot = Quaternion.Euler(newrot_euler);
		//Basket.transform.localRotation = Quaternion.Slerp(Basket.transform.localRotation, newrot, Time.deltaTime * 15);

		Vector3 targ = pivoter.transform.position - Basket.transform.position;
		float angle = Vector3.Angle(targ, pivoter.transform.forward);
		if(angle > 30)
		{
			int sign = Vector3.Cross(targ, pivoter.transform.forward).y < 0 ? 1 : -1;
			angle *= sign;
			angle /= 30;

			if(CamSpeed > 0.05F) CamSpeed -= CamDecay;
			else if(CamSpeed < -0.05F) CamSpeed += CamDecay;
			else CamSpeed = 0.0F;

			CamSpeed = Mathf.Clamp(CamSpeed + angle, -CamMaxSpeed, CamMaxSpeed);
			pivoter.transform.rotation = pivoter.transform.rotation * Quaternion.Euler(0,CamSpeed, 0);
			//pivoter.transform.rotation = Quaternion.Slerp(pivoter.transform.rotation, CamRotation, Time.deltaTime * 50);	
			
		}	
		
	}

	public void Collect(Collectable c)
	{
		if(c is Fruit) 
		{
			Fruit f = c as Fruit;
			if(f.Ripeness > 0.75F) RipeScore ++;
			else RottenScore++;
		}
	}

	public void UnCollect(Collectable c)
	{
		if(c is Fruit) 
		{
			Fruit f = c as Fruit;
			if(f.Ripeness > 0.75F) RipeScore --;
			else RottenScore--;
		}
	}
}

public enum CollectType{
	Fruit, FruitBad, Bomb, Coin
}

[System.Serializable]
public class LevelPools
{
	public ObjectPool Fruit, FruitBad, Bomb, Coin;
	public void Init(Transform m)
	{
		if(Fruit) Fruit.Init(m);
		if(FruitBad) FruitBad.Init(m);
		if(Bomb) Bomb.Init(m);
		if(Coin) Coin.Init(m);
	}
}
