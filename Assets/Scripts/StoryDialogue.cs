using UnityEditor.Rendering;
using UnityEngine;
using System.Collections;   
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class StoryDialogue : MonoBehaviour
{
    public bool isTyping;
    public bool skip;
    public TMP_Text dialogue;
    public float typingSpeed = 0.05f;

    [SerializeField]
    private TextAsset inkJSONAsset = null;
    private Story story;

    public List<TextAsset> stories;

    [SerializeField]
    private Canvas buttonCanvas;
    [SerializeField]
    CanvasGroup buttonGroup;
    [SerializeField]
    private Button buttonPrefab;

    public static event Action<Story> OnCreateStory;

    public bool choiceGiven = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartStory(0);
    }

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StartStory(int index)
    {
        DeleteButtons();
        story = new Story (stories[index].text);
        if (OnCreateStory != null)
        {
            OnCreateStory(story);
        }
        PlayStory();
    }

    public void PlayStory()
    {
        DeleteButtons();
        if (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            StartCoroutine(TypeLine(text));
        }
        else if (!story.canContinue && story.currentChoices.Count <= 0)
        {
            GameManager.instance.EndStory();
        }
        if (story.currentChoices.Count > 0)
        {
            buttonGroup.alpha = 0;
            for (int i = 0; i < story.currentChoices.Count; ++i)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate {
                    OnClickChoiceButton(choice);
                });
            }
            StartCoroutine(ButtonFadeIn());
            choiceGiven = true;
        }
    }

    IEnumerator ButtonFadeIn()
    {
        while (buttonGroup.alpha < 1)
        {
            buttonGroup.alpha += Time.deltaTime / 1.5f;
            yield return null;
            Debug.Log("Fading in");
        }
    }

    void OnClickChoiceButton(Choice choice)
    {
        choiceGiven = false;
        story.ChooseChoiceIndex(choice.index);
        PlayStory();
    }

    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(buttonCanvas.transform, false);

        // Gets the text from the button prefab
        TMP_Text choiceText = choice.GetComponentInChildren<TMP_Text>();
        choiceText.text = text;

        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    void DeleteButtons()
    {
        int childCount = buttonCanvas.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            Destroy(buttonCanvas.transform.GetChild(i).gameObject);
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogue.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogue.text += c;
            if (skip)
            {
                dialogue.text = line;
                break;
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        skip = false;
    }

}
