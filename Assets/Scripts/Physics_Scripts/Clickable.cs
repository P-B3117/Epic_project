using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author : Justin
 */
public class Clickable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private bool draggable = false;
    public void Draggable(bool isDraggable)
    {
        draggable= isDraggable;
    }

    private bool isDragging = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && draggable)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            // Move the object with the mouse
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);

        }

    }
}
