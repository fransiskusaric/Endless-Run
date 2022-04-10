using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    private List<GameObject> characters = new List<GameObject>();
    private CharacterModel activeCharacter;
    private int characterIndex;

    public Button buttonL;
    public Button buttonR;

    private void Start()
    {
        characters.Add(GameObject.Find("Boy"));
        characters.Add(GameObject.Find("Rin"));
        characters.Add(GameObject.Find("Satomi"));

        characterIndex = 0;
        SetActiveCharacter(characters[characterIndex], () => {
            activeCharacter.TriggerAnimation("bowTrigger");
        });
    }

    private void Update()
    {
        if (buttonL != null && buttonR != null)
        {
            if (characterIndex == 0 && buttonL.IsInteractable())
            {
                buttonL.interactable = false;
            }
            if (characterIndex == 2 && buttonR.IsInteractable())
            {
                buttonR.interactable = false;
            }
            if (characterIndex == 1)
            {
                buttonL.interactable = true;
                buttonR.interactable = true;
            }
        }        
    }

    public void SwipeLeft()
    {
        //print("button right is pressed");
        for (int i = 0; i < 3; i++)
        {
            MoveCharacter(-2.5f, i);
        }
        characterIndex++;
        SetActiveCharacter(characters[characterIndex], () => {
            activeCharacter.TriggerAnimation("bowTrigger");
        });
    }
    
    public void SwipeRight()
    {
        //print("button left is pressed");
        for (int i = 2; i >= 0; i--)
        {
            MoveCharacter(2.5f, i);
        }
        characterIndex--;
        SetActiveCharacter(characters[characterIndex], () => {
            activeCharacter.TriggerAnimation("bowTrigger");
        });
    }
    public void PlayGame(string sceneName)
    {
        //print("play button is pressed");
        PlayerPrefs.SetInt("characterIndex", characterIndex);
        SceneManager.LoadScene(sceneName);
    }

    private void MoveCharacter(float x, int index)
    {
        characters[index].transform.position += new Vector3(x, 0, 0);
    }

    private void SetActiveCharacter(GameObject character, System.Action OnActiveCharacterSet)
    {
        activeCharacter = character.GetComponent<CharacterModel>();
        OnActiveCharacterSet();
    }
}
