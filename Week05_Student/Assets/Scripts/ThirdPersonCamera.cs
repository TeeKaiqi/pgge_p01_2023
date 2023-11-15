using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static Vector3 CameraAngleOffset { get; set; }
    public static Vector3 CameraPositionOffset { get; set; }
    public static float Damping { get; set; }
    public static float RotationSpeed { get; set; }
    public static float MinPitch { get; set; }
    public static float MaxPitch { get; set; }


}

// The base class for all third-person camera controllers
public abstract class TPCBase
{
    protected Transform mCameraTransform;
    protected Transform mPlayerTransform;

    public Transform CameraTransform
    {
        get
        {
            return mCameraTransform;
        }
    }
    public Transform PlayerTransform
    {
        get
        {
            return mPlayerTransform;
        }
    }

    public TPCBase(Transform cameraTransform, Transform playerTransform)
    {
        mCameraTransform = cameraTransform;
        mPlayerTransform = playerTransform;
    }

    public abstract void Update();

}

public class TPCTrack : TPCBase
{
    public TPCTrack(Transform cameraTransform, Transform playerTransform)
        : base(cameraTransform, playerTransform)
    {
    }

    public override void Update()
    {
        Vector3 targetPos = mPlayerTransform.position;
        targetPos.y += GameConstants.CameraPositionOffset.y;
        mCameraTransform.LookAt(targetPos);
    }
}
public abstract class TPCFollow : TPCBase
{
    public TPCFollow(Transform cameraTransform, Transform playerTransform)
        : base(cameraTransform, playerTransform)
    {
    }

    public override void Update()
    {
        // Now we calculate the camera transformed axes.
        // We do this because our camera's rotation might have changed
        // in the derived class Update implementations. Calculate the new 
        // forward, up and right vectors for the camera.
        Vector3 forward = mCameraTransform.rotation * Vector3.forward;
        Vector3 right = mCameraTransform.rotation * Vector3.right;
        Vector3 up = mCameraTransform.rotation * Vector3.up;

        // We then calculate the offset in the camera's coordinate frame. 
        // For this we first calculate the targetPos
        Vector3 targetPos = mPlayerTransform.position;

        // Add the camera offset to the target position.
        // Note that we cannot just add the offset.
        // You will need to take care of the direction as well.
        Vector3 desiredPosition = targetPos
            + forward * GameConstants.CameraPositionOffset.z
            + right * GameConstants.CameraPositionOffset.x
            + up * GameConstants.CameraPositionOffset.y;

        // Finally, we change the position of the camera, 
        // not directly, but by applying Lerp.
        Vector3 position = Vector3.Lerp(mCameraTransform.position,
            desiredPosition, Time.deltaTime * GameConstants.Damping);
        mCameraTransform.position = position;
    }
}
public class TPCFollowTrackPosition : TPCFollow
{
    public TPCFollowTrackPosition(Transform cameraTransform, Transform playerTransform)
        : base(cameraTransform, playerTransform)
    {
    }

    public override void Update()
    {
        // Create the initial rotation quaternion based on the 
        // camera angle offset.
        Quaternion initialRotation =
           Quaternion.Euler(GameConstants.CameraAngleOffset);

        // Now rotate the camera to the above initial rotation offset.
        // We do it using damping/Lerp
        // You can change the damping to see the effect.
        mCameraTransform.rotation =
            Quaternion.RotateTowards(mCameraTransform.rotation,
                initialRotation,
                Time.deltaTime * GameConstants.Damping);

        // We now call the base class Update method to take care of the
        // position tracking.
        base.Update();
    }
}
public class TPCFollowTrackPositionAndRotation : TPCFollow
{
    public TPCFollowTrackPositionAndRotation(Transform cameraTransform, Transform playerTransform)
        : base(cameraTransform, playerTransform)
    {
    }

    public override void Update()
    {
        // We apply the initial rotation to the camera.
        Quaternion initialRotation =
            Quaternion.Euler(GameConstants.CameraAngleOffset);

        // Allow rotation tracking of the player
        // so that our camera rotates when the Player rotates and at the same
        // time maintain the initial rotation offset.
        mCameraTransform.rotation = Quaternion.Lerp(
            mCameraTransform.rotation,
            mPlayerTransform.rotation * initialRotation,
            Time.deltaTime * GameConstants.Damping);

        base.Update();
    }
}
public class TPCTopDown : TPCBase
{
    public TPCTopDown(Transform cameraTransform, Transform playerTransform)
        : base(cameraTransform, playerTransform)
    {
    }

    public override void Update()
    {
        // For topdown camera we do not use the x and z offsets.
        Vector3 targetPos = mPlayerTransform.position;
        targetPos.y += GameConstants.CameraPositionOffset.y;
        Vector3 position = Vector3.Lerp(mCameraTransform.position, targetPos, Time.deltaTime * GameConstants.Damping);
        mCameraTransform.position = position;
        mCameraTransform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }
}
public class TPCFollowIndependentRotation : TPCBase
{
    FixedTouchField mTouchField;
    private float angleX = 0.0f;
    public TPCFollowIndependentRotation(Transform cameraTransform, Transform playerTransform)
        : base(cameraTransform, playerTransform)
    {
    }

#if UNITY_ANDROID
    public TPCFollowIndependentRotation(Transform cameraTransform, Transform playerTransform, FixedTouchField fixedTouch)
        : base(cameraTransform, playerTransform)
    {
        mTouchField = fixedTouch;
    }
#endif

    public override void Update()
    {
        //implement the Update for this camera controls    public override void Update()
#if UNITY_STANDALONE
        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");
#endif
#if UNITY_ANDROID
        float mx, my;
        mx = mTouchField.TouchDist.x * Time.deltaTime;
        my = mTouchField.TouchDist.y * Time.deltaTime;
#endif

        // We apply the initial rotation to the camera.
        Quaternion initialRotation = Quaternion.Euler(GameConstants.CameraAngleOffset);

        Vector3 eu = mCameraTransform.rotation.eulerAngles;

        angleX -= my * GameConstants.RotationSpeed;

        // We clamp the angle along the Xaxis to be between the min and max pitch.
        angleX = Mathf.Clamp(angleX, GameConstants.MinPitch, GameConstants.MaxPitch);

        eu.y += mx * GameConstants.RotationSpeed;
        Quaternion newRot = Quaternion.Euler(angleX, eu.y, 0.0f) * initialRotation;

        mCameraTransform.rotation = newRot;

        Vector3 forward = mCameraTransform.rotation * Vector3.forward;
        Vector3 right = mCameraTransform.rotation * Vector3.right;
        Vector3 up = mCameraTransform.rotation * Vector3.up;

        Vector3 targetPos = mPlayerTransform.position;
        Vector3 desiredPosition = targetPos
            + forward * GameConstants.CameraPositionOffset.z
            + right * GameConstants.CameraPositionOffset.x
            + up * GameConstants.CameraPositionOffset.y;

        Vector3 position = Vector3.Lerp(mCameraTransform.position,
            desiredPosition,
            Time.deltaTime * GameConstants.Damping);

        mCameraTransform.position = position;
    }
}


public enum CameraType
{
    Track,
    Follow_Track_Pos,
    Follow_Track_Pos_Rot,
    Topdown,
    Follow_Independent,
}


public class ThirdPersonCamera : MonoBehaviour
{
    public Transform mPlayer;

    TPCBase mThirdPersonCamera;
    // Get from Unity Editor.
    public Vector3 mPositionOffset = new Vector3(0.0f, 2.0f, -2.5f);
    public Vector3 mAngleOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [Tooltip("The damping factor to smooth the changes in position and rotation of the camera.")]
    public float mDamping = 1.0f;

    public float mMinPitch = -30.0f;
    public float mMaxPitch = 30.0f;
    public float mRotationSpeed = 50.0f;
    public FixedTouchField mTouchField;

    public CameraType mCameraType = CameraType.Follow_Track_Pos;
    Dictionary<CameraType, TPCBase> mThirdPersonCameraDict = new Dictionary<CameraType, TPCBase>();

    void Start()
    {
        // Set to GameConstants class so that other objects can use.
        GameConstants.Damping = mDamping;
        GameConstants.CameraPositionOffset = mPositionOffset;
        GameConstants.CameraAngleOffset = mAngleOffset;
        GameConstants.MinPitch = mMinPitch;
        GameConstants.MaxPitch = mMaxPitch;
        GameConstants.RotationSpeed = mRotationSpeed;


        //mThirdPersonCamera = new TPCTrack(transform, mPlayer);
        //mThirdPersonCamera = new TPCFollowTrackPosition(transform, mPlayer);
        //mThirdPersonCamera = new TPCFollowTrackPositionAndRotation(transform, mPlayer);
        //mThirdPersonCamera = new TPCTopDown(transform, mPlayer);

        mThirdPersonCameraDict.Add(CameraType.Track, new TPCTrack(transform, mPlayer));
        mThirdPersonCameraDict.Add(CameraType.Follow_Track_Pos, new TPCFollowTrackPosition(transform, mPlayer));
        mThirdPersonCameraDict.Add(CameraType.Follow_Track_Pos_Rot, new TPCFollowTrackPositionAndRotation(transform, mPlayer));
        mThirdPersonCameraDict.Add(CameraType.Topdown, new TPCTopDown(transform, mPlayer));


        // We instantiate and add the new third-person camera to the dictionary
#if UNITY_STANDALONE
        mThirdPersonCameraDict.Add(CameraType.Follow_Independent, new TPCFollowIndependentRotation(transform, mPlayer));
#endif
#if UNITY_ANDROID
        mThirdPersonCameraDict.Add(CameraType.Follow_Independent, new TPCFollowIndependentRotation(transform, mPlayer, mTouchField));
#endif

        mThirdPersonCamera = mThirdPersonCameraDict[mCameraType];

    }

    private void Update()
    {
        // Update the game constant parameters every frame 
        // so that changes applied on the editor can be reflected
        GameConstants.Damping = mDamping;
        GameConstants.CameraPositionOffset = mPositionOffset;
        GameConstants.CameraAngleOffset = mAngleOffset;
        GameConstants.MinPitch = mMinPitch;
        GameConstants.MaxPitch = mMaxPitch;
        GameConstants.RotationSpeed = mRotationSpeed;

        mThirdPersonCamera = mThirdPersonCameraDict[mCameraType];
    }

    void LateUpdate()
    {
        mThirdPersonCamera.Update();
    }
}
