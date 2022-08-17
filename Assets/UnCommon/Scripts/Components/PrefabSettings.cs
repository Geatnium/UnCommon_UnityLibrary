using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// プレハブのセッティング
    /// </summary>
    public class PrefabSettings : MonoBehaviour
    {
#if UNITY_EDITOR

        [SerializeField, Multiline(5)]
        public string description = "プレハブの説明";

#endif
        
    }
}
