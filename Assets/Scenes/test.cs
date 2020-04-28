using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class test : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 o1 = Vector3.zero;
    public Vector3 o2 = 2 * Vector3.right + Vector3.up;

    public Vector3 f = Vector3.up; 

    public Vector3 pole = Vector3.up;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Vector3.Cross(o2 - o1, f));
    }

    public Vector3 calculate(Vector3 A, Vector3 B, Vector3 T, Vector3 G){
        // Plane plane = new Plane(A, B, T);
        // Vector3 val = Vector3.zero;
        // return plane.ClosestPointOnPlane(G);

        Plane plane = new Plane(B-A, T);
        return plane.ClosestPointOnPlane(G);
    }

    public float size = 0.25f;

     private Vector3 ClosestPoint(Vector3 limit1, Vector3 limit2, Vector3 point)
     {
         Vector3 lineVector = (limit2 - limit1).normalized * 100;
 
         float lineVectorSqrMag = lineVector.sqrMagnitude;
 
         // Trivial case where limit1 == limit2
         if(lineVectorSqrMag < 1e-3f)
             return limit1;
 
         float dotProduct = Vector3.Dot(lineVector, limit1 - point);
 
         float t = - dotProduct / lineVectorSqrMag;
 
         return limit1 + Mathf.Clamp01(t) * lineVector;
     }

     private void v1(){
        Gizmos.color = Color.green;
        //var p = transform.position + calculate(o1, o2, f);
        // var p = ClosestPoint(o1, o2, f) + transform.position;
        // Gizmos.DrawSphere(p, size);
        // Handles.DrawLine(p, f + transform.position);

        var p = calculate(o1,o2,f,o1) + transform.position;
        Gizmos.DrawSphere(p, size);
        Handles.DrawLine(p, f + transform.position);

        Gizmos.color = Color.cyan;
        //var pl = ClosestPoint(p - transform.position, f, pole) + transform.position;
        var pl = ClosestPoint(f, p - transform.position, pole) + transform.position;
        Gizmos.DrawSphere(pl, size);
        Handles.DrawLine(pl, pole + transform.position);

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(calculate(o1,o2,f, pole) + transform.position, size);
     }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + o1, size);
        Gizmos.DrawSphere(transform.position + o2, size);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + f, size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + pole, size);

        Handles.DrawLine(o1 + transform.position, o2 + transform.position);

        ///////////////////////
        /////////
        ////////////////

        var target = PlainCalculator.getClosestWithRespectVisualized(o1, o2, f, pole, transform.position);

        //var target2 = PlainCalculator.getClosestWithRespectVisualized(o1 + Vector3.up * 5, o2 + Vector3.up * 5, f + Vector3.up * 5, pole + Vector3.up * 5, transform.position);
        //PlainCalculator.getClosestWithRespectVisualized(o1 + transform.position, o2 + transform.position, f + transform.position, pole + transform.position, Vector3.up * 5);
        
        //Gizmos.color = Color.grey;
        //Gizmos.DrawSphere(target + transform.position, size);
    }
}
