using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnCommon
{

    /// <summary>
    /// <br>UIプレハブのデータ</br>
    /// </summary>
    [CreateAssetMenu(
      fileName = "UIData",
      menuName = "ScriptableObject/UIData",
      order = 0)
    ]
    public class UIData : ScriptableObject
    {
        /// <summary>
        /// UIのキーとプレハブ
        /// </summary>
        public NameToAssetReferenceDictionary uiAssets;
    }
}