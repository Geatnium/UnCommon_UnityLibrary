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
    /// <br>サウンドのマネージャークラス</br>
    /// <br>サウンドリソースのロードから再生、音量などの管理などを行なう。</br>
    /// </summary>
    public class SoundManager : ManagerBase<ISoundManager>, ISoundManager
    {
        //---------------------------- パラメータ ----------------------------//
        #region パラメータ



        #endregion


        //---------------------------- 定数・静的変数 ----------------------------//
        #region 定数・静的変数



        #endregion


        //---------------------------- メンバー変数 ----------------------------//
        #region メンバー変数



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
            isResident = true; // シーンを跨いでも破棄しないようにするか
        }

        // デバッグ（ギズモ）表示用のイベント
        protected override void OnDebugDraw()
        {
            base.OnDebugDraw();
        }

#endif

        #endregion


        //---------------------------- 静的関数 ----------------------------//
        #region 静的関数



        #endregion


        //---------------------------- 仮想関数 ----------------------------//
        #region 仮想関数



        #endregion


        //---------------------------- オーバーライド関数 ----------------------------//
        #region オーバーライド関数



        #endregion


        //---------------------------- メンバー関数 ----------------------------//
        #region メンバー関数



        #endregion


        //---------------------------- オーバーライドインターフェース関数 ----------------------------//
        #region オーバーライドインターフェース関数



        #endregion


        //---------------------------- インターフェース関数 ----------------------------//
        #region インターフェース関数



        #endregion


        //---------------------------- イベント ----------------------------//
        #region イベント

        // 初期化イベント
        // インスタンスにアクセスされた時に一度だけに呼ばれる
        protected override void Init()
        {
            base.Init();
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