using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFluidManager : MonoBehaviour
{
	//Change the variable numberOfStepsPerSecond to change the timerate calculations
	private int numberOfStepsPerSecond = 60;
	private float stepLength;
	private float numberOfUpdateCounter = 0;

	public Vector2 Gravity;
	public float Viscosity;
	FluidManager fluidManager;

	public PrefabsHolder prefabHolder;

	public void Start()
	{

		fluidManager = new FluidManager();
		fluidManager.InitialiseParticlesSystem(15, prefabHolder);



		ChangeNumberOfStepsPerSecond(numberOfStepsPerSecond);
		numberOfUpdateCounter = 0;


	}




	//Update the physics objects on a fixed time rate
	public void Update()
	{
		numberOfUpdateCounter += UniversalVariable.GetTime() * Time.deltaTime / stepLength;

		while (numberOfUpdateCounter > 1)
		{

			PhysicCalculations();

			

			numberOfUpdateCounter--;
		}

	}



	//Simulate all the physics behaviours
	private void PhysicCalculations()
	{

		fluidManager.FluidPhysicsCalculations(stepLength, Gravity);

	}


	

	//Change the number of steps per second and update the Step length in consequence
	public void ChangeNumberOfStepsPerSecond(int newNumberOfStepsPerSecond)
	{
		if (newNumberOfStepsPerSecond <= 0) { return; }
		else
		{
			numberOfStepsPerSecond = newNumberOfStepsPerSecond;
			stepLength = 1.0f / numberOfStepsPerSecond;
		}
	}

	public void RemoveAllParticles() 
	{
		fluidManager.RemoveAllParticles();
	}
	public void AddParticle(Vector3 pos, PrefabsHolder ph) 
	{
		fluidManager.AddParticle(pos, ph);
	}
	public float GetParticleSize() 
	{
		return fluidManager.GetParticleSize();
	}

}
