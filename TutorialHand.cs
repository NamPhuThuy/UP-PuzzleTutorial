/*
Author: NamPhuThuy
Github: https://github.com/NamPhuThuy
*/

using System.Collections;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if USE_SPINE
using Spine.Unity;
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

        [Header("Flags")] [SerializeField] private bool isFollowing = false;
        [SerializeField] private HandType handType = HandType.NONE;

        [Header("Components")]
        [SerializeField] private Image handImage;
#if USE_SPINE
        [SerializeField] private SkeletonGraphic handSkeGraphic;
#endif

        [SerializeField] private Vector3 pivotOffset;
        [SerializeField] private Transform currentTarget;

        [Header("Animation Names")] private string _animAction = "action";
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
            Debug.Log(message: $"[TutorialHand.EnableHand]");

            switch (handType)
            {
                case HandType.IMAGE:
                    if (handImage == null)
                    {
                        Debug.Log(message: $"[TutorialHand.EnableHand()] handImage is null");
                        break;
                    }

                    handImage.gameObject.SetActive(true);
                    break;
                case HandType.SKELETON_GRAPHIC:
                    
#if USE_SPINE
                    if (handSkeGraphic == null)
                    {
                        Debug.Log(message: $"[TutorialHand.EnableHand()] handSkeGraphic is null");
                        break;
                    }

                    handSkeGraphic.gameObject.SetActive(true);
#endif
                    break;
                case HandType.NONE:
                    Debug.Log(message: $"[TutorialHand.EnableHand()] handType is NONE");
                    break;
            }
        }

        public void DisableHand()
        {
            Debug.Log(message: $"[TutorialHand.DisableHand()]");
            switch (handType)
            {
                case HandType.IMAGE:
                    if (handImage == null)
                    {
                        Debug.Log(message: $"[TutorialHand.DisableHand()] handImage is null");
                        break;
                    }

                    handImage.gameObject.SetActive(false);
                    break;
                case HandType.SKELETON_GRAPHIC:
#if USE_SPINE
                    if (handSkeGraphic == null)
                    {
                        Debug.Log(message: $"[TutorialHand.DisableHand()] handSkeGraphic is null");
                        break;
                    }

                    handSkeGraphic.gameObject.SetActive(false);
#endif
                    break;
                case HandType.NONE:
                    Debug.Log(message: $"[TutorialHand.DisableHand()] handType is NONE");
                    break;
            }
        }

        public void DisableAllHands()
        {
            Debug.Log(message: $"[TutorialHand.DisableAllHands()]");
            if (handImage != null)
            {
                handImage.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log(message: $"[TutorialHand.DisableAllHands()] handImage is null");
            }
            
#if USE_SPINE
            if (handSkeGraphic != null)
            {
                handSkeGraphic.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log(message: $"[TutorialHand.DisableAllHands()] handSkeGraphic is null");
            }
#endif
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
            Debug.Log(message: $"[TutorialHand.DisableAllHands()] SetWorldPosition");
            transform.position = worldPos + pivotOffset;
        }

        public void SetScreenPosition(Vector2 screenPos)
        {
            Debug.Log(message: $"[TutorialHand.DisableAllHands()] SetScreenPosition");

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

        /// <summary>
        /// Moves the hand from one screen position to another using DOTween.
        /// Useful for UI-space tutorials where start and end points are already in screen coordinates.
        /// </summary>
        /// <param name="fromScreenPos">Starting screen position (pixels)</param>
        /// <param name="toScreenPos">Target screen position (pixels)</param>
        /// <param name="duration">Duration of the tween in seconds</param>
        /// <param name="ease">Easing function</param>
        /// <param name="loopPingPong">Whether to loop back and forth</param>
        /// <param name="loops">Number of loops (-1 = infinite)</param>
        /// <returns>The DOTween Tween instance (so caller can chain/kill it)</returns>
        public Tween MoveInScreenSpaceTween(
            Vector2 fromScreenPos,
            Vector2 toScreenPos,
            float duration = 0.5f,
            Ease ease = Ease.InOutSine,
            bool loopPingPong = false,
            int loops = -1)
        {
            Debug.Log(
                message:
                $"[TutorialHand.DisableAllHands()] From: {fromScreenPos}, To: {toScreenPos}, Duration: {duration}");

            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            // Apply pivotOffset as screen-space offset (z ignored for screen)
            Vector3 from = new Vector3(
                fromScreenPos.x + pivotOffset.x,
                fromScreenPos.y + pivotOffset.y,
                pivotOffset.z
            );
            Vector3 to = new Vector3(
                toScreenPos.x + pivotOffset.x,
                toScreenPos.y + pivotOffset.y,
                pivotOffset.z
            );

            Tween t;
            if (rectTransform != null)
            {
                rectTransform.DOKill();
                rectTransform.position = from;
                t = rectTransform.DOMove(to, duration).SetEase(ease);
            }
            else
            {
                transform.DOKill();
                transform.position = from;
                t = transform.DOMove(to, duration).SetEase(ease);
            }

            if (loopPingPong)
                t.SetLoops(loops, LoopType.Yoyo);

            return t;
        }

        public void MoveHandToWorldObject(Transform targetTransform)
        {
            Debug.Log(message: $"[TutorialHand.MoveHandToWorldObject()]");

            if (targetTransform == null)
            {
                Debug.Log(message: $"[TutorialHand.MoveHandToWorldObject()] Return");
                return;
            }

            SetWorldPosition(targetTransform.position);
        }

        public void MoveToScreenPointFromWorldFast(Vector3 worldPosition)
        {
            Debug.Log(message: $"[TutorialHand.MoveToScreenPointFromWorldFast()]");
            if (Camera.main == null)
            {
                Debug.Log(message: $"[TutorialHand.MoveToScreenPointFromWorldFast()] Return");
                return;
            }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            SetScreenPosition(screenPos);
        }

        /// <summary>
        /// If the input target were worldPosition
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="duration"></param>
        public void MoveToScreenPointFromWorldTween(Vector3 worldPosition, float duration = 0.5f)
        {
            if (Camera.main == null)
            {
                Debug.Log(message: $"[TutorialHand.MoveToScreenPointFromWorldTween()] Return");
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

        public void MoveToTargetRectTransformTween(RectTransform target, float duration = 0.5f,
            Vector3 offset = default)
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

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, cam,
                    out Vector2 localPoint))
                return;

            Vector3 targetLocalPos = (Vector3)localPoint + pivotOffset + offset;

            rectTransform.DOKill();
            rectTransform.DOLocalMove(targetLocalPos, duration);
        }

        public IEnumerator IE_TurnOffWithDelay(float delay)
        {
            Debug.Log(message: $"[TutorialHand.IE_TurnOffWithDelay()] delay: {delay}");
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

#if USE_SPINE
        public void PlayAnimation(string animName, bool loop = true)
        {
            Debug.Log(message: $"[TutorialHand.PlayAnimation()] PlayAnimation: {animName}, loop: {loop}");
            if (handSkeGraphic != null)
            {
                // DebugLogger.Log(message: $"Play Animation: {animName}, loop: {loop}");
                handSkeGraphic.AnimationState.SetAnimation(0, animName, loop);
            }
        }

        public void StopAnimation()
        {
            Debug.Log(message: $"[TutorialHand.PlayAnimation()] StopAnimation");
            if (handSkeGraphic != null)
            {
                handSkeGraphic.AnimationState.ClearTrack(0);
            }
        }
#endif
        
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