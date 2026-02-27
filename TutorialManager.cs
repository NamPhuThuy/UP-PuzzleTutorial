/*
Author: NamPhuThuy
Github: https://github.com/NamPhuThuy
*/

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

        public TutorialAdapterBase tutorialAdapter;
        
        public TutorialHand TutorialHand
        {
            get
            {
                return _tutorialHand;
            }
            
        }

        [Header("Prefabs")] 
        [SerializeField] private TutorialPanel tutorialPanelPrefab;
        [SerializeField] private TutorialPanel tutorialPanel;

        public TutorialPanel TutorialPanel
        {
            get
            {
                if (tutorialPanel == null)
                {
                    tutorialPanel = Instantiate(tutorialPanelPrefab, parent: transform);
                }

                return tutorialPanel;
            }
        }

        #region MonoBehaviour Callbacks

        #endregion

    }
}