using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleGrip : MonoBehaviour
{
    public GrabPoint LeftHandGrip;
    public GrabPoint RightHandGrip;

    private Transform LeftHand;
    private Transform RightHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LeftHand)
        {
            Vector3 newpose = Vector3.Project((LeftHand.position - transform.position)*100, transform.up)/100;
            if (newpose.magnitude < GetComponent<CapsuleCollider>().height / 2 && !LeftHandGrip.Gripped)
            {
                LeftHandGrip.transform.parent.position = newpose + transform.position;
                LeftHandGrip.transform.parent.rotation = Quaternion.LookRotation(-((LeftHand.position) - LeftHandGrip.transform.parent.position), transform.up);

                LeftHandGrip.UpdateOffset();
                Debug.Log("set new pose");

            }
            else if (!LeftHandGrip.Gripped && newpose.magnitude > GetComponent<CapsuleCollider>().height / 2)
            {
                LeftHandGrip.transform.localPosition = new Vector3();
            }
        }
        if (RightHand)
        {
            Vector3 newpose = Vector3.Project((RightHand.position - transform.position) * 100, transform.up)/100;
            if (newpose.magnitude < GetComponent<CapsuleCollider>().height / 2 && !RightHandGrip.Gripped)
            {
                RightHandGrip.transform.parent.position = newpose + transform.position;
                RightHandGrip.transform.parent.rotation = Quaternion.LookRotation(-((RightHand.position) - RightHandGrip.transform.parent.position), transform.up);

                RightHandGrip.UpdateOffset();
                Debug.Log("set new pose");

            }
            else if (!RightHandGrip.Gripped&& newpose.magnitude > GetComponent<CapsuleCollider>().height / 2)
            {
                RightHandGrip.transform.localPosition = new Vector3();
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Grabber>())
        {
            if (other.transform.parent.GetComponent<GripController>().Hand == Valve.VR.SteamVR_Input_Sources.LeftHand)
            {
                LeftHand = other.transform;
            }
            else if (other.transform.parent.GetComponent<GripController>().Hand == Valve.VR.SteamVR_Input_Sources.RightHand)
            {
                RightHand = other.transform;
            }
            Debug.Log("found a grabber!");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Grabber>())
        {
            if (other.transform.parent.GetComponent<GripController>().Hand == Valve.VR.SteamVR_Input_Sources.LeftHand)
            {
                LeftHand = null;
            }
            else if (other.transform.parent.GetComponent<GripController>().Hand == Valve.VR.SteamVR_Input_Sources.RightHand)
            {
                RightHand = null;
            }
        }
    }
}
