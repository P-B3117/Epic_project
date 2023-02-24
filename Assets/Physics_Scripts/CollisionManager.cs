using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CollisionManager
{
    public CollisionManager() { }

    public List<object> CollisionHasHappened(Vector3 objectVelocity, Vector3 otherObjectVelocity, Vector3 normal, float objectMass, float otherObjectMass, float coefficientOfRestitution, float objectAngularVelocity,
        float otherObjectAngularVelocity, Vector3 rVectorObject, Vector3 rVectorOtherObject, float objectInertia, float otherObjectInertia)
    {
        normal = normal.normalized; // Calculate the normal 
        
        Vector3 relativeVelocity = otherObjectVelocity - objectVelocity; // Calculate the relative velocity

        float speedAlongNormal = Vector3.Dot(relativeVelocity, normal); // Calculate the speed of the object along the normal vector


        if (speedAlongNormal > 0)
        {
            // The objects are moving away from each other, so there's no collision
            // for example, time tick was big and object is already moving away on second time step. we will not recalculate 
            return new List<object> { objectVelocity, otherObjectVelocity, objectAngularVelocity, otherObjectAngularVelocity }; ;
        }

        // Calculate the new velocity vectors after the collision
        
        float restitutionCollisionCoefficient = coefficientOfRestitution; // The coefficient of restitution, which determines how bouncy the collision is

        /*
         * 'j' est l'impulsion, un concept physique qui se rapporte au changement de vitesse d'un objet
         * L'impulsion est d�finie comme le produit de la force appliqu�e � un objet et de la dur�e de cette force. 
         * Dans le contexte de la r�action � une collision, l'impulsion est utilis�e pour calculer le changement de 
         * vitesse des objets impliqu�s dans la collision. L'impulsion est proportionnelle � la vitesse des objets 
         * le long du vecteur normal et au coefficient de restitution, qui d�termine le degr� de rebondissement de la collision.
         * En appliquant l'impulsion aux vecteurs de vitesse des objets, nous pouvons calculer leurs nouvelles 
         * vitesses apr�s la collision, ce qui d�termine comment ils continueront � se d�placer dans l'espace.
         */

        rVectorObject = new Vector3(-rVectorObject.y, rVectorObject.x, rVectorObject.z);
        rVectorOtherObject = new Vector3(-rVectorOtherObject.y, rVectorOtherObject.x, rVectorOtherObject.z);

        float j = -(1 + restitutionCollisionCoefficient) * speedAlongNormal / ((1 / objectMass + 1 / otherObjectMass) + (Mathf.Pow(Vector3.Dot(rVectorObject, normal), 2) / objectInertia)
            + (Mathf.Pow(Vector3.Dot(rVectorOtherObject, normal), 2) / otherObjectInertia));

        Vector3 impulse = j * normal;

        Vector3 newVelocity = objectVelocity - (1 / objectMass) * impulse;
        
        Vector3 newOtherVelocity = otherObjectVelocity + (1 / otherObjectMass) * impulse;

        // maintenant: angular rotation
        float newAngularVelocity = objectAngularVelocity + (Vector3.Dot(rVectorObject, normal * j) / objectInertia);
        float newOtherAngularVelocity = otherObjectAngularVelocity + (Vector3.Dot(rVectorOtherObject, j * normal) / otherObjectInertia);


        if (Mathf.Abs(newAngularVelocity) < 0.02) newAngularVelocity = 0;
        if (Mathf.Abs(newOtherAngularVelocity) < 0.02) newOtherAngularVelocity = 0;
        Debug.Log(newVelocity + " " + objectVelocity);
        return new List<object> { newVelocity, newOtherVelocity, newAngularVelocity, newOtherAngularVelocity};
    }

}
