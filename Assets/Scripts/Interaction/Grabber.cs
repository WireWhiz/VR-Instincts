using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public ConfigurableJoint StrongGrip;
    public ConfigurableJoint WeakGrip;
    public FixedJoint FixedJoint;


    public List<GameObject> NearObjects = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GrabPoint>())
        {
            //Debug.Log("It didn't crash!");
            if (!other.GetComponent<GrabPoint>().Gripped)
            {
                NearObjects.Add(other.gameObject);
            }
            
        }
        //Debug.Log(NearObjects);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GrabPoint>())
        {
            NearObjects.Remove(other.gameObject);
        }
    }
    public GameObject ClosestGrabbable()
    {
        GameObject ClosestGameObj = null;
        float Distance = float.MaxValue;
        if (NearObjects != null)
        {
            foreach (GameObject GameObj in NearObjects)
            {
                if (!GameObj.GetComponent<GrabPoint>().RestrictByRotation || GameObj.GetComponent<GrabPoint>().RotationLimit > Quaternion.Angle(transform.rotation, GameObj.transform.rotation))
                {
                    if ((GameObj.transform.position - transform.position).sqrMagnitude < Distance)
                    {
                        ClosestGameObj = GameObj;
                        Distance = (GameObj.transform.position - transform.position).sqrMagnitude;
                    }
                }
            }
        }
        return ClosestGameObj;
    }
}
