using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMovment : MonoBehaviour
{
    public float StrideLength;
    public float StepHeight;
    public Transform LeftFoot;
    public Transform RightFoot;

    public float HeightMultiplyer;

    public float StridePosition;
    private Vector3 FootOffsetL;
    private Vector3 FootOffsetR;
    private float Length;
    // Start is called before the first frame update
    void Start()
    {
        FootOffsetL = LeftFoot.localPosition;
        FootOffsetR = RightFoot.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        PositionFoot(LeftFoot, FootOffsetL, false);
        PositionFoot(RightFoot, FootOffsetR, true);

        UpdateHeight();

    }
    void PositionFoot(Transform Foot, Vector3 FootOffset, bool side)
    {

        float height;
        if (side)
        {
            height = Mathf.Sin(StridePosition * 2 * Mathf.PI) * StepHeight;

            if (fixStride(1 - StridePosition) < .5f) Length = (StrideLength / 2) - (StridePosition * 2) * StrideLength + (StrideLength / 2) + StrideLength / 2;
            else Length = ((StridePosition - .5f) * 2) * StrideLength + (StrideLength / 2);
            if (height < 0) height = 0;
        }
        else
        {
            height = -Mathf.Sin(StridePosition * 2 * Mathf.PI) * StepHeight;
            if (fixStride(StridePosition) > .5f) Length = ((StridePosition - .5f) * 2) * StrideLength - (StrideLength / 2);
            else Length = -(StridePosition * 2) * StrideLength + StrideLength / 2;
            if (height < 0) height = 0;
        }
        Foot.localPosition = FootOffset + new Vector3(0, height*HeightMultiplyer, Length);
    }
    float fixStride(float stride)
    {
        if (StridePosition > 1)
        {
            return StridePosition -= 1;
        }
        else if (StridePosition < 0)
        {
            return StridePosition += 1;
        }
        else
        {
            return stride;
        }
    }
    public void WalkM(float Meeters)
    {
        StridePosition += Meeters / (StrideLength * 2);
        UpdateHeight();
        while (true)
        {
            if (StridePosition > 1)
            {
                StridePosition--;
            }
            else if (StridePosition < 0)
            {
                StridePosition++;
            }
            else
            {
                break;
            }
        }
    }
    private void UpdateHeight()
    {
        if (HeightMultiplyer > 1)
        {
            HeightMultiplyer = 1;
        }
        else if(HeightMultiplyer < 0)
        {
            HeightMultiplyer = 0;
        }
    }
}