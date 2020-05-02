using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {
    public Rigidbody2D body;
    public float force = 100;
    private void Start () {

    }
    // Update is called once per frame
    private void LateUpdate () {
        RotateTo (body, 0, force);
    }

    void RotateTo (Rigidbody2D rigidbody, float angle, float force) {
        angle = Mathf.DeltaAngle (transform.eulerAngles.z, angle);

        var x = angle > 0 ? 1 : -1;
        angle = Mathf.Abs (angle * .1f);
        if (angle > 2) {
            angle = 2;
        }
        angle *= .5f;
        angle *= (1 + angle);

        rigidbody.angularVelocity *= .5f;
        rigidbody.AddTorque (angle * force * x);
    }
}