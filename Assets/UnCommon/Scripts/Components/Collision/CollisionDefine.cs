using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// コリジョンの形状
    /// </summary>
    public enum ShapeType
    {
        [Tooltip("なし")]
        None,
        [Tooltip("キューブ、ボックス")]
        Box,
        [Tooltip("球")]
        Sphere,
        [Tooltip("カプセル")]
        Capsule,
        [Tooltip("メッシュ")]
        Mesh,
    }

    /// <summary>
    /// コリジョンの判定タイプ
    /// </summary>
    public enum CollisionType
    {
        [Tooltip("コリジョンなし")]
        NoCollision,
        [Tooltip("衝突はするがイベントは発行しない")]
        BlockingOnly,
        [Tooltip("衝突した時にイベントを発行する")]
        BlockingAndSendEvent,
        [Tooltip("衝突はしないが、重なった時にイベントを発行する")]
        Trigger
    }

    /// <summary>
    /// コリジョンのレイヤー
    /// </summary>
    public enum CollisionLayer
    {
        [InspectorName("Dynamic"), Tooltip("動的")]
        Dynamic,
        [InspectorName("Static"), Tooltip("静的")]
        Static,
        [InspectorName("")]
        Empty2,
        [InspectorName("")]
        Empty3,
        [InspectorName("")]
        Empty4,
        [InspectorName("")]
        Empty5,
        [InspectorName("Character"), Tooltip("キャラクターの移動判定")]
        Character,
        [InspectorName("Vehicle"), Tooltip("乗り物の移動判定")]
        Vehicle,
        [InspectorName("")]
        Empty8,
        [InspectorName("")]
        Empty9,
        [InspectorName("")]
        Empty10,
        [InspectorName("")]
        Empty11,
        [InspectorName("CharacterBody"), Tooltip("キャラクターの体")]
        CharacterBody,
        [InspectorName("VehicleBody"), Tooltip("乗り物の体")]
        VehicleBody,
        [InspectorName("")]
        Empty14,
        [InspectorName("")]
        Empty15,
        [InspectorName("")]
        Empty16,
        [InspectorName("")]
        Empty17,
        [InspectorName("Trigger"), Tooltip("トリガー")]
        Trigger,
        [InspectorName("")]
        Empty19,
        [InspectorName("")]
        Empty20,
        [InspectorName("")]
        Empty21,
        [InspectorName("")]
        Empty22
    }
}
