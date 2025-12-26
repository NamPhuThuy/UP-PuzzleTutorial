using System;
using System.Collections;
using NamPhuThuy.Common;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.PuzzleTutorial
{
    public class TutorialAdapterBase : MonoBehaviour
    {
        #region Private Serializable Fields

        [Header("Flags")]
        [SerializeField] protected  bool isForceFollow = false;
        public bool IsForceFollow => isForceFollow;
        
        [Header("Stats")] 
        [SerializeField] protected  int levelId;
        
        [Header("Components")]
        [SerializeField] protected  TutorialRecord tutorialRecord;
        public TutorialRecord TutorialRecord => tutorialRecord;
        [SerializeField] protected  TutorialStepRecord currentStepRecord;

        #endregion

        #region Private Fields
        
        protected Coroutine _tutorialRoutine;
        protected int _currentStepIndex;
        protected bool _isCurrentStepCompleted;
        #endregion

        #region Public Methods
                
        public virtual void ResetState()
        {
            if (_tutorialRoutine != null)
            {
                StopCoroutine(_tutorialRoutine);
                _tutorialRoutine = null;
            }

            if (TutorialManager.Ins != null && TutorialManager.Ins.TutorialHand != null)
            {
                TutorialManager.Ins.TutorialHand.DisableAllHands();
            }
        }

        public void InitData(int _levelId)
        {
            DebugLogger.Log();

            // Set Values
            this.levelId = _levelId;
            tutorialRecord = TutorialManager.Ins.Data.GetTutRecord(levelId);
            if (tutorialRecord == null)
            {
                DebugLogger.LogWarning(message: $"No tutorial record found for levelId: {levelId}");
                isForceFollow = false;
                return;
            }

            _currentStepIndex = 0;
            currentStepRecord = tutorialRecord.Steps[_currentStepIndex];

            if (tutorialRecord.TutType == TutorialRecord.Type.HAND_CLICK)
            {
                isForceFollow = true;
            }
            else
            {
                isForceFollow = false;
            }
        }

        public void ActiveCurrentTut()
        {
            DebugLogger.Log();
            if (!isForceFollow || tutorialRecord == null || tutorialRecord.Steps == null ||
                tutorialRecord.Steps.Count == 0)
            {
                DebugLogger.Log(message: $"No tutorial to run for levelId: {levelId}");
                return;
            }

            if (_tutorialRoutine != null)
            {
                DebugLogger.Log(message: $"Tutorial already running.");
                return;
            }

            _currentStepIndex = 0;
            
            _tutorialRoutine = StartCoroutine(RunTutorialSequence());
            
        }
        
        /// <summary>
        /// External call from gameplay when the current step is completed.
        /// For example: called from a listener when user taps correct piece, etc.
        /// </summary>
        public void OnStepCompletedFromGameplay()
        {
            DebugLogger.Log(message: $"Step completed from gameplay at index: {_currentStepIndex}");
            _isCurrentStepCompleted = true;
        }
        
        private IEnumerator RunTutorialSequence()
        {
            DebugLogger.LogFrog();
            
            while (_currentStepIndex < tutorialRecord.Steps.Count)
            {
                currentStepRecord = tutorialRecord.Steps[_currentStepIndex];
                DebugLogger.Log(message: $"Starting step index: {_currentStepIndex}");

                _isCurrentStepCompleted = false;
                StartStep(currentStepRecord);

                // Wait until gameplay notifies completion
                yield return new WaitUntil(() => _isCurrentStepCompleted);

                DebugLogger.Log(message: $"Finished step index: {_currentStepIndex}");
                _currentStepIndex++;
            }

            DebugLogger.Log(message: "All tutorial steps completed.");
            EndTutorialSequence();

            _tutorialRoutine = null;
        }

        /// <summary>
        /// Start a single step: show hand, highlight piece, lock input, etc.
        /// Customize this logic for each step type.
        /// </summary>
        private void StartStep(TutorialStepRecord step)
        {
            DebugLogger.LogFrog(message:$"Type: {step.Type}");
            switch (step.Type)
            {
                case TutorialStepType.CLICK_THE_SOURCE:
                    TutorialManager.Ins.TutorialHand.EnableHand();
                    TutorialManager.Ins.TutorialHand.MoveToScreenPointFromWorldTween(transform.position, 0.4f);
                    break;
                case TutorialStepType.CLICK_THE_TARGET:
                 
                    break;
            }
        }

        private void EndTutorialSequence()
        {
            DebugLogger.Log();
            if (TutorialManager.Ins != null && TutorialManager.Ins.TutorialHand != null)
            {
                TutorialManager.Ins.TutorialHand.DisableHand();
            }

            // Optionally trigger an "all steps done" event here.
        }
        #endregion

        #region LEVEL 1 TUTORIAL

        public void ResetDataLevel1()
        {
            DebugLogger.LogFrog();
        }
        
        public void InitDataLevel1()
        {
            DebugLogger.LogFrog(message:$"");
       
        }


        #endregion
        
        #region Events Listen

        public struct ESampleTutorial
        {
            public int eventID;
        }
        
        
        public void OnReceiveEvent(ESampleTutorial @event)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }

        #endregion


        
    }

    /*#if UNITY_EDITOR
    [CustomEditor(typeof(TutorialAdapter))]
    [CanEditMultipleObjects]
    public class TutorialAdapterEditor : Editor
    {
        private TutorialAdapter script;
        private Texture2D frogIcon;
        
        private void OnEnable()
        {
            frogIcon = Resources.Load<Texture2D>("frog"); // no extension needed
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (TutorialAdapter)target;

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