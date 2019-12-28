using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class Interactable : MonoBehaviour
{
    public int touchCount;
    public SteamVR_Input_Sources Hand;
    public bool gripped;
    public bool SecondGripped;
    public GameObject GrippedBy;
    void start()
    {
        if (gameObject.tag != "Grabbable") {
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        touchCount++;
    }
    private void OnCollisionExit(Collision collision)
    {
        touchCount--;
    }
}
