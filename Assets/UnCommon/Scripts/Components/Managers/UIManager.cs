using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using MarkupAttributes;
using static UnCommon.StandardUtility;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnCommon
{

    /// <summary>
    /// UIのマネージャー
    /// </summary>
    public class UIManager : ManagerBase<IUIManager>, IUIManager
    {
        //---------------------------- パラメータ ----------------------------//
        #region パラメータ

        [Foldout("Setup")]

        [SerializeField, Tooltip("UIのデータベース"), AssetReferenceUILabelRestriction("Database")]
        private List<AssetReference> uiDataObjects;

        [SerializeField, Tooltip("読み込み直後に生成したいUI")]
        private List<Name> defaultUILabels;

        [Foldout("Setup/Fade")]

        [SerializeField, Tooltip("シーン開始時のフェード時間（sec）")]
        private float fadeInDurationWhenStartScene = 1.0f;

        [SerializeField, Tooltip("シーン開始時のフェード遅延（sec）")]
        private float fadeInDelayWhenStartScene = 1.0f;

        [SerializeField, Tooltip("フェードUIのソートオーダー")]
        private int faderSortOrder = 10000;

        #endregion


        //---------------------------- メンバー変数 ----------------------------//
        #region メンバー変数

        /// <summary>
        /// 全UIの参照リスト
        /// </summary>
        private Dictionary<Name, AssetReference> assets = new();

        /// <summary>
        /// 現在インスタンスされているUIリスト
        /// </summary>
        private Dictionary<Name, GameObject> instances = new();

        /// <summary>
        /// フェードインフェードアウトに使うUIオブジェクト
        /// </summary>
        private GameObject faderCanvasObject;

        /// <summary>
        /// フェードインフェードアウト完了コールバック
        /// </summary>
        private Action fadeCallback;

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
            isResident = false; // シーンを跨いでも破棄しないようにするか
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

        public bool GetUIInstance(Name label, out Widget instance)
        {
            GameObject instanceObject;
            if(GetUIInstance(label, out instanceObject))
            {
                instance = instanceObject.GetComponent<Widget>();
                return true;
            }
            else
            {
                instance = null;
                return false;
            }
        }

        public bool GetUIInstance(Name label, out GameObject instance)
        {
            if (instances.ContainsKey(label))
            {
                if(instances.TryGetValue(label, out instance))
                {
                    return true;
                }
                else
                {
                    instance = null;
                    instances.Remove(label);
                    DebugLogger.LogWarning($"{label}は生成されている判定だが、インスタンスを取得できなかった。");
                    return false;
                }
            }
            else
            {
                if(assets.TryGetValue(label, out AssetReference uiAsset))
                {
                    instance = uiAsset.InstantiateAsync().WaitForCompletion();
                    instances.Add(label, instance);
                    return true;
                }
                else
                {
                    instance = null;
                    DebugLogger.Log($"{label}のUIデータが存在しない！UIDataに登録してください！");
                    return false;
                }
            }
        }

        public void DestroyUI(Name label)
        {
            if (instances.ContainsKey(label))
            {
                if(instances.TryGetValue(label, out GameObject instance))
                {
                    if (assets.TryGetValue(label, out AssetReference asset))
                    {
                        asset.ReleaseInstance(instance);
                        instances.Remove(label);
                    }
                }
                else
                {
                    DebugLogger.LogWarning($"UI破棄要求：{label}は生成されている判定だが、インスタンスが取得できず削除できなかった。");
                }
            }
            else
            {
                DebugLogger.Log($"UI破棄要求：{label}は生成されていない。");
            }
        }

        public bool ExistsUIInDatabase(Name label)
        {
            return assets.ContainsKey(label);
        }

        public bool ExistsUIInstance(Name label)
        {
            return instances.ContainsKey(label);
        }


        public void FadeIn(Color color, float duration = 1.0f, float delay = 1.0f, Action finishedCallback = null)
        {
            if (!fadeCallback.IsValid()) return;
            fadeCallback = finishedCallback;
            FadeIn(color, duration, delay).Forget();
        }

        public void FadeOut(Color color, float duration = 1.0f, float delay = 0.0f, Action finishedCallback = null)
        {
            if (!fadeCallback.IsValid()) return;
            fadeCallback = finishedCallback;
            FadeOut(color, duration, delay).Forget();
        }

        public async UniTask FadeIn(Color color, float duration = 1.0f, float delay = 1.0f)
        {
            Image image;
            if (faderCanvasObject == null)
            {
                faderCanvasObject = new GameObject("Fader");
                Canvas canvas = faderCanvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = faderSortOrder;
                image = faderCanvasObject.AddComponent<Image>();
            }
            else
            {
                image = faderCanvasObject.GetComponent<Image>();
            }
            image.color = color;
            image.SetAlpha(Onef);
            await UniTask.Delay(delay.ToMilliSecondsInt());
            float timer = Zerof;
            while(timer < duration)
            {
                float alpha = Mathf.Lerp(Onef, Zerof, timer / duration);
                image.SetAlpha(alpha);
                await UniTask.DelayFrame(One);
                timer += Time.deltaTime;
            }
            Destroy(faderCanvasObject);
            faderCanvasObject = null;
            if (fadeCallback.IsValid())
            {
                fadeCallback.Invoke();
                fadeCallback = null;
            }
        }

        public async UniTask FadeOut(Color color, float duration = 1.0f, float delay = 0.0f)
        {
            Image image;
            if (faderCanvasObject == null)
            {
                faderCanvasObject = new GameObject("Fader");
                Canvas canvas = faderCanvasObject.AddComponent<Canvas>();
                canvas.sortingOrder = faderSortOrder;
                image = faderCanvasObject.AddComponent<Image>();
            }
            else
            {
                image = faderCanvasObject.GetComponent<Image>();
            }
            image.color = color;
            image.SetAlpha(Zerof);
            await UniTask.Delay(delay.ToMilliSecondsInt());
            float timer = Zerof;
            while (timer < duration)
            {
                float alpha = Mathf.Lerp(Zerof, Onef, timer / duration);
                image.SetAlpha(alpha);
                await UniTask.DelayFrame(One);
                timer += Time.deltaTime;
            }
            if (fadeCallback.IsValid())
            {
                fadeCallback.Invoke();
                fadeCallback = null;
            }
        }

        #endregion


        //---------------------------- イベント ----------------------------//
        #region イベント

        // 初期化イベント
        // インスタンスにアクセスされた時に一度だけに呼ばれる
        protected override void Init()
        {
            base.Init();
            foreach (AssetReference uiDataObject in uiDataObjects)
            {
                UIData uiData = uiDataObject.LoadAssetAsync<UIData>().WaitForCompletion();
                NameToAssetReferenceDictionary uiDataAssets = uiData.uiAssets;
                int assetsCount = uiDataAssets.Count;
                Name[] dataKeys = new Name[assetsCount];
                AssetReference[] dataAssets = new AssetReference[assetsCount];
                uiDataAssets.Keys.CopyTo(dataKeys, Zero);
                uiDataAssets.Values.CopyTo(dataAssets, Zero);
                for (int i = 0; i < assetsCount; i++)
                {
                    assets.TryAdd(dataKeys[i], dataAssets[i]);
                }
            }
            foreach (Name defaultUILabel in defaultUILabels)
            {
                GetUIInstance(defaultUILabel, out Widget instance);
                instance.Open(true, Onef, null);
            }
            FadeIn(Color.black, fadeInDurationWhenStartScene, fadeInDelayWhenStartScene).Forget();
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
            Name[] labels = new Name[instances.Keys.Count];
            instances.Keys.CopyTo(labels, Zero);
            foreach (Name label in labels)
            {
                DestroyUI(label);
            }
            foreach (AssetReference uiDataObject in uiDataObjects)
            {
                if (uiDataObject.IsValid())
                {
                    uiDataObject.ReleaseAsset();
                }
            }
        }

        #endregion
    }
}