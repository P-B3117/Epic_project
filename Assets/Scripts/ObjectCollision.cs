using UnityEngine;
using System.Collections;

public class ObjectCollision : MonoBehaviour
{

    private void OnCollisionEnter(Collision other) {
        Debug.Log("We hit something!");
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("We hit something!");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("We hit something!");
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("We hit something!");
    }

}
