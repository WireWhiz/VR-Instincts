using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class IK : MonoBehaviour
{
    public Transform Upper;
    public Transform Lower;
    public Transform End;
    public Transform Target;
    public Transform Pole;
    public bool ReversePole;
    public float UpperElbowRotation;
    public float LowerElbowRotation;
    public bool SetRotation;
    public Vector3 OffsetEndRotation;
    // Start is called before the first frame update
    private float a;
    private float b;
    private float c;
    private Vector3 en;
    public bool CantReach;

    // Update is called once per frame
    void Update()
    {
        UpdateIK();
    }
    public void UpdateIK()
    {
        a = (Lower.position - Upper.position).magnitude;
        b = (End.position - Lower.position).magnitude;
        c = Vector3.Distance(Upper.position, Target.position);
        en = Vector3.Cross(Target.position - Upper.position, Pole.position - Upper.position);
        if (ReversePole) en *= -1;
        //Debug.Log("The angle is: " + CosAngle(a,b,c));
        Debug.DrawLine(Upper.position, Target.position);
        Debug.DrawLine((Upper.position + Target.position) / 2, Lower.position);
        //Debug.Log(en);
        Upper.rotation = Quaternion.LookRotation(Target.position - Upper.position, Quaternion.AngleAxis(UpperElbowRotation, Lower.position - Upper.position) * (en));
        Upper.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, Lower.localPosition));
        Upper.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -en) * Upper.rotation;

        Lower.rotation = Quaternion.LookRotation(Target.position - Lower.position, Quaternion.AngleAxis(LowerElbowRotation, End.position - Lower.position) * (en));
        Lower.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, End.localPosition));
        if (SetRotation) End.rotation = Target.rotation * Quaternion.Euler(OffsetEndRotation.x, OffsetEndRotation.y, OffsetEndRotation.z);
        //Debug.Log("Distance is: "+(End.position - Target.position).magnitude );
        if ((End.position-Target.position).magnitude >.01)
        {
            CantReach = true;
        }
        else
        {
            CantReach = false;
        }
    }
    float CosAngle(float a, float b, float c) {
        if ( !float.IsNaN(Mathf.Acos((-(c * c) + (a * a) + (b * b)) / (-2 * a * b)) * Mathf.Rad2Deg))
        {
            return Mathf.Acos((-(c * c) + (a * a) + (b * b)) / (2 * a * b)) * Mathf.Rad2Deg;
        }
        else
        {
            return 1;
        }
    }
}
