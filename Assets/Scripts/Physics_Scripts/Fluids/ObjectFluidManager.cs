using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFluidManager : MonoBehaviour
{
	//Change the variable numberOfStepsPerSecond to change the timerate calculations
	private int numberOfStepsPerSecond = 100;
	private float stepLength;
	private float numberOfUpdateCounter = 0;

	public Vector2 Gravity;
	FluidManager fluidManager;

	public void Start()
	{

		fluidManager = new FluidManager();
		fluidManager.InitialiseParticlesSystem(10);



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

}
