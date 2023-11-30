using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class Flying : UdonSharpBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    GameObject flyPlaneRightB, flyPlaneLeftB, flyPlaneRightW, flyPlaneLeftW;

    [SerializeField]
    Vector3 gravity;

    [SerializeField]
    float velocityClamp = 1f;
    float velocityClampSquared;

    Vector3 velocity;

    [SerializeField]
    float convertionRatioPlane = 0.4f;

    [SerializeField, Range(0, 1)]
    float convertionRatioTurning = 0.5f;

    [SerializeField, Range(0, 1)]
    float AirfrictionLoss = 0.3f;

    [SerializeField]
    float flappingScaleF = 1f, flappingScaleU = 1f, flappingScaleR = 0.5f;

    [SerializeField]
    int interactLayer = 0; //default

    [SerializeField]
    VRCStation flySeat;

    [SerializeField]
    float playerVelocityScale = 0.1f;

    float oldFlyPlaneRightLocalPosition, oldFlyPlaneLeftLocalPosition;

    bool isFlying;

    VRCPlayerApi localPlayer;

    [SerializeField]
    bool testing = false;

    Vector3 ChairRotation;

    [SerializeField]
    GameObject directionIndicator;

    float delayTime;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        velocityClampSquared = velocityClamp * velocityClamp;
    }
    void Update()
    {
        if (player.OwnerPlayer == null) return;
        if (player.OwnerPlayer != Networking.LocalPlayer) return; //only for local player
        KeepStationValuesSet();
        keepPositionOfFlapsOnHands();
        if (isFlying)
        {
            if (delayTime > 0f)
            {
                delayTime -= Time.deltaTime;
                velocity = Vector3.zero;
            }
            directionIndicator.transform.position = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            CalculateGravity();
            CalculateFlaping();
            CalculatePlane();
            ClampVelocity();
            CalculateTurning();
            AirFriction();
            transform.position += velocity;
            StopFlyingIfColliders();
        }
        else
        {
            delayTime = 0.3f;
            transform.position = localPlayer.GetBonePosition(HumanBodyBones.Hips);
            ChairRotation = Vector3.zero;
            transform.rotation = Quaternion.Euler(ChairRotation);
            PlayerFlyIfJump();
        }
       
    }

    private void AirFriction()
    {
        velocity = velocity * (1 - AirfrictionLoss);
    }

    private void KeepStationValuesSet() //can delete this function when vrc fixes bugs
    {
        flySeat.disableStationExit = true;
        flySeat.canUseStationFromStation = false;
        flySeat.PlayerMobility = VRCStation.Mobility.ImmobilizeForVehicle;
    }

    private void keepPositionOfFlapsOnHands()
    {  
        flyPlaneRightB.transform.position = localPlayer.GetBonePosition(HumanBodyBones.LeftHand);
        flyPlaneRightB.transform.rotation = localPlayer.GetBoneRotation(HumanBodyBones.LeftHand);
        flyPlaneLeftB.transform.position = localPlayer.GetBonePosition(HumanBodyBones.RightHand);
        flyPlaneLeftB.transform.rotation = localPlayer.GetBoneRotation(HumanBodyBones.RightHand);
    }

    private void StopFlyingIfColliders()
    {
        if (IsCollisionsInArea(0.5f))
        {
            StopFlaying();
        }
    }

    private void StopFlaying()
    {
        isFlying = false;
        flySeat.disableStationExit = false;
        flySeat.ExitStation(localPlayer);
        canfly = false;
    }

    bool canfly;

    private void PlayerFlyIfJump()
    {
        if (!localPlayer.IsPlayerGrounded())
        {
            if (canfly)
            {
                if (!IsCollisionsInArea(3.5f))
                {
                    StartFlying();
                }
            }
            
        }
        else
        {
            canfly = true;
        }
    }

    public void StartFlying()
    {
        transform.position = localPlayer.GetBonePosition(HumanBodyBones.Hips);
        ChairRotation = localPlayer.GetRotation().eulerAngles;
        ChairRotation.x = ChairRotation.z = 0f;
        transform.rotation = Quaternion.Euler(ChairRotation);

        Vector3 velocitytmp = localPlayer.GetVelocity();
        velocity = new Vector3(velocitytmp.x, 0f, velocitytmp.z) * playerVelocityScale;
        flySeat.UseStation(localPlayer);
        isFlying = true;

    }

    private bool IsCollisionsInArea(float checkDistance) //if any colliders in an area
    {
        Collider[] hitColliders = Physics.OverlapSphere(localPlayer.GetPosition(), checkDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null)
            {
                if (hitCollider.gameObject.layer == interactLayer)
                {
                    return true;
                }
            }
        }
        return false;
    }

    [SerializeField]
    float anglaPanScale = 1f, angleRollScale = 1f, angleUnrollScale = 1f; 

    private void CalculateTurning()
    {
        Vector3 fraction = velocity * convertionRatioTurning * Time.deltaTime;
        velocity -= fraction;
        
        float opposite = flyPlaneLeftB.transform.localPosition.y - flyPlaneRightB.transform.localPosition.y;
        float adjacent = Vector3.Distance(flyPlaneRightB.transform.position, flyPlaneLeftB.transform.position);
        float angle = Mathf.Atan2(opposite, adjacent);

        float anglePerSec = angle / Time.deltaTime;
        if (anglePerSec > 5 || anglePerSec < -5)
        {
            ChairRotation.x = 0f;
            ChairRotation.y -= angle * anglaPanScale;
            ChairRotation.z += angle * angleRollScale; //rolls which seems cool
            transform.rotation = Quaternion.Euler(ChairRotation);
        }
        else
        {
            if (ChairRotation.z > 0)
            {
                ChairRotation.x = 0f;
                ChairRotation.z -= Time.deltaTime * angleUnrollScale;
                transform.rotation = Quaternion.Euler(ChairRotation);
            }
            else
            {
                ChairRotation.x = 0f;
                ChairRotation.z += Time.deltaTime * angleUnrollScale;
                transform.rotation = Quaternion.Euler(ChairRotation);
            }
           
        }

        velocity += fraction.magnitude * transform.forward;
    }

    private void CalculateFlaping()
    {
        float positionR = flyPlaneRightB.transform.localPosition.y;
        float positionL = flyPlaneLeftB.transform.localPosition.y;

        if (positionR < oldFlyPlaneRightLocalPosition)
        {
            velocity -= (positionR - oldFlyPlaneRightLocalPosition) * (transform.forward * flappingScaleF + transform.up * flappingScaleU + transform.right * flappingScaleR) * Time.deltaTime;
        }
        if (positionL < oldFlyPlaneLeftLocalPosition)
        {
            velocity -= (positionL - oldFlyPlaneLeftLocalPosition) * (transform.forward * flappingScaleF + transform.up * flappingScaleU - transform.right * flappingScaleR) * Time.deltaTime;
        }
        oldFlyPlaneLeftLocalPosition = positionL;
        oldFlyPlaneRightLocalPosition = positionR;
    }

    private void CalculatePlane()
    {

        Debug.DrawLine(flyPlaneRightW.transform.position, flyPlaneRightW.transform.position + velocity, Color.blue);
        Debug.DrawLine(flyPlaneLeftW.transform.position, flyPlaneLeftW.transform.position + velocity, Color.blue);

        Debug.DrawLine(flyPlaneRightW.transform.position, flyPlaneRightW.transform.position + flyPlaneRightW.transform.up, Color.green);
        Debug.DrawLine(flyPlaneLeftW.transform.position, flyPlaneLeftW.transform.position + flyPlaneLeftW.transform.up, Color.green);

        Vector3 reflectionRight = Vector3.Reflect(velocity, flyPlaneRightW.transform.up);
        Vector3 reflectionLeft = Vector3.Reflect(velocity, flyPlaneLeftW.transform.up);

        velocity -= velocity * convertionRatioPlane * Time.deltaTime;

        Debug.DrawLine(flyPlaneRightW.transform.position, flyPlaneRightW.transform.position + reflectionLeft, Color.cyan);
        Debug.DrawLine(flyPlaneLeftW.transform.position, flyPlaneLeftW.transform.position + reflectionLeft, Color.cyan);

        reflectionRight = reflectionRight * convertionRatioPlane * 0.5f * Time.deltaTime;

        reflectionLeft = reflectionLeft * convertionRatioPlane * 0.5f * Time.deltaTime;

        velocity += reflectionRight + reflectionLeft;
    }

    private void ClampVelocity()
    {
        if (velocity.sqrMagnitude > velocityClampSquared)
        {
            velocity = Vector3.Normalize(velocity) * velocityClamp;
        }
    }

    private void CalculateGravity()
    {
        velocity += gravity * Time.deltaTime;
    }

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        isFlying = false;
    }
}


//Physics.OverlapSphere
//Collider.ClosestPoint
/*
  [SerializeField]
    GameObject flyPlaneLeft, flyPlaneRight;

    [SerializeField]
    GameObject playerObject;

    Rigidbody rb;

    [SerializeField]
    Vector3 gravity;

    [SerializeField]
    float velocityClamp = 1f;
    float velocityClampSquared;

    [SerializeField]
    Vector3 startVelocity;

    Vector3 velocity;

    [SerializeField, Range(0, 1)]
    float convertionRatioPlane = 0.4f;

    [SerializeField, Range(0, 1)]
    float convertionRatioTurning = 0.5f;

    [SerializeField]
    float liftScale = 1f;



    Vector3 oldFlyPlaneLeftLocalPosition, oldFlyPlaneRightLocalPosition;

    void Start()
    {
        rb = playerObject.GetComponent<Rigidbody>();
        velocity = startVelocity;

        velocityClampSquared = velocityClamp * velocityClamp;
    }
    void Update()
    {
        CalculateGravity();
        CalculateFlaping();
        CalculatePlane();
        ClampVelocity();
        CalculateTurning();
        //playerObject.transform.position += velocity;
        rb.AddForce(velocity, ForceMode.VelocityChange);

    }

    private void CalculateTurning()
    {
        Vector3 fraction = velocity * convertionRatioTurning * Time.deltaTime;
        velocity -= fraction;

        float opposite = flyPlaneRight.transform.localPosition.y - flyPlaneLeft.transform.localPosition.y;
        float adjacent = Vector3.Distance(flyPlaneLeft.transform.position, flyPlaneRight.transform.position);
        float angle = Mathf.Atan2(opposite, adjacent);

        Vector3 EulerlocalRotation = transform.localRotation.eulerAngles;
        Vector3 newRotation = EulerlocalRotation + new Vector3(0f, angle, angle);
        Quaternion rotation = Quaternion.Euler(newRotation);
        rb.MoveRotation(rotation);
        //transform.Rotate(0f, angle, angle, Space.Self); //rolls which seems cool
        //transform.Rotate(0f, angle, 0f, Space.Self);

        velocity += fraction.magnitude * transform.forward;
    }

    private void CalculateFlaping()
    {
        Vector3 upliftLeft = flyPlaneLeft.transform.localPosition - oldFlyPlaneLeftLocalPosition; //movement relative to local
        Vector3 upliftRight = flyPlaneRight.transform.localPosition - oldFlyPlaneRightLocalPosition;
        if (upliftLeft.y < 0) //only move up if plane moves down
        {
            velocity -= upliftLeft.y * transform.forward * liftScale;
        }

        if (upliftRight.y < 0) //only move up if plane moves down
        {
            velocity -= upliftRight.y * transform.forward * liftScale;
        }

        oldFlyPlaneLeftLocalPosition = flyPlaneLeft.transform.localPosition;
        oldFlyPlaneRightLocalPosition = flyPlaneRight.transform.localPosition;
    }

    private void CalculatePlane()
    {
        Vector3 planeNormalLeft = flyPlaneLeft.transform.TransformVector(flyPlaneLeft.transform.up);
        Vector3 reflectionLeft = Vector3.Reflect(velocity, planeNormalLeft);

        Vector3 planeNormalRight = flyPlaneRight.transform.TransformVector(flyPlaneRight.transform.up);
        Vector3 reflectionRight = Vector3.Reflect(velocity, planeNormalRight);

        velocity -= velocity * convertionRatioPlane * Time.deltaTime;


        reflectionLeft = reflectionLeft * convertionRatioPlane * 0.5f * Time.deltaTime;

        reflectionRight = reflectionRight * convertionRatioPlane * 0.5f * Time.deltaTime;

        velocity += reflectionLeft + reflectionRight;
    }

    private void ClampVelocity()
    {
        if (velocity.sqrMagnitude > velocityClampSquared)
        {
            velocity = Vector3.Normalize(velocity) * velocityClamp;
        }
    }

    private void CalculateGravity()
    {
        velocity += gravity * Time.deltaTime;
    } 
 */