using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private float itemTraveling = 0.5f;
    private Rigidbody rgInteractable;
    
    [SerializeField]private Transform itemGrabOffSet = null;
    private bool grabed;


    private void Awake()
    {
        grabed = false;
        rgInteractable = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (grabed)
        {
            TarvelToGrabingPoint();
        }
    }

    private void TarvelToGrabingPoint()
    {
        if (itemGrabOffSet!=null)
        {
            transform.position = Vector3.MoveTowards(transform.position,itemGrabOffSet.position,itemTraveling);
        }
    }

    public void InitGrab(Transform _itemGrabOffset)
    {
        itemGrabOffSet = _itemGrabOffset;
        grabed = true;
        rgInteractable.useGravity = false;
    }

    public void StopGrab()
    {
        itemGrabOffSet = null;
        grabed = false;
        rgInteractable.useGravity = true;
    }
}
