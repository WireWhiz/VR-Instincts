using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTracking : MonoBehaviour
{
    public float DefaultHeight;// = 1.62 is the normal height of a human;
    public float rotationWhenCrouched;

    public GameObject Head;
    public GameObject CameraRig;

    public GameObject BodyRoot;

    public GameObject HeadRoot;
    public GameObject Torso;
    public GameObject Hips;
    public Transform FeetRoot;
    public Vector3 HeadOffset;
    public IK LeftArm;
    public IK RightArm;


    private Quaternion HeadRotation;
    private Quaternion TorsoRotation;

    private Vector3 TorsoOffset;
    private Vector3 HipOffset;
    private Quaternion HipOffsetRot;
    private Quaternion TorsoOffsetRotation;

    private Quaternion HipOffsetRotation;
    private Vector3 PastPos;

    // Start is called before the first frame update
    void Awake()
    {
        HeadRotation = HeadRoot.transform.rotation;
        TorsoRotation = BodyRoot.transform.rotation;
        TorsoOffset = Torso.transform.position -HeadRoot.transform.position;
        HipOffset = Hips.transform.position - HeadRoot.transform.position;
        HipOffsetRot = Hips.transform.rotation;
        TorsoOffsetRotation = Torso.transform.rotation;
        HipOffsetRotation = Torso.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        BodyRoot.transform.position = new Vector3(Head.transform.position.x, CameraRig.transform.position.y, Head.transform.position.z);
        if ((BodyRoot.transform.position - PastPos).magnitude > .005f)
        {
            FeetRoot.GetComponent<FootMovment>().HeightMultiplyer += .01f;
            Debug.Log("Angle is: "+Quaternion.Angle(FeetRoot.rotation, Quaternion.Euler(0, BodyRoot.transform.rotation.eulerAngles.y, 0)));
            if (Quaternion.Angle(Quaternion.LookRotation(BodyRoot.transform.position - PastPos), Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0)) < 100)
            {
                Hips.transform.rotation =Quaternion.RotateTowards(Hips.transform.rotation, Quaternion.Euler(0, Quaternion.LookRotation(BodyRoot.transform.position - PastPos).eulerAngles.y, 0) * HipOffsetRot,3);
                FeetRoot.rotation = Quaternion.RotateTowards(FeetRoot.rotation, Quaternion.Euler(0, Quaternion.LookRotation(BodyRoot.transform.position - PastPos).eulerAngles.y, 0), 3) ;
                if (CameraRig.GetComponent<Player>().TouchingGround)
                {
                    FeetRoot.GetComponent<FootMovment>().WalkM((BodyRoot.transform.position - PastPos).magnitude);
                }
            }
            else
            {
                Hips.transform.rotation = Quaternion.RotateTowards(Hips.transform.rotation, Quaternion.Euler(0, Quaternion.LookRotation(-(BodyRoot.transform.position - PastPos)).eulerAngles.y, 0) * HipOffsetRot,3);
                FeetRoot.rotation = Quaternion.RotateTowards(FeetRoot.rotation, Quaternion.Euler(0, Quaternion.LookRotation(-(BodyRoot.transform.position - PastPos)).eulerAngles.y, 0),3);
                if (CameraRig.GetComponent<Player>().TouchingGround)
                {
                    FeetRoot.GetComponent<FootMovment>().WalkM(-(BodyRoot.transform.position - PastPos).magnitude);
                }
            }
        }
        else
        {
            Hips.transform.rotation =  Quaternion.RotateTowards(Hips.transform.rotation, Quaternion.Euler(0, BodyRoot.transform.rotation.eulerAngles.y, 0) * HipOffsetRot, 2.5f) ;
            FeetRoot.rotation = Quaternion.RotateTowards(FeetRoot.rotation, Quaternion.Euler(0, BodyRoot.transform.rotation.eulerAngles.y, 0), 2) ;

            FeetRoot.GetComponent<FootMovment>().HeightMultiplyer -= .01f;
        }
        for (int i = 0; i < 5; i++)
        {
            if (Quaternion.Angle(Quaternion.Euler(0, BodyRoot.transform.rotation.eulerAngles.y, 0), Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0)) > 90)
            {
                BodyRoot.transform.rotation = Quaternion.Slerp(BodyRoot.transform.rotation, Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0) * TorsoRotation, .02f);
            }
            else
            {
                if (RightArm.CantReach)
                {
                    BodyRoot.transform.rotation = Quaternion.RotateTowards(BodyRoot.transform.rotation, Quaternion.Euler(0, -90, 0) * Quaternion.Euler(0, Quaternion.FromToRotation(Vector3.forward, RightArm.Target.position - BodyRoot.transform.position).eulerAngles.y, 0) * TorsoRotation, 3);
                }
                if (LeftArm.CantReach)
                {
                    BodyRoot.transform.rotation = Quaternion.RotateTowards(BodyRoot.transform.rotation, Quaternion.Euler(0, 90, 0) * Quaternion.Euler(0, Quaternion.FromToRotation(Vector3.forward, LeftArm.Target.position - BodyRoot.transform.position).eulerAngles.y, 0) * TorsoRotation, 3);
                }
            }
            RightArm.UpdateIK();
            LeftArm.UpdateIK();
        }
        
        Torso.transform.position= BodyRoot.transform.rotation * Quaternion.Euler((1-Head.transform.localPosition.y/DefaultHeight)*rotationWhenCrouched,0,0) * (TorsoOffset+HeadOffset)+Head.transform.position-(Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0) * TorsoRotation*Vector3.forward  *(0.3f)* (FixEuler(Head.transform.rotation.eulerAngles.x)/ 180));
        Hips.transform.position= BodyRoot.transform.rotation * Quaternion.Euler((1 - Head.transform.localPosition.y / DefaultHeight) * rotationWhenCrouched, 0, 0) * (HipOffset + HeadOffset) + Head.transform.position - (Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0) * TorsoRotation * Vector3.forward * (0.3f) * (FixEuler(Head.transform.rotation.eulerAngles.x) / 180));
        Torso.transform.rotation = BodyRoot.transform.rotation * Quaternion.Euler((1 - Head.transform.localPosition.y / DefaultHeight) * rotationWhenCrouched, 0, 0) * TorsoOffsetRotation;
        //Debug.Log(FixEuler(Head.transform.rotation.eulerAngles.x) + 90);
        HeadRoot.transform.position = Head.transform.position+ ( Head.transform.rotation * HeadOffset);
        HeadRoot.transform.rotation = Head.transform.rotation* HeadRotation;
        FeetRoot.position = new Vector3(Torso.transform.position.x,CameraRig.transform.position.y,Torso.transform.position.z);

        
        
        PastPos = BodyRoot.transform.position;
        RightArm.UpdateIK();
        LeftArm.UpdateIK();

    }
    float FixEuler(float angle)
    {
        if(angle < 180)
        {
            return angle;
        }
        else
        {
            return angle - 360;
        }
    }

}
