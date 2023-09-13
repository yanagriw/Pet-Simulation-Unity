using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class UIManager : MonoBehaviour
{
    private const int MAX_NAME_LENGTH = 8;
    public GameObject panel;
    public GameObject warningPanel;
    public GameObject foodPanel;
    public GameObject interactionButtons;
    public Button[] interactionButtons_array;
    public TMP_InputField inputField;
    public Button checkButton;
    public Button confirmButton;
    public Button goOut;
    public Button wash;
    public Button deleteButton;

    public string chosenName = "";
    public int indexOfChosenAnimal = -1; // index of animal chosen on the main panel 
    public int indexOfButton = -1;
    public int indexOfCurrentAnimal = -1; // index of current selected animal during the game

    public Animal[] currentAnimals = new Animal[3];
    public GameObject[] prefabsOfAnimals = new GameObject[6];
    public Sprite[] spritesOfAnimals = new Sprite[6];
    public Button[] buttons = new Button[3];
    public TMP_Text[] buttonsText = new TMP_Text[3];
    public Image[] buttonsImage = new Image[3];

    public TMP_Text informationName;
    public TMP_Text informationAge;
    public TMP_Text informationHealth;
    public TMP_Text informationHunger;
    public TMP_Text informationEnergy;
    public TMP_Text informationBladder;
    public TMP_Text informationSocial;
    public TMP_Text informationHygiene;

    public TMP_Text relation1;
    public TMP_Text relation2;
    public TMP_Text relation_text;

    public TMP_Text warningText;

    void Start ()
    {
        InitializeButtons();
        // Load saved animals from binary file
        LoadAnimals();
        GoOutDisable();
    }

    // Assigns the ActivatePanel method to the onClick event of each button.
    private void InitializeButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            int buttonIndex = i;
            buttons[buttonIndex].onClick.AddListener(() => ActivatePanel(buttonIndex));
        }
    }


    // functions to enable/disable some buttons when panels are activated
    private void SetButtonsInteractableState(bool state)
    {
        for (int i = 0; i < 3; i++)
        {
            buttons[i].interactable = state;
        }
    }

    private void EnableRelations(bool enable)
    {
        relation1.enabled = enable;
        relation2.enabled = enable;
    }

    private void EnableInteractions(bool enable)
    {
        interactionButtons.SetActive(enable);
        deleteButton.gameObject.SetActive(enable);
    }

    public void UpdateUIInteractions(bool isFree)
    {
        deleteButton.interactable = isFree;
        
        for (int i = 0; i < 4; i++)
        {
            interactionButtons_array[i].interactable = isFree;
        }
    }

    private void EnableCheckButtons(bool enable)
    {
        checkButton.interactable = enable;
        confirmButton.interactable = enable;
    }

    private void BlockButtons()
    {
        EnableRelations(false);
        WashDisable();
        GoOutDisable();
        EnableInteractions(false);
        SetButtonsInteractableState(false);
    }

    // Activates main UI panel to add new animal and adjusts other interactions accordingly
    public void ActivatePanel(int buttonNumber)
    {
        indexOfButton = buttonNumber;

        Time.timeScale = 0;
        panel.SetActive(true);
        BlockButtons();

        EnableCheckButtons(true);
    }

    // Activates panel with food
    public void ActivateFoodPanel()
    {
        Time.timeScale = 0;
        foodPanel.SetActive(true);
        BlockButtons();
    }

    public void DeactivateFoodPanel()
    {
        Time.timeScale = 1;
        foodPanel.SetActive(false);

        EnableRelations(true);

        if (currentAnimals[indexOfCurrentAnimal] is Dog)
        {
            WashEnable();
        }

        EnableInteractions(true);
        SetButtonsInteractableState(true);
    }
    public void DeactivatePanel()
    {
        inputField.text = "";

        Time.timeScale = 1;
        panel.SetActive(false);
        warningPanel.SetActive(false);

        EnableRelations(true);

        if (indexOfCurrentAnimal != -1 && currentAnimals[indexOfCurrentAnimal] is Dog)
        {
            WashEnable();
        }
        if (indexOfCurrentAnimal != -1)
        {
            EnableInteractions(true);
        }

        SetButtonsInteractableState(true);

        indexOfChosenAnimal = -1;
    }
    public void DeactivateWarningPanel()
    {
        warningPanel.SetActive(false);
        EnableCheckButtons(true);
    }
    public void DisplayWarning(string warning, bool deleteName = true)
    {
        if (deleteName)
        {
            inputField.text = "";
            chosenName = inputField.text;
        }
        warningPanel.SetActive(true);
        warningText.text = warning;
        EnableCheckButtons(false);
    }

    // function is called when user enter the name of animal on the main panel and click check button
    public void GetInputFieldText()
    {
        if (IsNameTooLong())
        {
            DisplayWarning("Name must be under 8 letters!");
        }
        else if (IsNameEmpty())
        {
            DisplayWarning("Name must contain at least 1 letter!");
        }
        else if (HasInvalidCharacters())
        {
            DisplayWarning("Name must contain letters only!");
        }
        else
        {
            chosenName = inputField.text;
        }
    }

    private bool IsNameTooLong()
    {
        return inputField.text.Length > MAX_NAME_LENGTH;
    }

    private bool IsNameEmpty()
    {
        return inputField.text.Length == 0;
    }

    private bool HasInvalidCharacters()
    {
        return inputField.text.Any(c => !char.IsLetter(c));
    }

    // function is called when user choose new animal on the main panel
    public void ChooseAnimal(int index)
    {
        indexOfChosenAnimal = index;
    }

    /// Executes when the confirm button is pressed.
    /// Validates the animal selection and name, instantiates the animal, and sets up its properties.
    public void ConfirmButton()
    {  
         // Check if a name has been chosen
        if (chosenName == "")
        {
            DisplayWarning("Confirm name with the check button!", false);
        }
        // Check if an animal has been selected
        else if (indexOfChosenAnimal == -1)
        {
            DisplayWarning("Select an animal!", false);
        }
        else
        {
            // Create an instance of the chosen animal
            GameObject animalObject = Instantiate(prefabsOfAnimals[indexOfChosenAnimal]);
            Animal animal;
            // Check animal type and get appropriate component
            if (indexOfChosenAnimal <= 2)
            {
                animal = animalObject.GetComponent<Dog>();
            }
            else 
            {
                animal = animalObject.GetComponent<Cat>();
            }

            // Set properties for the instantiated animal
            animal.animal_name = chosenName;
            animal.index_of_animal = indexOfButton;
            animal.index_of_prefab = indexOfChosenAnimal;
            currentAnimals[indexOfButton] = animal;

            // Update the button's text and image to reflect the chosen animal
            
            buttonsText[indexOfButton].text = ""; // Change button text
            buttonsImage[indexOfButton].sprite = spritesOfAnimals[indexOfChosenAnimal]; // Change button image
            // Change button behavior
            buttons[indexOfButton].onClick.RemoveAllListeners(); // Remove all current click events
            int indexOfButtonCopy = indexOfButton;
            buttons[indexOfButton].onClick.AddListener(() => ShowInformation(indexOfButtonCopy)); // Set the new click event function
            // Close the panel and display animal information
            DeactivatePanel();
            ShowInformation(indexOfButtonCopy);
        }
    }

    // Displays detailed information about the animal corresponding to the pressed button.
    public void ShowInformation(int buttonNumber)
    {
        // Initialize UI elements
        WashDisable();
        GoOutDisable();
        relation_text.text = "Relations Level:";
        relation1.color = new Color32(255, 255, 255, 255);
        relation2.color = new Color32(255, 255, 255, 255);
        EnableInteractions(true);

        indexOfCurrentAnimal = buttonNumber;

        // Determine if the current animal's interactions can be enabled based on its state
        bool canBeInteractable = !(currentAnimals[indexOfCurrentAnimal].currentState is ToiletState);

        // Enable certain interactions based on the animal's type and state
        deleteButton.interactable = canBeInteractable;
        for (int i = 0; i < 4; i++)
        {
            interactionButtons_array[i].interactable = canBeInteractable;
        }

        if (currentAnimals[indexOfCurrentAnimal] is Dog && canBeInteractable)
        {
            WashEnable();
        }
        if (currentAnimals[indexOfCurrentAnimal] is Dog && !(canBeInteractable) && currentAnimals[indexOfCurrentAnimal].going_to_toilet)
        {
            GoOutEnable();
        }

        // Invoke the UpdateInformation method repeatedly to ensure the displayed info is current
        CancelInvoke("UpdateInformation");
        InvokeRepeating("UpdateInformation", 0f, 1f);

    }

    // Update UI texts to display the current animal's statistics
    public void UpdateInformation()
    {
        // Update the primary statistics for the current animal
        UpdateTextAndColor(informationName, currentAnimals[indexOfCurrentAnimal].animal_name, false);
        UpdateTextAndColor(informationAge, "Age:" + System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].age), false);
        UpdateTextAndColor(informationHealth, "Health:" + System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].health));
        UpdateTextAndColor(informationHunger, "Hunger:" + System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].hunger));
        UpdateTextAndColor(informationEnergy, "Energy:" + System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].sleep));
        UpdateTextAndColor(informationBladder, "Bladder:" + System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].toilet));
        UpdateTextAndColor(informationSocial, "Social:" + System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].social));
        UpdateTextAndColor(informationHygiene, "Hygiene:" + System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].hygiene));

        // Display the current animal's relation levels with other animals
        int number_of_relation = 1;
        for (int i = 0; i < 3; i++)
        {
            if (i != indexOfCurrentAnimal && currentAnimals[i] != null)
            {
                if (number_of_relation == 1)
                {
                    relation1.text = $"{currentAnimals[i].animal_name}:{System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].ReturnCommunicationLevel(i))}";
                    number_of_relation += 1;
                }
                else if (number_of_relation == 2)
                {
                    relation2.text = $"{currentAnimals[i].animal_name}:{System.Math.Round((double)currentAnimals[indexOfCurrentAnimal].ReturnCommunicationLevel(i))}";
                }
            }
        }
    }

    // Updates the given text component with the provided value and optionally changes its color.
    void UpdateTextAndColor(TMP_Text textComponent, string value, bool applyColor = true)
    {
        textComponent.text = value;

        // Apply color to the text component based on the parsed value.
        if (applyColor)
        {
            double parsedValue;
            if (double.TryParse(value.Split(':')[1], out parsedValue))
            {
                textComponent.color = GetValueColor(parsedValue);
            }
        }
    }

    // Determines the color to be applied based on the given value.
    Color GetValueColor(double value)
    {
        if (value < 10)
        {
            return new Color32(200, 0, 20, 255); // Red
        }
        else if (value < 30)
        {
            return new Color32(255, 200, 0, 255); // Orange
        }
        else
        {
            return new Color32(255, 255, 255, 255); // White
        }
    }

    // Feeds the currently selected animal with the specified food type.
    public void ChooseFoodButton(int index)
    {
        currentAnimals[indexOfCurrentAnimal].FeedAnimal(index);
        DeactivateFoodPanel();
    }

    // Triggers the social interaction for the currently selected animal.
    public void SocializeButton(int index)
    {
        currentAnimals[indexOfCurrentAnimal].Socialize(index);
    }

    // Enables the go-out button and its functionality.
    public void GoOutEnable()
    {
        goOut.gameObject.SetActive(true);
        goOut.onClick.AddListener(() => currentAnimals[indexOfCurrentAnimal].GoToToilet());
    }

    // Disables the go-out button and its functionality.
    public void GoOutDisable()
    {
        goOut.onClick.RemoveAllListeners();
        goOut.gameObject.SetActive(false);
    }

    // Enables the wash button and its functionality.
    public void WashEnable()
    {
        wash.gameObject.SetActive(true);
        wash.onClick.AddListener(() => currentAnimals[indexOfCurrentAnimal].Wash());
    }

    // Disables the wash button and its functionality.
    public void WashDisable()
    {
        wash.onClick.RemoveAllListeners();
        wash.gameObject.SetActive(false);
    }

    // Deletes the currently selected animal from the game.
    public void DestroyAnimal()
    {
        CancelInvoke("UpdateInformation"); // Stop updating the information panel for the animal.

        Destroy(currentAnimals[indexOfCurrentAnimal].gameObject); // Destroy the animal game object.
        currentAnimals[indexOfCurrentAnimal] = null;
        // Reset the corresponding button to its default state.
        buttonsText[indexOfCurrentAnimal].text = "+";  // Change button text
        buttonsImage[indexOfCurrentAnimal].sprite = null; // Change button image
        // Change button behavior
        buttons[indexOfCurrentAnimal].onClick.RemoveAllListeners(); // Remove all current click events
        int indexOfButtonCopy = indexOfCurrentAnimal;
        buttons[indexOfCurrentAnimal].onClick.AddListener(() => ActivatePanel(indexOfButtonCopy)); // Set the new click event function

        // Clear the displayed information about the animal.
        relation_text.text = "";

        UpdateTextAndColor(informationName, "", false);
        UpdateTextAndColor(informationAge, "", false);
        UpdateTextAndColor(informationHealth, "", false);
        UpdateTextAndColor(informationHunger, "", false);
        UpdateTextAndColor(informationEnergy, "", false);
        UpdateTextAndColor(informationBladder, "", false);
        UpdateTextAndColor(informationSocial, "", false);
        UpdateTextAndColor(informationHygiene, "", false);

        WashDisable();
        GoOutDisable();

        relation1.text = "";
        relation2.text = "";

        indexOfCurrentAnimal = -1;

        EnableInteractions(false);
    }

    private void OnApplicationQuit()
    {
        SaveAnimals(); // Saves animal data when the application quits.
    }

    // Saves the current state of all animals to a file.
    void SaveAnimals()
    {
        List<SerializableAnimal> animalsToSave = new List<SerializableAnimal>();

        foreach (var animal in currentAnimals)
        {
            if(animal != null)
            {
                // Fill the serializable animal data.
                SerializableAnimal serializableAnimal = new SerializableAnimal();
                serializableAnimal.currentState = animal.currentState;
                serializableAnimal.animal_name = animal.animal_name;
                serializableAnimal.age = animal.age;
                serializableAnimal.social_remain = animal.social_remain;
                serializableAnimal.health = animal.health;
                serializableAnimal.energy = animal.energy;
                serializableAnimal.hunger = animal.hunger;
                serializableAnimal.hygiene = animal.hygiene;
                serializableAnimal.toilet = animal.toilet;
                serializableAnimal.social = animal.social;
                serializableAnimal.sleep = animal.sleep;
                serializableAnimal.index_of_animal = animal.index_of_animal;
                serializableAnimal.index_of_prefab = animal.index_of_prefab;
                serializableAnimal.going_to_toilet = animal.going_to_toilet;

                animalsToSave.Add(serializableAnimal);
            }
        }
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/animals.save";
        // Write the serialized data to the file.
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, animalsToSave);
        }
    }

    // Loads the saved animals from the file and recreates them in the game.
    void LoadAnimals()
    {
        string path = Application.persistentDataPath + "/animals.save";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                List<SerializableAnimal> loadedAnimals = formatter.Deserialize(stream) as List<SerializableAnimal>;
                
                for (int i = 0; i < loadedAnimals.Count; i++)
                {
                    // Instantiate the animal object and assign the loaded properties.
                    GameObject animalObject = Instantiate(prefabsOfAnimals[loadedAnimals[i].index_of_prefab]);
                    Animal animal;
                    if (loadedAnimals[i].index_of_prefab <= 2)
                    {
                        animal = animalObject.GetComponent<Dog>();
                    }
                    else
                    {
                        animal = animalObject.GetComponent<Cat>();
                    }
                    animal.currentState = loadedAnimals[i].currentState;
                    animal.animal_name = loadedAnimals[i].animal_name;
                    animal.age = loadedAnimals[i].age;
                    animal.social_remain = loadedAnimals[i].social_remain;
                    animal.health = loadedAnimals[i].health;
                    animal.energy = loadedAnimals[i].energy;
                    animal.hunger = loadedAnimals[i].hunger;
                    animal.hygiene = loadedAnimals[i].hygiene;
                    animal.toilet = loadedAnimals[i].toilet;
                    animal.social = loadedAnimals[i].social;
                    animal.sleep = loadedAnimals[i].sleep;
                    animal.index_of_animal = loadedAnimals[i].index_of_animal;
                    animal.index_of_prefab = loadedAnimals[i].index_of_prefab;
                    animal.going_to_toilet = loadedAnimals[i].going_to_toilet;
                    currentAnimals[i] = animal;

                    // Update the corresponding button to display the loaded animal.
                    buttonsText[animal.index_of_animal].text = ""; // Change button text
                    // Change button image
                    buttonsImage[animal.index_of_animal].sprite = spritesOfAnimals[animal.index_of_prefab];
                    // Change button behavior
                    buttons[animal.index_of_animal].onClick.RemoveAllListeners(); // Remove all current click events
                    int indexOfButtonCopy = animal.index_of_animal;
                    buttons[animal.index_of_animal].onClick.AddListener(() => ShowInformation(indexOfButtonCopy)); // Set the new click event function
                }
            }
        }
    }
}