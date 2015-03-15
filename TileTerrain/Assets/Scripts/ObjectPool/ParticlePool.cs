/*
 * @Author: David Crook
 *
 * Use the object pools to help reduce object instantiation time and performance
 * with objects that are frequently created and used.
 *
 *
 */
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// The object pool is a list of already instantiated game objects of the same type.
/// </summary>
public class ParticlePool
{
    //the list of objects.
    private List<ParticleSystem> pooledObjects;

    //sample of the actual object to store.
    //used if we need to grow the list.
    private ParticleSystem pooledObj;

    //maximum number of objects to have in the list.
    private int targetPoolSize;


    /// <summary>
    /// Constructor for creating a new Object Pool.
    /// </summary>
    /// <param name="obj">Game Object for this pool</param>
    /// <param name="initialPoolSize">Initial and default size of the pool.</param>
    /// <param name="maxPoolSize">Maximum number of objects this pool can contain.</param>
    /// <param name="shouldShrink">Should this pool shrink back to the initial size.</param>
    public ParticlePool(ParticleSystem particle, int targetPoolSize, bool shouldShrink)
    {
        //instantiate a new list of game objects to store our pooled objects in.
        pooledObjects = new List<ParticleSystem>();

        //create and add an object based on initial size.
        for (int i = 0; i < targetPoolSize; i++)
        {
            //instantiate and create a game object with useless attributes.
            //these should be reset anyways.
            ParticleSystem nParticle = GameObject.Instantiate(particle, Vector3.zero, particle.transform.rotation) as ParticleSystem;

            //make sure the particle doesn't play when created.
            nParticle.playOnAwake = false;

            //add the object too our list.
            pooledObjects.Add(nParticle);

            //Don't destroy on load, so
            //we can manage centrally.
            GameObject.DontDestroyOnLoad(nParticle);
        }

        //store our other variables that are useful.
        this.pooledObj = particle;
        this.targetPoolSize = targetPoolSize;

        
    }

    /// <summary>
    /// Returns an active object from the object pool without resetting any of its values.
    /// You will need to set its values and set it inactive again when you are done with it.
    /// </summary>
    /// <returns>Game Object of requested type if it is available, otherwise null.</returns>
    public ParticleSystem GetObject()
    {
        //iterate through all pooled objects.
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //look for the first one that is inactive.
            if (pooledObjects[i].isPlaying == false)
            {
                //set the object to active.
                //pooledObjects[i].Play();
                //return the object we found.
                return pooledObjects[i];
            }
        }
        //if we make it this far, we obviously didn't find an inactive object.
        //so we need to see if we can grow beyond our current count.
        
        //Instantiate a new object.
        ParticleSystem nParticle = GameObject.Instantiate(pooledObj, Vector3.zero, pooledObj.transform.rotation) as ParticleSystem;
        //set it to active since we are about to use it.

        //nParticle.Play();

        //add it to the pool of objects
        pooledObjects.Add(nParticle);
        //return the object to the requestor.
        return nParticle;
        
    }

    /// <summary>
    /// Iterate through the pool and releases as many objects as
    /// possible until the pool size is back to the initial default size.
    /// </summary>
    public void Shrink()
    {
        //how many objects are we trying to remove here?
        int objectsToRemoveCount = pooledObjects.Count - targetPoolSize;
        //Are there any objects we need to remove?
        if (objectsToRemoveCount <= 0)
        {
            //cool lets get out of here.
            return;
        }

        //iterate through our list and remove some objects
        //we do reverse iteration so as we remove objects from
        //the list the shifting of objects does not affect our index
        //Also notice the offset of 1 to account for zero indexing
        //and i >= 0 to ensure we reach the first object in the list.
        for (int i = pooledObjects.Count - 1; i >= 0; i--)
        {
            //Is this object active?
            if (!pooledObjects[i].isPlaying)
            {
                //Guess not, lets grab it.
                ParticleSystem particle = pooledObjects[i];
                //and kill it from the list.
                pooledObjects.Remove(particle);
            }
        }
    }

}