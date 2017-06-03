using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	void Awake() {instance = this;}
	public LevelPools Level_Collects;

	public Transform SpawnPoint;
	public float BasketDepth = 2.4F;
	public float BasketLength = 0.3F;

	public TextMesh Score;
	public int RipeScore = 0, RottenScore = 0;
	public float ControlSpeed = 0.3F, ControlAcc = 0.1F, ControlDecay = 0.004F;
	public float ControlVelocity;
	// Use this for initialization
	void Start () {
		

		Level_Collects.Init(this.transform);
		StartCoroutine(SpawnFruit());

	}
	
	// Update is called once per frame
	void Update () {
		if(ControlVelocity > 0.05F) ControlVelocity -= ControlDecay;
		else if(ControlVelocity < -0.05F) ControlVelocity += ControlDecay;
		else ControlVelocity = 0.0F;

	if(Application.isMobilePlatform)
	{
		float acc = -Input.acceleration.x * ControlAcc * 2;
		ControlVelocity += acc;
	}
	else
	{
		if(Input.GetKey(KeyCode.A)) ControlVelocity += ControlAcc;
		if(Input.GetKey(KeyCode.D)) ControlVelocity -= ControlAcc;
	}

		ControlVelocity = Mathf.Clamp(ControlVelocity, -1.0F, 1.0F);
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
