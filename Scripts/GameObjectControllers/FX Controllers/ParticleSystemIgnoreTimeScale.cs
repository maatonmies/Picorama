using UnityEngine;
using System.Collections;

public class ParticleSystemIgnoreTimeScale : MonoBehaviour
{
	private double lastTime;
	private ParticleSystem particle;
	
	private void Awake ()
	{
		particle = GetComponent<ParticleSystem> ();
	}
	
	void Start ()
	{
		lastTime = Time.realtimeSinceStartup;
	}
	
	void Update ()
	{
		float deltaTime = Time.realtimeSinceStartup - (float)lastTime;
		particle.Simulate (deltaTime, true, false);
		lastTime = Time.realtimeSinceStartup;
	}

}
