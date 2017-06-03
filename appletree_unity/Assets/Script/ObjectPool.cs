using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PoolType {Spawn, Reference}


public class ObjectPool : ScriptableObject {
	public Object Obj;
	public PoolType Type;
	public Transform ParentTrans;

	public int InitPoolSize, MaxPoolSize = 10;
	private int PoolSize;
	public int Size
	{
		get{return PoolSize;}
	}
	List<Object> Available = new List<Object>();
	List<Object> Active = new List<Object>();

	public void Init (Transform m) {
		if(Obj == null) return;
		if(ParentTrans == null)
		{
			ParentTrans = new GameObject(Obj.name).transform;
			ParentTrans.SetParent(m);
		}
		for(int i = 0; i < InitPoolSize; i++) CreateObject();

		PoolSize = InitPoolSize;
	}
	

	public Object Get()
	{
		if(Type == PoolType.Reference) return Obj;
		else if(Type == PoolType.Spawn)
		{
			return Spawn();
		}
		return null;
	}

	public Object Spawn()
	{
		if(Available.Count == 0)
		{
			if(PoolSize > MaxPoolSize) return null;
			else 
			{
				PoolSize ++;
				CreateObject();
			}
		}
	
		Object o = Available[0];
		Available.RemoveAt(0);
		Active.Add(o);
		if(o is GameObject) (o as GameObject).SetActive(true);
		return o;
	}

	public void Unspawn(Object o)
	{
		if(Active.Contains(o))
		{
			Active.Remove(o);
			Available.Add(o);
			SetToParent(o);
		}
	}

	void CreateObject()
	{
		Object o = GameObject.Instantiate(Obj);
		SetToParent(o);
		Available.Add(o);
		GameObject g = o as GameObject;
		if(g != null)
		{
			ObjectPoolRef r = g.GetComponent<ObjectPoolRef>();
			if(r == null)
			{
				r = g.AddComponent<ObjectPoolRef>();
				r.Pool = this;
			} 
		} 
	}

	void SetToParent(Object o)
	{
		GameObject g = o as GameObject;
		if(g == null) return;
		g.transform.SetParent(ParentTrans);
		g.transform.position = ParentTrans.position;
		g.SetActive(false);
	}

#if UNITY_EDITOR
	[MenuItem("Assets/Filth/Create Pooler")]
	public static void CreateObjectPooler()
	{
		ObjectPool asset = ScriptableObject.CreateInstance<ObjectPool>();
		AssetDatabase.CreateAsset(asset, "Assets/ObjectPool.asset");
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

#endif
}
