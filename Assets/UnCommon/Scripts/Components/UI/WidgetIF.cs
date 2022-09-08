using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// ウィジェットのインターフェース
    /// </summary>
    public interface IWidget : IComponent
    {
        /// <summary>
        /// ウィジェットを開く
        /// </summary>
        /// <param name="playsAnimation">開くアニメーションを再生するか</param>
        /// <param name="animationPlayRate">開くアニメーションの再生速度倍率</param>
        /// <param name="finishedCallback">開くアニメーションが終わった時のイベント</param>
        void Open(bool playsAnimation = true, float animationPlayRate = 1.0f, Action finishedCallback = null);

        /// <summary>
        /// ウィジェットを開く（非同期処理版）
        /// </summary>
        /// <param name="animationPlayRate">開くアニメーションの再生速度倍率</param>
        /// <returns></returns>
        UniTask Open(float animationPlayRate = 1.0f);

        /// <summary>
        /// ウィジェットを閉じる
        /// </summary>
        /// <param name="playsAnimation">閉じるアニメーションを再生するか</param>
        /// <param name="animationPlayRate">閉じるアニメーションの再生速度倍率</param>
        /// <param name="finishedCallback">閉じるアニメーションが終わった時のイベント</param>
        void Close(bool playsAnimation = true, float animationPlayRate = 1.0f, Action finishedCallback = null);

        /// <summary>
        /// ウィジェットを閉じる（非同期処理版）
        /// </summary>
        /// <param name="animationPlayRate">閉じるアニメーションの再生速度</param>
        /// <returns></returns>
        UniTask Close(float animationPlayRate = 1.0f);

        /// <summary>
        /// ウィジェットのアニメーションを再生する
        /// </summary>
        /// <param name="playName">アニメーション名</param>
        /// <param name="playRate">再生速度倍率</param>
        /// <param name="finishedCallback">終了イベント</param>
        void PlayAnimation(Name playName, float playRate = 1.0f, Action finishedCallback = null);

        /// <summary>
        /// ウィジェットのアニメーションを再生する
        /// </summary>
        /// <param name="playName"></param>
        /// <param name="playRate"></param>
        /// <returns></returns>
        UniTask PlayAnimation(Name playName, float playRate = 1.0f);

        /// <summary>
        /// 表示中か
        /// </summary>
        /// <returns></returns>
        bool IsShowing();
    }
}