/*
Author: NamPhuThuy
Github: https://github.com/NamPhuThuy
*/

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.PuzzleTutorial
{
    public class TutorialPanel : MonoBehaviour
    {
        #region Private Serializable Fields

        //[Header("Flags")]

        //[Header("Stats")]

        [Header("Components")] 
        [SerializeField] private Button skipButton;
        [SerializeField] private RectTransform panelRT;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            skipButton.onClick.AddListener(OnClickSkip);
        }

        private void OnDestroy()
        {
            skipButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        public void ChangeDescriptionText(string message)
        {
            descriptionText.text = message;
        }

        public void Show(float duration = 0.5f)
        {
            Debug.Log(message:$"[TutorialPanel].Show()");
            gameObject.SetActive(true);
            StartCoroutine(IE_Show(duration));
            
            IEnumerator IE_Show(float duration = 0.5f)
            {
                float timer = 0f;
                float stepInterval = duration / 12f;
                WaitForSeconds waitForSeconds = new WaitForSeconds(stepInterval);
                
                while (timer < duration)
                {
                    timer += stepInterval;
                    canvasGroup.alpha = 1 * (timer / duration);
                    yield return waitForSeconds;
                }

                canvasGroup.alpha = 1;
            }
        }

        public void Hide(float duration = 0.3f)
        {
            Debug.Log(message:$"[TutorialPanel].Hide()");
            StartCoroutine(IE_Hide(duration));
            
            IEnumerator IE_Hide(float duration = 0.5f)
            {
                float timer = 0f;
                float stepInterval = duration / 12f;
                WaitForSeconds waitForSeconds = new WaitForSeconds(stepInterval);
                
                while (timer < duration)
                {
                    timer += stepInterval;
                    canvasGroup.alpha = 1 * (1 - timer / duration);
                    yield return waitForSeconds;
                }

                canvasGroup.alpha = 0;
                gameObject.SetActive(false);
            }
        }
        
        #endregion

        #region Private Methods
        #endregion

        #region Button Events

        private void OnClickSkip()
        {
            Debug.Log(message:$"[TutorialPanel].OnClickSkip()");
            Hide();
        }

        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }

        #endregion
    }

    /*#if UNITY_EDITOR
    [CustomEditor(typeof(TutorialPanel))]
    [CanEditMultipleObjects]
    public class TutorialPanelEditor : Editor
    {
        private TutorialPanel script;
        private Texture2D frogIcon;
        
        private void OnEnable()
        {
            frogIcon = Resources.Load<Texture2D>("frog"); // no extension needed
            script = (TutorialPanel)target;
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
           

            ButtonResetValues();
        }

        private void ButtonResetValues()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Reset Values", frogIcon), GUILayout.Width(InspectorConst.BUTTON_WIDTH_MEDIUM)))
            {
                script.ResetValues();
                EditorUtility.SetDirty(script); // Mark the object as dirty
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
    #endif*/
}
