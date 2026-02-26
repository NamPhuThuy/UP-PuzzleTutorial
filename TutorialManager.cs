using UnityEngine;

namespace NamPhuThuy.PuzzleTutorial
{
    public partial class TutorialManager : Common.Singleton<TutorialManager>
    {
        [Header("Flags")] 
        [SerializeField] private bool isEnable;
        public bool IsEnable => isEnable;
        
        [Header("Components")]
        [SerializeField] private TutorialData data;
        public TutorialData Data => data;
        [SerializeField] private TutorialHand _tutorialHand;
        public TutorialHand TutorialHand
        {
            get
            {
                return _tutorialHand;
            }
            
        }

        #region MonoBehaviour Callbacks

        #endregion

    }
}