using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Filename : UISliderScript
 * Author(s): Louis, Charles
 * Goal : Contains all the functionalities of the SLIDER in the Basic Physic Simulation
 * 
 * Requirements : Attach this script to the UIManager
 */
public class UISliderScript : MonoBehaviour
{
    //Universal references
    public PhysicsManager physicsManager;
    public UiGameManager gameManager;
    public UIInspectorScript inspectorScript;
    public PrefabsHolder prefabHolder;
    public GameObject GamePanel;

    //Slider functions variables
    private int MOUSESTATE;
    List<GameObject> meshCreatorPoints;
    List<GameObject> JointCreatorPoints;
    List<GameObject> JointReference;
    List<DistanceJoints> physicsJoints;
    [Header("Reference for the slider")]
    public LineRenderer meshCreatorLineRenderer;
    public LineRenderer JointCreatorLineRenderer;
    GameObject currentShadowObject;
    GameObject jo;

    //sound variables
    public AudioSource SolidDropSound;
    public AudioSource SoftDropSound;
    public AudioSource JointSound;

    //Other variables
    private int counter = 0;
    


    // Start is called before the first frame update
    void Start()
    {
        //Initialize the slider function variables
        MOUSESTATE = -1;
        meshCreatorPoints = new List<GameObject>();
        JointCreatorPoints = new List<GameObject>();
        JointReference = new List<GameObject>();
        physicsJoints = new List<DistanceJoints>();
        physicsJoints = physicsManager.Joints();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            ResetMouseState();
        }


        //!!!!!Functionality of the slider!!!!!
        //Mouse functionality for basic objects
        //Move the shadow object of the basic physics object selected with the mouse
        if (MOUSESTATE >= 0 && MOUSESTATE <= 4)
        {
            BasicObjectSliderUpdate();
        }
        //Mouse functinality for the Mesh creator
        //Allows player to add points on the game scene to create custom mesh
        else if (MOUSESTATE == 5)
        {
            MeshCreatorSliderUpdate();
        }
        //Mouse functionality of the Joint creator
        else if (MOUSESTATE == 6)
        {
            JointCreatorSliderUpdate();
        }
        //Mouse functionality for the softbody
        //Allows user to add softbody 1 to the scene
        else if (MOUSESTATE == 7)
        {
            SoftBody1SliderUpdate();
        }
        //Mouse functionality for the softbody
        //Allows user to add softbody 2 to the scene
        else if (MOUSESTATE == 8)
        {
            SoftBody2SliderUpdate();
        }

        else 
        {
            //When there is no mousestate, and the user click
            if (Input.GetMouseButtonDown(0))
            {
                //Event that 
                NoMouseStateClick();
            }
            //When there is no mouseState, and the user releases the click
            else if(Input.GetMouseButtonUp(0))
            {
                physicsManager.ResetGrabJoint();
            }
        }



    }


    //Function of the MOUSESTATE 1 to 4, allows for the addition of basic physics objects
    private void BasicObjectSliderUpdate() 
    {
        //Get the position of the mouse on the screen
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentShadowObject.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0.0f);
        //The worldPoint boundaries are : (-28.25, 20)      - (28.25, 20)
        //                                (-28.25, -20)   - (28.25, -20)

        //Check boundaries
        MeshColliderScript mc = currentShadowObject.GetComponent<MeshColliderScript>();
        mc.UpdateColliderOrientation();
        Rect AABB = mc.GetBoundariesAABB();
        if (AABB.position.x > -28.25f && AABB.position.x + AABB.width < 28.25 &&
            AABB.position.y > -20 && AABB.position.y + AABB.height < 20)
        {
            //If the mouse is in bound, change the color to green and if the mouse is clicked, initialize the new object
            currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
            if (Input.GetMouseButtonDown(0))
            {
                GameObject newGO = Instantiate(currentShadowObject);
                physicsManager.AddPhysicObject(newGO);
                SolidDropSound.Play();
            }
        }
        //If the mouse isn't in bound, change the material color to red
        else
        {
            currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

        }
    }

    //Function of the MOUSESTATE 5, allows for the user to create custom convex polygons by placing points
    private void MeshCreatorSliderUpdate() 
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentShadowObject.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0.0f);

        //move the last point of line renderer based on the mouse
        if (meshCreatorPoints.Count > 0)
        {
            meshCreatorLineRenderer.SetPosition(meshCreatorPoints.Count, currentShadowObject.transform.position);
        }

        float radius = 0.5f;
        Rect AABB = new Rect(new Vector2(worldPoint.x - 0.5f, worldPoint.y - 0.5f), new Vector2(radius * 2, radius * 2));


        //Look if angle is good (smaller than 180)
        float angle = 0;

        if (meshCreatorPoints.Count >= 3)
        {
            Vector3 p0 = meshCreatorPoints[meshCreatorPoints.Count - 3].transform.position;
            Vector3 p1 = meshCreatorPoints[meshCreatorPoints.Count - 2].transform.position;
            Vector3 p2 = meshCreatorPoints[meshCreatorPoints.Count - 1].transform.position;
            Vector3 p3 = currentShadowObject.transform.position;

            Vector3 v1 = p1 - p0;
            Vector3 v2 = p2 - p1;
            Vector3 v3 = p3 - p2;

            Vector3 r1 = Vector3.Cross(v1, v2).normalized;
            Vector3 r2 = Vector3.Cross(v2, v3).normalized;

            Vector3 pBegin = meshCreatorPoints[0].transform.position;
            Vector3 pBegin2 = meshCreatorPoints[1].transform.position;

            Vector3 v4 = pBegin - p3;
            Vector3 v5 = pBegin2 - pBegin;

            Vector3 r3 = Vector3.Cross(v3, v4).normalized;
            Vector3 r4 = Vector3.Cross(v4, v5).normalized;

            if (!(r1 == r2 && r2 == r3 && r3 == r4)) { angle = 360; }


        }

        bool noLineIntersect = true;
        //Look if line intersect 
        if (meshCreatorPoints.Count >= 3)
        {
            Vector3 m1 = meshCreatorPoints[meshCreatorPoints.Count - 1].transform.position;
            Vector3 m2 = currentShadowObject.transform.position;


            for (int i = 0; i < meshCreatorPoints.Count - 2; i++)
            {
                Vector3 m3 = meshCreatorPoints[i].transform.position;
                Vector3 m4 = meshCreatorPoints[i + 1].transform.position;

                if (HelperFunctionClass.LineIntersect(m1, m2, m3, m4))
                {

                    noLineIntersect = false;
                    break;
                }


            }

        }

        //Check if there is colinearity in the wrong direction with the other segment
        if (meshCreatorPoints.Count >= 2)
        {
            Vector3 m1 = meshCreatorPoints[meshCreatorPoints.Count - 1].transform.position;
            Vector3 m2 = currentShadowObject.transform.position;
            Vector3 v1 = m2 - m1;
            Vector3 m5 = meshCreatorPoints[meshCreatorPoints.Count - 2].transform.position;
            Vector3 m6 = meshCreatorPoints[meshCreatorPoints.Count - 1].transform.position;
            Vector3 v2 = m6 - m5;
            if (Vector3.Dot(v1.normalized, v2.normalized) == -1)
            {
                noLineIntersect = false;
            }
        }

        //Look if a point isn't already there
        bool noPointAlreadyThere = true;
        if (meshCreatorPoints.Count > 1)
        {
            for (int i = 1; i < meshCreatorPoints.Count; i++)
            {
                if (meshCreatorPoints[i].transform.position == currentShadowObject.transform.position)
                {
                    noPointAlreadyThere = false;
                    break;
                }
            }
        }
        else if (meshCreatorPoints.Count > 0)
        {
            if (meshCreatorPoints[0].transform.position == currentShadowObject.transform.position)
            {
                noPointAlreadyThere = false;
            }
        }


        ///Mesh Creators Conditions
        bool isPoint = false;
        Vector3 initialPoint = Vector3.zero;
        if (meshCreatorPoints.Count > 0) { isPoint = true; initialPoint = meshCreatorPoints[0].transform.position; }

        //If the current point is close to the initial point
        if (meshCreatorPoints.Count >= 3 && isPoint && (currentShadowObject.transform.position - initialPoint).magnitude < 2)
        {
            currentShadowObject.transform.position = initialPoint;
            currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
            meshCreatorLineRenderer.SetColors(Color.cyan, Color.cyan);
            meshCreatorLineRenderer.SetPosition(meshCreatorPoints.Count, initialPoint);

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 moy = Vector3.zero;
                for (int i = 0; i < meshCreatorPoints.Count; i++)
                {
                    moy += meshCreatorPoints[i].transform.position;
                }
                moy /= meshCreatorPoints.Count;
                List<Vector3> modelPoints = new List<Vector3>();


                Vector3 first = meshCreatorPoints[0].transform.position - moy;
                Vector3 second = meshCreatorPoints[1].transform.position - moy;
                Vector3 dir = Vector3.Cross(first, second).normalized;
                if (dir == Vector3.back)
                {
                    for (int i = 0; i < meshCreatorPoints.Count; i++)
                    {
                        modelPoints.Add(meshCreatorPoints[i].transform.position - moy);
                    }
                }
                else
                {
                    for (int i = meshCreatorPoints.Count - 1; i >= 0; i--)
                    {
                        modelPoints.Add(meshCreatorPoints[i].transform.position - moy);
                    }
                }



                GameObject empty = new GameObject();
                empty.transform.position = moy;
                empty.AddComponent<BasicPhysicObject>();
                empty.AddComponent<MeshColliderScript>();
                MeshColliderScript mc = empty.GetComponent<MeshColliderScript>();
                mc.Initialize(modelPoints, prefabHolder);
                mc.SetUpMesh();
                mc.UpdateColliderOrientation();
                GameObject newGO = Instantiate(empty);
                Destroy(empty);
                physicsManager.AddPhysicObject(newGO);
                SolidDropSound.Play();
                ResetMouseState();
            }
        }
        //If the current point isn't close the initial point
        else
        {
            //If the current segment is valid, instantiate it
            if (AABB.position.x > -28.25f && AABB.position.x + AABB.width < 28.25 &&
                AABB.position.y > -20 && AABB.position.y + AABB.height < 20 &&
                angle < 180 && noLineIntersect && noPointAlreadyThere)
            {
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                meshCreatorLineRenderer.SetColors(Color.magenta, Color.magenta);
                if (Input.GetMouseButtonDown(0))
                {
                    //Instantiate the mesh creator point
                    GameObject newGO = Instantiate(currentShadowObject);
                    meshCreatorPoints.Add(newGO);


                    //Update the line renderer
                    meshCreatorLineRenderer.positionCount++;
                    Vector3[] points = new Vector3[meshCreatorPoints.Count + 1];
                    for (int i = 0; i < meshCreatorPoints.Count; i++)
                    {
                        points[i] = meshCreatorPoints[i].transform.position;
                    }
                    points[meshCreatorPoints.Count] = currentShadowObject.transform.position;
                    meshCreatorLineRenderer.SetPositions(points);

                }
            }
            //If the current segment is invalid
            else
            {
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                meshCreatorLineRenderer.SetColors(Color.red, Color.red);
            }
        }
    }

    //Function of the MOUSESTATE 6, allows for the user to create custom joints between objects
    private void JointCreatorSliderUpdate() 
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint = new Vector3(worldPoint.x, worldPoint.y, 0.0f);
        currentShadowObject.transform.position = worldPoint;
        //update the line thing 
        if (JointCreatorPoints.Count == 1)
        {
            JointCreatorPoints[0].transform.position = JointReference[0].transform.position;
            Vector3[] position = new Vector3[] { JointCreatorPoints[0].transform.position, currentShadowObject.transform.position };
            position[0].z = -8;
            position[1].z = -8;
            Color col = Color.cyan;
            JointCreatorLineRenderer.SetColors(col, col);
            JointCreatorLineRenderer.SetPositions(position);
        }


        if (counter == 2)
        {
            //create joint here 
            jo = new GameObject();
            jo.AddComponent<DistanceJoints>();
            DistanceJoints joint = jo.GetComponent<DistanceJoints>();

            joint.bo1 = JointReference[0];
            joint.bo2 = JointReference[1];
            joint.frequency = 1.0f;
            joint.dampingRatio = 0.5f;
            joint.length = (JointReference[1].transform.position - JointReference[0].transform.position).magnitude;
            physicsManager.AddDistanceJoints(jo);

            JointReference.Remove(JointReference[1]);
            JointReference.Remove(JointReference[0]);
            JointCreatorLineRenderer.SetPosition(0, Vector3.zero);
            JointCreatorLineRenderer.SetPosition(1, Vector3.zero);
            inspectorScript.JointInspectorInitialize(physicsManager.GetAllNonSoftBodyJoints());
            counter = 0;
            JointSound.Play();
            ResetMouseState();
        }
        else if (Input.GetMouseButtonDown(0) && JointCreatorPoints.Count < 2)
        {

            //Check boundaries when click
            if (worldPoint.x > -28.25f && worldPoint.x < 28.25 && worldPoint.y > -20 && worldPoint.y < 20)
            {

                //Check if mouse click was in a specific object
                //return the index of the object if there is one
                //return -1 of there is no object
                inspectorScript.selectedIndex = (physicsManager.FindClickIndex(worldPoint));

                BasicPhysicObject bo;
                inspectorScript.SELECTEDOBJECT = inspectorScript.selectedIndex;
                if (inspectorScript.selectedIndex == -1)
                {
                    inspectorScript.SetOffInspectorContent();
                    if (gameManager.jmState)gameManager.JointManager();
                }
                
                bo = physicsManager.SelectSpecificObject(inspectorScript.selectedIndex, prefabHolder, inspectorScript.SELECTEDOBJECTGAMEOBJECT);

                if (bo != null)
                {
                    bool cond = true;
                    if (counter == 1 && JointReference[0] == bo.gameObject) cond = false;

                    if (cond == true)
                    {
                        currentShadowObject.transform.position = bo.transform.position;
                        JointReference.Add(bo.gameObject);
                        //Instantiate the mesh creator point
                        GameObject newGO = Instantiate(currentShadowObject);
                        JointCreatorPoints.Add(newGO);
                        //Update the line renderer
                        JointCreatorLineRenderer.positionCount++;
                        counter++;
                        cond = true;
                    }

                }

            }
        }
    }
    
    //Function of the MOUSESTATE 7, allows for the user to instantiate the prefab of the softbody1
    private void SoftBody1SliderUpdate() 
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //The worldPoint boundaries are : (-28.25, 20)      - (28.25, 20)
        //                                (-28.25, -20)   - (28.25, -20)
        //Check boundaries


        currentShadowObject.transform.position = new Vector3(worldPoint.x / 2, worldPoint.y / 2, 0.0f); //I have literally no clue why i have to do this??? But it works !
        if (worldPoint.x > -28.25f && worldPoint.x < 28.25 &&
            worldPoint.y > -20 && worldPoint.y < 20)
        {
            //If the mouse is in boundaries, change color to good color and if moused clicked add softbody to scene
            currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
            if (Input.GetMouseButtonDown(0))
            {

                currentShadowObject.transform.position = Vector3.zero;
                GameObject newGO = Instantiate(currentShadowObject);
                newGO.GetComponent<SoftBody>().SetBasicMaterial();
                BasicPhysicObject[] bos = newGO.GetComponentsInChildren<BasicPhysicObject>();
                for (int i = 0; i < bos.Length; i++)
                {
                    bos[i].transform.position += new Vector3(worldPoint.x, worldPoint.y, 0.0f);
                }



                physicsManager.AddSoftBody(newGO);
                SoftDropSound.Play();
            }
        }
        //If mouse is out of the boundaries, change color to red
        else
        {
            currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

        }
    }

    //Function of the MOUSESTATE 8, allows for the user to instantiate the prefab of the softbody2
    private void SoftBody2SliderUpdate() 
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //The worldPoint boundaries are : (-28.25, 20)      - (28.25, 20)
        //                                (-28.25, -20)   - (28.25, -20)
        //Check boundaries


        currentShadowObject.transform.position = new Vector3(worldPoint.x / 2, worldPoint.y / 2, 0.0f); //I have literally no clue why i have to do this??? But it works !
        if (worldPoint.x > -28.25f && worldPoint.x < 28.25 &&
            worldPoint.y > -20 && worldPoint.y < 20)
        {
            //If the mouse is in boundaries, change color to good color and if moused clicked add softbody to scene
            currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
            if (Input.GetMouseButtonDown(0))
            {

                currentShadowObject.transform.position = Vector3.zero;
                GameObject newGO = Instantiate(currentShadowObject);
                newGO.GetComponent<SoftBody>().SetBasicMaterial();
                BasicPhysicObject[] bos = newGO.GetComponentsInChildren<BasicPhysicObject>();
                for (int i = 0; i < bos.Length; i++)
                {
                    bos[i].transform.position += new Vector3(worldPoint.x, worldPoint.y, 0.0f);
                }



                physicsManager.AddSoftBody(newGO);
                SoftDropSound.Play();
            }
        }
        //If mouse is out of the boundaries, change color to red
        else
        {
            currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

        }
    }

    //Function that triggers when the user click and there is no active MouseState
    private void NoMouseStateClick() 
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint = new Vector3(worldPoint.x, worldPoint.y, 0.0f);

        //Check boundaries when click
        if (worldPoint.x > -28.25f && worldPoint.x < 28.25 && worldPoint.y > -20 && worldPoint.y < 20)
        {
            //Check if mouse click was in a specific object
            //return the index of the object if there is one
            //return -1 of there is no object

            inspectorScript.MouseClickInsideBoundaries(worldPoint);

		}
    }



    public void SetMouseState0() { ResetMouseState(); MOUSESTATE = 0; currentShadowObject = prefabHolder.GetLittleCircle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState1() { ResetMouseState(); MOUSESTATE = 1; currentShadowObject = prefabHolder.GetMiddleCircle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState2() { ResetMouseState(); MOUSESTATE = 2; currentShadowObject = prefabHolder.GetBigCircle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState3() { ResetMouseState(); MOUSESTATE = 3; currentShadowObject = prefabHolder.GetMiddleTriangle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState4() { ResetMouseState(); MOUSESTATE = 4; currentShadowObject = prefabHolder.GetBigSquare(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState5()
    {
        ResetMouseState();
        MOUSESTATE = 5;
        currentShadowObject = prefabHolder.GetMeshCreatorPoint();
        currentShadowObject.transform.SetParent(GamePanel.transform);
    }
    public void SetMouseState6()
    {
        ResetMouseState();
        MOUSESTATE = 6;
        currentShadowObject = prefabHolder.GetMeshCreatorPoint();
        currentShadowObject.transform.SetParent(GamePanel.transform);
    }
    public void SetMouseState7()
    {
        ResetMouseState();
        MOUSESTATE = 7;
        currentShadowObject = prefabHolder.GetSoftBody1();
        currentShadowObject.transform.SetParent(GamePanel.transform);
    }
    public void SetMouseState8()
    {
        ResetMouseState();
        MOUSESTATE = 8;
        currentShadowObject = prefabHolder.GetSoftBody2();
        currentShadowObject.transform.SetParent(GamePanel.transform);
    }

    public void ResetMouseState()
    {
        MOUSESTATE = -1;
        Destroy(currentShadowObject);
        currentShadowObject = null;
       

        for (int i = meshCreatorPoints.Count - 1; i >= 0; i--)
        {
            Destroy(meshCreatorPoints[i]);
        }
        for (int i = JointCreatorPoints.Count - 1; i >= 0; i--)
        {
            Destroy(JointCreatorPoints[i]);
        }
        for (int i = JointReference.Count - 1; i >= 0; i--)
        {
            JointReference.RemoveAt(i);
        }
        meshCreatorPoints = new List<GameObject>();
        JointCreatorPoints = new List<GameObject>();
        JointReference = new List<GameObject>();
        meshCreatorLineRenderer.positionCount = 1;
        JointCreatorLineRenderer.positionCount = 1;
        counter = 0;

    }

}
