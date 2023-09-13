using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dog : Animal
{
    // Constants to define age milestones and their corresponding scales
    private const float ADULT_AGE = 5f;
    private const float ELDERLY_AGE = 10f;
    private const int ADULT_SCALE = 9;
    private const int ELDERLY_SCALE = 10;

    public bool isDirty = false; // Flag to determine if the dog is dirty
    public GameObject[] dirt = new GameObject[4]; // Array to hold dirt GameObjects for visual representation
    
    public override void Start()
    {
        base.Start();
        ResetDirt(); // Ensure that the dog starts clean
    }

    public override void GoToToilet()
    {
        going_to_toilet = false;
        gameObject.SetActive(false); // Deactivate the dog when it's in the toilet
        ui_manager.GoOutDisable();  // Disable the "Go Out" button in UI
    }

    public override void Wash() 
    {
        ResetDirt(); // Remove dirt visuals
        currentState = new WashingState();
        isDirty = false;
    }

    // Update dog's state based on various conditions
    public override void UpdateState()
    {
        base.UpdateState();

        CheckHygieneState();
        HandleToiletState();
        HandleSittingState();
        HandleDirtDisplay();
        CheckAgeAndUpdateSize();
    }

    // Method to check if the dog's hygiene level requires it to be marked as dirty
    private void CheckHygieneState()
    {
        if (hygiene <= 10 && currentState is not WashingState)
        {
            isDirty = true;
        }
    }

    // Handle behaviors and UI elements when the dog is in the toilet state
    private void HandleToiletState()
    {
        if (!(currentState is ToiletState)) return;

        if (going_to_toilet)
        {
            if (index_of_animal == ui_manager.indexOfCurrentAnimal)
            {
                ui_manager.GoOutEnable();  // Enable the "Go Out" button in UI
            }
        }
        else 
        {
            if (index_of_animal == ui_manager.indexOfCurrentAnimal)
            {
                ui_manager.UpdateUIInteractions(false);
                ui_manager.WashDisable();  // Disable the wash button in UI
            }
        }

        // Logic for when the dog's toilet need is fully satisfied
        if (toilet >= 99)
        {
            if (index_of_animal == ui_manager.indexOfCurrentAnimal)
            {
                ui_manager.UpdateUIInteractions(true);
                ui_manager.WashEnable();   // Enable the wash button in UI
            }

            movement.ChangeToPreviousPos(-1);
            gameObject.SetActive(true);
            currentState = new SittingState();
        }
    }

    // Define the dog's behavior when it's in a sitting state
    private void HandleSittingState()
    {
        if (!(currentState is SittingState)) return;

        if (toilet <= 10 || sleep <= 10 || energy >= 90)
        {
            currentState = new MovingState();
        }
    }

    // Handle the display of dirt based on the dog's state and movement direction
    private void HandleDirtDisplay()
    {
        if (!isDirty) return;

        if (currentState is MovingState && movement.movementVector.y > 0)
        {
            SetDirtActive(0);
        }
        else if (currentState is MovingState && movement.movementVector.y < 0)
        {
            SetDirtActive(3);
        }
        else if (currentState is SittingState || currentState is ToiletState)
        {
            SetDirtActive(2);
        }
    }

    // Activate a specific dirt GameObject based on the index
    private void SetDirtActive(int activeIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            dirt[i].SetActive(i == activeIndex);
        }
    }

    // Deactivate all the dirt GameObjects
    private void ResetDirt()
    {
        foreach (var d in dirt)
        {
            d.SetActive(false);
        }
    }

    // Check the dog's age and update its size when it reaches certain age
    private void CheckAgeAndUpdateSize()
    {
        if (age > ADULT_AGE && age < ADULT_AGE + 0.01)
        {
            UpdateSizeAndWarn(ADULT_SCALE, $"{animal_name} has grown up, now {animal_name} is an adult.");
        }
        else if (age > ELDERLY_AGE && age < ELDERLY_AGE + 0.01)
        {
            UpdateSizeAndWarn(ELDERLY_SCALE, $"{animal_name} has grown up, now {animal_name} is an elderly.");
        }
    }

    // Update the dog's size, adjust the dirt visuals and display a warning
    private void UpdateSizeAndWarn(int scale, string warning)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, 1);
        foreach (var d in dirt)
        {
            d.transform.localScale = new Vector3(scale, scale, 1);
        }
        ui_manager.DisplayWarning(warning);
    }
}
