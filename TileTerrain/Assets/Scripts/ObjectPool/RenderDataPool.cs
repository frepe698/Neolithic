
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RenderDataPool
{
	
	//the variable is declared to be volatile to ensure that
	//assignment to the instance variable completes before the
	//instance variable can be accessed.
	private static volatile RenderDataPool instance;
	
	//look up list of various object pools.
	private Dictionary<String, RenderData> eyecandyPool;
	
	//object for locking
	private static object syncRoot = new System.Object();
	
	/// <summary>
	/// Constructor for the class.
	/// </summary>
	private RenderDataPool()
	{
		//Ensure object pools exists.
		this.eyecandyPool = new Dictionary<String, RenderData>();
	}
	
	/// <summary>
	/// Property for retreiving the singleton.  See msdn documentation.
	/// </summary>
	public static RenderDataPool Instance
	{
		get
		{
			//check to see if it doesnt exist
			if (instance == null)
			{
				//lock access, if it is already locked, wait.
				lock (syncRoot)
				{
					//the instance could have been made between
					//checking and waiting for a lock to release.
					if (instance == null)
					{
						//create a new instance
						instance = new RenderDataPool();
					}
				}
			}
			//return either the new instance or the already built one.
			return instance;
		}
	}
	
	/// <summary>
	/// Create a new object pool of the objects you wish to pool
	/// </summary>
	/// <param name="objToPool">The object you wish to pool.  The name property of the object MUST be unique.</param>
	/// <param name="initialPoolSize">Number of objects you wish to instantiate initially for the pool.</param>
	/// <param name="maxPoolSize">Maximum number of objects allowed to exist in this pool.</param>
	/// <param name="shouldShrink">Should this pool shrink back down to the initial size when it receives a shrink event.</param>
	/// <returns></returns>
	public bool CreateEyecandyData(GameObject objToPool)
	{
		//Check to see if the pool already exists.
		if (RenderDataPool.Instance.eyecandyPool.ContainsKey(objToPool.name))
		{
			//let the caller know it already exists, just use the pool out there.
			return false;
		}
		else
		{
			//create a new pool using the properties
			RenderData nData = new RenderData(objToPool);
			//Add the pool to the dictionary of pools to manage
			//using the object name as the key and the pool as the value.
			RenderDataPool.Instance.eyecandyPool.Add(objToPool.name.ToLower(), nData);
			//We created a new pool!
			return true;
		}
	}
	
	/// <summary>
	/// Get an object from the pool.
	/// </summary>
	/// <param name="objName">String name of the object you wish to have access to.</param>
	/// <returns>A GameObject if one is available, else returns null if all are currently active and max size is reached.</returns>
	public RenderData GetEyecandyData(string objName)
	{
		//Find the right pool and ask it for an object.
		return RenderDataPool.Instance.eyecandyPool[objName.ToLower()];
	}
}