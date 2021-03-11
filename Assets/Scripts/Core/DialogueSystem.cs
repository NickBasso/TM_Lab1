using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core
{
    public class DialogueSystem : MonoBehaviour
    {
        public static DialogueSystem instance;
        public ELEMENTS elements;

        void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void Say(string speech, bool additive, string speaker)
        {
            StopSpeaking();
            speechText.text = targetSpeech;
            speaking = StartCoroutine(Speaking(speech, additive, speaker));
        }

        public void StopSpeaking()
        {
            if (isSpeaking)
            {
                StopCoroutine(speaking);
                speaking = null;
            }
        }

        public bool isSpeaking
        {
            get { return speaking != null; }
        }

        [HideInInspector] public bool isWaitingForUserInput = false;

        private string targetSpeech = "";
        Coroutine speaking = null;
        IEnumerator Speaking(string speech, bool additive, string speaker = "")
        {
            speechPanel.SetActive(true);
            targetSpeech = speech;
            
            if (!additive)
                speechText.text = "";
            else
                targetSpeech = speechText.text + targetSpeech;
            
            speakerNameText.text = DetermineSpeaker(speaker);
            speakerNamePanel.SetActive(speakerNameText.text != "");
            
            isWaitingForUserInput = false;

            while (speechText.text != targetSpeech)
            {
                speechText.text += targetSpeech[speechText.text.Length];
                speechText.text = speechText.text.Contains("|") ? speechText.text.Replace("|", "\n") : speechText.text;
                targetSpeech = targetSpeech.Contains("|") ? targetSpeech.Replace("|", "\n") : targetSpeech;
                yield return new WaitForEndOfFrame();
            }

            isWaitingForUserInput = true;
            while (isWaitingForUserInput)
            {
                yield return new WaitForEndOfFrame();
            }
            StopSpeaking();
        }

        string DetermineSpeaker(string s)
        {
            string retVal = speakerNameText.text;
            if (s != speakerNameText.text && s != "")
                retVal = (s.ToLower().Contains("narrator")) ? "" : s;
            return retVal;
        }

        public void Close()
        {
            StopSpeaking();
            speechPanel.SetActive(false);
        }

        [System.Serializable]
        public class ELEMENTS
        {
            public GameObject speechPanel;
            public GameObject speakerNamePanel;
            public Text speakerNameText;
            public Text speechText;
        }
        // Update is called once per frame
        void Update()
        {
        
        }
        
        public GameObject speechPanel
        {
            get { return elements.speechPanel; }
        }
        public GameObject speakerNamePanel
        {
            get { return elements.speakerNamePanel; }
        } 
        public Text speakerNameText
        {
            get { return elements.speakerNameText; }
        }
        public Text speechText
        {
            get { return elements.speechText; }
        }
    }
}
