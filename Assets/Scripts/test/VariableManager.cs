using UnityEngine;

public class VariableManager : MonoBehaviour
{
    public float HingeIkRelativeReflectionForce = 1.25f;
    // Start is called before the first frame update
    void Start()
    {
        //Time.fixedDeltaTime *= 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (HingeIkRelativeReflectionForce != Hinge2DIkSolver.reflectionForce)
        {
            Hinge2DIkSolver.reflectionForce = HingeIkRelativeReflectionForce;
        }
    }
}