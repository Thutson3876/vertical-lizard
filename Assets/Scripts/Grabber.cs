using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    [SerializeField] private float maxGrabDistance = 5f;
    [SerializeField] private LayerMask grabLayers;
    public AudioClip grabSound;
    private ConfigurableJoint _heldJoint;
    private Camera _cam;
    private float _holdDistance;
    private Rigidbody _currentHeldBody;
    private Ray _cameraRay;
    private Rigidbody _holder;

    private void Awake()
    {
        _cam = Camera.main;
        if (_cam == null)
        {
            Debug.LogError("Camera not found");
        }
        _holder = new GameObject("Holder").AddComponent<Rigidbody>();
        _holder.transform.parent = _cam.transform;
        _holder.transform.localPosition = Vector3.zero;
        _holder.transform.localRotation = Quaternion.identity;
        _holder.isKinematic = true;
    }

    private void Update()
    {
        _cameraRay = new Ray(_cam.transform.position, _cam.transform.forward);
        
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (_currentHeldBody == null)
        {
            if (!Physics.SphereCast(_cameraRay, 0.15f, out RaycastHit hit, maxGrabDistance, grabLayers)) return;
            if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
            {
                PickupObject(hit);
            }
        }
        else
        {
            ReleaseObject();
        }
    }

    private void FixedUpdate()
    {
        if (_currentHeldBody == null) return;
        _currentHeldBody.velocity *= 0.75f;
        _currentHeldBody.angularVelocity *= 0.75f;
    }

    private void PickupObject(RaycastHit hit)
    {
        if (_heldJoint != null)
        {
            Destroy(_heldJoint);
        }
        _currentHeldBody = hit.rigidbody;
        _holdDistance = hit.distance;
        var anchor = hit.point;
        _holder.transform.position = anchor;
        _heldJoint = _currentHeldBody.gameObject.AddComponent<ConfigurableJoint>();
        _heldJoint.anchor = Vector3.zero;
        _heldJoint.connectedBody = _holder;
        _heldJoint.autoConfigureConnectedAnchor = false;
        _heldJoint.connectedAnchor = Vector3.zero;
        _heldJoint.xDrive = new JointDrive
        {
            positionSpring = 10000,
            positionDamper = 1000,
            maximumForce = 1000,
            useAcceleration = true
        };
        _heldJoint.yDrive = _heldJoint.zDrive = _heldJoint.slerpDrive = _heldJoint.xDrive;
        _heldJoint.rotationDriveMode = RotationDriveMode.Slerp;
        AudioManager.PlaySound(grabSound, transform.position, 0.8f, 1f);
    }

    private void ReleaseObject()
    {
        Destroy(_heldJoint);
        _currentHeldBody = null;
    }
}