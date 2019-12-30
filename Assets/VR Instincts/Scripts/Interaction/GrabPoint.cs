using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GrabPoint : MonoBehaviour
{
    public bool RestrictByRotation;
    public float RotationLimit;
    public bool HelperGrip;
    public Vector3 Offset;
    public Quaternion RotationOffset;

    public bool Gripped;
    public Interactable ParentInteractable;
    private void Awake()
    {
        
        if (!ParentInteractable && transform.parent.GetComponent<Interactable>())
        {
            ParentInteractable = transform.parent.GetComponent<Interactable>();
        }
        UpdateOffset();

    }
    public void UpdateOffset()
    {
        Offset = Quaternion.Inverse(ParentInteractable.transform.rotation) * (-ParentInteractable.transform.position + transform.position);
        RotationOffset = Quaternion.Inverse(ParentInteractable.transform.rotation) * transform.rotation;
    }

}
