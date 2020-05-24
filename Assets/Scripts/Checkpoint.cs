using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] string LayerHitName = "CarCollider";

    List<string> AllGuids = new List<string>(); 

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(LayerHitName)) 
        {
            Car CarComponent = other.transform.parent.GetComponent<Car>(); 
            string CarGuid = CarComponent.id;

            if (!AllGuids.Contains(CarGuid)) 
            {
                AllGuids.Add(CarGuid);
                CarComponent.CheckpointHit(); 
            }
        }
    }
}
