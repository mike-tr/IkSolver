using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlainCalculator
{
    private Plane plane;
    public PlainCalculator(Vector3 A, Vector3 B, Vector3 pointOnPlane){
        plane = new Plane(B - A, pointOnPlane);

    }

    public static Vector3 getClosestWithRespectTo(Vector3 A, Vector3 B, Vector3 respectPoint, Vector3 targetPoint, Vector3 Zero){
        // this method, will return the closest point on the plane, to the respect point
        // while it will give as a point, that has the same langth, as respectPoint, from the center of the plain

        // create a plain, while the Z axis is the  B-A vector,
        // move the plain so respectPoint, is a point on the plane.
        Plane plane = new Plane(B - A, respectPoint);
        // get the center of the plane, A.k.a, the point at witch respectPoint is closest to the RespectPoint (in world space).
        // a.k.a 90 degree angle between respect point and the Line AB (infinite line)
        Vector3 center = plane.ClosestPointOnPlane(A);

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(Zero + center, 0.3f);
        Handles.DrawLine(B + Zero, A + Zero);
        Handles.DrawLine(center + Zero, respectPoint + Zero);
        // get the closest point on plane to target
        Vector3 closest = plane.ClosestPointOnPlane(targetPoint);

        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(closest + Zero, 0.1f);
        Handles.DrawLine(closest + Zero, targetPoint + Zero);
        Handles.DrawLine(closest + Zero, center + Zero);

        // find the closest point, to target.
        // this point has the same distance to A, and to B from respectPoint, and yet the closest one to targetPoint
        Vector3 dir = (closest - center).normalized;
        dir *= (respectPoint - center).magnitude;
        Debug.Log(center + " , " + closest);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(dir + Zero + center, 0.1f);
        Handles.DrawLine(dir + Zero + center, center + Zero);
        Handles.DrawLine(dir + Zero + center, respectPoint + Zero);

        return dir;
    }
}
