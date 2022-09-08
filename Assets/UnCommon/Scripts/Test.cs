using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnCommon;

/// <summary>
/// MonoBehaviour継承のスクリプトテンプレート
/// </summary>
public class Test : MonoBehaviour
{
    [SerializeField]
    private IntToStringDictionary ooo;

    [SerializeField]
    private IntHashSet iii;

    [SerializeField]
    private HashSet<string> sss;

    [SerializeField]
    private Name a;

    private Name playerName = "Player";

    /// <summary>
    /// インスタンスされてから初回のUpdateの一回目だけ呼ばれる
    /// </summary>    
    private void Start()
    {
        iii.Add(0);
        if(a == playerName)
        {
            DebugLogger.Log("ooo");
        }
    }

    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    private void Update()
    {
        
    }
}
