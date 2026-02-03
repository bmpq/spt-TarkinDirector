using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        foreach (var anim in GetComponentsInChildren<Animator>())
        {
            anim.enabled = false;
        }

        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.WakeUp();
        }
    }

    void OnDisable()
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody>(true))
        {
            rb.isKinematic = true;
        }

        foreach (var anim in GetComponentsInChildren<Animator>(true))
        {
            anim.enabled = true;
        }
    }
}
