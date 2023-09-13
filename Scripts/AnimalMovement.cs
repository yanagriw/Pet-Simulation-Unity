using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    // Constants for the base speed and movement vector
    private const float BASE_SPEED = 2f;
    private static Vector3 BASE_MOVEMENT_VECTOR = new Vector3(1, 1, 0);

    // Variables for movement
    private float speed;
    public Vector3 movementVector;
    private Vector3 previousPosition;

    // References to other components
    private Animator animator;
    private Animal animal;
    private AnimalCommunication communication;
    public Toilet toilet;
    public GameObject door;
    public UIManager ui_manager;


    public void Start()
    {
        movementVector = BASE_MOVEMENT_VECTOR;
        speed = BASE_SPEED;
        // Initialize components and set the initial position
        InitializeComponents();
        ChangePos(new Vector2(-3, -3), 0);
    }

    private void InitializeComponents()
    {
        // Initialize references to components
        animator = GetComponent<Animator>();
        communication = GetComponent<AnimalCommunication>();
        animal = gameObject.GetComponent<Animal>();
        // Find objects in the scene
        toilet = GameObject.FindObjectOfType<Toilet>();
        door = GameObject.FindWithTag("Door");
        ui_manager = GameObject.FindObjectOfType<UIManager>();
    }

    public void Walk()
    {
        // Handle animal movements, update animations, and move the animal
        HandleAnimalMovements();
        UpdateAnimationStates();
        MoveAnimal();
    }

    // Handle different animal movements based on their state
    private void HandleAnimalMovements()
    {
        if (animal.going_to_rat && animal is Cat)
        {
            HandleCatChasingRat();
        }
        else if (animal.going_to_toilet)
        {
            ChangeDirectionToToilet();
        }
    }

    // Move towards the rat and reset if close
    private void HandleCatChasingRat()
    {
        movementVector = (animal.rat.transform.position - gameObject.transform.position).normalized;
        if (Mathf.Abs(gameObject.transform.position.x) <= 1)
        {
            ResetRat();
        }
    }

    // Reset rat-related variables and position
    private void ResetRat()
    {
        animal.going_to_rat = false;
        changeDirectionX();
        changeDirectionY();
        animal.rat.active = false;
        animal.rat.transform.position = new Vector3(7.4f, -2, 0);
        animal.rat.currentState = Rat.RatState.MoveLeft;
        animal.rat.animator.SetInteger("State", (int) animal.rat.currentState);
        animal.rat.gameObject.SetActive(false);
    }

    private void UpdateAnimationStates()
    {
        // Update animation based on movement direction
        animator.SetBool("WasMovingUp", movementVector.y > 0);
    }

    private void MoveAnimal()
    {
        // Move the animal based on speed and movement vector
        float step = speed * Time.deltaTime;
        gameObject.transform.position += movementVector * step;
        previousPosition = gameObject.transform.position;
    }

    void ChangeDirectionToToilet()
    {
        // Change movement direction to the toilet or door based on the animal type
        if (animal.currentState is MovingState)
        {
            if (animal is Cat)
            {
                movementVector = (toilet.transform.position - gameObject.transform.position).normalized;
            }
            else if (animal is Dog)
            {
                movementVector = (door.transform.position - gameObject.transform.position).normalized;
            }
        }
    }

    // Change the position of the animal while avoiding collisions
    public void ChangePos(Vector3 pos, int direction)
    {

        Collider2D hitCollider = Physics2D.OverlapBox(pos, Vector2.one, 0f);

        if (hitCollider == null || hitCollider.isTrigger || direction == 2)
        {
            gameObject.transform.position = pos;
        }
        else
        {
            gameObject.transform.position = FindFreePoint(pos, direction);
        }
    }

    public void ChangeToPreviousPos(int direction)
    {
        // Change the position of the animal back to the previous position
        gameObject.transform.position = FindFreePoint(previousPosition, direction);
    }

    Vector2 FindFreePoint(Vector2 previousPosition, int direction)
    {
        // Find a free position to avoid collisions
        Vector2 size = Vector2.one;
        for (float i = 0.5f; i < 5f; i += 0.1f)
        {
            // Generate test positions in different directions
            Vector2 test1, test2, test3, test4;
            if (direction == 1)
            {
                test1 = new Vector2(previousPosition.x + i, previousPosition.y + i);
                test2 = new Vector2(previousPosition.x + i, previousPosition.y);
                test3 = new Vector2(previousPosition.x + i, previousPosition.y - i);
                test4 = new Vector2(previousPosition.x, previousPosition.y + i);
            }
            else if (direction == -1)
            {
                test1 = new Vector2(previousPosition.x - i, previousPosition.y - i);
                test2 = new Vector2(previousPosition.x - i, previousPosition.y);
                test3 = new Vector2(previousPosition.x - i, previousPosition.y + i);
                test4 = new Vector2(previousPosition.x, previousPosition.y - i);
            }
            else
            {
                test1 = new Vector2(previousPosition.x + i, previousPosition.y - i);
                test2 = new Vector2(previousPosition.x + i, previousPosition.y);
                test3 = new Vector2(previousPosition.x + i, previousPosition.y + i);
                test4 = new Vector2(previousPosition.x, previousPosition.y + i);
            }


            if (Physics2D.OverlapBox(test1, size, 0f) == null)
            {
                return test1;
            }
            else if (Physics2D.OverlapBox(test2, size, 0f) == null)
            {
                return test2;
            }
            else if (Physics2D.OverlapBox(test3, size, 0f) == null)
            {
                return test3;
            }
            else if (Physics2D.OverlapBox(test4, size, 0f) == null)
            {
                return test4;
            }
        }
        return new Vector2(previousPosition.x + 0.1f, previousPosition.y + 0.1f);
    }

    void changeDirectionX(bool random = false)
    {
        // Change the X direction of movement
        if (animal.currentState is MovingState)
        {
            if (random)
            {
                float alpha = movementVector.y / System.Math.Abs(movementVector.y);
                float newY = alpha * BASE_MOVEMENT_VECTOR.y;
                movementVector = new Vector3(-movementVector.x, Random.Range((newY / 4), (newY * 3))).normalized;
            }
            else
            {
                movementVector = new Vector3(-movementVector.x, movementVector.y).normalized;
            }
        }

    }

    void changeDirectionY(bool random = false)
    {
        // Change the Y direction of movement
        if (animal.currentState is MovingState)
        {
            if (random)
            {
                float alpha = movementVector.x / System.Math.Abs(movementVector.x);
                float newX = alpha * BASE_MOVEMENT_VECTOR.x;
                movementVector = new Vector3(Random.Range(newX / 4, (newX * 3)), -movementVector.y).normalized;
            }
            else
            {
                movementVector = new Vector3(movementVector.x, -movementVector.y).normalized;
            }
        }
    }

    // handle collisions with toilet or door
    void OnTriggerEnter2D(Collider2D collider)
    {
        speed = Random.Range(2f, 5f);

        if (collider.gameObject.CompareTag("Wall"))
        {
            // If triggered by a "Wall," change the Y direction
            changeDirectionY(true);
        }
        else if (collider.gameObject.CompareTag("BoarderWall") || collider.gameObject.CompareTag("Door"))
        {
            // If triggered by a "BoarderWall" or "Door," change the X direction
            changeDirectionX(true);
        }

        // Check if the animal is currently going to the toilet
        if (animal.going_to_toilet)
        {
            // If the animal is going to the toilet, check if it's going to the correct toilet
            if (CorrectToilet())
            {
                // Transition the animal to the "ToiletState"
                animal.currentState = new ToiletState();
            }
        }
    }

    // Handle collisions with walls and other animals
    void OnCollisionEnter2D(Collision2D collision)
    {
        speed = Random.Range(2f, 5f);

        if (collision.gameObject.CompareTag("Wall"))
        {
            HandleWallCollision();
        }
        else if (collision.gameObject.CompareTag("BoarderWall"))
        {
            HandleBoarderWallCollision();
        }
        else if (collision.gameObject.CompareTag("Cat") || collision.gameObject.CompareTag("Dog"))
        {
            HandleAnimalCollision(collision.gameObject);
        }
    }

    // Handle collision with a wall
    void HandleWallCollision()
    {
        // If collided with a "Wall," change the Y direction
        changeDirectionY(true);
    }

    // Handle collision with a boarder wall
    void HandleBoarderWallCollision()
    {
        // If collided with a "BoarderWall," change the X direction
        changeDirectionX(true);
    }

    // Handle collision with another animal
    void HandleAnimalCollision(GameObject collidedAnimalObj)
    {
        // Retrieve components from the collided animal
        Animal collidedAnimal = collidedAnimalObj.GetComponent<Animal>();
        AnimalMovement collidedAnimalMovement = collidedAnimalObj.GetComponent<AnimalMovement>();
        AnimalCommunication collidedAnimalCommunication = collidedAnimalObj.GetComponent<AnimalCommunication>();

        // Check if the collided animal has a higher instance ID, avoiding duplicate collision handling
        if (collidedAnimalObj.GetInstanceID() > gameObject.GetInstanceID())
        {
            if (animal.currentState is MovingState && collidedAnimal.currentState is MovingState)
            {
                HandleBothAnimalsInMovingState(collidedAnimal, collidedAnimalMovement, collidedAnimalCommunication);
            }
            else if (animal.currentState is MovingState)
            {
                HandleCurrentAnimalInMovingState(collidedAnimal);
            }
            else if (collidedAnimal.currentState is MovingState)
            {
                HandleCollidedAnimalInMovingState(collidedAnimalMovement, collidedAnimalCommunication);
            }
        }
    }

    // Handle collision when both animals are in MovingState
    void HandleBothAnimalsInMovingState(Animal collidedAnimal, AnimalMovement collidedAnimalMovement, AnimalCommunication collidedAnimalCommunication)
    {
        if (animal.energy <= 80 && collidedAnimal.energy <= 80)
        {
            // Transition both animals to the "SittingState" and start communication
            animal.currentState = new SittingState();
            collidedAnimal.currentState = new SittingState();
            communication.StartCommunication(collidedAnimal);
            collidedAnimalCommunication.StartCommunication(animal);
        }

        // Check movement directions of both animals
        if (movementVector.x > 0 && collidedAnimalMovement.movementVector.x > 0)
        {
            if (gameObject.transform.position.x < collidedAnimal.transform.position.x)
            {
                // Change direction of the current animal
                changeDirectionX(true);
                changeDirectionY(true);
            }
            else
            {
                // Change direction of the collided animal
                collidedAnimalMovement.changeDirectionX(true);
                collidedAnimalMovement.changeDirectionY(true);
            }
        }
        else if (movementVector.x < 0 && collidedAnimalMovement.movementVector.x < 0)
        {
            if (gameObject.transform.position.x < collidedAnimal.transform.position.x)
            {
                // Change direction of the collided animal
                collidedAnimalMovement.changeDirectionX(true);
                collidedAnimalMovement.changeDirectionY(true);
            }
            else
            {
                // Change direction of the current animal
                changeDirectionX(true);
                changeDirectionY(true);
            }
        }
        else
        {
            // Change direction for both animals
            changeDirectionX(true);
            changeDirectionY(true);
            collidedAnimalMovement.changeDirectionX(true);
            collidedAnimalMovement.changeDirectionY(true);
        }
    }

    // Handle collision when the current animal is in MovingState
    void HandleCurrentAnimalInMovingState(Animal collidedAnimal)
    {
        if (animal.going_to_toilet && collidedAnimal.going_to_toilet && CorrectToilet())
        {
            // Transition the current animal to the "ToiletState"
            animal.currentState = new ToiletState();
        }

        // Change direction for both animals
        changeDirectionX(true);
        changeDirectionY(true);
    }

    // Handle collision when the collided animal is in MovingState
    void HandleCollidedAnimalInMovingState(AnimalMovement collidedAnimalMovement, AnimalCommunication collidedAnimalCommunication)
    {
        if (animal.going_to_toilet && collidedAnimalMovement.CorrectToilet())
        {
            // Transition the collided animal to the "ToiletState"
            collidedAnimalMovement.animal.currentState = new ToiletState();
        }

        // Change direction for both animals
        collidedAnimalMovement.changeDirectionX(true);
        collidedAnimalMovement.changeDirectionY(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the animal is exiting a trigger collider tagged as "floor"
        if (collision.gameObject.CompareTag("floor"))
        {
            PreventCollision();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the animal is exiting a collision with an object tagged as "Cat" or "Dog"
        if (collision.gameObject.CompareTag("Cat") || collision.gameObject.CompareTag("Dog"))
        {
            // Get the AnimalCommunication component of the collided animal
            AnimalCommunication collidedAnimalCommunication = collision.gameObject.GetComponent<AnimalCommunication>();
            
            // End communication between the current animal and the collided animal
            communication.EndCommunication();
            collidedAnimalCommunication.EndCommunication();
        }
    }

    void PreventCollision()
    {
        // Move the animal to a specific position to prevent collisions
        ChangePos(new Vector2(-3, -3), 0);
        
        // Display a warning message through the UI manager
        ui_manager.DisplayWarning($"{animal.name} attempted to run away, but was returned home!");
    }

    bool CorrectToilet()
    {
        // Check if the animal is going to the correct toilet based on its type and position
        return (animal is Dog && gameObject.transform.position.x > 0) || (animal is Cat && gameObject.transform.position.x < 0);
    }

}

