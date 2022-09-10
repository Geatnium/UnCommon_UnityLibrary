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
    /// <br>アクターの子になるゲームオブジェクト</br>
    /// </summary>
    public class ChildActor : ComponentBase, IChildActor, ITag
    {
        //---------------------------- パラメータ ----------------------------//
        #region パラメータ

        [Foldout("Child Actor")]

        [SerializeField, Tooltip("タグ")]
        private NameHashSet tags;

        #endregion


        //---------------------------- メンバー変数 ----------------------------//
        #region メンバー変数

        /// <summary>
        /// 親のアクター
        /// </summary>
        [SerializeField, HideInInspector]
        private Actor parentActor;

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



        #endregion


        //---------------------------- インターフェース関数 ----------------------------//
        #region インターフェース関数

        public Actor GetParent()
        {
            return parentActor;
        }

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
            parentActor = ownerGameObject.GetComponentInParent<Actor>();

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