using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MathTest : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    public Vector2 a, b, c;
    public Vector2 offset;
    void triangle () {
        Handles.DrawLine (a + offset, b + offset);
        Handles.DrawLine (b + offset, c + offset);
        Handles.DrawLine (a + offset, c + offset);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere (a + offset, size);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere (b + offset, size);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere (c + offset, size);

        Debug.Log ("ab : " + MathHelper.AngleBetween (a, b));
        Debug.Log ("bc : " + MathHelper.AngleBetween (b, c));
        Debug.Log ("ac : " + MathHelper.AngleBetween (a, c));
    }

    public Vector2 FindNearestPointOnLine (Vector2 origin, Vector2 direction, Vector2 point) {
        direction.Normalize ();
        Vector2 lhs = point - origin;

        Gizmos.color = Color.black;

        float dotP = Vector2.Dot (lhs, direction);
        return origin + direction * dotP;
    }

    [Range (0, 1f)] public float size = 0.1f;

    [Space] public Vector2 A, B, target, pole;

    public void findPole (Vector2 A, Vector2 B, Vector2 target, Vector2 pole) {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere (A, size);
        Gizmos.DrawSphere (B, size);

        Handles.DrawLine (A, B);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (target, size);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere (pole, size);

        var mp = FindNearestPointOnLine (A, A - B, target);
        Gizmos.DrawSphere (mp, size * 0.5f);
        //Debug.Log (" ab : " + Vector2.Dot (a, b));

        Gizmos.color = Color.white;
        //Debug.Log (Vector2.Dot (mp - target, mp - pole));
        if (Vector2.Dot (mp - target, mp - pole) < 0) {
            var dir = mp - target;
            var np = 2 * mp - target;
            //Debug.Log (" NPA : " + (np - A).magnitude + " NPB : " + (np - B).magnitude);
            //Debug.Log (" OPA : " + (target - A).magnitude + " OPB : " + (target - B).magnitude);
            Gizmos.DrawSphere (np, size * 0.5f);
        }

        //Gizmos.DrawSphere (a, size);
    }
    private void OnDrawGizmos () {
        findPole (A, B, target, pole);
    }
}