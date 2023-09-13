using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button continueButton; // Reference to the "Continue" button in the UI.

    string path; // A string to store the path to the saved game data file.

    void Start()
    {
        // Determine the path to the saved game data file in the device's persistent data directory.
        path = Application.persistentDataPath + "/animals.save";

        // Check if the saved game data file exists, and enable the "Continue" button accordingly.
        continueButton.interactable = File.Exists(path);
    }

    public void LoadScene(int button)
    {
        if (button == 0)
        {
            if (File.Exists(path))
            {
                // If the "New Game" button is pressed and there's an existing saved game file,
                // delete the saved game data to start a new game.
                File.Delete(path);
            }
        }

        // Load a different scene (scene index 1 in this case).
        SceneManager.LoadScene(1);
    }
}
