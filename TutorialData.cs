using System;
using System.Collections.Generic;
using NamPhuThuy.Tutorial;
using UnityEditor;
using UnityEngine;

namespace NamPhuThuy.PuzzleTutorial
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "Game/Tutorial Data", order = 1)]
    public class TutorialData : ScriptableObject
    {
        public TutorialRecord[] data;

        private Dictionary<int, TutorialRecord> _dictData;

        public Dictionary<int, TutorialRecord> DictData
        {
            get
            {
                if (_dictData == null)
                {
                    EnsureIndex();
                }

                return _dictData;
            }
        }

        public TutorialRecord GetTutRecord(int levelId)
        {
            EnsureIndex();
            return _dictData.GetValueOrDefault(levelId);
        }

        public bool isCurrentLevelHasTut(int levelId)
        {
            EnsureIndex();
            return _dictData.ContainsKey(levelId);
        }

        #region Unity Callbacks

        private void OnValidate()
        {
#if UNITY_EDITOR
            // optional: ensure unique non\-negative levelId and keep dictionary in sync
            bool changed = false;

            // Auto\-assign level ids for entries with negative id
            for (int i = 0; i < data.Length; i++)
            {
                var r = data[i];
                if (r == null) continue;

                if (r.LevelId < 0)
                {
                    // r.SetLevelId(i);
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(this);
                _dictData = null; // force rebuild after edits
            }
#endif
        }

        #endregion

        #region Private Methods

        private void EnsureIndex()
        {
            if (_dictData != null) return;
            BuildIndex();
        }

        private void BuildIndex()
        {
            if (data == null || data.Length == 0)
            {
                _dictData = new Dictionary<int, TutorialRecord>(0);
                return;
            }

            _dictData = new Dictionary<int, TutorialRecord>(data.Length);
            foreach (var r in data)
            {
                if (r == null) continue;
                _dictData[r.LevelId] = r; // last one wins if duplicates
            }
        }

        #endregion
    }

    // Use this to override the inspector of Odin Inspector
#if UNITY_EDITOR
    [CustomEditor(typeof(TutorialData))]
    public class TutorialDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
#endif

    [Serializable]
    public class TutorialRecord
    {
        [SerializeField] public int levelId;

        [SerializeField] private Type type = Type.NONE;
        [SerializeField] private string tutName;
        [SerializeField] private List<BoosterRule> rules = new();

        [SerializeField] private bool isUnlockBooster;
        [SerializeField] private string description;

        [Tooltip("Ordered steps that define the tutorial flow for this level")] [SerializeField]
        private List<TutorialStepRecord> steps = new();

        public int LevelId => levelId;
        public Type TutType => type;
        public string TutName => tutName;
        public List<BoosterRule> Rules => rules;
        public bool IsUnlockBooster => isUnlockBooster;
        public string Description => description;
        public List<TutorialStepRecord> Steps => steps;

        public enum Type
        {
            NONE = 0,
            HAND_CLICK = 1,
            GUI = 2,
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TutorialStepType
    {
        NONE = 0,
        CLICK_THE_SOURCE = 1,
        CLICK_THE_TARGET = 2,
        SHOW_GUI = 3,

        /// <summary>
        /// 
        /// </summary>
        HAND_POINT_AND_WAIT_FOR_TARGET_HOLD = 5,
        HAND_POINT_AND_WAIT_FOR_TARGET_CLICK = 6,

        /// <summary>
        /// For special custom steps handled in code
        /// </summary>
        CUSTOM = 999,
    }

    [Serializable]
    public class TutorialStepRecord
    {
        [SerializeField] private string id;
        [SerializeField] private TutorialStepType type;

        [Tooltip("Optional text shown in popup / speech bubble")] [TextArea] [SerializeField]
        private string message;

        [Tooltip("Optional object reference (e.g. a transform, button, tile root etc.)")] [SerializeField]
        private string targetTagOrId;

        [Tooltip("Optional delay before step starts")] [SerializeField]
        private float delayBefore = 0f;

        [Tooltip("Optional timeout after which step auto-completes (0 = never)")] [SerializeField]
        private float autoCompleteAfter = 0f;

        public string Id => id;
        public TutorialStepType Type => type;
        public string Message => message;
        public string TargetTagOrId => targetTagOrId;
        public float DelayBefore => delayBefore;
        public float AutoCompleteAfter => autoCompleteAfter;
    }
}