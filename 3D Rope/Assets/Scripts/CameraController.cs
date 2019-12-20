using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject ReferenceObject, leftDrawer, rightDrawer;

    private Vector3 offset;
    public float x, y, z;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - ReferenceObject.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetButtonDown("RESET"))
        {
            leftDrawer.SendMessage("cutRope");
            rightDrawer.SendMessage("cutRope");
            foreach(GameObject temp in GameObject.FindGameObjectsWithTag("Body")){
                if(temp.GetComponent<Rigidbody>() != null)
                {
                    temp.GetComponent<Rigidbody>().isKinematic = true;
                    temp.GetComponent<Rigidbody>().velocity = Vector3.zero;

                }
            }
            ReferenceObject.SetActive(false);
            ReferenceObject.transform.position = new Vector3(x, y, z);
            ReferenceObject.SetActive(true);
            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("Body"))
            {
                if (temp.GetComponent<Rigidbody>() != null)
                {
                    temp.GetComponent<Rigidbody>().isKinematic = false;
                }
            }


        }

        //Debug.Log(Input.GetAxisRaw("Mouse X"));
        offset = Quaternion.AngleAxis(100 * Time.deltaTime * Input.GetAxisRaw("Mouse X"), Vector3.up) * offset;
        Vector3 offsetHoriz = new Vector3(offset[0], 0, offset[2]);
        if (Vector3.Angle(Quaternion.AngleAxis(100 * Time.deltaTime * Input.GetAxisRaw("Mouse Y"), Vector3.Cross(Vector3.up, offsetHoriz)) * offset, offsetHoriz) < 80 )
        {
            offset = Quaternion.AngleAxis(100 * Time.deltaTime * Input.GetAxisRaw("Mouse Y"), Vector3.Cross(Vector3.up, offset)) * offset;
        }
        //.RotateAround(ReferenceObject.transform.position, Vector3.up, 30 * Time.deltaTime * Input.GetAxis("Mouse X"));
        //offset = new Vector3(0, 10, -20);
        transform.position = ReferenceObject.transform.position + offset;
        transform.LookAt(ReferenceObject.transform.position);
        //if(ReferenceObject.GetComponent<Rigidbody>().velocity.magnitude != 0)
        //{
        //transform.position = ReferenceObject.transform.position + ReferenceObject.GetComponent<Rigidbody>().velocity.normalized * 20;
        //}
        //transform.Rotate(Vector3.RotateTowards(transform.eulerAngles, ReferenceObject.GetComponent<Rigidbody>().velocity.normalized, 4, 2));
    }
}
