using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Player : MonoBehaviour
{
    private Vector2 trackpad;
    private Vector3 moveDirection;
    private CapsuleCollider CapCollider;
    private Rigidbody RBody;

    public SteamVR_Input_Sources MovementHand;//Set Hand To Get Input From
    public SteamVR_Action_Vector2 TrackpadAction;
    public SteamVR_Action_Boolean JumpAction;
    public float jumpHeight;
    public float MovementSpeed;
    public float Deadzone;//the Deadzone of the trackpad. used to prevent unwanted walking.
    public GameObject Head;
    public GameObject AxisHand;//Hand Controller GameObject
    public PhysicMaterial NoFrictionMaterial;
    public PhysicMaterial FrictionMaterial;
    public bool TouchingGround;

    private void Start()
    {
        CapCollider = GetComponent<CapsuleCollider>();
        RBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Rigidbody RBody = GetComponent<Rigidbody>();
        moveDirection = Quaternion.AngleAxis(Angle(trackpad) + AxisHand.transform.localRotation.eulerAngles.y, Vector3.up) * Vector3.forward* trackpad.magnitude;//get the angle of the touch and correct it for the rotation of the controller
        updateInput();
        updateCollider();
        CheckGround();
        if (trackpad.magnitude > Deadzone)
        {//make sure the touch isn't in the deadzone and we aren't going to fast.


            CapCollider.material = NoFrictionMaterial;
            if (TouchingGround) {
                if (JumpAction.GetStateDown(MovementHand))
                {

                    float jumpSpeed = Mathf.Sqrt(2 * jumpHeight * 9.81f);
                    RBody.AddForce(0, jumpSpeed, 0, ForceMode.VelocityChange);
                    //RBody.AddForce(moveDirection.x * MovementSpeed - RBody.velocity.x, 0, moveDirection.z * MovementSpeed - RBody.velocity.z, ForceMode.VelocityChange);
                }
                RBody.AddForce(moveDirection.x * MovementSpeed-RBody.velocity.x, 0, moveDirection.z * MovementSpeed-RBody.velocity.z, ForceMode.VelocityChange);

                Debug.Log("Velocity" + moveDirection);
                Debug.Log("Movement Direction:" + moveDirection);
            }
            else
            {
                Debug.Log((moveDirection.x * MovementSpeed / (Mathf.Sqrt(2 * jumpHeight * 9.81f) / (9.81f))*Time.fixedDeltaTime, 0, moveDirection.z * MovementSpeed / (Mathf.Sqrt(2 * jumpHeight * 9.81f) / (9.81f)) * Time.fixedDeltaTime));
                RBody.AddForce(moveDirection.x*MovementSpeed/( Mathf.Sqrt(2 * jumpHeight * 9.81f)/(9.81f)) * Time.fixedDeltaTime, 0, moveDirection.z *MovementSpeed/ (Mathf.Sqrt(2 * jumpHeight * 9.81f) / (9.81f)) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
        else
        {
            CapCollider.material = FrictionMaterial;
        }
    }
    public void CheckGround()
    {
        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        RaycastHit Hit;
        if (Physics.Raycast(transform.position + Vector3.up* Head.transform.localPosition.y, Vector3.down, out Hit, Head.transform.localPosition.y+.2f, layerMask))
        {
            TouchingGround = true;
        }
        else
        {
            TouchingGround = false;
        }
    }
    public static float Angle(Vector2 p_vector2)
    {
        
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }
    private void updateCollider()
    {
        CapCollider.height = Head.transform.localPosition.y;
        CapCollider.center = new Vector3(Head.transform.localPosition.x, Head.transform.localPosition.y / 2, Head.transform.localPosition.z);
    }
    private void updateInput()
    {
        if(TrackpadAction.GetActive(MovementHand)) trackpad = TrackpadAction.GetAxis(MovementHand);
    }
    
}
