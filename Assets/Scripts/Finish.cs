using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Finish : MonoBehaviour
{
    [SerializeField] string LayerHitName = "CarCollider";
    [SerializeField] Manager evoManager;
    [SerializeField] Button start;

    Car car;
    List<string> AllGuids = new List<string>();


    private void Start()
    {
        start.onClick.AddListener(Button);
    }
    private void OnTriggerEnter(Collider other) 
    {
        car = other.transform.parent.GetComponent<Car>();
        car.CheckpointHit();
        evoManager.pathEnd(car);
        

    }
    public void Button()
    {
        car.WallHit();

    }

  
}
