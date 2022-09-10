using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MarkupAttributes;
using UnCommon;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnCommon
{

    /// <summary>
    /// コリジョンの判定を監視して、イベントを発行する
    /// </summary>
    public class CollisionEvent : ChildActorComponentBase, ITag
    {
        //---------------------------- パラメータ ----------------------------//
        #region パラメータ

        [Foldout("Collision")]

        [SerializeField, Tooltip("コリジョンのタグ")]
        private NameHashSet tags;

        [SerializeField, Tooltip("コライダーの形状")]
        private ShapeType colliderShape;

        [SerializeField, Tooltip("コリジョンのレイヤー")]
        private CollisionLayer layer;

        [SerializeField, Tooltip("コリジョンの判定タイプ")]
        private CollisionType collisionType;

        #endregion


        //---------------------------- メンバー変数 ----------------------------//
        #region メンバー変数

        /// <summary>
        /// コライダー
        /// </summary>
        [SerializeField, HideInInspector]
        private Collider selfCollider = null;

        /// <summary>
        /// コリジョンイベントの発行先
        /// </summary>
        [SerializeField, HideInInspector]
        private ICollisionEventHandler targetCollisionEventHandler;

        /// <summary>
        /// トリガーイベントの発行先
        /// </summary>
        [SerializeField, HideInInspector]
        private ITriggerEventHandler targetTriggerEventHandler;

        #endregion


        //---------------------------- エディタ ----------------------------//
        #region エディタ

#if UNITY_EDITOR

        // コンポーネントをアタッチした時に一回だけ呼ばれる（もしくは右クリックメニューのResetを押した時）（Editor Only）
        protected override void OnReset()
        {
            base.OnReset();
            // ここでどのイベント関数を行うか決める
            SetComponentEventsEnabled(
                isUpdateEnabled: false,
                isUpdateJobEnabled: false,
                isLateUpdateEnabled: false,
                isFixedUpdateEnabled: false,
                isFixedUpdateJobEnabled: false,
                isTickEnabled: false);
            // ここでイベント関数の優先順位を決める
            SetComponentEventsOrder(
                updateOrder: 0,
                fixedUpdateOrder: 0);
        }

        // エディタでパラメータなどが変更された時に呼ばれる
        protected override void OnConstruct()
        {
            base.OnConstruct();
            // 形状の設定によってコライダーのコンポーネントを追加する
            switch (colliderShape)
            {
                case ShapeType.None:
                    EditorApplication.delayCall += () =>
                    {
                        if (selfCollider != null)
                        {
                            DestroyImmediate(selfCollider, true);
                            selfCollider = null;
                            while (true)
                            {
                                if (TryGetComponent(out Collider collider))
                                {
                                    DestroyImmediate(collider, true);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    };
                    break;
                case ShapeType.Box:
                    if (!ownerGameObject.TryGetComponent(out BoxCollider boxCollider))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (selfCollider != null)
                                DestroyImmediate(selfCollider, true);
                            selfCollider = ownerGameObject.AddComponent<BoxCollider>();
                        };

                    }
                    break;
                case ShapeType.Sphere:
                    if (!ownerGameObject.TryGetComponent(out SphereCollider sphereCollider))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (selfCollider != null)
                                DestroyImmediate(selfCollider, true);
                            selfCollider = ownerGameObject.AddComponent<SphereCollider>();
                        };
                    }
                    break;
                case ShapeType.Capsule:
                    if (!ownerGameObject.TryGetComponent(out CapsuleCollider capsuleCollider))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (selfCollider != null)
                                DestroyImmediate(selfCollider, true);
                            selfCollider = ownerGameObject.AddComponent<CapsuleCollider>();
                        };
                    }
                    break;
                case ShapeType.Mesh:
                    if (!ownerGameObject.TryGetComponent(out MeshCollider meshCollider))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (selfCollider != null)
                                DestroyImmediate(selfCollider, true);
                            selfCollider = ownerGameObject.AddComponent<MeshCollider>();
                        };
                    }
                    break;
                default:
                    break;
            }
            // コリジョンの判定タイプによって、コライダーの設定を変える
            if (ownerGameObject.TryGetComponent(out Collider collider))
            {
                switch (collisionType)
                {
                    case CollisionType.NoCollision:
                        collider.enabled = false;
                        break;
                    case CollisionType.BlockingOnly:
                    case CollisionType.BlockingAndSendEvent:
                        collider.enabled = true;
                        collider.isTrigger = false;
                        break;
                    case CollisionType.Trigger:
                        collider.enabled = true;
                        if (ownerGameObject.TryGetComponent(out MeshCollider meshCollider))
                        {
                            meshCollider.convex = true;
                        }
                        collider.isTrigger = true;
                        break;
                    default:
                        break;
                }
            }
            // レイヤーの設定
            if (ownerGameObject.layer != LayerMask.NameToLayer(layer.ToString()))
            {
                ownerGameObject.layer = LayerMask.NameToLayer(layer.ToString());
            }
            // レイヤーがStaticだったら、ゲームオブジェクトをStaticにする
            ownerGameObject.isStatic = layer == CollisionLayer.Static;
        }

        // デバッグ（ギズモ）表示用のイベント
        protected override void OnDebugDraw()
        {
            base.OnDebugDraw();
        }

#endif

        #endregion


        //---------------------------- メンバー関数 ----------------------------//
        #region メンバー関数

        public void SetCollisionEnabled(bool isEnabled)
        {
            selfCollider.enabled = isEnabled;
        }

        #endregion

        //---------------------------- インターフェース関数 ----------------------------//
        #region インターフェース関数

        public void SetTags(NameHashSet tags)
        {
            this.tags = tags;
        }

        public void AddTag(Name tag)
        {
            this.tags.Add(tag);
        }

        public NameHashSet GetTags()
        {
            return this.tags;
        }

        public bool HasTag(Name tag)
        {
            return this.tags.Contains(tag);
        }

        #endregion


        //---------------------------- コリジョンイベント ----------------------------//
        #region コリジョンイベント

        private void OnCollisionEnter(Collision collision)
        {
            if (collisionType != CollisionType.BlockingAndSendEvent) return;

            if (!targetCollisionEventHandler.IsValid())
            {
                targetCollisionEventHandler = ownerParentActor.gameObject.GetComponent<ICollisionEventHandler>();
            }
            if (targetCollisionEventHandler.IsValid())
            {
                GameObject other = collision.gameObject;
                Actor othersActor = null;
                other.Execute<IChildActor>(ExecuteType.OnlyOne, (r) => { othersActor = r.GetParent(); });
                if (!othersActor.IsValid())
                {
                    othersActor = other.GetComponent<Actor>();
                }
                if (othersActor.IsValid())
                {
                    targetCollisionEventHandler.OnCollisionHitEnter(this, other, othersActor, collision);
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collisionType != CollisionType.BlockingAndSendEvent) return;

            if (!targetCollisionEventHandler.IsValid())
            {
                targetCollisionEventHandler = ownerParentActor.gameObject.GetComponent<ICollisionEventHandler>();
            }
            if (targetCollisionEventHandler.IsValid())
            {
                GameObject other = collision.gameObject;
                Actor othersActor = null;
                other.Execute<IChildActor>(ExecuteType.OnlyOne, (r) => { othersActor = r.GetParent(); });
                if (!othersActor.IsValid())
                {
                    othersActor = other.GetComponent<Actor>();
                }
                if (othersActor.IsValid())
                {
                    targetCollisionEventHandler.OnCollisionHitting(this, other, othersActor, collision);
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collisionType != CollisionType.BlockingAndSendEvent) return;

            if (!targetCollisionEventHandler.IsValid())
            {
                targetCollisionEventHandler = ownerParentActor.gameObject.GetComponent<ICollisionEventHandler>();
            }
            if (targetCollisionEventHandler.IsValid())
            {
                GameObject other = collision.gameObject;
                Actor othersActor = null;
                other.Execute<IChildActor>(ExecuteType.OnlyOne, (r) => { othersActor = r.GetParent(); });
                if (!othersActor.IsValid())
                {
                    othersActor = other.GetComponent<Actor>();
                }
                if (othersActor.IsValid())
                {
                    targetCollisionEventHandler.OnCollisionHitExit(this, other, othersActor, collision);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (collisionType != CollisionType.Trigger) return;

            if (!targetTriggerEventHandler.IsValid())
            {
                targetTriggerEventHandler = ownerParentActor.gameObject.GetComponent<ITriggerEventHandler>();
            }
            if (targetTriggerEventHandler.IsValid())
            {
                GameObject otherObj = other.gameObject;
                Actor othersActor = null;
                otherObj.Execute<IChildActor>(ExecuteType.OnlyOne, (r) => { othersActor = r.GetParent(); });
                if (!othersActor.IsValid())
                {
                    othersActor = other.GetComponent<Actor>();
                }
                if (othersActor.IsValid())
                {
                    targetTriggerEventHandler.OnTriggerOverlapEnter(this, other.gameObject, othersActor, other);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (collisionType != CollisionType.Trigger) return;

            if (!targetTriggerEventHandler.IsValid())
            {
                targetTriggerEventHandler = ownerParentActor.gameObject.GetComponent<ITriggerEventHandler>();
            }
            if (targetTriggerEventHandler.IsValid())
            {
                GameObject otherObj = other.gameObject;
                Actor othersActor = null;
                otherObj.Execute<IChildActor>(ExecuteType.OnlyOne, (r) => { othersActor = r.GetParent(); });
                if (!othersActor.IsValid())
                {
                    othersActor = other.GetComponent<Actor>();
                }
                if (othersActor.IsValid())
                {
                    targetTriggerEventHandler.OnTriggerOverlapStay(this, other.gameObject, othersActor, other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (collisionType != CollisionType.Trigger) return;

            if (!targetTriggerEventHandler.IsValid())
            {
                targetTriggerEventHandler = ownerParentActor.gameObject.GetComponent<ITriggerEventHandler>();
            }
            if (targetTriggerEventHandler.IsValid())
            {
                GameObject otherObj = other.gameObject;
                Actor othersActor = null;
                otherObj.Execute<IChildActor>(ExecuteType.OnlyOne, (r) => { othersActor = r.GetParent(); });
                if (!othersActor.IsValid())
                {
                    othersActor = other.GetComponent<Actor>();
                }
                if (othersActor.IsValid())
                {
                    targetTriggerEventHandler.OnTriggerOverlapExit(this, other.gameObject, othersActor, other);
                }
            }
        }

        #endregion


        //---------------------------- イベント ----------------------------//
        #region イベント

        // コンポーネントがインスタンスされた直後に一回だけ呼ばれる。
        protected override async UniTask OnAwake()
        {
            await base.OnAwake();
        }

        // コンポーネントがインスタンスされてから、次のフレームの直前に一回だけ呼ばれる。
        protected override async UniTask OnStart()
        {
            await base.OnStart();
            selfCollider = GetComponent<Collider>();
        }

        // フレームの更新時に毎回呼ばれる。(OnUpdate より先に呼ばれる)
        // 計算処理やジョブの発行をここで行う。
        // Unity標準のコンポーネントへの反映はここでは行わず、OnUpdate で行う。
        protected override void OnUpdateJob()
        {
            base.OnUpdateJob();
        }

        // フレームの更新時に毎回呼ばれる。（OnUpdateJob の後に呼ばれる)
        // OnUpdateJobなどで計算した値を反映する場合などに使う。
        // Unity標準コンポーネントへの値の反映などはここで行う。
        protected override async UniTask OnUpdate()
        {
            await base.OnUpdate();
        }

        // フレームの更新時に毎回呼ばれる。（更新の最後に呼ばれる。）
        // 全てのOnUpdateやアニメーション更新が終わった後に行いたい場合に使う。
        protected override async UniTask OnLateUpdate()
        {
            await base.OnLateUpdate();
        }

        // 固定周期で呼ばれる。（デフォルトでは0.016s間隔）（OnFixedUpdate より先に呼ばれる)
        // 計算処理やジョブの発行をここで行う
        // Unity標準のコンポーネントへの反映はここでは行わず、OnFixedUpdateで行う。
        // 主に物理計算などに使う。
        protected override void OnFixedUpdateJob()
        {
            base.OnFixedUpdateJob();
        }

        // 固定周期で呼ばれる。（デフォルトでは0.016s間隔）(OnFixedUpdateJob の後に呼ばれる)
        // OnFixedUpdateJobなどで計算した値を反映する場合などに使う。
        // Unity標準コンポーネントへの値の反映などはここで行う。
        protected override async UniTask OnFixedUpdate()
        {
            await base.OnFixedUpdate();
        }

        // 指定した一定時間の間隔で呼ばれる（一定時間の指定はtickInterval）。
        // 毎フレームは処理しなくていい時に使う。
        protected override async UniTask OnTick()
        {
            await base.OnTick();
        }

        // OnStartの直後と、コンポーネントがアクティブになった時に呼ばれる。
        protected override async UniTask OnComponentEnabled()
        {
            await base.OnComponentEnabled();
        }

        // コンポーネントが非アクティブになった時に呼ばれる。
        protected override async UniTask OnComponentDisabled()
        {
            await base.OnComponentDisabled();
        }

        // コンポーネントが破棄された時に呼ばれる。
        protected override async UniTask OnComponentDestroyed()
        {
            await base.OnComponentDestroyed();
        }

        #endregion
    }
}