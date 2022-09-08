using UnCommon;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// トリガーの判定を受け取るためのインターフェース
/// </summary>
public interface ITriggerEventHandler : IEventSystemHandler
{
    /// <summary>
    /// 入れ子のトリガーが他のコライダーに触れた時に呼ばれる
    /// </summary>
    /// <param name="ownerCollider">イベントを発行したコライダー</param>
    /// <param name="other">触れたコライダー</param>
    /// <param name="othersActor">触れたコライダーのルートオブジェクト</param>
    /// <param name="hitResult">ヒットしたコライダー情報</param>
    void OnTriggerOverlapEnter(ColliderEvent ownerCollider, GameObject other, Actor othersActor, Collider hitResult);

    /// <summary>
    /// 入れ子のトリガーが他のコライダーに触れている間に呼ばれる
    /// </summary>
    /// <param name="ownerCollider">イベントを発行したコライダー</param>
    /// <param name="other">触れているコライダー</param>
    /// <param name="othersActor">触れているコライダーのルートオブジェクト</param>
    /// <param name="collider">ヒットしたコライダー情報</param>
    void OnTriggerOverlapStay(ColliderEvent ownerCollider, GameObject other, Actor othersActor, Collider hitResult);

    /// <summary>
    /// 入れ子のトリガーが他のコライダーに触れていた状態から離れた時に呼ばれる
    /// </summary>
    /// <param name="ownerCollider">イベントを発行したコライダー</param>
    /// <param name="other">離れたコライダー</param>
    /// <param name="othersActor">離れたコライダーのルートオブジェクト</param>
    /// <param name="collider">ヒットしたコライダー情報</param>
    void OnTriggerOverlapExit(ColliderEvent ownerCollider, GameObject other, Actor othersActor, Collider hitResult);
}