using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGGE.Patterns;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public FSM mFsm = new FSM();
    public Animator mAnimator;
    public PlayerMovement mPlayerMovement;

    [HideInInspector]
    public bool[] mAttackButtons = new bool[3];

    // This is the maximum number of bullets that the player 
    // needs to fire before reloading.
    public int mMaxAmunitionBeforeReload = 40;

    // This is the total number of bullets that the 
    // player has.
    [HideInInspector]
    public int mAmunitionCount = 100;

    // This is the count of bullets in the magazine.
    [HideInInspector]
    public int mBulletsInMagazine = 40;


    // Start is called before the first frame update
    void Start()
    {
        mFsm.Add(new PlayerState_MOVEMENT(this));
        mFsm.Add(new PlayerState_ATTACK(this));
        mFsm.Add(new PlayerState_RELOAD(this));
        mFsm.SetCurrentState((int)PlayerStateType.MOVEMENT);
    }

    // Update is called once per frame
    void Update()
    {
        mFsm.Update();
        Aim();

        // For Student ----------------------------------------------------------//
        // Implement the logic of button clicks for shooting. 
        //-----------------------------------------------------------------------//
    }

    public void Aim()
    {
        // For Student ----------------------------------------------------------//
        // Implement the logic of aiming and showing the crosshair
        // if there is an intersection.
        //
        // Hints:
        // Find the direction of fire.
        // Find gunpoint as mentioned in the worksheet.
        // Find the layer mask for objects that you want to intersect with.
        //
        // Do the Raycast
        // if (intersected)
        // {
        //     // Draw a line as debug to show the aim of fire in scene view.
        //     // Find the transformed intersected point to screenspace
        //     // and then transform the crosshair position to this
        //     // new position.
        //     // Enable or set active the crosshair gameobject.
        // }
        // else
        // {
        //     // Hide or set inactive the crosshair gameobject.
        // }
        //-----------------------------------------------------------------------//

        // Uncomment the code below and start adding your codes.
        /*
        Vector3 dir = your code here;
        // Find gunpoint as mentioned in the worksheet.
        Vector3 gunpoint = mGunTransform.transform.position +
                           dir * 1.2f -
                           mGunTransform.forward * 0.1f;
        // Fine the layer mask for objects that you want to intersect with.
        LayerMask objectsMask = your code here;

        // Do the Raycast
        RaycastHit hit;
        bool flag = Physics.Raycast(gunpoint, dir,
                        out hit, 50.0f, objectsMask);
        if (flag)
        {
            // Draw a line as debug to show the aim of fire in scene view.
            Debug.DrawLine(gunpoint, gunpoint +
                (dir * hit.distance), Color.red, 0.0f);

            // Find the transformed intersected point to screenspace
            // and then transform the crosshair position to this
            // new position.

            // Enable or set active the crosshair gameobject.
            mCrossHair.gameObject.SetActive(true);
        }
        else
        {
            // Hide or set inactive the crosshair gameobject.
            mCrossHair.gameObject.SetActive(true);
        }
        */
    }

    public void NoAmmo()
    {
    }

    public void Reload()
    {
    }

    public void Fire(int id)
    {
    }
}
