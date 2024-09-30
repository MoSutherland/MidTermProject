using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Time in seconds to move")]
    public float moveTime = 1.0f;

    [Header("Movement Speed (Units per Second)")]
    public float moveDistance = 2.0f;

    [Header("Player Transform")]
    [SerializeField] private Transform playerTransform;

    [Header("Rotate Time (In Seconds)")]
    public float rotationTime = 1.0f;

    [Header("Rotate Increments (total amount of rotation in one turn)")]
    public float rotationIncrement = 90.0f;



    //enum for movement
    public enum movementType { IDLE = 0, WALK = 1, PUSH = 2, CHEER = 3 };

    //enum for space state
    public enum gameSpaceState { EMPTY = 0, CRATE = 1, COLLIDER = 2 };

    //PLAYER STATE
    private movementType moveState = movementType.IDLE;

    [Header("Reference to Animator Component")]
    [SerializeField] private Animator playerAnimator;

    //Level Colliders
    [SerializeField] private Collider[] Colliders;

    [Header("Reference to last tested crate")]
    [SerializeField] private Transform lastBox;



    //----------------------------------------------------------------

    // ANIMATION STATE PROPERTY
    public movementType PlayerState
    {
        get { 
            return moveState; 
        }

        set
        {
            moveState = value;

            //Set animation state
           
            playerAnimator.SetInteger("intState", (int)moveState);
        }
    }


    //----------------------------------------------------------------
    //----------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;

        playerAnimator = GetComponent<Animator>();

        //Get all colliders in scene
        Colliders = Object.FindObjectsOfType<Collider>();

        //Set starting state to idle
        PlayerState = movementType.IDLE;

        //Start input handling loop for player controller
        StartCoroutine(UserPlayerInput());





    }

    //----------------------------------------------------------------
    //----------------------------------------------------------------

    //Called each frame to handle input
    public IEnumerator UserPlayerInput()
    {
        /*
        //Loop forever, reading player input
        while (true)
        {
            if (Mathf.CeilToInt(Input.GetAxis("Vertical Keyboard")) > 0)
            {
                //Set walk

                PlayerState = movementType.WALK;
                Debug.Log("playerstate: " + PlayerState);

                //Move player 1 increment
                yield return StartCoroutine(Move(moveDistance));
            }

            //Set to idle
            PlayerState = movementType.IDLE; //Set to idle state - default

            yield return null;
        }
        */

        //Loop forever, reading player input
        while (true)
        {
            if (Mathf.CeilToInt(Input.GetAxis("Vertical Keyboard")) > 0)
            {
                //Validate movement -should we remain idle, walk or push?
                PlayerState = ValidateWalk();


                Debug.Log("playerstate: " + PlayerState);

                //If we are not idle, then move
                if (PlayerState != movementType.IDLE)
                {
                    //Move player 1 increment
                    yield return StartCoroutine(Move(moveDistance));
                }
            }
            else
                PlayerState = movementType.IDLE; //Set to idle state - default


            if (Input.GetAxis("Horizontal Keyboard") < 0.0f)
                yield return StartCoroutine(Rotate(-rotationIncrement));

            if (Input.GetAxis("Horizontal Keyboard") > 0.0f)
                yield return StartCoroutine(Rotate(rotationIncrement));

            yield return null;
        }

    }


    //----------------------------------------------------------------
    //Moves root transform one increment (2 units)
    /*
    public IEnumerator Move(float moveDistanceParam)////old

    {
        //Start position
        Vector3 startPosition = playerTransform.position;

        //Dest Position
        Vector3 destinationPosition = playerTransform.position + playerTransform.forward * moveDistanceParam;

        //Elapsed Time
        float elapsedTime = 0.0f;

        while (elapsedTime < moveTime)
        {
            //Calculate interpolated angle
            Vector3 finalPosition = Vector3.Lerp(startPosition, destinationPosition, elapsedTime / moveTime);

            //Updates position
            playerTransform.position = finalPosition;


            //Wait until next frame
            yield return null;

            //Update time
            elapsedTime += Time.deltaTime;
        }

        //Complete move
        playerTransform.position = destinationPosition;

        yield break;
    }
    */
    public IEnumerator Move(float moveDistanceParam)
    {
        //Start position
        Vector3 StartPos = playerTransform.position;

        //Dest Position
        Vector3 DestPos = playerTransform.position + playerTransform.forward * moveDistanceParam;

        //Elapsed Time
        float ElapsedTime = 0.0f;

        while (ElapsedTime < moveTime)
        {
            //Calculate interpolated angle
            Vector3 FinalPos = Vector3.Lerp(StartPos, DestPos, ElapsedTime / moveTime);

            //Update pos
            playerTransform.position = FinalPos;

            //If we are pushing then update box pos
            if (PlayerState == movementType.PUSH)
                lastBox.position = new Vector3(playerTransform.position.x, lastBox.position.y, playerTransform.position.z) + playerTransform.forward * moveDistanceParam;

            //Wait until next frame
            yield return null;

            //Update time
            ElapsedTime += Time.deltaTime;
        }

        //Complete move
        playerTransform.position = DestPos;

        if (PlayerState == movementType.PUSH)
            lastBox.position = new Vector3(playerTransform.position.x, lastBox.position.y, playerTransform.position.z) + playerTransform.forward * moveDistanceParam;

        yield break;
    }




    //----------------------------------------------------------------

    //Rotates root transform one increment (90 degrees)
    public IEnumerator Rotate(float rotationIncrementParam)
    {
        //Get Original Y Rotation
        float startRotation = playerTransform.rotation.eulerAngles.y;

        //Get Destination Rot
        float destinationRotation = startRotation + rotationIncrementParam;

        //Elapsed Time
        float elapsedTime = 0.0f;

        while (elapsedTime < rotationTime)
        {
            //Calculate interpolated angle
            float Angle = Mathf.LerpAngle(startRotation, destinationRotation, elapsedTime / rotationTime);

            playerTransform.eulerAngles = new Vector3(0, Angle, 0);

            //Wait until next frame
            yield return null;

            //Update time
            elapsedTime += Time.deltaTime;
        }

        //Final rotation
        playerTransform.eulerAngles = new Vector3(0, Mathf.FloorToInt(destinationRotation), 0);
    }
    //----------------------------------------------------------------

    //Tests a point in the scene to determine whether it intersects a collider
    public gameSpaceState PointState(Vector3 Point)
    {
        //Cycle through colliders and test for collision
        foreach (Collider C in Colliders)
        {
            //Point intersects a collider - determine type
            if (C.bounds.Contains(Point) && !C.gameObject.CompareTag("End"))
            {
                Debug.Log("collider tag: " + C.gameObject.tag);

                if (C.gameObject.CompareTag("Crate"))
                {
                    //Get reference to crate
                    lastBox = C.gameObject.transform;

                    return gameSpaceState.CRATE; //Point is in crate
                }
                else
                    return gameSpaceState.COLLIDER; //Else point is in collider
            }
        }

        //Point not in collider - space is empty
        return gameSpaceState.EMPTY;
    }

    //----------------------------------------------------------------



    //Based on surrounding objects, determine move type allowed in direction
    public movementType ValidateWalk()
    {
        //Update next box
        lastBox = null;

        //Get destination point in next tile
        Vector3 DestinationPosition = playerTransform.position + playerTransform.forward * moveDistance;

        //Get double destination point (two tiles away). For checking move destination if box is on next tile
        Vector3 dblDestinationPosition = playerTransform.position + playerTransform.forward * moveDistance * 2.0f;

        //Status of next space
        gameSpaceState nextStatus = PointState(DestinationPosition);
        Debug.Log("nextSatus: " + nextStatus);

        //Get last tested box
        Transform nextBox = lastBox;

        //Status of two spaces away
        gameSpaceState dblSpaceStatus = PointState(dblDestinationPosition);
        Debug.Log("gameSpaceSatus: " + dblSpaceStatus);

        //Update last box
        lastBox = nextBox;

        //If next space is empty then walk
        if (nextStatus == gameSpaceState.EMPTY) 
            return movementType.WALK;

        //If next space has crate and two spaces is empty, then push
        if (nextStatus == gameSpaceState.CRATE && dblSpaceStatus == gameSpaceState.EMPTY) 
            return movementType.PUSH;

        //Else cannot move
        return movementType.IDLE;
    }
    //--------------------------------------



}
