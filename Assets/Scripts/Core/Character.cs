using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Character
{
    public string characterName;
    [HideInInspector]public RectTransform root;

    public bool enabled
    {
        get { return root.gameObject.activeInHierarchy; }
        set { root.gameObject.SetActive(value);}
    }

    public Vector2 anchorPadding
    {
        get { return root.anchorMax - root.anchorMin; }
    }
    
    DialogueSystem dialogue;

    public void Say(string speech, bool additive = false)
    {
        if (!enabled)
            enabled = true;
        dialogue.Say(speech, additive,characterName);
    }

    Vector2 targetPosition;
    
    public void MoveTo(Vector2 target)
    {
        targetPosition = target;
        Vector2 padding = anchorPadding;
        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
        root.anchorMin = minAnchorTarget;
        root.anchorMax = root.anchorMin + padding;
    }

    #region Transitioning Images
    
    
    public Sprite GetSprite(string emotion)
    {
        Sprite sprite = Resources.Load<Sprite>("images/Characters/"+characterName+"/"+characterName+"_"+emotion);
        Debug.Log("images/Characters/"+characterName+"/"+characterName+"_"+emotion);
        return sprite;
    }

    public void SetSprite(string emotion)
    {
        renderers.charRenderer.sprite = GetSprite(emotion);
    }
    
    public void SetSprite(Sprite sprite)
    {
        renderers.charRenderer.sprite = sprite;
    }
    
    //Transition Body
    bool isTransitioningBody {get{ return transitioningBody != null;}}
    Coroutine transitioningBody = null;

    public void TransitionBody(Sprite sprite, float speed, bool smooth)
    {
        if (renderers.charRenderer.sprite == sprite)
            return;
		
        StopTransitioningBody ();
        transitioningBody = CharacterManager.instance.StartCoroutine (TransitioningBody (sprite, speed, smooth));
    }

    void StopTransitioningBody()
    {
        if (isTransitioningBody)
            CharacterManager.instance.StopCoroutine (transitioningBody);
        transitioningBody = null;
    }

    public IEnumerator TransitioningBody(Sprite sprite, float speed, bool smooth)
    {
        for (int i = 0; i < renderers.allCharRenderers.Count; i++) 
        {
            Image image = renderers.allCharRenderers [i];
            if (image.sprite == sprite) 
            {
                renderers.charRenderer = image;
                break;
            }
        }

        if (renderers.charRenderer.sprite != sprite) 
        {
            Image image = GameObject.Instantiate (renderers.charRenderer.gameObject, renderers.charRenderer.transform.parent).GetComponent<Image> ();
            renderers.allCharRenderers.Add (image);
            renderers.charRenderer = image;
            image.color = GlobalF.SetAlpha (image.color, 0f);
            image.sprite = sprite;
        }

        while (GlobalF.TransitionImages (ref renderers.charRenderer, ref renderers.allCharRenderers, speed, smooth))
            yield return new WaitForEndOfFrame ();

        Debug.Log ("done");
        StopTransitioningBody ();
    }

    #endregion
    
    
    public Character(string _name, bool enabledOnStart = true)
    {
        CharacterManager cm = CharacterManager.instance;
        GameObject prefab = Resources.Load("Characters/Character["+_name+"]") as GameObject;
        GameObject ob = GameObject.Instantiate(prefab, cm.characterPanel);

        root = ob.GetComponent<RectTransform>();
        characterName = _name;

        //renderers.renderer = ob.GetComponentInChildren<RawImage>();
        renderers.charRenderer = ob.transform.Find("Character").GetComponentInChildren<Image>();
        renderers.allCharRenderers.Add(renderers.charRenderer);

        dialogue = DialogueSystem.instance;

        enabled = enabledOnStart;
    }

    [System.Serializable]
    public class Renderers
    {
        public Image charRenderer;

        public List<Image> allCharRenderers = new List<Image>();
    }

    public Renderers renderers = new Renderers();
    
}
