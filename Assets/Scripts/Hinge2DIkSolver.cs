﻿using UnityEngine;

public class Hinge2DIkSolver : MonoBehaviour {
    public static float reflectionForce = 1f;
    public Transform target;
    public Transform pole;
    public int chainLength = 2;
    public float SetForce = 75;
    private float _force;
    private float dforce;
    public float force {
        get {
            if (SetForce != _force) {
                force = SetForce;
            }
            return _force;
        }
        set {
            _force = value;
            SetForce = value;
            dforce = 1f / (value * value);
        }
    }
    public float delta = 0.25f;
    public int iterations = 10;
    public float poleMinDistance = 1;
    public float SnapStrength = 1f;
    public int updateEveryXFrames = 5;
    public bool drawGizmos = true;

    private Vector2[] positions;
    private float[] bonesLength;
    private AnchoredJoint2D[] bones;

    private Rigidbody2D[] bonesR;
    private Transform[] bonesT;
    private Vector2[] StartDir;
    private float completeLength = 0;
    private Transform root;
    private Rigidbody2D rootrb;
    // Start is called before the first frame update
    void Start () {
        init ();
    }

    void init () {
        positions = new Vector2[chainLength + 1];
        bones = new AnchoredJoint2D[chainLength + 1];
        bonesT = new Transform[chainLength + 1];
        bonesR = new Rigidbody2D[chainLength + 1];
        StartDir = new Vector2[chainLength + 1];
        bonesLength = new float[chainLength];
        completeLength = 0;
        var current = GetComponent<AnchoredJoint2D> ();
        for (int i = chainLength; i >= 0; i--) {
            bones[i] = current;
            bonesT[i] = current.transform;
            bonesR[i] = current.attachedRigidbody;
            if (i == chainLength) {
                StartDir[i] = (Vector2) target.position - getBonePos (i);
            } else {
                var dir = getBonePos (i + 1) - getBonePos (i);
                StartDir[i] = dir;
                bonesLength[i] = dir.magnitude;
                completeLength += bonesLength[i];
            }
            current = current.connectedBody.GetComponent<AnchoredJoint2D> ();
        }
        root = bones[0].connectedBody.transform;
        rootrb = root.GetComponent<Rigidbody2D> ();
        if (!rootrb) {
            rootrb = rootrb.GetComponentInParent<Rigidbody2D> ();
        }
        Debug.Log (root + " , " + rootrb);
    }

    private Vector2 getBonePos (int index) {
        // get the position iof the anchor in world pos
        return bonesT[index].rotation * bones[index].anchor + bonesT[index].position;
    }

    private Vector2 BonePosToPos (int index, Vector2 pos) {
        // convert anchor world pos, to anchor.transform world pos.
        return pos + (Vector2) (bonesT[index].rotation * bones[index].anchor);
    }
    private void LateUpdate () {
        // System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
        // sw.Start ();
        // for (int i = 0; i < 10000; i++) {
        //     Solve ();
        // }

        // sw.Stop ();
        // Debug.Log ("time per 10k iterations : " + sw.ElapsedMilliseconds);
        // // the Ik solver

        //basically nothing would really change every frame so wh
        if (Time.frameCount % updateEveryXFrames == 0) {
            Solve ();
        }
        ApplyByTorque ();
        ApplyPositions ();
    }
    void Solve () {
        if (target == null)
            return;
        if (bonesLength.Length != chainLength)
            init ();

        // nothing much to say here
        Vector2 targetPos = target.position;
        for (int i = 0; i < chainLength + 1; i++) {
            positions[i] = getBonePos (i);
        }

        // calculate the desireble position if the target is outside reach
        var direction = positions[0] - targetPos;
        if (direction.sqrMagnitude > completeLength * completeLength) {
            for (int i = 1; i < chainLength + 1; i++) {
                positions[i] = positions[i - 1] - direction.normalized * bonesLength[i - 1];
            }
        } else {
            // in here we want to "get back" to the origin starting poss,
            // just to mentain some basic form.
            for (int i = 0; i < chainLength; i++) {
                positions[i + 1] = Vector3.Lerp (positions[i + 1],
                    positions[i] + StartDir[i], SnapStrength);
            }

            // the old good calculation for inverse ik, for 3d but with 2d vectors.
            // same stuff.
            for (int k = 0; k < iterations; k++) {
                // first step lets assume we are onto the target
                positions[chainLength] = targetPos;
                for (int i = chainLength - 1; i > 0; i--) {
                    //second step lets calculate out way from each position toward wahtever direction we are in currently.
                    positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];
                }

                for (int i = 1; i < chainLength + 1; i++) {
                    //this step, lets make sure our bones are actually not stranded off from the root positiion.
                    var dir = positions[i] - positions[i - 1];
                    positions[i] = positions[i - 1] + dir.normalized * bonesLength[i - 1];
                }

                // if we are close enough lets get out
                if ((targetPos - positions[chainLength]).sqrMagnitude < delta * delta) {
                    break;
                }
            }

            if (pole) {
                // if there is pole we want to make sure we are facing it.
                Vector2 polePos = (pole.position - root.position) * 100 + root.position;
                for (int i = 1; i < chainLength; i++) {
                    var dir = positions[i + 1] - positions[i - 1];
                    // we want to get the closest point in the line between pos - 1, + 1, to out current position
                    // then we will know in what direction the pole, and the currenct pose facing relative to the line
                    var closest = PlainMath.ClosestOnLine (positions[i + 1], dir, positions[i]);
                    var ttoCenter = closest - positions[i];
                    var ptoCenter = closest - polePos;
                    // if we are facing the opposite derection from the center, as the pole we will flip out direction
                    // note that it will have the same distance from -1, +1, just flipped direction
                    // also we dont wanna flip anything if the pole to the center, reason being,
                    // we will get to much flipping (in one frame the position is the same direciton as us, and the other frame its
                    // the opposide derection)
                    if (ptoCenter.sqrMagnitude > poleMinDistance && Vector2.Dot (ttoCenter, ptoCenter) < 0) {
                        positions[i] = closest + ttoCenter;
                    }
                    //var test = Vector2.SignedAngle
                }
            }
        }
    }

    private void ApplyByTorque () {
        for (int i = 1; i <= chainLength - 1; i++) {
            //calculate the direction from anchor to position[i]
            var dir = (positions[i] - getBonePos (i));
            var anglea = MathHelper.AngleBetween (positions[i], root.position);
            var angleb = MathHelper.AngleBetween (getBonePos (i), root.position);
            var angle = Mathf.DeltaAngle (anglea, angleb);

            var x = angle > 0 ? 1 : -1;
            angle = Mathf.Abs (angle * .05f);
            angle = Mathf.Clamp (angle, 0, 1);

            var vel = bonesR[i - 1].angularVelocity;
            rootrb.AddTorque (vel * 0.25f * reflectionForce);
            bonesR[i - 1].AddTorque (-vel * 0.25f * reflectionForce);

            var a = (chainLength + 2 - i);
            var index = chainLength - i + 1;
            var f = 1;
            rootrb.AddTorque (angle * force * x * f * 0.2f);
            bonesR[i - 1].AddTorque (-angle * force * x * f * 0.2f);
        }
    }
    private void ApplyPositions () {
        // well this method just doing one thing
        // its trying to set our transform position into the positions we calculated 
        // you can implement it in any way.

        // note!, the positions[i] are the desired position for the hinge.anchor so setting transform.position
        // would not work at all, in this case i calculate the "direction" that the anchor should move toward position[i]
        // and just add that force to our transform (it would also move the anchor so we get what we wanted)
        // also that would automatically rotate the transform as its all done via the rigidbodies physics.

        Vector2 vel = Vector2.zero;
        float n = 1 / rootrb.velocity.magnitude;
        n = Mathf.Clamp01 (n);
        //Debug.Log (n + " ," + rootrb.velocity.magnitude);
        for (int i = 0; i <= chainLength; i++) {
            //calculate the direction from anchor to position[i]
            var dir = (positions[i] - getBonePos (i));
            var mag = dir.magnitude;
            var cmag = bonesR[i].velocity.magnitude;

            // each frame we would reduce the velocity of the bone by magnitude of (force * 90%)
            // that way we get "semi" realistic moving parts that would still get effected by physics in the right way,
            // ( possibly would have problems with gravity(gravity would be to slow) )
            // if (cmag > force * 0.01f) {
            //     // if the velocity is to high, we would want to slow it by factor of force.
            //     var f = (mag / cmag) * reflectionForce;
            //     var velr = bonesR[i].velocity * f;
            //     rootrb.AddForce (velr, ForceMode2D.Impulse);
            //     bonesR[i].AddForce (-bonesR[i].velocity * f, ForceMode2D.Impulse);
            //     Debug.Log (velr);
            //     //vel += (bonesR[i].velocity * (1 - f));
            //     //bonesR[i].velocity *= f;
            // } else {
            //     //vel += (bonesR[i].velocity * 0.9f);
            //     var velr = bonesR[i].velocity * 0.9f;
            //     rootrb.AddForce (velr * reflectionForce, ForceMode2D.Impulse);
            //     //Debug.Log (-bonesR[i].velocity * 0.9f + " , " + velr);
            //     bonesR[i].AddForce (-velr * reflectionForce, ForceMode2D.Impulse);
            //     //bonesR[i].velocity *= 0.1f;
            // }

            var velr = bonesR[i].velocity * n * 0.5f;
            rootrb.AddForce (velr * reflectionForce, ForceMode2D.Impulse);
            bonesR[i].AddForce (-velr * reflectionForce, ForceMode2D.Impulse);

            //bonesR[i].velocity *= 0.1f;
            //Debug.Log (reflectionForce);
            float relativeForce = Mathf.Log10 (chainLength - i + 10) * force;
            //Debug.Log (relativeForce);

            var fr = force * (chainLength + 2 - i);
            bonesR[i].AddForce (dir * relativeForce);
            rootrb.AddForce (-dir * relativeForce);
        }
        //rootrb.velocity += vel * Time.deltaTime;
    }

#if UNITY_EDITOR
    public float gsize = 0.1f;
    private void OnDrawGizmos () {
        if (!drawGizmos)
            return;
        Gizmos.color = Color.white;
        try {
            if (bones != null) {
                Random.InitState (10);
                // draw the anchors
                for (int i = 0; i < bones.Length; i++) {
                    PlainMath.NextGizmosColor ();
                    Gizmos.DrawSphere (getBonePos (i), gsize);
                }

                // draw the desired anchor position
                for (int i = 0; i < chainLength + 1; i++) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere (positions[i], gsize);
                }
            }
        } catch {

        }
    }
#endif
}