using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toilet : MonoBehaviour
{
    public bool is_free; // A boolean variable to indicate if the toilet is currently available.
    public Button shitButton; // Reference to the button in the UI used for cleaning the toilet.

    void Start()
    {
        // Attach the Clean method to be called when the "shitButton" is clicked.
        shitButton.onClick.AddListener(() => Clean());

        // Initialize the toilet to be free when the game starts.
        MakeFree();

        // Hide the "shitButton" when the game starts.
        shitButton.gameObject.SetActive(false);
    }

    // Mark the toilet as free.
    public void MakeFree()
    {
        is_free = true;
    }

    // Mark the toilet as occupied.
    public void MakeBusy()
    {
        is_free = false;
    }

    // Activate/show the "shitButton" indicating the toilet needs cleaning.
    public void Finish()
    {
        shitButton.gameObject.SetActive(true);
    }

    // Handle the cleaning process for the toilet.
    public void Clean()
    {
        // Hide the "shitButton" after cleaning.
        shitButton.gameObject.SetActive(false);

        // Mark the toilet as free after cleaning.
        MakeFree();
    }
}