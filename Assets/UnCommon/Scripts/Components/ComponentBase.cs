using UnityEngine;
using MarkupAttributes;
using System;
using Cysharp.Threading.Tasks;
using Unity.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnCommon
{
    /// <summary>
    /// <br>コンポーネントのベースクラス</br>
    /// <br>ComponentManagerと連携して、イベント関数などを扱いやすくする</br>
    /// </summary>
    public abstract partial class ComponentBase : MonoBehaviour, IComponent
    {
        //---------------------------- パラメータ ----------------------------//
        #region パラメータ

        [Foldout("Component Event")]

        [Foldout("Component Event/Update")]

        [SerializeField, Tooltip("Updateイベントを行うか。毎フレーム呼ばれるイベント。")]
        protected bool isUpdateEnabled = true;

        [SerializeField, Tooltip("Updateイベントでのジョブ発行を行うか。別スレッドで処理したい時に使う。")]
        protected bool isUpdateJobEnabled = false;

        [SerializeField, Tooltip("LateUpdateイベント行うか。毎フレームの最後に呼ばれるイベント。")]
        protected bool isLateUpdateEnabled = false;

        [SerializeField, Tooltip("Updateイベントが発行される順番の優先順位。小さい方が先に呼ばれる。")]
        protected int updateOrder = 0;


        [Foldout("Component Event/FixedUpdate")]

        [SerializeField, Tooltip("FixedUpdateイベントを行うかどうか。固定時間間隔で呼ばれる。物理挙動の更新など。")]
        protected bool isFixedUpdateEnabled = true;

        [SerializeField, Tooltip("FixedUpdateイベントのジョブ発行を行うかどうか。別スレッドで処理したい時に使う。")]
        protected bool isFixedUpdateJobEnabled = false;

        [SerializeField, Tooltip("FixedUpdateイベントが発行される順番の優先順位。小さい方が先に呼ばれる。")]
        protected int fixedUpdateOrder = 0;


        [Foldout("Component Event/Tick")]

        [SerializeField, Tooltip("Tickイベントを行うかどうか。TickIntervalに指定した時間間隔で呼ばれる。毎フレームは処理しなくていいところで使う。")]
        protected bool isTickEnabled = false;

        [SerializeField, Tooltip("Tickイベントが呼ばれる時間間隔（s）。フレームレートより多くは呼ばれない。")]
        protected float tickInterval = 0.1f;


        [Foldout("Debug")]

        [SerializeField, Tooltip("デバッグ表示を有効にするか")]
        protected bool drawsDebugGizmos = true;

        #endregion


        //---------------------------- メンバー変数 ----------------------------//
        #region メンバー変数

        [Obsolete("selfGameObjectを参照すること", true)]
        protected new GameObject gameObject;

        [Obsolete("selfTransformを参照すること", true)]
        protected new Transform transform;

        /// <summary>
        /// 自身のゲームオブジェクト
        /// </summary>
        [SerializeField, HideInInspector]
        protected GameObject selfGameObject;

        /// <summary>
        /// 自身のトランスフォーム
        /// </summary>
        [SerializeField, HideInInspector]
        protected Transform selfTransform;

        /// <summary>
        /// OnUpdateの時間間隔（Time.deltaTime に何度もアクセスしないようにキャッシュ）
        /// </summary>
        protected float deltaTime;

        /// <summary>
        /// OnFixedUpdateの時間間隔（Time.fixedDeltaTime に何度もアクセスしないようにキャッシュ）
        /// </summary>
        protected float fixedDeltaTime;

        /// <summary>
        /// ComponentManagerのインスタンス
        /// </summary>
        protected IComponentManager componentManager;

        /// <summary>
        /// Startイベントが呼び終わったか
        /// </summary>
        private bool isFinishedStartEvent = false;

        /// <summary>
        /// エディタで止めた時の検知用
        /// </summary>
        private bool isQuitting = false;

        #endregion


        //---------------------------- エディタ ----------------------------//
        #region エディタ

#if UNITY_EDITOR

        /// <summary>
        /// コンポーネントをアタッチした時に一回だけ呼ばれる（もしくは右クリックメニューのResetを押した時）（Editor Only）
        /// </summary>
        protected virtual void OnReset() { }

        /// <summary>
        /// デバッグ（ギズモ）表示用のイベント
        /// </summary>
        protected virtual void OnDebugDraw() { }

        /// <summary>
        /// エディタでパラメータなどが変更された時に呼ばれる
        /// </summary>
        protected virtual void OnConstruct() { }

        // リセットイベントを呼ぶ　派生クラスではOnResetをオーバーライドする
        private void Reset()
        {
            // 派生クラスでも使えるように取得
            selfGameObject = base.gameObject;
            selfTransform = base.transform;
            // イベント発行
            OnReset();
        }

        // エディタでパラメータなどが変更された時に呼ばれる
        private void OnValidate()
        {
            // 派生クラスでも使えるように取得
            selfGameObject = base.gameObject;
            selfTransform = base.transform;
            // プレイ中じゃない時だけ発行する
            if (!Application.isPlaying)
            {
                OnConstruct();
            }
        }

        // ギズモ表示のイベント
        private void OnDrawGizmos()
        {
            // デバッグ表示が有効になっていたら発行
            if (drawsDebugGizmos)
            {
                OnDebugDraw();
            }
        }

#endif

        #endregion


        //---------------------------- メンバー関数 ----------------------------//
        #region メンバー関数

        /// <summary>
        /// コンポーネントのイベントをまとめて設定する
        /// </summary>
        /// <param name="isUpdateEnabled">Updateイベント</param>
        /// <param name="isUpdateJobEnabled">UpdateJobイベント</param>
        /// <param name="isFixedUpdateEnabled">FixedUpdate</param>
        /// <param name="isFixedUpdateJobEnabled">FixedUpdateJobイベント</param>
        /// <param name="isTickEnabled">Tickイベント</param>
        protected void SetComponentEventsEnabled(bool isUpdateEnabled, bool isUpdateJobEnabled, bool isLateUpdateEnabled, bool isFixedUpdateEnabled, bool isFixedUpdateJobEnabled, bool isTickEnabled)
        {
            this.isUpdateEnabled = isUpdateEnabled;
            this.isUpdateJobEnabled = isUpdateJobEnabled;
            this.isLateUpdateEnabled = isLateUpdateEnabled;
            this.isFixedUpdateEnabled = isFixedUpdateEnabled;
            this.isFixedUpdateJobEnabled = isFixedUpdateJobEnabled;
            this.isTickEnabled = isTickEnabled;
        }

        /// <summary>
        /// コンポーネントのイベントのオーダー（実行順序の優先順位）をまとめて設定する
        /// </summary>
        /// <param name="updateOrder">Updateイベントのオーダー</param>
        /// <param name="fixedUpdateOrder">FixedUpdateイベントのオーダー</param>
        protected void SetComponentEventsOrder(int updateOrder, int fixedUpdateOrder)
        {
            this.updateOrder = updateOrder;
            this.fixedUpdateOrder = fixedUpdateOrder;
        }

        /// <summary>
        /// Updateイベントの優先順位を設定
        /// </summary>
        /// <param name="order">オーダー</param>
        protected void SetUpdateOrder(int order)
        {
            this.updateOrder = order;
        }

        /// <summary>
        /// FixedUpdateイベントの優先順位を設定
        /// </summary>
        /// <param name="order">オーダー</param>
        protected void SetFixedUpdateOrder(int order)
        {
            this.fixedUpdateOrder = order;
        }

        #endregion


        //---------------------------- インターフェース関数 ----------------------------//
        #region インターフェース関数

        public virtual void SetUpdateEnabled(bool isUpdateEnabled)
        {
            this.isUpdateEnabled = isUpdateEnabled;
            // 有効にするときは、Updateイベントが行われるように登録する
            if (isUpdateEnabled)
            {
                componentManager.AddUpdateEvent(
                        updateOrder: updateOrder,
                        registerUpdateEvent: OnUpdate,
                        registerUpdateJobEvent: isUpdateJobEnabled ? OnUpdateJob : null);
            }
            else // 無効にするときは、Updateイベントを行わないようにから除外する
            {
                componentManager.RemoveUpdateEvent(
                        updateOrder: updateOrder,
                        removeUpdateEvent: OnUpdate,
                        removeUpdateJobEvent: isUpdateJobEnabled ? OnUpdateJob : null);
            }
        }

        public virtual void SetLateUpdateEnabled(bool isLateUpdateEnabled)
        {
            this.isLateUpdateEnabled = isLateUpdateEnabled;
            // 有効にするときは、LateUpdateイベントが行われるように登録する
            if (isLateUpdateEnabled)
            {
                componentManager.AddLateUpdateEvent(
                        updateOrder: updateOrder,
                        registerLateUpdateEvent: OnLateUpdate);
            }
            else // 無効にするときは、LateUpdateイベントを行わないようにから除外する
            {
                componentManager.RemoveLateUpdateEvent(
                        updateOrder: updateOrder,
                        removeLateUpdateEvent: OnLateUpdate);
            }
        }

        public virtual void SetFixedUpdateEnabled(bool isFixedUpdateEnabled)
        {
            this.isFixedUpdateEnabled = isFixedUpdateEnabled;
            // 有効にするときは、FixedUpdateイベントが行われるように登録する
            if (isFixedUpdateEnabled)
            {
                componentManager.AddFixedUpdateEvent(
                        fixedUpdateOrder: fixedUpdateOrder,
                        registerFixedUpdateEvent: OnFixedUpdate,
                        registerFixedUpdateJobEvent: isFixedUpdateJobEnabled ? OnFixedUpdateJob : null);
            }
            else // 無効にするときは、FixedUpdateイベントを行わないようにから除外する
            {
                componentManager.RemoveFixedUpdateEvent(
                        fixedUpdateOrder: fixedUpdateOrder,
                        removeFixedUpdateEvent: OnFixedUpdate,
                        removeFixedUpdateJobEvent: isFixedUpdateJobEnabled ? OnFixedUpdateJob : null);
            }
        }

        public virtual void SetTickEnabled(bool isTickEnabled)
        {
            this.isTickEnabled = isTickEnabled;
            // 有効にするときは、Tickイベントが行われるように登録する
            if (isTickEnabled)
            {
                componentManager.AddTickEvent(
                        registerTickEvent: OnTick,
                        tickInterval: tickInterval);
            }
            else // 無効にするときは、Tickイベントを行わないようにから除外する
            {
                componentManager.RemoveTickEvent(
                    removeTickEvent: OnTick);
            }
        }

        public virtual void SetTickInterval(int tickInterval)
        {
            this.tickInterval = tickInterval;
            // tickInterbal更新のためすでに登録されていたら一回除外してから再度登録する
            if (isTickEnabled)
            {
                componentManager.RemoveTickEvent(
                    removeTickEvent: OnTick);
                componentManager.AddTickEvent(
                    registerTickEvent: OnTick,
                    tickInterval: this.tickInterval);
            }
        }

        public GameObject GetGameObject()
        {
            return selfGameObject;
        }

        public Transform GetTransform()
        {
            return selfTransform;
        }

        #endregion


        //---------------------------- イベント ----------------------------//
        #region イベント

        /// <summary>
        /// コンポーネントがインスタンスされた直後に一回だけ呼ばれる。<br></br>
        /// 基本使い所はない。他のコンポーネントより早く初期化したい時に使う。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnAwake()
        {
            // 自身の参照を持っておく
            selfGameObject = base.gameObject;
            selfTransform = base.transform;
        }

        /// <summary>
        /// コンポーネントがインスタンスされてから、次のフレームの直前に一回だけ呼ばれる。<br></br>
        /// 変数の初期化や、ゲームオブジェクトやコンポーネントを参照する時に使う。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnStart() { }

        /// <summary>
        /// フレームの更新時に毎回呼ばれる。(OnUpdate より先に呼ばれる)<br></br>
        /// 計算処理やジョブの発行をここで行う。<br></br>
        /// Unity標準のコンポーネントへの反映はここでは行わず、OnUpdate で行う。
        /// </summary>
        protected virtual void OnUpdateJob()
        {
            deltaTime = Time.deltaTime;
        }

        /// <summary>
        /// フレームの更新時に毎回呼ばれる。（OnUpdateJob の後に呼ばれる）<br></br>
        /// OnUpdateJobなどで計算した値を反映する場合などに使う。<br></br>
        /// Unity標準コンポーネントへの値の反映などはここで行う。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnUpdate()
        {
            deltaTime = Time.deltaTime;
        }

        /// <summary>
        /// フレームの更新時に毎回呼ばれる。（更新の最後に呼ばれる。）<br></br>
        /// 全てのOnUpdateやアニメーション更新が終わった後に行いたい場合に使う。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnLateUpdate() { }

        /// <summary>
        /// 指定した一定時間の間隔で呼ばれる（一定時間の指定はtickInterval）。<br></br>
        /// 毎フレームは処理しなくていい時に使う。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnTick() { }

        /// <summary>
        /// 固定周期で呼ばれる。（デフォルトでは0.016s間隔）（OnFixedUpdate より先に呼ばれる）<br></br>
        /// 計算処理やジョブの発行をここで行う<br></br>
        /// Unity標準のコンポーネントへの反映はここでは行わず、OnFixedUpdateで行う。<br></br>
        /// 主に物理計算などに使う。
        /// </summary>
        protected virtual void OnFixedUpdateJob()
        {
            fixedDeltaTime = Time.fixedDeltaTime;
        }

        /// <summary>
        /// 固定周期で呼ばれる。（デフォルトでは0.016s間隔）(OnFixedUpdateJob の後に呼ばれる)
        /// OnFixedUpdateJobなどで計算した値を反映する場合などに使う。<br></br>
        /// Unity標準コンポーネントへの値の反映などはここで行う。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnFixedUpdate()
        {
            fixedDeltaTime = Time.fixedDeltaTime;
        }

        /// <summary>
        /// コンポーネントがアクティブになった時に呼ばれる。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnComponentEnabled()
        {
            // アクティブ化されたら、イベントが呼ばれるように登録
            SetUpdateEnabled(isUpdateEnabled: isUpdateEnabled);
            SetLateUpdateEnabled(isLateUpdateEnabled: isLateUpdateEnabled);
            SetFixedUpdateEnabled(isFixedUpdateEnabled: isFixedUpdateEnabled);
            SetTickEnabled(isTickEnabled: isTickEnabled);
        }

        /// <summary>
        /// コンポーネントが非アクティブになった時に呼ばれる。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnComponentDisabled()
        {
            // 非アクティブのときは、イベントが呼ばれないよう除外
            componentManager.RemoveUpdateEvent(
                    updateOrder: updateOrder,
                    removeUpdateEvent: OnUpdate,
                    isUpdateJobEnabled ? OnUpdateJob : null);
            componentManager.RemoveLateUpdateEvent(
                    updateOrder: updateOrder,
                    removeLateUpdateEvent: OnLateUpdate);
            componentManager.RemoveTickEvent(removeTickEvent: OnTick);
            componentManager.RemoveFixedUpdateEvent(
                    fixedUpdateOrder: fixedUpdateOrder,
                    removeFixedUpdateEvent: OnFixedUpdate,
                    isFixedUpdateJobEnabled ? OnFixedUpdateJob : null);
        }

        /// <summary>
        /// コンポーネントが破棄された時に呼ばれる。
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask OnComponentDestroyed()
        {
            // そもそもComponentManagerが先に破棄されていたら、イベントも解除されているので何もしない
            if (!componentManager.IsValid()) return;

            // 破棄されたら、イベントが呼ばれないよう除外
            componentManager.RemoveUpdateEvent(
                    updateOrder: updateOrder,
                    removeUpdateEvent: OnUpdate,
                    isUpdateJobEnabled ? OnUpdateJob : null);
            componentManager.RemoveLateUpdateEvent(
                    updateOrder: updateOrder,
                    removeLateUpdateEvent: OnLateUpdate);
            componentManager.RemoveTickEvent(
                    removeTickEvent: OnTick);
            componentManager.RemoveFixedUpdateEvent(
                    fixedUpdateOrder: fixedUpdateOrder,
                    removeFixedUpdateEvent: OnFixedUpdate,
                    isFixedUpdateJobEnabled ? OnFixedUpdateJob : null);
        }

        // 起動イベントを呼ぶ　派生クラスではOnAwakeをオーバーライドする
        private void Awake()
        {
            OnAwake().Forget();
        }

        // スタートイベントを呼ぶ　派生クラスではOnStartをオーバーライドする
        private void Start()
        {
            // Startイベントはインスタンスされてから一回しか行われない
            if (isFinishedStartEvent) return;
            NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
            // ComponentManager のインスタンスを取得
            componentManager = ServiceLocator.GetInstance<IComponentManager>();
            // イベント発行
            OnStart().Forget();
            OnComponentEnabled().Forget();
            // Startイベント終わったフラグ
            isFinishedStartEvent = true;
        }

        // 有効化イベントを呼ぶ　派生クラスではOnComponentEnableをオーバーライドする
        private void OnEnable()
        {
            // インスタンス時のStartイベントよりは先に呼ばれて欲しくない（単純にインスタンス後に有効化された時のみ呼ばれてほしい。）
            if (!isFinishedStartEvent) return;

            OnComponentEnabled().Forget();
        }

        // アプリケーションの停止イベント
        private void OnApplicationQuit()
        {
            // エディタの停止ボタンによるものか
            isQuitting = true && Application.isEditor;
        }

        // 無効化イベントを呼ぶ　派生クラスではOnComponentDisableをオーバーライドする
        private void OnDisable()
        {
            // 停止時は呼ばない
            if (isQuitting) return;
            OnComponentDisabled().Forget();
        }

        // 破棄イベントを呼ぶ　派生クラスではOnComponentDestroyをオーバーライドする
        private void OnDestroy()
        {
            // 停止時は呼ばない
            if (isQuitting) return;
            OnComponentDestroyed().Forget();
        }

        #endregion
    }
}