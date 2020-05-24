using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
   
    [SerializeField] LayerMask SensorMask; 
    [SerializeField] float FitnessUnchangedDie = 5;
    
    public static NeuralNetwork nextNetwork = new NeuralNetwork(new uint[] { 6, 5, 5, 2 }, null); 

    public string id { get; private set; }

    public int fitness { get; private set; }
    public float timeAtLast { get; private set; }

    public NeuralNetwork netowork { get; private set; } 

    Rigidbody rigibody;
    
 
    private void Awake()
    {
        id = Guid.NewGuid().ToString();
        
        netowork = nextNetwork; 
        nextNetwork = new NeuralNetwork(nextNetwork.Topology, null); 

        rigibody = GetComponent<Rigidbody>();
    

        StartCoroutine(IsNotImproving()); 

 
    }

    private void FixedUpdate()
    {
       
            float Vertical;
            float Horizontal;

            NeuralMoving(out Vertical, out Horizontal);

            Move(Vertical, Horizontal);
       
    }

    void NeuralMoving (out float Vertical, out float Horizontal)
    {
        double[] NeuralInput = new double[nextNetwork.Topology[0]];

        NeuralInput[0] = CastRay(transform.forward, Vector3.forward, 1);
        NeuralInput[1] = CastRay(-transform.forward, -Vector3.forward, 3);
        NeuralInput[2] = CastRay(transform.right, Vector3.right, 5);
        NeuralInput[3] = CastRay(-transform.right, -Vector3.right, 7);


        float SqrtHalf = Mathf.Sqrt(0.5f);
        NeuralInput[4] = CastRay(transform.right * SqrtHalf + transform.forward * SqrtHalf, Vector3.right * SqrtHalf + Vector3.forward * SqrtHalf, 9);
        NeuralInput[5] = CastRay(transform.right * SqrtHalf + -transform.forward * SqrtHalf, Vector3.right * SqrtHalf + -Vector3.forward * SqrtHalf, 13);


        double[] NeuralOutput = netowork.FeedForward(NeuralInput);
        
        if (NeuralOutput[0] <= 0.35f)
            Vertical = -1;
        else if (NeuralOutput[0] >= 0.65f)
            Vertical = 1;
        else
            Vertical = 0;

        if (NeuralOutput[1] <= 0.35f)
            Horizontal = -1;
        else if (NeuralOutput[1] >= 0.65f)
            Horizontal = 1;
        else
            Horizontal = 0;

        if (Vertical == 0 && Horizontal == 0)
            Vertical = 1;
    }

    IEnumerator IsNotImproving ()
    {
        while(true)
        {
            int OldFitness = fitness; 
            yield return new WaitForSeconds(FitnessUnchangedDie); 
            if (OldFitness == fitness) 
                WallHit(); 
        }
    }


    double CastRay (Vector3 RayDirection, Vector3 LineDirection, int LinePositionIndex)
    {
        float Length = 4; 

        RaycastHit Hit;
        if (Physics.Raycast(transform.position, RayDirection, out Hit, Length, SensorMask))
        {
            float Dist = Vector3.Distance(Hit.point, transform.position); 
            return Dist;
        }
        else
        {
            return Length; 
        }
    }
    
  
    public void Move (float v, float h)
    {
        rigibody.velocity = transform.right * v * 4;
        rigibody.angularVelocity = transform.up * h * 3;
    }

    
    public void CheckpointHit ()
    {
        fitness++;
        timeAtLast = Manager.Singleton.time;
    }

 
    public void WallHit()
    {
        Manager.Singleton.Dead(this, fitness); 
        gameObject.SetActive(false);
    }
}
