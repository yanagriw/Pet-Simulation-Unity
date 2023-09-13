using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : MonoBehaviour
{
    // Enumeration to represent the states the rat can be in
    public enum RatState { MoveLeft, MoveRight, Dance };
    public RatState currentState = RatState.MoveLeft;

    public bool active = false; // Flag to determine if the rat is active
    private const int PROBABILITY_OF_RAT_ACTIVATION = 25; // Chance of rat getting activated
    private const float RAT_SPEED = 4f; // Speed at which the rat moves
    private Vector3 movementDirection; // Vector direction in which the rat is moving
    private int danceDuration = 0; // Counter to keep track of dance duration

    public Animator animator; // Reference to the animator component

    private void Start()
    {
        animator = GetComponent<Animator>();
        // Schedule regular checks to possibly activate the rat
        ScheduleStateUpdate();
        // Set the rat to an inactive state initially
        DeactivateRat();
    }

    // Called once per frame
    private void Update()
    {
        MoveRat();
    }

    // Determine and execute the rat's behavior based on its current state
    private void MoveRat()
    {
        switch (currentState)
        {
            case RatState.MoveLeft:
                MoveLeft();
                break;
            case RatState.MoveRight:
                MoveRight();
                break;
            case RatState.Dance:
                Dance();
                break;
        }
    }

    private void MoveLeft()
    {
        movementDirection = Vector3.left; // Set the movement direction to left
        Move(); // Execute the move
        // If the rat has moved past a certain point on the left, make it start dancing
        if (gameObject.transform.position.x < 0)
        {
            TransitionToState(RatState.Dance);
        }
    }

    private void MoveRight()
    {
        movementDirection = Vector3.right; // Set the movement direction to right
        Move(); // Execute the move
        // If the rat has moved past a certain point on the right, revert its direction and deactivate it
        if (gameObject.transform.position.x > 7)
        {
            TransitionToState(RatState.MoveLeft);
            DeactivateRat();
        }
    }

    private void Dance()
    {
        danceDuration++; // Increment the dance duration counter
        // If the rat has danced long enough, change its state to moving right
        if (danceDuration > 10000)
        {
            danceDuration = 0; // Reset dance duration counter
            TransitionToState(RatState.MoveRight);
        }
    }

    // Move the rat in the specified direction at a given speed
    private void Move()
    {
        gameObject.transform.position += movementDirection * RAT_SPEED * Time.deltaTime;
    }

    // Transition the rat to a new state and update its animation
    private void TransitionToState(RatState newState)
    {
        currentState = newState;
        animator.SetInteger("State", (int)newState); // Reflect the new state in the animation
    }

    // Deactivate the rat
    private void DeactivateRat()
    {
        gameObject.SetActive(false);
        active = false;
    }

    // Schedule periodic checks to determine if the rat should become active
    private void ScheduleStateUpdate()
    {
        // Invoke the 'CheckRatActivation' method every 30 seconds, starting after a delay of 30 seconds
        InvokeRepeating("CheckRatActivation", 30f, 30f);
    }

    // Check whether the rat should be activated based on a probability
    private void CheckRatActivation()
    {
        if (!active)
        {
            int randomValue = Random.Range(1, 101);
            // If the random value is less than or equal to the probability of rat activation, activate the rat
            if (randomValue <= PROBABILITY_OF_RAT_ACTIVATION)
            {
                ActivateRat();
            }
        }
    }

    // Set the rat to an active state
    private void ActivateRat()
    {
        active = true;
        gameObject.SetActive(true);
    }
}