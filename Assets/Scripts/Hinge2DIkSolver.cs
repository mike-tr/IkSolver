using UnityEngine;

public class Hinge2DIkSolver : MonoBehaviour
{
    public Transform target;
    public Transform Pole;
    public int chainLength = 2;
    public float force = 1;
    public float delta = 0.25f;
    public int iterations = 10;
    
    private Transform Root;
    private Vector2[] positions;
    private float[] bonesLength;
    private HingeJoint2D[] bones;
    private Rigidbody2D[] bonesR;
    private Transform[] bonesT;
    private float completeLength = 0;


    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void init(){
        positions = new Vector2[chainLength + 1];
        bones = new HingeJoint2D[chainLength + 1];
        bonesT = new Transform[chainLength + 1];
        bonesR = new Rigidbody2D[chainLength + 1];
        bonesLength = new float[chainLength];
        completeLength = 0;
        var current = GetComponent<HingeJoint2D>();
        for (int i = chainLength; i >= 0; i--)
        {
            bones[i] = current;
            bonesT[i] = current.transform;
            bonesR[i] = current.attachedRigidbody;
            if(i == chainLength){

            }else{
                var dir = getBonePos(i + 1) - getBonePos(i);
                bonesLength[i] = dir.magnitude;
                completeLength += bonesLength[i];
            }
            current = current.connectedBody.GetComponent<HingeJoint2D>();
        }
    }

    private Vector2 getBonePos(int index){
        return bonesT[index].rotation * bones[index].anchor + bonesT[index].position;
    }
    private void LateUpdate() {
        Solve();    
    }
    void Solve(){
        if(target == null)
            return;
        if(bonesLength.Length != chainLength)
            init();

        Vector2 targetPos = target.position;

        for (int i = 0; i < chainLength + 1; i++)
        {
            positions[i] = getBonePos(i);
        }

        var direction = positions[0] - targetPos;
        if(direction.sqrMagnitude > completeLength * completeLength){
            for (int i = 1; i < chainLength; i++)
            {
                positions[i] = positions[i - 1] - direction.normalized * bonesLength[i - 1]; 
            }
        }else{
            for (int k = 0; k < iterations; k++)
            {
                positions[chainLength] = targetPos;
                positions[chainLength] = targetPos;
                for (int i = chainLength - 1; i > 0; i--)
                {   
                    positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];      
                }

                for (int i = 1; i < chainLength + 1; i++)
                {
                    var dir = positions[i] - positions[i - 1];
                    positions[i] = positions[i - 1] + dir.normalized * bonesLength[i - 1];
                }

                // if((targetPos - positions[chainLength]).sqrMagnitude < delta * delta){
                //     Debug.Log(targetPos);
                //     Debug.Log("???");
                //     break;
                // }
            }
        }

        SetPositionsByForce();
    }

    private void SetPositionsByForce(){
        for (int i = 0; i < chainLength; i++)
        {
            // var dir = (positions[i]);
            // var ex = (chainLength - i);
            // var vx = Mathf.Log(ex + 3);
            // bonesR[i].velocity *= 0.5f;
            // bonesR[i].AddForce(dir * force * ex * vx);
            // if(i > 0){
            //     bonesR[i - 1].AddForce(-dir * force * ex * vx);
            // }


            // var dir = (positions[i] - getBonePos(i));
            
            bonesR[i].MovePosition(positions[i] - getBonePos(i) + bonesR[i].position);
            //bonesR[i].MovePosition(bonesT[i].position);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.2f);    
        try{
            if(bones != null){
                Random.InitState(10);
                for (int i = 0; i < bones.Length; i++)
                {
                    Gizmos.color = PlainCalculator.Next();
                    Gizmos.DrawSphere(getBonePos(i), 0.25f);
                }

                float cl = 0;
                for (int i = 0; i < chainLength + 1; i++)
                {
                    var dir = (getBonePos(i) - positions[i]);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere((Vector2)bonesT[i].position, 0.33f);

                    cl += Random.value;
                    cl /= 2;
                
                    Gizmos.color = Color.HSVToRGB(cl, 1, 1);
                    Gizmos.DrawSphere(positions[i] - getBonePos(i) + bonesR[i].position, 0.5f);


                    cl += Random.value;
                    cl /= 2;
                    Gizmos.color = Color.HSVToRGB(cl, 1, 1);
                    Gizmos.DrawSphere(positions[i], 0.5f);
                }
            }
        }catch{

        }
    }
}
