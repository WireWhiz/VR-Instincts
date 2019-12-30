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
    private bool DomanantGrip;
    private void Update()
    {

        if (ConnectedObject != null )
        {
            if (DomanantGrip || !ConnectedObject.GetComponent<Interactable>().SecondGripped)
            {


                if (ConnectedObject.GetComponent<Interactable>().touchCount == 0&& !ConnectedObject.GetComponent<Interactable>().SecondGripped)
                {
                    grabber.FixedJoint.connectedBody = null;
                    grabber.StrongGrip.connectedBody = null;

                    ConnectedObject.transform.position = Vector3.MoveTowards(ConnectedObject.transform.position, transform.position - ConnectedObject.transform.rotation * OffsetObject.GetComponent<GrabPoint>().Offset, .25f);
                    ConnectedObject.transform.rotation = Quaternion.RotateTowards(ConnectedObject.transform.rotation, transform.rotation*Quaternion.Inverse( OffsetObject.GetComponent<GrabPoint>().RotationOffset), 10);
                    grabber.FixedJoint.connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                }
                else if (ConnectedObject.GetComponent<Interactable>().touchCount > 0|| ConnectedObject.GetComponent<Interactable>().SecondGripped)
                {

                    grabber.FixedJoint.connectedBody = null;
                    grabber.StrongGrip.connectedAnchor = OffsetObject.GetComponent<GrabPoint>().Offset;
                    grabber.StrongGrip.connectedBody = ConnectedObject.GetComponent<Rigidbody>();

                }else if(ConnectedObject.GetComponent<Interactable>().touchCount < 0)
                {
                    ConnectedObject.GetComponent<Interactable>().touchCount = 0;
                }
                
            }
            else
            {
                grabber.FixedJoint.connectedBody = null;
                grabber.StrongGrip.connectedBody = null;
                grabber.WeakGrip.connectedBody = ConnectedObject.GetComponent<Rigidbody>();
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
                OffsetObject = grabber.ClosestGrabbable().transform;
                if (grabber.ClosestGrabbable().GetComponent<SteamVR_Skeleton_Poser>())
                {
                    if (!OffsetObject.GetComponent<GrabPoint>().Gripped)
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
            ConnectedObject = OffsetObject.GetComponent<GrabPoint>().ParentInteractable.gameObject;//find the Closest Grabbable and set it to the connected object
            ConnectedObject.GetComponent<Rigidbody>().useGravity = false;

            OffsetObject.GetComponent<GrabPoint>().Gripped = true;
            if (ConnectedObject.GetComponent<Interactable>().gripped)
            {
                ConnectedObject.GetComponent<Interactable>().SecondGripped = true;
                if (OffsetObject.GetComponent<GrabPoint>().HelperGrip)
                {
                    DomanantGrip = false;
                    grabber.WeakGrip.connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                    grabber.WeakGrip.connectedAnchor = OffsetObject.GetComponent<GrabPoint>().Offset;
                }
                grabber.WeakGrip.connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                grabber.WeakGrip.connectedAnchor = OffsetObject.GetComponent<GrabPoint>().Offset;
            }
            else
            {
                ConnectedObject.GetComponent<Interactable>().Hand = Hand;
                ConnectedObject.GetComponent<Interactable>().gripped = true;
                if (!OffsetObject.GetComponent<GrabPoint>().HelperGrip)
                {
                    DomanantGrip = true;
                    ConnectedObject.GetComponent<Interactable>().GrippedBy = transform.parent.gameObject;
                }
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
        if (!ConnectedObject.GetComponent<Interactable>().SecondGripped)
        {
            
            ConnectedObject.GetComponent<Interactable>().gripped = false;

            ConnectedObject.GetComponent<Interactable>().GrippedBy =null;

        }
        else
        {
            ConnectedObject.GetComponent<Interactable>().SecondGripped = false;
        }
        
        ConnectedObject = null;
        if (OffsetObject.GetComponent<SteamVR_Skeleton_Poser>() && HandSkeleton)
        {
            HandSkeleton.transform.SetParent(transform, false);
            HandSkeleton.BlendToSkeleton();
        }
        OffsetObject.GetComponent<GrabPoint>().Gripped = false;
        OffsetObject = null;
    }
    
}
