
namespace UnCommon
{
    /// <summary>
    /// SceneManagerのインターフェース
    /// </summary>
    public interface ISceneManager : IManager
    {
        /// <summary>
        /// シーンを開く
        /// </summary>
        /// <param name="nextScene">開くシーン</param>
        /// <param name="fadeDuration">フェードアウトの時間</param>
        void OpenScene(Name nextScene, float fadeDuration = 1.0f);

        /// <summary>
        /// シーンロード中の場合の進捗を取得する
        /// </summary>
        /// <returns></returns>
        float GetLoadingSceneProgress();

        /// <summary>
        /// 次に遷移するシーン名（読み込み中のシーン）が設定されていれば取得する。
        /// </summary>
        /// <returns></returns>
        Name GetNextSceneName();

        /// <summary>
        /// 前のシーン名を取得する
        /// </summary>
        /// <returns></returns>
        Name PrevSceneName();
    }
}