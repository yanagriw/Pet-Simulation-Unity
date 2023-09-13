using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Constants related to the animal's attributes and characteristics.
public class AnimalConstants
{
    public const int MAX_VALUE = 100;
    public const int MIN_VALUE = 0;
    public const int MAX_AGE = 20;
    public const int PROB_OF_DEATH = 5; // Probability of an animal dying due to age.
}

// Serializable class to manage and save an animal's state.
[System.Serializable]
public class SerializableAnimal
{
    public IAnimalState currentState;
    public string animal_name;
    public double age;
    public int social_remain;
    public double health;
    public double energy;
    public double hunger;
    public double hygiene;
    public double toilet;
    public double social;
    public double sleep;
    public int index_of_animal;
    public int index_of_prefab;
    public bool going_to_toilet;
}

// Interface for defining different animal states and related operations.
public interface IAnimalState
{
    void Enter(Animal animal);
    void UpdateState(Animal animal);
}

[System.Serializable]
public class MovingState : IAnimalState
{
    public void Enter(Animal animal)
    {
        animal.animator.SetInteger("State", 0);
    }

    public void UpdateState(Animal animal)
    {
        animal.DecreaseEnergy(2);
        animal.DecreaseHygiene(1);
        animal.DecreaseToilet(0.5);
        animal.DecreaseSocial(0.2);
        animal.DecreaseHunger(0.1);
        animal.DecreaseSleep(0.1);
    }
}

[System.Serializable]
public class SittingState : IAnimalState
{
    public void Enter(Animal animal)
    {
        animal.animator.SetInteger("State", 1);
    }

    public void UpdateState(Animal animal)
    {
        animal.IncreaseEnergy(2);
        animal.DecreaseHygiene(0.5);
        animal.DecreaseToilet(0.5);
        animal.DecreaseSocial(0.2);
        animal.DecreaseHunger(0.1);
        animal.DecreaseSleep(0.05);
    }
}

[System.Serializable]
public class SocialState : IAnimalState
{
    public void Enter(Animal animal)
    {
        animal.animator.SetInteger("State", 5);
    }

    public void UpdateState(Animal animal)
    {
        animal.IncreaseEnergy(2);
        animal.DecreaseHygiene(0.5);
        animal.DecreaseToilet(0.5);
        animal.DecreaseHunger(0.1);
        animal.DecreaseSleep(0.05);

        animal.social_remain = System.Math.Max(AnimalConstants.MIN_VALUE, animal.social_remain - 2);
        animal.IncreaseSocial(2);
    }
}

[System.Serializable]
public class ToiletState : IAnimalState
{
    public void Enter(Animal animal)
    {
        animal.animator.SetInteger("State", 4);
    }

    public void UpdateState(Animal animal)
    {
        if (animal.going_to_toilet)
        {
            animal.IncreaseEnergy(2);
            animal.DecreaseHygiene(0.5);
            animal.DecreaseToilet(0.5);
            animal.DecreaseSocial(0.2);
            animal.DecreaseHunger(0.1);
            animal.DecreaseSleep(0.05);
        }
        else
        {
            animal.IncreaseToilet(10);
            animal.DecreaseHygiene(10);
            animal.DecreaseEnergy(0.5);
            animal.DecreaseSocial(0.2);
            animal.DecreaseHunger(0.2);
            animal.DecreaseSleep(0.05);
        }

    }
}

[System.Serializable]
public class SleepingState : IAnimalState
{
    public void Enter(Animal animal)
    {
        animal.animator.SetInteger("State", 3);
    }

    public void UpdateState(Animal animal)
    {
        animal.IncreaseEnergy(10);
        animal.IncreaseSleep(1);
        animal.DecreaseToilet(0.5);
        animal.DecreaseHunger(0.1);
        animal.DecreaseSocial(0.2);
        animal.DecreaseHygiene(0.05);
    }
}

[System.Serializable]
public class WashingState : IAnimalState
{
    public void Enter(Animal animal)
    {
        animal.animator.SetInteger("State", 2);
    }

    public void UpdateState(Animal animal)
    {
        animal.IncreaseHygiene(5);
        animal.DecreaseToilet(0.5);
        animal.DecreaseSocial(0.2);
        animal.DecreaseHunger(0.1);
        animal.DecreaseSleep(0.05);
    }
}

// Helper class to handle animal aging.
public class AnimalAging
{
    private Animal animal;

    public AnimalAging(Animal animal)
    {
        this.animal = animal;
    }

    /// Ages the animal by incrementing its age. If the animal reaches the maximum age,
    /// it has a chance (based on a predefined probability) of being killed due to aging.
    public void AgeAnimal()
    {
        // Increase the age by 0.01 every time called
        animal.age += 0.01;

        if(animal.age > AnimalConstants.MAX_AGE)
        {
            int randomValue = Random.Range(1, 101);
            // If the generated value is within the probability range for the animal to die from aging
            // then kill the animal
            if (randomValue <= AnimalConstants.PROB_OF_DEATH)
            {
                animal.KillAnimal("its age");
            }
        }
    }
}

// Helper class to check the animal's health.
public class AnimalHealthChecker
{
    private Animal animal;

    public AnimalHealthChecker(Animal animal)
    {
        this.animal = animal;
    }

    /// Evaluates the health status of the animal. 
    public void CheckHealth()
    {
        /// If the animal is not in a happy state, its health decreases.
        if (!animal.AnimalHappy())
        {
            animal.DecreaseHealth(0.1);
        }
        /// If the health reaches a minimum threshold, the animal will be killed due to health issues.
        if (animal.health == AnimalConstants.MIN_VALUE)
        {
            animal.KillAnimal("health problems");
        }
    }
}

// Main class that defines an animal's behavior and attributes in the game.
public class Animal : MonoBehaviour
{
    // Various components and scripts associated with the animal.
    AnimalCommunication communication;
    public AnimalMovement movement;
    public UIManager ui_manager;
    public Animator animator;
    public Rat rat;

    // Helper classes to manage aging and health-check operations.
    private AnimalAging aging;
    private AnimalHealthChecker healthChecker;

    // Various attributes and states related to the animal.
    public IAnimalState currentState;
    public string animal_name;
    public double age = 0;
    public int social_remain = 0;
    public double health = 100;
    public double energy = 100;
    public double hunger = 100;
    public double hygiene = 100;
    public double toilet = 100;
    public double social = 100;
    public double sleep = 100;

    public int index_of_animal;
    public int index_of_prefab;

    public bool going_to_toilet;
    public bool going_to_rat;
    public Toilet toilet_obj;


    public virtual void Start()
    {
        currentState = new MovingState();

        // Obtain components attached to the same game object.
        movement = GetComponent<AnimalMovement>();
        animator = GetComponent<Animator>();
        communication = GetComponent<AnimalCommunication>();
        // Find the Toilet object in the scene.
        toilet_obj = GameObject.FindObjectOfType<Toilet>();
        // Find the UIManager object in the scene.
        ui_manager = GameObject.FindObjectOfType<UIManager>();

        aging = new AnimalAging(this);
        healthChecker = new AnimalHealthChecker(this);

        // Invoke the UpdateState method repeatedly every second.
        InvokeRepeating("UpdateState", 0f, 1f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // update animation
        currentState.Enter(this);

        // update position
        if (currentState is MovingState)
        {
            movement.Walk();
        }
    }
    public virtual void UpdateState()
    {
        // Age the animal and check its health.
        aging.AgeAnimal();
        healthChecker.CheckHealth();

        // Update the current behavior state of the animal.
        currentState.UpdateState(this);

        if (currentState is MovingState)
        {
            if (!going_to_rat && toilet <= 10)
            {
                going_to_toilet = true; // Indicate the animal is heading to the toilet.
            }
            else if (!going_to_rat && (energy <= 10 || hygiene <= 10 || sleep <= 10))
            {
                currentState = new SittingState();
            }
        }

        else if (currentState is SleepingState)
        {
            if (sleep >= 99)
            {
                currentState = new SittingState();
            }
        }

        else if (currentState is WashingState)
        {
            if (hygiene >= 99)
            {
                currentState = new SittingState();
            }
        }

        else if (currentState is SocialState)
        {
            // If the remaining social interactions are depleted to the minimum value.
            if (social_remain == AnimalConstants.MIN_VALUE)
            {
                currentState = new SittingState();
            }
        }
    }

    public virtual void GoToToilet() {}
    public virtual void Wash() {}

    public bool AnimalHappy() => hunger > 1 && toilet > 1 && social > 1 && sleep > 1; // Determines if the animal is happy based on its attributes.

    // Returns communication level with another animal with specified index
    public double ReturnCommunicationLevel(int index)
    {
        return communication.relations[index];
    }

    // Increase social attribute value depending on yhe button clicked (index)
    public void Socialize(int index)
    {
        switch (index)
        {
            case 0:
                social_remain = 10;
                break;
            case 1:
                social_remain = 20;
                break;
            case 2:
                social_remain = 30;
                break;
        }
        currentState = new SocialState();
    }

    // Increase hunger attribute value depending on yhe button clicked (index)
    // Can also increase/decrease health
    public void FeedAnimal(int index)
    {
        switch(index)
        {
            case 1:
            case 2:
                IncreaseHunger(30);
                break;
            case 3:
                IncreaseHunger(20);
                break;
            case 4:
                IncreaseHunger(40);
                break;
            case 5:
                IncreaseHunger(5);
                break;
            case 6:
                IncreaseHunger(15);
                break;
            case 7:
                IncreaseHunger(10);
                IncreaseHealth(2);
                break;
            case 8:
            case 9:
                IncreaseHunger(50);
                DecreaseHealth(1);
                break;
        }
    }

    public void KillAnimal(string reason)
    {
        ui_manager.ShowInformation(index_of_animal);
        ui_manager.DestroyAnimal();
        ui_manager.DisplayWarning($"{animal_name} died due to {reason} :(");
    }



    // Various methods to increase or decrease the animal's attributes.
    public void DecreaseEnergy(double value)
    {
        energy = System.Math.Max(AnimalConstants.MIN_VALUE, energy - value);
    }

    public void IncreaseEnergy(double value)
    {
        energy = System.Math.Min(AnimalConstants.MAX_VALUE, energy + value);
    }

    public void DecreaseSleep(double value)
    {
        sleep = System.Math.Max(AnimalConstants.MIN_VALUE, sleep - value);
    }

    public void IncreaseSleep(double value)
    {
        sleep = System.Math.Min(AnimalConstants.MAX_VALUE, sleep + value);
    }

    public void DecreaseHygiene(double value)
    {
        hygiene = System.Math.Max(AnimalConstants.MIN_VALUE, hygiene - value);
    }

    public void IncreaseHygiene(double value)
    {
        hygiene = System.Math.Min(AnimalConstants.MAX_VALUE, hygiene + value);
    }

    public void DecreaseToilet(double value)
    {
        toilet = System.Math.Max(AnimalConstants.MIN_VALUE, toilet - value);
    }

    public void IncreaseToilet(double value)
    {
        toilet = System.Math.Min(AnimalConstants.MAX_VALUE, toilet + value);
    }

    public void DecreaseSocial(double value)
    {
        social =System.Math.Max(AnimalConstants.MIN_VALUE, social - value);
    }

    public void IncreaseSocial(double value)
    {
        social = System.Math.Min(AnimalConstants.MAX_VALUE, social + value);
    }

    public void DecreaseHunger(double value)
    {
        hunger = System.Math.Max(AnimalConstants.MIN_VALUE, hunger - value);
    }

    public void IncreaseHunger(double value)
    {
        hunger = System.Math.Min(AnimalConstants.MAX_VALUE, hunger + value);
    }

    public void DecreaseHealth(double value)
    {
        health = System.Math.Min(AnimalConstants.MAX_VALUE, health - value);
    }
    public void IncreaseHealth(double value)
    {
        health = System.Math.Min(AnimalConstants.MAX_VALUE, health + value);
    }
}