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

            _currentStepIndex = 0;
            _isCurrentStepCompleted = false;

            if (TutorialManager.Ins != null && TutorialManager.Ins.TutorialHand != null)
            {
                TutorialManager.Ins.TutorialHand.DisableAllHands();
            }
        }

        public virtual void InitData(int _levelId)
        {
            DebugLogger.Log();

            if (TutorialManager.Ins == null || TutorialManager.Ins.Data == null)
            {
                DebugLogger.LogWarning("TutorialManager or Data is not initialized.");
                return;
            }

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
            if (tutorialRecord.Steps != null && tutorialRecord.Steps.Count > 0)
            {
                currentStepRecord = tutorialRecord.Steps[_currentStepIndex];
            }

            isForceFollow = tutorialRecord.TutType == TutorialRecord.Type.HAND_CLICK;
        }

        public virtual void ActivateTutorial()
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
        protected virtual void StartStep(TutorialStepRecord step)
        {
            DebugLogger.LogFrog(message:$"Type: {step.Type}");
            
            Transform target = GetTargetForStep(step);
            
            switch (step.Type)
            {
                case TutorialStepType.CLICK_THE_SOURCE:
                case TutorialStepType.CLICK_THE_TARGET:
                    if (target != null && TutorialManager.Ins.TutorialHand != null)
                    {
                        TutorialManager.Ins.TutorialHand.EnableHand();
                        TutorialManager.Ins.TutorialHand.MoveToScreenPointFromWorldTween(target.position, 0.4f);
                    }
                    break;
            }
        }

        protected virtual Transform GetTargetForStep(TutorialStepRecord step)
        {
            return transform;
        }

        protected virtual void EndTutorialSequence()
        {
            DebugLogger.Log();
            if (TutorialManager.Ins != null && TutorialManager.Ins.TutorialHand != null)
            {
                TutorialManager.Ins.TutorialHand.DisableHand();
            }

            // Optionally trigger an "all steps done" event here.
        }
        #endregion

        #region Events Listen

        public struct ESampleTutorial
        {
            public int eventID;
        }
        
        
        public virtual void OnReceiveEvent(ESampleTutorial @event)
        {
            
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