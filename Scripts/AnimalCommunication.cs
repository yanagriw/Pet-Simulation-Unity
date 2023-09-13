using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalCommunication : MonoBehaviour
{
    UIManager ui_manager;
    private Animal animal;
    private Animal friend_animal;
    public bool communicating = false;
    public double[] relations = new double[3];

    void Start()
    {
        // Initialize references to components and objects
        animal = gameObject.GetComponent<Animal>();
        ui_manager = GameObject.FindObjectOfType<UIManager>();
    }

    public void StartCommunication(Animal animal2)
    {
        // Start the communication process with another animal
        communicating = true;
        friend_animal = animal2;
        InvokeRepeating("CommunicationProcess", 0f, 1f);
    }

    public void EndCommunication()
    {
        // End the communication process
        CancelInvoke("CommunicationProcess");
        communicating = false;
        friend_animal = null;
        // Reset UI color for relations
        ui_manager.relation1.color = new Color32(255, 255, 255, 255);
        ui_manager.relation2.color = new Color32(255, 255, 255, 255);
    }

    void CommunicationProcess()
    {
        // Check if both animals are in a SittingState and happy
        if (animal.currentState is SittingState && friend_animal.currentState is SittingState)
        {
            if (animal.AnimalHappy() && friend_animal.AnimalHappy())
            {
                // Increase relations between animals
                RelationsPlus(friend_animal.index_of_animal);
                // Update UI color for positive relations
                UpdateRelationUIColor(180, 255, 0);
            }
            else
            {
                // Decrease relations between animals
                RelationsMinus(friend_animal.index_of_animal);
                // Update UI color for negative relations
                UpdateRelationUIColor(200, 0, 20);
            }
        }
    }

    void RelationsPlus(int index)
    {
        // Increase the relations with the given index, capping at 100
        relations[index] = System.Math.Min(100, relations[index] + 0.1);
    }

    void RelationsMinus(int index)
    {
        // Decrease the relations with the given index, capping at -100
        relations[index] = System.Math.Max(-100, relations[index] - 0.1);
    }

    void UpdateRelationUIColor(byte r, byte g, byte b)
    {
        // Update UI color for relations based on the current animal's index
        if (animal.index_of_animal == ui_manager.indexOfCurrentAnimal)
        {
            if (ReturnRelationNumber() == 1)
            {
                ui_manager.relation1.color = new Color32(r, g, b, 255);
            }
            else if (ReturnRelationNumber() == 2)
            {
                ui_manager.relation2.color = new Color32(r, g, b, 255);
            }
        }
    }

    int ReturnRelationNumber()
    {
        // Determine which relation number corresponds to the current animal's index
        switch (animal.index_of_animal)
        {
            case 0:
                return (friend_animal.index_of_animal == 1) ? 1 : 2;
            case 1:
                return (friend_animal.index_of_animal == 0) ? 1 : 2;
            case 2:
                return (friend_animal.index_of_animal == 0) ? 1 : 2;
        }
        return 0; // Default case
    }
}
