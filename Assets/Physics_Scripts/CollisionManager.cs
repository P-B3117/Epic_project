using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CollisionManager
{
    public CollisionManager() { }

    public List<Vector3> CollisionHasHappened(Vector3 objectVelocity, Vector3 otherObjectVelocity, Vector3 collisionPointObject, Vector3 otherObjectPosition, int objectMass, int otherObjectMass, int coefficientOfRestitution)
    {
        Vector3 normal = (collisionPointObject - otherObjectPosition).normalized; // Calculate the normal vector
        
        Vector3 relativeVelocity = objectVelocity - otherObjectVelocity; // Calculate the relative velocity

        float speedAlongNormal = Vector3.Dot(relativeVelocity, normal); // Calculate the speed of the object along the normal vector

        if (speedAlongNormal > 0)
        {
            // The objects are moving away from each other, so there's no collision
            // for example, time tick was big and object is already moving away on second time step. we will not recalculate it
            return new List<Vector3> { objectVelocity, otherObjectVelocity }; ;
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
        float j = -(1 + restitutionCollisionCoefficient) * speedAlongNormal;

        Vector3 impulse = j * normal;

        Vector3 newVelocity = objectVelocity + (1 / objectMass) * impulse;

        Vector3 newOtherVelocity = otherObjectVelocity - (1 / otherObjectMass) * impulse;

        return new List<Vector3> { newVelocity, newOtherVelocity};
    }
    
    

}
