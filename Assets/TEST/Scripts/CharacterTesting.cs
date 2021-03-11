using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;

public class CharacterTesting : MonoBehaviour
{
    public Character Rebecca;

    public float speed = 5;
    public bool smoothness = false;
    public string expression = "Unfriendly_1";
    
    // Start is called before the first frame update
    void Start()
    {
        Rebecca = CharacterManager.instance.GetCharacter(("Rebecca"), enabledCreatedCharacterOnStart: false);
    }

    public string[] speech;
    int i = 0;

    public Vector2 moveTarget;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (i < speech.Length)
                Rebecca.Say(speech[i]);
            else
            {
                DialogueSystem.instance.Close();
            }
            i++;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Rebecca.MoveTo(moveTarget);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Rebecca.TransitionBody(Rebecca.GetSprite(expression), speed, smoothness);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Rebecca.SetSprite(Rebecca.GetSprite(expression));
        }
    }
}
