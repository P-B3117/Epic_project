using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Filename : CollisinManager
 * Author: Justin, Clovis
 * Goal : Class that holds the key function of resolving collisions
 * 
 * Requirements : No instance required to use this class, just reference it to access the function
 */
public class CollisionManager
{
    public CollisionManager() { }


    //Solve collisions
    public List<object> CollisionHasHappened(Vector3 objectVelocity, Vector3 otherObjectVelocity, Vector3 normal, float objectMass, float otherObjectMass, float coefficientOfRestitution, float objectAngularVelocity,
        float otherObjectAngularVelocity, Vector3 rVectorObject, Vector3 rVectorOtherObject, float objectInertia, float otherObjectInertia, float friction1Static, float friction1Dynamic, float friction2Static, float friction2Dynamic)
    {

        normal = normal.normalized; // Calculate the normal vector

        Vector3 perpVectorOtherObject = new Vector3(-rVectorOtherObject.y, rVectorOtherObject.x, 0);
        Vector3 perpVectorObject = new Vector3(-rVectorObject.y, rVectorObject.x, 0);

        Vector3 velocityObject = objectVelocity - objectAngularVelocity * perpVectorObject;
        Vector3 velocityOtherObject = otherObjectVelocity - otherObjectAngularVelocity * perpVectorOtherObject;

        Vector3 relativeVelocity = velocityObject - velocityOtherObject; // Calculate the relative velocity

        float speedAlongNormal = Vector3.Dot(relativeVelocity, normal); // Calculate the speed of the object along the normal vector

        if (speedAlongNormal > 0)
        {
            // The objects are moving away from each other, so there's no collision
            // for example, time tick was big and object is already moving away on second time step. we will not recalculate it
            return new List<object> { objectVelocity, otherObjectVelocity, objectAngularVelocity, otherObjectAngularVelocity }; ;
        }

        float momentOfInertiaObjectImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpVectorObject, normal), 2) / objectInertia;
        float momentOfInertiaOtherObjectImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpVectorOtherObject, normal), 2) / otherObjectInertia;

       

        float massImpulseInhibitor = (1f / objectMass + 1f / otherObjectMass);

        // Calculate the new velocity vectors after the collision

        /*
         * 'j' est l'impulsion, un concept physique qui se rVectorObjectporte au changement de vitesse d'un objet
         * L'impulsion est définie comme le produit de la force VectorObjectpliquée à un objet et de la durée de cette force. 
         * Dans le contexte de la réaction à une collision, l'impulsion est utilisée pour calculer le changement de 
         * vitesse des objets impliqués dans la collision. L'impulsion est proportionnelle à la vitesse des objets 
         * le long du vecteur normal et au coefficient de restitution, qui détermine le degré de rebondissement de la collision.
         * En VectorObjectpliquant l'impulsion aux vecteurs de vitesse des objets, nous pouvons calculer leurs nouvelles 
         * vitesses VectorObjectrès la collision, ce qui détermine comment ils continueront à se déplacer dans l'espace.
         */

        float j = -(1 + coefficientOfRestitution) * speedAlongNormal;
        j /= (momentOfInertiaObjectImpulseInhibitor + momentOfInertiaOtherObjectImpulseInhibitor + massImpulseInhibitor);

        Vector3 impulse = j * normal;

     





        //Friction
        Vector3 tangent = relativeVelocity - (Vector3.Dot(relativeVelocity, normal) * normal);
        tangent = tangent.normalized;


        float momentOfInertiaObjectImpulseInhibitorFRICTION = Mathf.Pow(Vector3.Dot(perpVectorObject, tangent), 2) / objectInertia;
        float momentOfInertiaOtherObjectImpulseInhibitorFRICTION = Mathf.Pow(Vector3.Dot(perpVectorOtherObject, tangent), 2) / otherObjectInertia;
        float speedAlongTangent = Vector3.Dot(relativeVelocity, tangent);
        float jt = -(0.2f)*Vector3.Dot(relativeVelocity, tangent);
        jt = jt / (massImpulseInhibitor + momentOfInertiaObjectImpulseInhibitorFRICTION +momentOfInertiaOtherObjectImpulseInhibitorFRICTION);

        float mu = Mathf.Sqrt(friction1Static* friction1Static + friction2Static* friction2Static);

        Vector3 frictionImpulse;
        if (Mathf.Abs(jt) <= j * mu)
        {
            frictionImpulse = jt * tangent * mu;
            
        }
        else
        {
            float dynamicFriction = Mathf.Sqrt(friction1Dynamic * friction1Dynamic + friction2Dynamic * friction2Dynamic);
            frictionImpulse = -j * tangent * dynamicFriction;
            
        }


        // maintenant: position update
        Vector3 newVelocity = objectVelocity + (1f / objectMass) * impulse;
        Vector3 newOtherVelocity = otherObjectVelocity - (1f / otherObjectMass) * impulse;
        // maintenant: angular rotation
        float newAngularVelocity = objectAngularVelocity + (Vector3.Dot(perpVectorObject, normal * -j) / objectInertia);
        float newOtherAngularVelocity = otherObjectAngularVelocity + (Vector3.Dot(perpVectorOtherObject, normal * j) / otherObjectInertia);


        //ApplyFriction
        newVelocity += (1f / objectMass) * frictionImpulse;
        newOtherVelocity -= (1f / otherObjectMass) * frictionImpulse;

		newAngularVelocity += (Vector3.Dot(perpVectorObject, -frictionImpulse) / objectInertia);
		newOtherAngularVelocity += (Vector3.Dot(perpVectorOtherObject, frictionImpulse) / otherObjectInertia);

		return new List<object> { newVelocity, newOtherVelocity, newAngularVelocity, newOtherAngularVelocity };
    }

}