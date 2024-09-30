using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCrateController : MonoBehaviour
{
    //Indicates whether crate is on a destination space
    public bool isBoxOnDestination = false;

    //--------------------------------------
    //When box is pushed onto destination
    void OnTriggerStay(Collider Other)
    {
        if (Other.CompareTag("End"))
            isBoxOnDestination = true;
        //AudioManager.instance.PlayMagicRingClip();
    }
    //--------------------------------------
    void OnTriggerExit(Collider Other)
    {
        if (Other.CompareTag("End"))
            isBoxOnDestination = false;
    }
    //--------------------------------------
}
