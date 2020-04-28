using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlainCalculator
{
    private Plane plane;
    public PlainCalculator(Vector3 A, Vector3 B, Vector3 pointOnPlane){
        plane = new Plane(B - A, pointOnPlane);

    }

    public static Vector3 getClosestWithRespectTo(Vector3 A, Vector3 B, Vector3 respectPoint, Vector3 targetPoint){
        // this method, will return the closest point on the plane, to the respect point
        // while it will give as a point, that has the same langth, as respectPoint, from the center of the plain

        // create a plain, while the Z axis is the  B-A vector,
        // move the plain so respectPoint, is a point on the plane.
        Plane plane = new Plane(B - A, respectPoint);
        // get the center of the plane, A.k.a, the point at witch respectPoint is closest to the RespectPoint (in world space).
        // a.k.a 90 degree angle between respect point and the Line AB (infinite line)
        Vector3 center = plane.ClosestPointOnPlane(A);
        // get the closest point on plane to target
        Vector3 closest = plane.ClosestPointOnPlane(targetPoint);

        // find the closest point, to target.
        // this point has the same distance to A, and to B from respectPoint, and yet the closest one to targetPoint
        Vector3 dir = (closest - center).normalized;
        dir *= (center - respectPoint).magnitude;

        return dir;
    }
}
