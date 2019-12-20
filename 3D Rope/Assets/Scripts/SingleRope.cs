using UnityEngine;


public class SingleRope : MonoBehaviour
{


    private GameObject hookPoint, swinger;
    public GameObject partPrefab, tempRigidBody;


    // Start is called before the first frame update
    void TheStart(GameObject[] gos)
    {
        //get game objects from input array
        hookPoint = gos[0];
        //hookPoint is frozen since it is the objec the rope is attached to
        hookPoint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        swinger = gos[1];
        //Configurable joint is initialized with temporary rigid body
        swinger.GetComponent<ConfigurableJoint>().connectedBody = tempRigidBody.GetComponent<Rigidbody>();

        //number of rope pieces used -> if the rope is short less pieces are used, and if a long rope a max of 20 pieces
        int count = 20;


        while(count*partPrefab.transform.localScale[1]*10 > (hookPoint.transform.position - swinger.transform.position).magnitude)
        {
            count--;
        }

        //offset of position of each piece
        Vector3 directionVector = (swinger.transform.position - hookPoint.transform.position) / count;

        //fence post problem, so <=
        for (int i = 0; i <= count; i++)
        {
            GameObject temp;
            
            //uniformly distributed segments
            temp = Instantiate(partPrefab, (hookPoint.transform.position + i * directionVector), Quaternion.identity, this.transform);

            //name used to keep track of has-a relations through joints (i.e. piece 6 has piece 5 connected to its joint)
            temp.name = transform.childCount.ToString();
            temp.tag = "RopePiece";

            //piece 0 has the hookPoint connected to its joint
            if (i == 0)
            {
                hookPoint.GetComponent<Rigidbody>().maxDepenetrationVelocity = 3f;
                temp.GetComponent<ConfigurableJoint>().connectedBody = hookPoint.GetComponent<Rigidbody>();
                temp.GetComponent<ConfigurableJoint>().connectedMassScale = hookPoint.GetComponent<Rigidbody>().mass / temp.GetComponent<Rigidbody>().mass;
            }
            else
            {
                //previous piece connected to current piece joint
                temp.GetComponent<ConfigurableJoint>().connectedBody = transform.Find((transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }

        //final piece connected to swinging body's joint
        swinger.GetComponent<ConfigurableJoint>().connectedBody = transform.Find((transform.childCount).ToString()).GetComponent<Rigidbody>();
        //could be used to fix physics issues with mass differences between swinger and rope piece
        //swinger.GetComponent<ConfigurableJoint>().connectedMassScale = transform.Find((transform.childCount).ToString()).GetComponent<Rigidbody>().mass / swinger.GetComponent<Rigidbody>().mass;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void End()
    {
        foreach (GameObject temp in GameObject.FindGameObjectsWithTag("RopePiece"))
        {
            //unused ropes have collision off to prevent jankiness
            //temp.GetComponent<ConfigurableJoint>().enableCollision = false;
        }
        //when not connected to a rope swinger needs a temporary rigid body connection
        swinger.GetComponent<ConfigurableJoint>().connectedBody = tempRigidBody.GetComponent<Rigidbody>();
    }


    public void Destroy()
    {
        Destroy(hookPoint);
        Destroy(this.gameObject);
    }

}
