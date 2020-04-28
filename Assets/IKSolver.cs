using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    [SerializeField] int chainLength = 3;
    [SerializeField] Transform target;
    [SerializeField] Transform pole;
    // Start is called before the first frame update
    Transform[] bones;
    Vector3[] positions;
    Vector3[] StartDir;
    float[] bonesLength;
    float completeLength = 0;

    public int iterations = 10;
    public float delta = 0.25f;

    public float speed;

    public float snapBackStrength = 1f;

    private Transform Root;

    private void Start() {
        Init();
    }

    void Init(){
        bonesLength = new float[chainLength];
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        StartDir = new Vector3[chainLength + 1];

        Transform current = transform;
        completeLength = 0;

        Root = transform;
        for (int i = 0; i <= chainLength; i++)
        {
            if(Root == null){
                break;
            }
            Root = Root.parent;
        }

        for (int i = chainLength; i >= 0; i--)
        {
            bones[i] = current;
            if(i == chainLength){
                StartDir[i] = (target.position - current.position);
            }else{
                StartDir[i] = (bones[i + 1].position - current.position);
                bonesLength[i] = StartDir[i].magnitude;
                completeLength += bonesLength[i];
            }
            current = current.parent;
        }
    }

    // Update is called once per frame
    private void LateUpdate() {
        // System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        // sw.Start();
        // for (int i = 0; i < 10000; i++)
        // {
        //     Solver();
        // }
        // sw.Stop();
        // print("IKS " + (sw.ElapsedMilliseconds));

        Solver();
    }

    private void Solver(){
        if(target == null)
            return;
        if(bonesLength.Length != chainLength){
            Init();
        }

        var targetPos = target.position;
        var rootRot = Root.rotation;

        for (int i = 0; i < chainLength + 1; i++)
        {       
            positions[i] = bones[i].position;
        }

        Vector3 direction = bones[0].position - targetPos;
        if(direction.sqrMagnitude > completeLength * completeLength){
            for (int i = 1; i < chainLength + 1; i++)
            {   
                positions[i] = positions[i-1] - direction.normalized * bonesLength[i - 1];
            }
        }else{
            //positions[chainLength] = target.position;
            for (int k = 0; k < iterations; k++)
            {
                if(!pole){
                    for (int i = 0; i < chainLength; i++){
                        positions[i + 1] = Vector3.Lerp(positions[i + 1], 
                            positions[i] + StartDir[i], snapBackStrength);
                    }
                }

                positions[chainLength] = targetPos;
                for (int i = chainLength - 1; i > 0; i--)
                {   
                    positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];      
                }

                for (int i = 1; i < chainLength + 1; i++)
                {   
                    direction = (positions[i] - positions[i - 1]).normalized;
                    positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
                }

                if((targetPos - positions[chainLength]).sqrMagnitude < delta * delta){
                    break;
                }
            }
        }

        if(pole){
            var polePos = pole.position;
            for (int i = chainLength - 1; i > 0; i--)
            {
                positions[i] = PlainCalculator.calculateClosestToRinPlaneABwD(positions[i - 1], positions[i+1], 
                     positions[i], polePos);
            }

            for (int i = 1; i < chainLength; i++)
            {
                positions[i] = PlainCalculator.calculateClosestToRinPlaneABwD(positions[i - 1], positions[i+1], 
                     positions[i], polePos);
            }
        }

        for (int i = 0; i < chainLength + 1; i++)
        {
            bones[i].position = positions[i];
            if(i != chainLength){
                // bones[i].rotation = Quaternion.FromToRotation(StartDir[i], (positions[i + 1] - positions[i]).normalized)
                //     * Quaternion.Inverse(startRotations[i]);
                Vector3 dir = rootRot * Vector3.up;
                bones[i].rotation = Quaternion.FromToRotation(dir, (positions[i + 1] - positions[i]));
                //bones[i].rotation = Quaternion.LookRotation(startRotations[i], bones[i].up);
            }else{
                bones[i].localRotation = target.rotation;
            }     

            if(Root){
                bones[i].rotation *= rootRot;
            }
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos() {
        var current = this.transform;
        if(pole){
            var positions = new Vector3[chainLength + 1];
            current = this.transform;
            
            Random.InitState(10);
            for (int i = 0; i < (chainLength + 1) && current != null && current.parent != null; i++)
            {
                positions[i] = current.position;
                current = current.parent;
                Gizmos.color = Color.HSVToRGB(Random.value ,1,1);
                Gizmos.DrawSphere(positions[i], 0.25f);
            }

            for (int i = 1; i < chainLength; i++)
            {
                PlainCalculator.getClosestWithRespectVisualized(positions[i - 1], positions[i+1], 
                    positions[i], pole.position, Vector3.zero);
            }   
        }

        current = this.transform;
        for (int i = 0; i < chainLength && current != null && current.parent != null; i++){
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, 
                current.parent.position - current.position), new Vector3(scale, 
                Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
    #endif
}
