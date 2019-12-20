using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject tempRigidBody;
    //public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<ConfigurableJoint>().connectedBody = tempRigidBody.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Console.WriteLine(velocity);
        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 1000, 0));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //velocity = collision.relativeVelocity;
    }

}
