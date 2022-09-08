using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using MarkupAttributes;
using static UnCommon.StandardUtility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnCommon
{

    /// <summary>
    /// <br>シーンマネージャー</br>
    /// <br>シーン切り替えやロードなどを取りまとめる</br>
    /// </summary>
    public class SceneManager : ManagerBase<ISceneManager>, ISceneManager
    {
        //---------------------------- パラメータ ----------------------------//
        #region パラメータ

        //[Foldout("Setup Parameters")]

        //[SerializeField, Tooltip("ロードUI")]

        #endregion


        //---------------------------- メンバー変数 ----------------------------//
        #region メンバー変数

        /// <summary>
        /// 現在いるシーン
        /// </summary>
        private Name currentScene;

        /// <summary>
        /// 次に設定されて現在読み込み中で遷移待機中のシーン
        /// </summary>
        private Name nextScene = Name.None;

        /// <summary>
        /// 前にいたシーン
        /// </summary>
        private Name previousScene = Name.None;

        /// <summary>
        /// ロード
        /// </summary>
        private AsyncOperationHandle<SceneInstance> loadingNextSceneHandle;

        /// <summary>
        /// ローディングで使用するシーン
        /// </summary>
        private readonly Name LoadingScene = "Loading";

        /// <summary>
        /// ロード完了とみなす進捗度
        /// </summary>
        private const float loadingCompleteThreshold = 0.9f;

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
            isResident = true;
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

        private async UniTask OpenSceneAfterLoad(float fadeDuration)
        {
            DebugLogger.Log($"{nextScene}に遷移開始");
            IUIManager uiManager;
            if (ServiceLocator.TryGetInstance(out uiManager))
            {
                await uiManager.FadeOut(Color.black, fadeDuration, Zerof);
            }
            AsyncOperationHandle<SceneInstance> loadingHandle = Addressables.LoadSceneAsync("Loading", LoadSceneMode.Single, false);
            await loadingHandle.ToUniTask(this);
            if(loadingHandle.Status == AsyncOperationStatus.Succeeded)
            {
                await loadingHandle.Result.ActivateAsync().ToUniTask();
                Addressables.Release(loadingHandle);
                DebugLogger.Log($"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}ロード画面遷移");
                if (loadingNextSceneHandle.IsValid())
                {
                    Addressables.Release(loadingNextSceneHandle);
                }
                loadingNextSceneHandle = Addressables.LoadSceneAsync(nextScene.ToString(), LoadSceneMode.Single, false);
                DebugLogger.Log($"{nextScene}のロード開始");
                await UniTask.Delay(2.0f.ToMilliSecondsInt());
                await loadingNextSceneHandle.ToUniTask(this);
                DebugLogger.Log($"{nextScene}のロード完了！");
                if (loadingNextSceneHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    await loadingNextSceneHandle.Result.ActivateAsync().ToUniTask();
                    previousScene = currentScene;
                    currentScene = nextScene;
                    nextScene = Name.None;
                }
            }
            
        }

        #endregion


        //---------------------------- インターフェース関数 ----------------------------//
        #region インターフェース関数

        public void OpenScene(Name nextScene, float fadeDuration = 1.0f)
        {
            if (this.nextScene != Name.None) return;
            this.nextScene = nextScene;
            OpenSceneAfterLoad(fadeDuration).Forget();
        }

        public Name GetNextSceneName()
        {
            return nextScene;
        }

        public Name PrevSceneName()
        {
            return previousScene;
        }

        public float GetLoadingSceneProgress()
        {
            if (!loadingNextSceneHandle.IsValid())
            {
                return Zerof;
            }
            //if (loadingNextSceneHandle.IsDone)
            //{
            //    return One;
            //}
            return (loadingNextSceneHandle.PercentComplete - 0.75f) / 0.25f;
        }

        #endregion


        //---------------------------- イベント ----------------------------//
        #region イベント

        // 初期化イベント
        // インスタンスにアクセスされた時に一度だけに呼ばれる
        protected override void Init()
        {
            base.Init();
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
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

        // コンポーネントがアクティブになった時に呼ばれる。
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