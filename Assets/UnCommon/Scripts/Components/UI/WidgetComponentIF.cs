using System;
using UnityEngine;
using UnCommon;

namespace UnCommon
{
    /// <summary>
    /// ウィジェットコンポーネントのインターフェース
    /// </summary>
    public interface IWidgetComponent : IComponent
    {
        /// <summary>
        /// ウィジェットが表示され、オープンアニメーションが開始された時
        /// </summary>
        void OnStartOpen();

        /// <summary>
        /// オープンアニメーションが終了した時（アニメーションが再生されない場合、OnStartOpen()の直後に呼ばれる）
        /// </summary>
        void OnFinishedOpen();

        /// <summary>
        /// ウィジェットのクローズアニメーション開始時
        /// </summary>
        void OnStartClose();

        /// <summary>
        /// クローズアニメーションも終わって、ウィジェットが非表示になる直前
        /// </summary>
        void OnFinishedClose();
    }
}