using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MarkupAttributes;
using UnCommon;
using static UnCommon.StandardUtility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnCommon
{
    /// <summary>
    /// UIのひとまとまり（ウィジェット）
    /// </summary>
    [RequireComponent(typeof(SimpleAnimation))]
    public class Widget : ComponentBase, IWidget
    {
        //---------------------------- パラメータ ----------------------------//
        #region パラメータ



        #endregion


        //---------------------------- メンバー変数 ----------------------------//
        #region メンバー変数

        /// <summary>
        /// SimpleAnimationコンポーネント保持用
        /// </summary>
        private SimpleAnimation simpleAnimation;

        /// <summary>
        /// 現在再生中のアニメーション
        /// </summary>
        private Name currentAnimationState = "None";

        /// <summary>
        /// 表示中か
        /// </summary>
        private bool isShowing;

        /// <summary>
        /// アニメーション再生終了イベント用
        /// </summary>
        private Action animationFinishedCallback;

        private readonly Name OpenAnimationState = "Open";

        private readonly Name CloseAnimationState = "Close";


        #endregion


        //---------------------------- エディタ ----------------------------//
        #region エディタ

#if UNITY_EDITOR

        // コンポーネントをアタッチした時に一回だけ呼ばれる（もしくは右クリックメニューのResetを押した時）（Editor Only）
        protected override void OnReset()
        {
            base.OnReset();
        }

        // エディタでパラメータなどが変更された時に呼ばれる
        protected override void OnConstruct()
        {
            base.OnConstruct();
            // このコンポーネントのイベント設定
            SetComponentEventsEnabled(
                    isUpdateEnabled: false, // OnUpdate() を行なうか
                    isUpdateJobEnabled: false, // OnUpdateJob() を行なうか
                    isLateUpdateEnabled: false, // OnLateUpdate() を行なうか
                    isFixedUpdateEnabled: false, // OnFixedUpdate() を行なうか
                    isFixedUpdateJobEnabled: false, // OnFixedUpdateJob() を行なうか
                    isTickEnabled: false); // OnTick を行なうか
            SetComponentEventsOrder(
                updateOrder: 0, // Update系イベントの優先順位
                fixedUpdateOrder: 0); // FixedUpdate系イベントの優先順位
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

        public void Open(bool playsAnimation = true, float animationPlayRate = 1.0f, Action finishedCallback = null)
        {
            ownerGameObject.SetActive(true);
            simpleAnimation = GetComponent<SimpleAnimation>();
            ownerGameObject.Execute<IWidgetComponent>(
               ExecuteType.AllComponents,
               (r) => r.OnStartOpen());
            if (playsAnimation)
            {
                Open(animationPlayRate).Forget();
            }
            else
            {
                ownerGameObject.Execute<IWidgetComponent>(
                   ExecuteType.AllComponents,
                   (r) => r.OnFinishedOpen());
            }
            isShowing = true;
        }

        public async UniTask Open(float animationPlayRate = 1.0f)
        {
            ownerGameObject.SetActive(true);
            await PlayAnimation(OpenAnimationState, animationPlayRate);
            ownerGameObject.Execute<IWidgetComponent>(
               ExecuteType.AllComponents,
               (r) => r.OnFinishedOpen());
        }

        public void Close(bool playsAnimation = true, float animationPlayRate = 1.0f, Action finishedCallback = null)
        {
            ownerGameObject.Execute<IWidgetComponent>(
               ExecuteType.AllComponents,
               (r) => r.OnStartClose());
            if (playsAnimation)
            {
                Close(animationPlayRate).Forget();
            }
            else
            {
                ownerGameObject.Execute<IWidgetComponent>(
                   ExecuteType.AllComponents,
                   (r) => r.OnFinishedClose());
                ownerGameObject.SetActive(false);
                isShowing = false;
            }
        }

        public async UniTask Close(float animationPlayRate = 1.0f)
        {
            await PlayAnimation(CloseAnimationState, animationPlayRate);
            ownerGameObject.Execute<IWidgetComponent>(
               ExecuteType.AllComponents,
               (r) => r.OnFinishedClose());
            ownerGameObject.SetActive(false);
            isShowing = false;
        }

        public void PlayAnimation(Name playName, float playRate = 1.0f, Action finishedCallback = null)
        {
            // すでに再生済みの場合何もしない
            if (currentAnimationState == playName) return;

            //前のコールバックが実行されているかチェック
            if (animationFinishedCallback != null)
            {
                DebugLogger.LogWarning($"「{playName}」の再生が開始されたため、「{currentAnimationState}」の再生を中止し、コールバックをすぐに実行します");
                animationFinishedCallback.Invoke();
            }

            // コールバック登録して再生開始
            animationFinishedCallback = finishedCallback;
            PlayAnimation(playName, playRate).Forget();
        }

        public async UniTask PlayAnimation(Name playName, float playRate = 1.0f)
        {
            // すでに再生済みの場合何もしない
            if (currentAnimationState == playName) return;

            // 指定されたアニメーションがあるかチェック
            if (simpleAnimation.GetState(playName) == null)
            {
                DebugLogger.LogWarning($"{ ownerGameObject.name} に {playName} というアニメーションはありません！");
                animationFinishedCallback?.Invoke(); // なければ即実行
                return;
            }

            // 再生速度を変えて再生開始
            simpleAnimation.animator.speed = playRate;
            simpleAnimation.Play(playName);
            currentAnimationState = playName;

            // 再生終了まで待つ
            while (true)
            {
                await UniTask.DelayFrame(One);
                if (simpleAnimation.GetState(playName).isValid)
                {
                    if (simpleAnimation.GetState(playName).normalizedTime >= 1.0f)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            // 再生終了したら、コールバック実行
            currentAnimationState = Name.None;
            animationFinishedCallback?.Invoke();
            animationFinishedCallback = null;
        }

        public bool IsShowing()
        {
            return isShowing;
        }

        #endregion


        //---------------------------- イベント ----------------------------//
        #region イベント

        // コンポーネントがインスタンスされた直後に一回だけ呼ばれる。
        protected override async UniTask OnAwake()
        {
            await base.OnAwake();
            // 生成されてもまだ表示はしない
            ownerGameObject.SetActive(false);
        }

        // コンポーネントがインスタンスされてから、次のフレームの直前に一回だけ呼ばれる。
        protected override async UniTask OnStart()
        {
            await base.OnStart();
            Canvas[] canvases = GetComponentsInChildren<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = Camera.main;
            }
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