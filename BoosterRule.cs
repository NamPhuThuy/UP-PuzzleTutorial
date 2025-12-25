using System;
using NamPhuThuy.Common;
using NamPhuThuy.Data;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.PuzzleTutorial
{
    public enum LockOverride
    {
        NONE = 0,
        DEFAULT = 1, 
        FORCE_LOCK = 2, 
        FORCE_UNLOCK = 3,
    }

    public enum NativeBoosterType
    {
        NONE = 0,
        FIRST_BOOSTER = 1,
        SECOND_BOOSTER = 2,
        THIRD_BOOSTER = 3,
        FOURTH_BOOSTER = 4,
        FIFTH_BOOSTER = 5,
        SIXTH_BOOSTER = 6,
        SEVENTH_BOOSTER = 7,
        EIGHTH_BOOSTER = 8,
        NINETH_BOOSTER = 9,
        TENTH_BOOSTER = 10,
    }

    [Serializable]
    public class BoosterRule
    {
        public BoosterType type;
        public NativeBoosterType nativeType = NativeBoosterType.NONE;
        public LockOverride lockOverride = LockOverride.DEFAULT;

        [Min(0)]
        [ShowIf(nameof(lockOverride), LockOverride.DEFAULT)]
        public int grantAmount = 0;
    }
}