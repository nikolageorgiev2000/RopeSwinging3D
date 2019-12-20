using System.Collections.Generic;
using UnityEngine;

public class DrawRopes : MonoBehaviour
{

    [SerializeField]
    public GameObject ropePrefab, swinger, playerCamera;
    //public hookPoint;

    public bool isSwinging;
    private bool isSwingingLeft;
    private bool isSwingingRight;
    private List<GameObject> ropes;
    private Vector3 startDir;

    // Start is called before the first frame update
    void Start()
    {
        isSwinging = false;
        ropes = new List<GameObject>();
        startDir = swinger.transform.position - playerCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        bool aimingLeft = false;
        bool aimingRight = false;
        if (Input.GetButtonDown("LeftSwing") && this.tag == "LeftHand")
        {
            aimingLeft = true;
        }
        if (Input.GetButtonDown("RightSwing") && this.tag == "RightHand")
        {
            aimingRight = true;
        }


        if (aimingLeft || aimingRight)
        {
            if (!isSwinging)
            {
                //Vector3 fwd = transform.TransformDirection(Vector3.forward);
                //aims ray at a 60 degree angle from Y-axis
                
                //right above
                Ray ray = new Ray(swinger.transform.position, new Vector3(0,0,0));

                if (aimingLeft) {
                    //ray = new Ray(swinger.transform.position, new Vector3(-1f, 5f, 2f));
                    //Debug.Log("Left");
                } else
                {
                    //Debug.Log("Right");
                    //ray = new Ray(swinger.transform.position, new Vector3(1f, 5f, 2f));
                }

                RaycastHit hit;

                //search for hookPoints
                float maxRopeLength = 220;
                float ropeFitness = -maxRopeLength;
                float tempFitness;
                float searchWidth = Mathf.PI / 8;
                Vector3 cameraOffset = swinger.transform.position - playerCamera.transform.position;

                //(0,0,1) is the assumed direction when using phi and theta later on
                //in DEGREES  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                float directionChangeAngle = Vector3.SignedAngle(new Vector3(0, 0, 1), new Vector3(cameraOffset.x, 0, cameraOffset.z), Vector3.up);
                Debug.Log(directionChangeAngle);
                float searchAngleHoriz = Mathf.Asin(Input.GetAxis("Vertical"));
                Debug.Log(searchAngleHoriz);

                int triesHoriz = 20;
                int triesVert = 10;
                float deltaPhi = searchWidth / triesHoriz;
                float deltaTheta = Mathf.PI / 2 / triesVert;
                float phi;
                float theta;
                Ray testRay;

                //multiply by 7/8 to avoid points directly above or to the side
                for (int i = triesHoriz/2; i >= -triesHoriz / 2; i--)
                {
                    for (int j = triesVert; j >= 0; j--)
                    {
                        phi = Mathf.Min(searchAngleHoriz + deltaPhi * i, Mathf.PI);
                        //Debug.Log(phi);
                        theta = deltaTheta * j;
                        //Quaternion.Euler takes DEGREES !!!!!!!!!!!!!!!!!!
                        testRay = new Ray(swinger.transform.position, Quaternion.Euler(0, directionChangeAngle, 0) * (new Vector3( (aimingRight ? 1 : -1)*Mathf.Cos(phi)* Mathf.Sin(theta), Mathf.Cos(theta), Mathf.Sin(phi) * Mathf.Sin(theta))));
                        Debug.DrawRay(testRay.origin, testRay.direction * 100, Color.red, 10);


                        //find best rope by maximising fitness using horizontal distance and closeness to horizontal angle
                        //old fitness: hit.point[1] - Mathf.Sqrt(Mathf.Pow(hit.point[0],2) + Mathf.Pow(hit.point[2], 2)) based on horizontal and vertical distance
                        //Mathf.Abs(triesHoriz / 2 - i) / (triesHoriz / 2) -  tried to center it around aimed at angle
                        //height check: (hit.point[1] - (swinger.transform.position - hit.point).magnitude) / hit.point[1]
                        if (Physics.Raycast(testRay, out hit, maxRopeLength) && (tempFitness = 4*(hit.point[1] / swinger.transform.position[1]) - Mathf.Abs(i / triesHoriz) - 4 * (swinger.transform.position - hit.point).magnitude / hit.point[1] - 2*Mathf.Abs(Vector3.Dot(swinger.GetComponent<Rigidbody>().velocity.normalized, (hit.point - swinger.transform.position).normalized))) > ropeFitness)
                        {
                            //Debug.Log(ropeFitness + " "+ swinger.GetComponent<Rigidbody>().velocity.normalized.ToString());
                            ropeFitness = tempFitness;
                            ray = testRay;
                        }
                        
                    }
                }

                //ropeFitness = -200;

                //check for ray collision and that the hookPoint is not too close, since the rope wouldn't work
                // && hit.distance > 2 for limiting distance
                if (Physics.Raycast(ray, out hit, maxRopeLength))
                {
                    Debug.Log("Collision Type: "+hit.transform.gameObject.name);
                    //creates basic cube to swing off of
                    GameObject hookPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    hookPoint.transform.position = hit.point;
                    hookPoint.transform.localScale = new Vector3(.1f,.1f,.1f);
                    hookPoint.AddComponent<Rigidbody>();

                    //creates a rope with this object as its parent (rope drawer has ropes, which then have rope pieces)
                    GameObject rope = Instantiate(ropePrefab, this.transform.position, Quaternion.identity, this.transform) as GameObject;
                    rope.transform.parent = this.transform;

                    //sends input to rope method
                    GameObject[] gos = new GameObject[] { hookPoint, swinger };
                    rope.SendMessage("TheStart", gos);
                    isSwinging = true;

                    //adds rope to list of ropes
                    ropes.Add(rope);
                    rope.tag = "Rope";
                }


            }
            else
            {
                //if already in a swing, RightSwing / LeftSwing means to end swing, so End method is called
                ropes[ropes.Count-1].SendMessage("End");
                isSwinging = false;
            }

        }
        while (ropes.Count > 3)
        {
            //Debug.Log("Destroy");
            ropes[0].SendMessage("Destroy");
            ropes.RemoveAt(0);
        }

    }

    public void cutRope()
    {
        ropes[ropes.Count - 1].SendMessage("End");
        isSwinging = false;
    }


}
