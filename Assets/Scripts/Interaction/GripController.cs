using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GripController : MonoBehaviour
{
    public SteamVR_Input_Sources Hand;
    public SteamVR_Action_Boolean ToggleGripButton;
    public SteamVR_Action_Pose position;
    public SteamVR_Behaviour_Skeleton HandSkeleton;
    public SteamVR_Behaviour_Skeleton PreviewSkeleton;
    public Grabber grabber;

    private GameObject ConnectedObject;
    private Transform OffsetObject;
    private bool SecondGrip;
    private void Update()
    {

        if (ConnectedObject != null )
        {
            if (!SecondGrip)
            {


                if (ConnectedObject.GetComponent<Interactable>().touchCount == 0&& !ConnectedObject.GetComponent<Interactable>().SecondGripped)
                {
                    grabber.FixedJoint.connectedBody = null;
                    grabber.StrongGrip.connectedBody = null;

                    ConnectedObject.transform.position = Vector3.MoveTowards(ConnectedObject.transform.position, transform.position - ConnectedObject.transform.rotation * OffsetObject.localPosition, .25f);
                    ConnectedObject.transform.rotation = Quaternion.RotateTowards(ConnectedObject.transform.rotation, transform.rotation * Quaternion.Inverse(OffsetObject.localRotation), 10);
                    grabber.FixedJoint.connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                }
                else if (ConnectedObject.GetComponent<Interactable>().touchCount > 0|| ConnectedObject.GetComponent<Interactable>().SecondGripped)
                {

                    grabber.FixedJoint.connectedBody = null;
                    grabber.StrongGrip.connectedAnchor = OffsetObject.localPosition;
                    grabber.StrongGrip.connectedBody = ConnectedObject.GetComponent<Rigidbody>();

                }else if(ConnectedObject.GetComponent<Interactable>().touchCount < 0)
                {
                    ConnectedObject.GetComponent<Interactable>().touchCount = 0;
                }
                
            }
            else if (SecondGrip)
            {
                
                if(!ConnectedObject.GetComponent<Interactable>().gripped)
                {
                    grabber.FixedJoint.connectedBody = null;
                    grabber.StrongGrip.connectedAnchor = OffsetObject.localPosition;
                    grabber.StrongGrip.connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                }
                else
                {
                    grabber.FixedJoint.connectedBody = null;
                    grabber.StrongGrip.connectedBody = null;
                    grabber.WeakGrip.connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                }
            }
            if (ToggleGripButton.GetStateUp(Hand))
            {
                Release();
            }
            if(PreviewSkeleton)
                PreviewSkeleton.transform.gameObject.SetActive(false);
        }
        else
        {
            if (grabber.ClosestGrabbable() && PreviewSkeleton)
            {
                PreviewSkeleton.transform.gameObject.SetActive(true);
                OffsetObject =grabber.ClosestGrabbable().transform;
                if (grabber.ClosestGrabbable().GetComponent<SteamVR_Skeleton_Poser>())
                {
                    if (!OffsetObject.GetComponent<GrabPoint>().SubGrip && !OffsetObject.transform.parent.GetComponent<Interactable>().gripped || OffsetObject.GetComponent<GrabPoint>().SubGrip && OffsetObject.transform.parent.GetComponent<Interactable>().gripped)
                    {
                        PreviewSkeleton.transform.SetParent(OffsetObject, false);
                        PreviewSkeleton.BlendToPoser(OffsetObject.GetComponent<SteamVR_Skeleton_Poser>(), 0f);
                    }
                }
            }
            else
            {
                PreviewSkeleton.transform.gameObject.SetActive(false);
            }
            if (ToggleGripButton.GetStateDown(Hand))
            {
                Grip();
            }
        }
    }
    private void Grip()
    {
        GameObject NewObject = grabber.ClosestGrabbable();
        if (NewObject != null)
        {
            OffsetObject = grabber.ClosestGrabbable().transform;
            ConnectedObject = OffsetObject.transform.parent.gameObject;//find the Closest Grabbable and set it to the connected object
            ConnectedObject.GetComponent<Rigidbody>().useGravity = false;
            if (ConnectedObject.GetComponent<Interactable>().gripped)
            {
                SecondGrip = true;
                ConnectedObject.GetComponent<Interactable>().SecondGripped = true;
                grabber.WeakGrip.connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                grabber.WeakGrip.connectedAnchor = OffsetObject.localPosition;
            }
            else
            {
                ConnectedObject.GetComponent<Interactable>().Hand = Hand;
                ConnectedObject.GetComponent<Interactable>().gripped = true;
            }
            if (OffsetObject.GetComponent<SteamVR_Skeleton_Poser>()&&HandSkeleton)
            {
                HandSkeleton.transform.SetParent(OffsetObject, false);
                HandSkeleton.BlendToPoser(OffsetObject.GetComponent<SteamVR_Skeleton_Poser>(), 0f);
            }


        }
    }
    private void Release()
    {
        grabber.FixedJoint.connectedBody = null;
        grabber.StrongGrip.connectedBody = null;
        grabber.WeakGrip.connectedBody = null;
        ConnectedObject.GetComponent<Rigidbody>().velocity = position.GetVelocity(Hand) + transform.parent.GetComponent<Rigidbody>().velocity;
        ConnectedObject.GetComponent<Rigidbody>().angularVelocity = position.GetAngularVelocity(Hand) + transform.parent.GetComponent<Rigidbody>().angularVelocity;
        ConnectedObject.GetComponent<Rigidbody>().useGravity = true;
        if (!SecondGrip)
        {
            
            ConnectedObject.GetComponent<Interactable>().gripped = false;
            
        }
        else
        {
            ConnectedObject.GetComponent<Interactable>().SecondGripped = false;
            SecondGrip = false;
        }
        
        ConnectedObject = null;
        if (OffsetObject.GetComponent<SteamVR_Skeleton_Poser>() && HandSkeleton)
        {
            HandSkeleton.transform.SetParent(transform, false);
            HandSkeleton.BlendToSkeleton();
        }

        OffsetObject = null;
    }
    
}
