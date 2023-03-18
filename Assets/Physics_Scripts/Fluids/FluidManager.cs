using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidManager
{

	List<Particle> particles;
	FluidGrid grid;
	Vector2 gridPosition;
	Vector2 gridDimension;

	float radius = 0.5f;
	int gridSize = 25;

	//Viscosity's linear dependence on the velocity
	float SIGMA = 80f;

	//Viscosity's quadratic dependence on the velocity
	float BETA = 69f;


	//Stiffness used in DoubleDensityRelaxation
	float k = 25.0f;
	//Near stiffness used in DoubleDensityRelaxation
	float kNear =7.0f;

	//Rest density
	float p0 =10.0f;
	
	public FluidManager() 
	{
		
	}
	public void InitialiseParticlesSystem(int numberOfParticles) 
	{
		particles = new List<Particle>();
		gridPosition = new Vector2(-7.5f, -7.5f);
		grid = new FluidGrid(radius, gridSize, gridPosition);
		
		gridDimension = new Vector2(gridPosition.x + gridSize * radius, gridPosition.y + gridSize * radius);

		for (int i = 0; i < numberOfParticles; i++)
		{
			for(int j = 0; j < numberOfParticles; j++) 
			{
				float r = Random.Range(0.0f, 1.0f);
				particles.Add(new Particle(new Vector3(gridPosition.x + i*r, gridPosition.y  + j*r, 0.0f), Vector3.zero, i*numberOfParticles + j, radius));
				grid.AddParticle(particles[i]);
			}
			
		}

	}
	public void FluidPhysicsCalculations(float timeStep, Vector2 g)
	{
		//Vector3 gravity = new Vector3(0, -UniversalVariable.GetGravity() * timeStep);
		Vector3 gravity = -g * timeStep;
		grid.ResetGrid();
		for (int i = 0; i < particles.Count; i++) 
		{
			Particle p = particles[i];

			//ApplyForces
			p.AddVelocity(gravity);


			//UpdatePositions
			p.SetPrevPosition(p.GetPosition());
			p.AddPosition(p.GetVelocity() * timeStep);


			//Update hashmap
			grid.AddParticle(particles[i]);



			//ApplyViscosity
			List<int> neighbors = p.GetNeighbors();
			for (int j = 0; j < neighbors.Count; j++)
			{
				Particle n = particles[neighbors[j]];
				Vector3 vPN = n.GetPosition() - p.GetPosition();
				float velInward = Vector3.Dot((p.GetVelocity() - n.GetVelocity()), vPN);
				if (velInward > 0)
				{
					float length = vPN.magnitude;
					velInward /= length;
					float q = length / radius;
					Vector3 I = 0.5f * timeStep * (1 - q) * (SIGMA * velInward + BETA * velInward * velInward) * vPN;
					p.AddVelocity(-I);
				}
			}


		}


		//Double Density Relaxation
		for (int i = 0; i < particles.Count; i++)
		{
			Particle part = particles[i];
			float p = 0;
			float pnear = 0;
			List<int> neighbors = part.GetNeighbors();
			for (int j = 0; j < neighbors.Count; j++)
			{

				float tempN = (part.GetPosition() - particles[neighbors[j]].GetPosition()).magnitude;
				float q = 1.0f - (tempN / radius);

				p += q * q;
				pnear += q * q * q;

			}

			float P = k * (p - p0);
			float PNear = kNear * pnear;
			Vector3 delta = Vector3.zero;
			for (int j = 0; j < neighbors.Count; j++)
			{
				float tempN = (part.GetPosition() - particles[neighbors[j]].GetPosition()).magnitude;
				float q = 1.0f - (tempN / radius);
				Vector3 vPN = (part.GetPosition() - particles[neighbors[j]].GetPosition()).normalized;
				Vector3 D = (0.5f * timeStep * timeStep * (P * q + PNear * q * q)) * vPN;
				particles[neighbors[j]].AddPosition(D);
				delta -= D;

			}
			part.AddPosition(delta);

		}

		





		//ResolveCollisions
		for (int i = 0; i < particles.Count; i++)
		{
			Particle p = particles[i];
			Vector3 position = p.GetPosition();
			Vector3 newPos = position;

			
			if (position.x <= gridPosition.x)
			{
				newPos.x = gridPosition.x;
				
			}
			else if (position.x >= gridDimension.x-1) 
			{
				newPos.x =gridDimension.x-1;
				
			}


			if (position.y <= gridPosition.y)
			{
				newPos.y = gridPosition.y;
				
			}
			else if (position.y >= gridDimension.y-1)
			{
				newPos.y =gridDimension.y-1;
				
			}

			
			p.SetPosition(newPos);
		}





		//Update neighbors
		for (int i = 0; i < particles.Count; i++)
		{
			Particle p = particles[i];
			p.RefreshNeighbors();
			List<int> possible = grid.PossibleNeighbors(p);
			for (int j = 0; j < possible.Count; j++)
			{

				if ((p.GetPosition() - particles[possible[j]].GetPosition()).magnitude < radius)
				{
					p.AddNeighbor(possible[j]);


				}
			}

		}







		//UpdateVelocities
		for (int i = 0; i < particles.Count; i++)
		{
			//UpdateParticles
			Particle p = particles[i];
			p.SetVelocity((p.GetPosition() - p.GetPrevPosition()) / timeStep);
		}





	}


	

	
}
