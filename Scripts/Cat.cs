using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Animal
{
    // Constants for age and scale values
    private const float ADULT_AGE = 5f;
    private const float ELDERLY_AGE = 10f;
    private const int ADULT_SCALE = 10;
    private const int ELDERLY_SCALE = 12;
    // Handles the logic for the cat going to the toilet.
    public override void GoToToilet()
    {
        // Moves the cat to the toilet's position.
        movement.ChangePos(new Vector3(toilet_obj.transform.position.x, toilet_obj.transform.position.y + 1), 2);
        // Set the toilet as occupied.
        toilet_obj.MakeBusy();
        // Reset the flag indicating the cat's intention to go to the toilet.
        going_to_toilet = false;
    }

    public override void UpdateState()
    {
        // Check if the rat is in a dancing state and update the cat's reaction accordingly.
        UpdateRatState();
        
        base.UpdateState();

        // Handle the different states that the cat can be in.
        HandleCatStates();
    }

    /// Determines if the cat should react to a dancing rat.
    private void UpdateRatState()
    {
        // Find any instance of a Rat in the scene.
        rat = GameObject.FindObjectOfType<Rat>();

        // If no rat is found or the found rat isn't dancing, the cat shouldn't react to it.
        if (rat == null || rat.currentState != Rat.RatState.Dance)
        {
            going_to_rat = false;
            return;
        }

        // If the cat isn't in the process of going to the toilet and is either moving or sitting, it should react to the dancing rat.
        if ((currentState is MovingState || currentState is SittingState) && !going_to_toilet)
        {
            going_to_rat = true;
        }
    }

    private void HandleCatStates()
    {
        if (currentState is SittingState) 
        {
            HandleSittingState();
        }
        else if (currentState is ToiletState)
        {
            HandleToiletState();
        }
        // Check the cat's age and update its size if necessary.
        CheckAgeAndUpdateSize();
    }

    private void HandleSittingState()
    {
        if (toilet <= 10 || going_to_rat || energy >= 90)
        {
            currentState = new MovingState();
        }
        else if (hygiene <= 10)
        {
            currentState = new WashingState();
        }
        else if (sleep <= 10)
        {
            currentState = new SleepingState();
        }
    }

    /// Manages the behavior and UI elements when the cat is in the toilet state.
    private void HandleToiletState()
    {
        if (index_of_animal == ui_manager.indexOfCurrentAnimal)
        {
            // Update the UI elements based on whether the toilet is free or occupied.
            ui_manager.UpdateUIInteractions(toilet_obj.is_free);
        }

        if (toilet_obj.is_free)
        {
            // If the toilet is available, let the cat go to the toilet.
            GoToToilet();
        }

        // If the toilet needs are fully satisfied, return the cat to a sitting state.
        if (toilet >= 99)
        {
            movement.ChangeToPreviousPos(1);
            currentState = new SittingState();
            toilet_obj.Finish();
        }
    }

    /// Checks the age of the cat and updates its size when it reaches certain age
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

    /// Updates the size of the cat and displays a warning message.
    private void UpdateSizeAndWarn(int scale, string warningMessage)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, 1);
        ui_manager.DisplayWarning(warningMessage);
    }
}
