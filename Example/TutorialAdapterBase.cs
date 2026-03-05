using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.PuzzleTutorial
{
    public class TutorialAdapterBase : MonoBehaviour
    {
        #region Private Serializable Fields

        /// <summary>
        /// Is force the playe follow the tutorial-step
        /// </summary>
        [Header("Flags")]
        [SerializeField] protected  bool isForceFollow = false;
        public bool IsForceFollow => isForceFollow;
        
        [Header("Stats")] 
        [SerializeField] protected  int levelId;
        
        [Header("Components")]
        [SerializeField] protected  TutorialRecord tutorialRecord;
        public TutorialRecord TutorialRecord => tutorialRecord;
        [SerializeField] protected  TutorialStepRecord currentStepRecord;
        public TutorialStepRecord CurrentStepRecord => currentStepRecord;
        
        [SerializeField] protected GameObject currentTarget;
        public GameObject CurrentTarget => currentTarget;
        
        #endregion

        #region Private Fields
        
        protected Coroutine _tutorialRoutine;
        [SerializeField] protected int _currentStepIndex;
        [SerializeField] protected bool _isCurrentStepCompleted;
        #endregion

        #region Public Methods
        
        /// <summary>
        /// External call from gameplay when the current step is completed.
        /// For example: called from a listener when user taps correct piece, etc.
        /// </summary>
        public virtual void OnStepCompleted()
        {
            Debug.Log(message: $"Step completed from gameplay at index: {_currentStepIndex}");
            _isCurrentStepCompleted = true;
        }
      
        #endregion

        #region Concrete Methods
        
        /// <summary>
        /// Init tutorial data for the current level
        /// Return value show that wether the Init-process is success or not
        /// </summary>
        /// <param name="_levelId"></param>
        public virtual bool TryInitData(int _levelId)
        {
            // Set Values
            this.levelId = _levelId;
            tutorialRecord = TutorialManager.Ins.Data.GetTutRecord(levelId);
            if (tutorialRecord == null)
            {
                Debug.LogWarning(message: $"No tutorial record found for levelId: {levelId}");
                isForceFollow = false;
                return false;
            }

            _currentStepIndex = 0;
            if (tutorialRecord.Steps != null && tutorialRecord.Steps.Count > 0)
            {
                currentStepRecord = tutorialRecord.Steps[_currentStepIndex];
            }

            isForceFollow = tutorialRecord.TutType == TutorialRecord.Type.HAND_CLICK;
            return true;
        }

        /// <summary>
        /// Return value show that wether the Init-process is success or not
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public virtual bool TryInitData(GameObject currentLevel, int levelId)
        {
            this.levelId = levelId;
            tutorialRecord = TutorialManager.Ins.Data.GetTutRecord(levelId);
            if (tutorialRecord == null)
            {
                Debug.LogWarning(message: $"No tutorial record found for levelId: {levelId}");
                isForceFollow = false;
                return false;
            }

            _currentStepIndex = 0;
            if (tutorialRecord.Steps != null && tutorialRecord.Steps.Count > 0)
            {
                currentStepRecord = tutorialRecord.Steps[_currentStepIndex];
            }

            isForceFollow = tutorialRecord.TutType == TutorialRecord.Type.HAND_CLICK;
            return true;
        }
        
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

        protected virtual void EndTutorialSequence()
        {
            if (TutorialManager.Ins != null && TutorialManager.Ins.TutorialHand != null)
            {
                TutorialManager.Ins.TutorialHand.DisableHand();
            }

            // Optionally trigger an "all steps done" event here.
        }
        
        public virtual void ActivateTutorial()
        {
            Debug.Log(message:$"[TutorialAdapterBase].ActiveTutorial()");
            if (tutorialRecord == null || tutorialRecord.Steps == null ||
                tutorialRecord.Steps.Count == 0)
            {
                Debug.Log(message: $"No tutorial to run for levelId: {levelId}");
                return;
            }

            if (_tutorialRoutine != null)
            {
                Debug.Log(message: $"Tutorial already running.");
                return;
            }

            _currentStepIndex = 0;
            _tutorialRoutine = StartCoroutine(IE_TutorialSequence());
        }

        private Coroutine _autoCompleteCo;
        
        private IEnumerator IE_TutorialSequence()
        {
            while (_currentStepIndex < tutorialRecord.Steps.Count)
            {
                currentStepRecord = tutorialRecord.Steps[_currentStepIndex];
                Debug.Log(message: $"Starting step index: {_currentStepIndex}");

                _isCurrentStepCompleted = false;
                StartStep(currentStepRecord);

                if (currentStepRecord.AutoCompleteAfter != 0)
                {
                    _autoCompleteCo = StartCoroutine(IE_AutoComplete(currentStepRecord.AutoCompleteAfter));
                }

                yield return new WaitUntil(() => _isCurrentStepCompleted);
                if (_autoCompleteCo != null) StopCoroutine(_autoCompleteCo); // Ensure the auto-complete mechanic is finished

                Debug.Log(message: $"Finished step index: {_currentStepIndex}");
                _currentStepIndex++;
            }

            Debug.Log(message: "All tutorial steps completed.");
            EndTutorialSequence();

            _tutorialRoutine = null;

            IEnumerator IE_AutoComplete(float delay)
            {
                yield return new WaitForSeconds(delay);
                _isCurrentStepCompleted = true;
            }
        }
        #endregion
        

        #region Template Methods

        protected virtual GameObject GetTargetForStep(TutorialStepRecord step)
        {
            switch (step.Type)
            {
                case TutorialStepType.CLICK_FIRST_ITEM:
                    break;
                case TutorialStepType.CLICK_SECOND_ITEM:
                    break;
                case TutorialStepType.CLICK_THIRD_ITEM:
                    break;
                case TutorialStepType.HAND_POINT_TARGET_WAIT_CLICK:
                    break;
            }
            
            
            return gameObject;
        }
        

        /// <summary>
        /// Start a single step: show hand, highlight piece, lock input, etc.
        /// Customize this logic for each step type.
        /// </summary>
        protected virtual void StartStep(TutorialStepRecord step)
        {
            GameObject target = GetTargetForStep(step);
            
            switch (step.Type)
            {
                case TutorialStepType.CLICK_FIRST_ITEM:
                case TutorialStepType.CLICK_SECOND_ITEM:
                    if (target != null && TutorialManager.Ins.TutorialHand != null)
                    {
                        TutorialManager.Ins.TutorialHand.EnableHand();
                        TutorialManager.Ins.TutorialHand.MoveToScreenPointFromWorldTween(target.transform.position, 0.4f);
                    }
                    break;
                case TutorialStepType.HAND_POINT_TARGET_WAIT_CLICK:
                    break;
            }
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