using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Singleton = null; 

    [SerializeField] int CarCount = 100; 
    [SerializeField] GameObject CarPrefab; 
    [SerializeField] Text GenerationNumberText;
    [SerializeField] Text sliderValue;
    [SerializeField] Text Arrived;
    [SerializeField] Text timer;
    [SerializeField] Text besttimer;
    [SerializeField] Text Arrivedinthis;
    [SerializeField] Button start;
    [SerializeField] Slider cutSlider;

    bool end = false;
    int GenerationCount = 0;
    int counter = 0;
    int counterGen = 0;

    float lastbesttime=10000.0f;
    public float time;
    List<Car> Cars = new List<Car>(); 

    NeuralNetwork BestNeuralNetwork = null; 
    int BestFitness = -1; 
    

    private void Start()
    {
        start.interactable = false;
        start.onClick.AddListener(button);
        
        if (Singleton == null) 
            Singleton = this; 
        else
            gameObject.SetActive(false);

        BestNeuralNetwork = new NeuralNetwork(Car.nextNetwork);
        cutSlider.value = 1.0f;

        StartGeneration(); 
    }

    void Update()
    {
        if (end != true)
        {
            time += Time.deltaTime;
            Time.timeScale = cutSlider.value;
            sliderValue.text = "Szybkość: " + cutSlider.value;
            timer.text = time.ToString();
        }
    }
    private void StartGeneration ()
    {
        time = 0.0f;
        start.interactable = false;
        counterGen = 0;
        Arrivedinthis.text ="W tej generacji: "+ counterGen;
        GenerationCount++;
        GenerationNumberText.text = "Generacja: " + GenerationCount; 

        for (int i = 0; i < CarCount; i++)
        {
            if (i == 0)
                Car.nextNetwork = BestNeuralNetwork;
            else
            {
                Car.nextNetwork = new NeuralNetwork(BestNeuralNetwork); 
                Car.nextNetwork.Mutate();
            }

            Cars.Add(Instantiate(CarPrefab, transform.position, Quaternion.identity, transform).GetComponent<Car>()); 
        }
    }

    public void Dead (Car DeadCar, int Fitness)
    {
        Cars.Remove(DeadCar); 
        Destroy(DeadCar.gameObject); 

        if (Fitness > BestFitness || (Fitness == BestFitness && DeadCar.timeAtLast<lastbesttime )) 
        {
            lastbesttime= DeadCar.timeAtLast;
            BestNeuralNetwork = DeadCar.netowork; 
            BestFitness = Fitness; 
        }

        if (Cars.Count <= 0) 
            StartGeneration(); 
    }


    public void pathEnd(Car car)
    {
        
        start.interactable = true;
        counter++;
        counterGen++;
        Arrived.text = "Auta: " + counter;
        Arrivedinthis.text = "W tej generacji: " + counterGen;
        end = true;
        Time.timeScale = 0;
        if (lastbesttime > car.timeAtLast || besttimer.text=="Najlepszy czas: ") {
            besttimer.text = "Naj. czas: " + car.timeAtLast; }
    }
    public void button()
    {
        end = false;

    }
}
