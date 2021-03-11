using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Core;
using UnityEngine;

public class NovelController : MonoBehaviour
{
    private List<string> data = new List<string>();
    
    int progress = 0;

    CultureInfo culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
    // Start is called before the first frame update
    void Start()
    {
        LoadChapterFile("chapter0.txt");
    }

    // Update is called once per frame
    void Update()
    {
        //testing
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            HandleLine(data[progress]);
            progress++;
        }
    }

    public void LoadChapterFile(string filename)
    {
        data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + filename);
        progress = 0;
        cachedLastSpeaker = "";
    }

    void HandleLine(string line)
    {
        string[] dialogueAndActions = line.Split('"');

        if (dialogueAndActions.Length == 3)
        {
            HandleDialogueFromLine(dialogueAndActions[0], dialogueAndActions[1]);
            HandleEventsFromLine(dialogueAndActions[2]);
        }
        else
        {
            HandleEventsFromLine(dialogueAndActions[0]);
        }
    }

    string cachedLastSpeaker = "";
    void HandleDialogueFromLine(string dialogueDetails, string dialogue)
    {
        string speaker = cachedLastSpeaker;
        bool additive = dialogueDetails.Contains("+");

        if (additive)
            dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length - 1);

        if (dialogueDetails.Length > 0)
        {
            if (dialogueDetails[dialogueDetails.Length-1] == ' ')
                dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length - 1);

            speaker = dialogueDetails;
            cachedLastSpeaker = speaker;
        }

        if (speaker != "narrator")
        {
            Character character = CharacterManager.instance.GetCharacter(speaker);
            character.Say(dialogue, additive);
        }
        else
        {
            DialogueSystem.instance.Say(dialogue, additive, speaker);
        }
    }

    void HandleEventsFromLine(string events)
    {
        string[] actions = events.Split(' ');

        foreach (string action in actions)
        {
            HandleAction(action);
        }
    }
    
    void HandleAction(string action)
    {
        print("Handle action [" + action + "]");
        string[] data = action.Split('(', ')');

        switch (data[0])
        {
            case "setBackground":
                Command_SetLayerImage(data[1], LayersController.instance.background);
                break;
            case "setForeground":
                Command_SetLayerImage(data[1], LayersController.instance.foreground);
                break;
            case "move":
                Command_MoveCharacter(data[1]);
                break;
            case "setExpression":
                Command_ChangeCharacterExpression(data[1]);
                break;
            case "enter":
                Command_Enter(data[1]);
                break;
            case "exit":
                Command_Exit(data[1]);
                break;
        }
    }

    void Command_SetLayerImage(string data, LayersController.LAYER layer)
    {
        string texName = data.Contains(",") ? data.Split(',')[0] : data;
        Texture2D tex = texName == "null" ? null : Resources.Load("images/Backgrounds/"+texName) as Texture2D;
        float spd = 2f;
        bool smooth = false;

        if (data.Contains(","))
        {
            string[] parameters = data.Split(',');
            foreach (string p in parameters)
            {
                float fVal = 0;
                bool bVal = false;
                if (float.TryParse(p, out fVal))
                {
                    spd = fVal;
                    continue;
                }

                if (bool.TryParse(p, out bVal))
                {
                    smooth = bVal;
                    continue;
                }
            }
        }
        print(texName);
        Debug.Log(tex);
        layer.TransitionToTexture(tex, spd, smooth);
    }

    void Command_MoveCharacter(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        
        float locationX = float.Parse(parameters[1], culture);
        float locationY = float.Parse(parameters[2], culture);

        Character c = CharacterManager.instance.GetCharacter(character);
        
        c.FadeOut(0.1f);
        c.MoveTo(new Vector2(locationX, locationY));
        if (!c.enabled)
        {
            c.renderers.charRenderer.color = new Color(1, 1, 1, 0);
            c.enabled = true;
            
            c.TransitionBody(c.renderers.charRenderer.sprite, 0.1f, false);
        }
        else
        {
            print("got here");
            c.FadeIn(0.1f);
        }
    }

    void Command_ChangeCharacterExpression(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        string expression = parameters[1];
        float speed = parameters.Length >= 3 ? float.Parse(parameters[2], culture) : 2f;
        bool smooth = parameters.Length == 4 && bool.Parse(parameters[3]);

        Character c = CharacterManager.instance.GetCharacter(character);
        c.TransitionBody(c.GetSprite(expression), speed, smooth);

    }

    void Command_Exit(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        float speed = parameters.Length >= 3 ? float.Parse(parameters[1], culture) : 3f;
        bool smooth = parameters.Length == 4 && bool.Parse(parameters[2]);

        Character c = CharacterManager.instance.GetCharacter(character);
        c.FadeOut(speed, smooth);
    }
    
    void Command_Enter(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        float speed = parameters.Length >= 3 ? float.Parse(parameters[1], culture) : 1f;
        bool smooth = parameters.Length == 4 && bool.Parse(parameters[2]);

        Character c = CharacterManager.instance.GetCharacter(character, enabledCreatedCharacterOnStart:false);

        if (!c.enabled)
        {
            c.renderers.charRenderer.color = new Color(1, 1, 1, 0);
            c.enabled = true;
            
            c.TransitionBody(c.renderers.charRenderer.sprite, speed, smooth);
        }
        else
        {
            c.FadeIn(speed, smooth);
        }
    }
}
