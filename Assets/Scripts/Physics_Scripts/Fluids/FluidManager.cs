using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Filename : FluidManager
 *  Author:Louis-E
 * Goal : handles the computation of the fluid simulation
 * 
 * Requirements : Create an instance of this script in the Objectfluidmanager
 */
public class FluidManager : MonoBehaviour
{
	List<Particle> particles;
	FluidGrid grid;
	Vector2 gridPosition;
	Vector2 gridDimension;

	float radius = 1.0f;
	int gridSizeX = 56;
	int gridSizeY = 40;

	//Viscosity's linear dependence on the velocity
	float SIGMA = 1.0f;

	//Viscosity's quadratic dependence on the velocity
	float BETA = 1.0f;


	//Stiffness used in DoubleDensityRelaxation
	float k = 25.0f;
	//Near stiffness used in DoubleDensityRelaxation
	float kNear = 7.0f;

	//Rest density
	float p0 = 10.0f;

	public FluidManager()
	{

	}

	//Create the basic particles
	public void InitialiseParticlesSystem(int numberOfParticles, PrefabsHolder ph)
	{
		particles = new List<Particle>();
		gridPosition = new Vector2(-27.75f, -19.25f);
		grid = new FluidGrid(radius, gridSizeX, gridSizeY, gridPosition);

		gridDimension = new Vector2(gridPosition.x + gridSizeX * radius, gridPosition.y + gridSizeY * radius);

		for (int i = 0; i < numberOfParticles; i++)
		{
			for (int j = 0; j < numberOfParticles; j++)
			{
				float r = Random.Range(0.1f, 1.0f);
				particles.Add(new Particle(new Vector3(gridPosition.x + i+ i * r, gridPosition.y + j+j * r, 0.0f), Vector3.zero, particles.Count, radius, ph));
				grid.AddParticle(particles[particles.Count-1]);
			}

		}

	}

	//Add a particle to the simulation
	public void AddParticle(Vector3 pos, PrefabsHolder ph) 
	{
		particles.Add(new Particle(pos, Vector3.zero, particles.Count, radius, ph));
		grid.AddParticle(particles[particles.Count - 1]);
		
	}

	//Simulation of the fluid
	public void FluidPhysicsCalculations(float timeStep, Vector2 g, bool isCursorClick, Vector3 mousePos)
	{
		
		Vector3 gravity = -g * timeStep;

		//Allows cursor to take particles
		if (isCursorClick) 
		{
			
			Vector3 realMousePos = new Vector3(mousePos.x,mousePos.y, 0);
			
			for (int i = 0; i < particles.Count; i++) 
			{

				Vector3 diff = realMousePos - particles[i].GetPosition();
				if(diff.magnitude < 8) 
				{
					particles[i].AddVelocity((25f * timeStep * diff.normalized));
				}
				
			}
		}


		for (int i = 0; i < particles.Count; i++)
		{
			Particle p = particles[i];
			//Add gravity
			particles[i].AddVelocity(gravity);

			//ApplyViscosity
			List<int> neighbors = p.GetNeighbors();
			for (int j = 0; j < neighbors.Count; j++)
			{
				Particle n = particles[neighbors[j]];
				Vector3 vPN = n.GetPosition() - p.GetPosition();
				if (vPN.magnitude > radius || vPN.magnitude < 0.01f) { continue; }
				float velInward = Vector3.Dot((p.GetVelocity() - n.GetVelocity()), vPN);

				if (velInward > 0)
				{
					float length = vPN.magnitude;
					if (length < 0.01f) { length = 0.01f; }
					if (length > radius) { length = radius; }
					velInward /= length;
					if (velInward > 10) { velInward = 10; }
					vPN.Normalize();
					float q = length / radius;
					Vector3 I = 0.5f * timeStep * (1 - q) * (SIGMA * velInward + BETA * velInward * velInward) * vPN;
					p.AddVelocity(-I);
				}
			}

		}

		for (int i = 0; i < particles.Count; i++) 
		{
			Particle p = particles[i];
			//UpdatePositions
			p.SetPrevPosition(p.GetPosition());
			p.AddPosition(p.GetVelocity() * timeStep);
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


		grid.ResetGrid();
		//ResolveCollisions
		for (int i = 0; i < particles.Count; i++)
		{
			Particle p = particles[i];
			Vector3 position = p.GetPosition();
			Vector3 newPos = position;


			if (position.x <= gridPosition.x)
			{
				newPos.x = gridPosition.x;
				p.SetPrevPosition(p.GetPrevPosition() + -Vector3.right * 0.03f);

			}
			else if (position.x >= gridDimension.x - 1)
			{
				newPos.x = gridDimension.x - 1;
				p.SetPrevPosition(p.GetPrevPosition() + Vector3.right * 0.03f);
			}


			if (position.y <= gridPosition.y)
			{
				newPos.y = gridPosition.y;
				p.SetPrevPosition(p.GetPrevPosition() + -Vector3.up * 0.03f);
			}
			else if (position.y >= gridDimension.y - 1)
			{
				newPos.y = gridDimension.y - 1;
				p.SetPrevPosition(p.GetPrevPosition() + Vector3.up * 0.03f);
			}


			p.SetPosition(newPos);
			//Update hashmap
			grid.AddParticle(particles[i]);
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
				Particle neighbor = particles[neighbors[j]];


					Vector3 diff = part.GetPosition() - neighbor.GetPosition();
					float tempN = (diff).magnitude;
					float q = 1.0f - (tempN / radius);
					
					Vector3 vPN = (diff).normalized;
				
					Vector3 D = (0.5f * timeStep * timeStep * (P * q + PNear * q * q)) * vPN;
				
			
					particles[neighbors[j]].AddPosition(D); 
				
					
					delta -= D;
				


			}
			part.AddPosition(delta);

		}






		




		







		//UpdateVelocities
		for (int i = 0; i < particles.Count; i++)
		{
			//UpdateParticles
			Particle p = particles[i];
			Vector3 newVel = (p.GetPosition() - p.GetPrevPosition()) / timeStep;
			//if (newVel.magnitude > 100) 
			//{
			//	newVel = Vector3.zero;
			//}
			p.SetVelocity(newVel);
			
		}





	}



	public void RemoveAllParticles() 
	{
		for (int i = 0; i < particles.Count; i++) 
		{
			particles[i].Delete();
		}
		particles.Clear();
	}

	public float GetParticleSize() 
	{
		return radius;
	}
}
