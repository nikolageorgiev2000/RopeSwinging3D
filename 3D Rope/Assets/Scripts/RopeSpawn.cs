using System.Collections.Generic;
using UnityEngine;

public class RopeSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject partPrefab, hookPoint, swinger, tempRigidbody;

    [SerializeField]
    [Range(1, 1000)]
    int length = 1;

    //    [SerializeField]
    //    float partLength = 0.21f;

    [SerializeField]
    bool reset, spawn, snapFirst, snapLast, isSwinging;



    // Start is called before the first frame update
    void Start()
    {
        isSwinging = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Jump"))
        {
            if (isSwinging)
            {
                foreach (GameObject temp in GameObject.FindGameObjectsWithTag("CurrentRope"))
                {
                    temp.tag = "OldRope";
                    temp.name = "OLD_" + temp.name;
                    //Vector3 tempV = swinger.GetComponent<Rigidbody>().velocity;
                    //swinger.GetComponent<Rigidbody>().velocity = tempV;
                }
                swinger.GetComponent<ConfigurableJoint>().connectedBody = tempRigidbody.GetComponent<Rigidbody>();
                isSwinging = false;
            }
            else
            {
                SpawnRope();
                isSwinging = true;
            }
        }

        if (spawn)
        {
            SpawnRope();

            spawn = false;
            isSwinging = true;
        }
    }

    public void SpawnRope()
    {
        //float partLength = partPrefab.transform.localScale[1];
        //float partLength = 2f;
        //int count = (int) ((swinger.transform.position - hookPoint.transform.position).magnitude / partLength);
        int count = 10;
        Vector3 directionVector = (swinger.transform.position - hookPoint.transform.position) / count;


        //rope built with Fibonacci-length pieces : 1,1,2,3,5,8,13,21,34...
        //start from 2,3 because 1,1 results in segments that are too short initially
        var segmentLengths = new List<int>();
        //0 at start because otherwise last segment (at swinger) isn't instatiated
        segmentLengths.Add(0);
        segmentLengths.Add(2);
        segmentLengths.Add(3);
        float fibSum = 5;
        float distanceFromHookPoint = (swinger.transform.position - hookPoint.transform.position).magnitude;
        while (fibSum * 5 * partPrefab.transform.localScale[1] <= distanceFromHookPoint)
        {
            fibSum += segmentLengths[segmentLengths.Count - 1] + segmentLengths[segmentLengths.Count - 2];
            segmentLengths.Add(segmentLengths[segmentLengths.Count - 1] + segmentLengths[segmentLengths.Count - 2]);
        }
        float segmentUnit = (swinger.transform.position - hookPoint.transform.position).magnitude / fibSum;
        Vector3 ropeDirection = (swinger.transform.position - hookPoint.transform.position).normalized;
        segmentLengths.Reverse();

        float segmentAcc = 0;

        //change count to segmentLengths.Count for Fibonacci method
        for (int i = 0; i < count; i++)
        {
            GameObject temp;

            //uniformly distributed segments
            temp = Instantiate(partPrefab, (hookPoint.transform.position + i * directionVector), Quaternion.identity, hookPoint.transform);
            //using Fibonacci lengths

            //temp = Instantiate(partPrefab, hookPoint.transform.position + segmentAcc * segmentUnit * ropeDirection, Quaternion.identity, hookPoint.transform);
            //segmentAcc += segmentLengths[i];

            temp.name = hookPoint.transform.childCount.ToString();
            temp.tag = "CurrentRope";

            //temp.GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
            //temp.GetComponent<Rigidbody>().inertiaTensor = Vector3.one;
            //swinger.GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
            //swinger.GetComponent<Rigidbody>().inertiaTensor = Vector3.one;
            //temp.transform.Rotate(Vector3.RotateTowards(mass.transform.position, hookPoint.transform.position,0.001f,2));

            if (i == 0)
            {
                //Destroy(temp.GetComponent<CharacterJoint>());
                //parentObject.transform.position = temp.transform.position - new Vector3(0,0.5f,0);
                hookPoint.GetComponent<Rigidbody>().maxDepenetrationVelocity = 3f;
                temp.GetComponent<ConfigurableJoint>().connectedBody = hookPoint.GetComponent<Rigidbody>();
                temp.GetComponent<ConfigurableJoint>().connectedMassScale = hookPoint.GetComponent<Rigidbody>().mass / temp.GetComponent<Rigidbody>().mass;

                if (snapFirst)
                {
                    //temp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    //temp.GetComponent<Rigidbody>().position = mass.transform.position;  
                    hookPoint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                }
            }
            else
            {
                temp.GetComponent<ConfigurableJoint>().connectedBody = hookPoint.transform.Find((hookPoint.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }

        swinger.GetComponent<ConfigurableJoint>().connectedBody = hookPoint.transform.Find((hookPoint.transform.childCount).ToString()).GetComponent<Rigidbody>();

        if (snapLast)
        {
            hookPoint.transform.Find((hookPoint.transform.childCount).ToString()).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            //parentObject.transform.Find((parentObject.transform.childCount).ToString()).GetComponent<CharacterJoint>().connectedBody = mass.GetComponent<Rigidbody>();
        }
    }

}
