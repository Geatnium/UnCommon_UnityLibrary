using UnCommon;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// コリジョンの判定を受け取るためのインターフェース
/// </summary>
public interface ICollisionEventHandler : IEventSystemHandler
{
    /// <summary>
    /// 入れ子のコライダーが他のコライダーに当たった時に呼ばれる
    /// </summary>
    /// <param name="ownerCollider">イベントを発行したコライダー</param>
    /// <param name="other">当たったコライダーのゲームオブジェクト</param>
    /// <param name="otherRootObject">当たったコライダーのルートオブジェクト</param>
    /// <param name="hitResult">ヒット情報</param>
    void OnCollisionHitEnter(ColliderEvent ownerCollider, GameObject other, Actor othersActor, Collision hitResult);

    /// <summary>
    /// 入れ子のコライダーが他のコライダーに触れている間に呼ばれる
    /// </summary>
    /// <param name="ownerCollisionObject">イベントを発行したコライダー</param>
    /// <param name="other">触れているコライダー</param>
    /// <param name="otherRootObject">触れているコライダーのルートオブジェクト</param>
    /// <param name="collision">ヒット情報</param>
    void OnCollisionHitting(ColliderEvent ownerCollider, GameObject other, Actor othersActor, Collision hitResult);

    /// <summary>
    /// 入れ子のコライダーが他のコライダーに触れていた状態から離れた時に呼ばれる
    /// </summary>
    /// <param name="ownerCollisionObject">イベントを発行したコライダー</param>
    /// <param name="other">離れたコライダー</param>
    /// <param name="otherRootObject">離れたコライダーを持っているルートオブジェクト</param>
    /// <param name="collision">ヒット情報</param>
    void OnCollisionHitExit(ColliderEvent ownerCollider, GameObject other, Actor othersActor, Collision hitResult);
}