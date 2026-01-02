using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NamPhuThuy.Common;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.PuzzleTutorial
{
    public class TutorialHand : MonoBehaviour
    {
        enum HandType
        {
            NONE = 0,
            IMAGE = 1,
            SKELETON_GRAPHIC = 2,
        }
        
        [Header("Flags")] 
        [SerializeField] private bool isFollowing = false;
        [SerializeField] private HandType handType = HandType.NONE;
        
        [Header("Components")]
        [SerializeField] private SkeletonGraphic handSkeGraphic;
        [SerializeField] private Image handImage;
        [SerializeField] private Vector3 pivotOffset;
        [SerializeField] private Transform currentTarget;
        
        [Header("Animation Names")]
        private string _animAction = "action";
        private string _animAction2 = "action2";
        private string _animBegin1 = "begin1";
        private string _animBegin2 = "begin2";
        private string _animEnd1 = "end1";
        private string _animEnd2 = "end2";
        
        /// <summary>
        /// The hand move except finger
        /// </summary>
        public string AnimAction => _animAction;
        
        /// <summary>
        /// Finger move
        /// </summary>
        public string AnimAction2 => _animAction2;
        public string AnimBegin1 => _animBegin1;
        public string AnimBegin2 => _animBegin2;
        public string AnimEnd1 => _animEnd1;
        public string AnimEnd2 => _animEnd2;
        
        
        #region Private Serializable Fields

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        #endregion

        #region Public Methods
        
        private RectTransform rectTransform;

       
        public void EnableHand()
        {
            DebugLogger.Log();

            switch (handType)
            {
                case HandType.IMAGE:
                    if (handImage == null)
                    {
                        DebugLogger.LogWarning(message: $"handImage is null");
                        break;
                    }
                    
                    handImage.gameObject.SetActive(true);
                    break;
                case HandType.SKELETON_GRAPHIC:
                    if (handSkeGraphic == null)
                    {
                        DebugLogger.LogWarning(message: $"handSkeGraphic is null");
                        break;
                    }
                    
                    handSkeGraphic.gameObject.SetActive(true);
                    break;
                case HandType.NONE:
                    DebugLogger.LogWarning(message: $"handType is NONE");
                    break;
            }
        }

        public void DisableHand()
        {
            DebugLogger.Log();
            switch (handType)
            {
                case HandType.IMAGE:
                    if (handImage == null)
                    {
                        DebugLogger.LogWarning(message: $"handImage is null");
                        break;
                    }
                    
                    handImage.gameObject.SetActive(false);
                    break;
                case HandType.SKELETON_GRAPHIC:
                    if (handSkeGraphic == null)
                    {
                        DebugLogger.LogWarning(message: $"handSkeGraphic is null");
                        break;
                    }
                    
                    handSkeGraphic.gameObject.SetActive(false);
                    break;
                case HandType.NONE:
                    DebugLogger.LogWarning(message: $"handType is NONE");
                    break;
            }
        }

        public void DisableAllHands()
        {
            DebugLogger.Log();
            if (handImage != null)
            {
                handImage.gameObject.SetActive(false);
            }
            else
            {
                DebugLogger.LogWarning(message: $"handImage is null");
            }
                    
            if (handSkeGraphic != null)
            {
                handSkeGraphic.gameObject.SetActive(false);
            }
            else
            {
                DebugLogger.LogWarning(message: $"handSkeGraphic is null");
            }
        }

        public void FollowTransform(Transform tartget)
        {
            isFollowing = true;
            currentTarget = tartget;
        }

        public void UnfollowTransform(Transform tartget)
        {
            isFollowing = false;
            currentTarget = null;
        }

        #endregion

        #region Hand Control
        
        public void SetWorldPosition(Vector3 worldPos)
        {
            DebugLogger.Log();
            transform.position = worldPos + pivotOffset;
        }

        public void SetScreenPosition(Vector2 screenPos)
        {
            DebugLogger.Log();
            
            // treat pivotOffset as screen\-space offset (x,y). z is ignored.
            Vector3 screenWithOffset = new Vector3(
                screenPos.x + pivotOffset.x,
                screenPos.y + pivotOffset.y,
                pivotOffset.z
            );
            
            if (rectTransform != null)
            {
                rectTransform.position = screenWithOffset;
            }
            else
            {
                transform.position = screenWithOffset;
            }
        }


        public void MoveHandToWorldObject(Transform targetTransform)
        {
            DebugLogger.Log();
            if (targetTransform == null)
            {
                DebugLogger.Log(message: $"Return");
                return;
            }
            SetWorldPosition(targetTransform.position);
        }

        public void MoveToScreenPointFromWorldFast(Vector3 worldPosition)
        {
            DebugLogger.Log();
            if (Camera.main == null)
            {
                DebugLogger.Log(message: $"Return");
                return;
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            SetScreenPosition(screenPos);
        }
        
        public void MoveToScreenPointFromWorldTween(Vector3 worldPosition, float duration = 0.5f)
        {
            DebugLogger.Log();
            if (Camera.main == null)
            {
                DebugLogger.Log(message: "Return");
                return;
            }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            
            // apply offset before tween
            Vector3 screenWithOffset = new Vector3(
                screenPos.x + pivotOffset.x,
                screenPos.y + pivotOffset.y,
                pivotOffset.z
            );

            // Kill any existing tween on this transform/rectTransform if needed
            if (rectTransform != null)
            {
                rectTransform.DOKill();
                rectTransform.DOMove(screenWithOffset, duration);
            }
            else
            {
                transform.DOKill();
                transform.DOMove(screenWithOffset, duration);
            }
        }
        
        public void MoveToTargetRectTransformTween(RectTransform target, float duration = 0.5f, Vector3 offset = default)
        {
            if (target == null) return;

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (rectTransform == null) return;

            RectTransform parentRect = rectTransform.parent as RectTransform;
            if (parentRect == null) return;

            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null) return;

            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : (canvas.worldCamera != null ? canvas.worldCamera : Camera.main);

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, target.position);

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, cam, out Vector2 localPoint))
                return;

            Vector2 targetAnchoredPos = localPoint + (Vector2)pivotOffset + (Vector2)offset;

            rectTransform.DOKill();
            rectTransform.DOAnchorPos(targetAnchoredPos, duration);
        }
        
        public IEnumerator IE_TurnOffWithDelay(float delay)
        {
            DebugLogger.Log(message: $"Delay: {delay}");
            yield return new WaitForSeconds(delay);
            DisableHand();
        }
        
        /// <summary>
        /// Move the hand along a curved path between 2 screen points.
        /// Uses a 3-point CatmullRom path: from -> control -> to.
        /// </summary>
        public Tween TweenMoveBetweenScreenPointsCurved(
            Vector2 fromScreenPos,
            Vector2 toScreenPos,
            float duration = 0.5f,
            float curveHeight = 120f,
            bool loopPingPong = false,
            int loops = -1,
            Ease ease = Ease.InOutSine)
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            // Apply pivotOffset as screen-space offset
            var from = new Vector3(fromScreenPos.x + pivotOffset.x, fromScreenPos.y + pivotOffset.y, pivotOffset.z);
            var to = new Vector3(toScreenPos.x + pivotOffset.x, toScreenPos.y + pivotOffset.y, pivotOffset.z);

            // Build a simple arc control point (perpendicular offset)
            Vector3 mid = (from + to) * 0.5f;
            Vector3 dir = (to - from);
            Vector3 perp = Vector3.zero;
            if (dir.sqrMagnitude > 0.0001f)
            {
                perp = new Vector3(-dir.y, dir.x, 0f).normalized;
            }

            Vector3 control = mid + (perp * curveHeight);

            Tween t;
            if (rectTransform != null)
            {
                rectTransform.DOKill();
                rectTransform.position = from;

                t = rectTransform
                    .DOPath(new[] { from, control, to }, duration, PathType.CatmullRom)
                    .SetEase(ease);
            }
            else
            {
                transform.DOKill();
                transform.position = from;

                t = transform
                    .DOPath(new[] { from, control, to }, duration, PathType.CatmullRom)
                    .SetEase(ease);
            }

            if (loopPingPong)
                t.SetLoops(loops, LoopType.Yoyo);

            return t;
        }
        
        public void PlayAnimation(string animName, bool loop = true)
        {
            DebugLogger.Log(message: $"PlayAnimation: {animName}, loop: {loop}");
            if (handSkeGraphic != null)
            {
                DebugLogger.Log(message: $"Play Animation: {animName}, loop: {loop}");
                handSkeGraphic.AnimationState.SetAnimation(0, animName, loop);
            }
        }
        
        public void StopAnimation()
        {
            DebugLogger.Log(message: $"StopAnimation");
            if (handSkeGraphic != null)
            {
                handSkeGraphic.AnimationState.ClearTrack(0);
            }
        }
        
        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (rectTransform == null) return;

            rectTransform.anchoredPosition = anchoredPosition;
        }

        public Vector2 GetAnchoredPosition()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (rectTransform == null) return Vector2.zero;

            return rectTransform.anchoredPosition;
        }

        #endregion
        
        #region Private Methods
        
        
        
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }

        #endregion
    }

    /*#if UNITY_EDITOR
    [CustomEditor(typeof(TutorialHand))]
    [CanEditMultipleObjects]
    public class TutorialHandEditor : Editor
    {
        private TutorialHand script;
        private Texture2D frogIcon;
        
        private void OnEnable()
        {
            frogIcon = Resources.Load<Texture2D>("frog"); // no extension needed
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (TutorialHand)target;

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