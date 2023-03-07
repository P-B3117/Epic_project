using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionManager
{
    public CollisionManager() { }

    public List<object> CollisionHasHappened(Vector3 objectVelocity, Vector3 otherObjectVelocity, Vector3 normal, float objectMass, float otherObjectMass, float coefficientOfRestitution, float objectAngularVelocity,
        float otherObjectAngularVelocity, Vector3 rVectorObject, Vector3 rVectorOtherObject, float objectInertia, float otherObjectInertia)
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

        Vector3 newVelocity = objectVelocity + (1f / objectMass) * impulse;

        Vector3 newOtherVelocity = otherObjectVelocity - (1f / otherObjectMass) * impulse;

        // maintenant: angular rotation
        float newAngularVelocity = objectAngularVelocity + (Vector3.Dot(perpVectorObject, normal * -j) / objectInertia);
        float newOtherAngularVelocity = otherObjectAngularVelocity + (Vector3.Dot(perpVectorOtherObject, normal * j) / otherObjectInertia);

        return new List<object> { newVelocity, newOtherVelocity, newAngularVelocity, newOtherAngularVelocity};
    }

}
