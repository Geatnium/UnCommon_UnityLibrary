using UnityEngine;
using UnityEngine.EventSystems;

namespace UnCommon
{
    /// <summary>
    /// ComponentBaseクラスのインターフェース
    /// </summary>
    public interface IComponent : IEventSystemHandler
    {
        /// <summary>
        /// OnUpdateイベントを行うかどうかを切り替える
        /// </summary>
        /// <param name="isUpdateEnabled"></param>
        void SetUpdateEnabled(bool isUpdateEnabled);

        /// <summary>
        /// OnLateUpdateイベントを行うかどうかを切り替える
        /// </summary>
        /// <param name="isLateUpdateEnabled"></param>
        void SetLateUpdateEnabled(bool isLateUpdateEnabled);

        /// <summary>
        /// OnTickを行うかどうかを切り替える
        /// </summary>
        /// <param name="isTickEnabled"></param>
        void SetTickEnabled(bool isTickEnabled);

        /// <summary>
        /// OnTickの時間間隔を変更する
        /// </summary>
        /// <param name="tickInterval">時間間隔</param>
        void SetTickInterval(int tickInterval);

        /// <summary>
        /// OnFixedUpdateを行うかどうかを切り替える
        /// </summary>
        /// <param name="isTickEnabled"></param>
        void SetFixedUpdateEnabled(bool isTickEnabled);

        /// <summary>
        /// インターフェースが実装されているコンポーネントを持っているゲームオブジェクト
        /// </summary>
        /// <returns></returns>
        GameObject GetOwnerGameObject();

        /// <summary>
        /// インターフェースが実装されているコンポーネントを持っているゲームオブジェクトのトランスフォーム
        /// </summary>
        /// <returns></returns>
        Transform GetOwnerTransform();

        /// <summary>
        /// コンポーネントをデタッチして破棄する
        /// </summary>
        void Destroy();

        /// <summary>
        /// アタッチされているゲームオブジェクトごと破棄する
        /// </summary>
        void DestroyOwnerGameObject();
    }
}