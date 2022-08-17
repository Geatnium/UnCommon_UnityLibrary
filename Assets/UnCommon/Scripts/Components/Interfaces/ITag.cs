using UnityEngine.EventSystems;

namespace UnCommon {

    /// <summary>
    /// タグの機能インターフェース
    /// </summary>
    public interface ITag : IEventSystemHandler
    {
        /// <summary>
        /// タグを設定する
        /// </summary>
        /// <param name="tags"></param>
        void SetTags(NameHashSet tags);

        /// <summary>
        /// タグを追加する
        /// </summary>
        /// <param name="tag"></param>
        void AddTag(Name tag);

        /// <summary>
        /// タグを取得
        /// </summary>
        /// <returns></returns>
        NameHashSet GetTags();

        /// <summary>
        /// 特定のタグを持っているか調べる
        /// </summary>
        /// <param name="tag">調べたいタグ</param>
        /// <returns></returns>
        bool HasTag(Name tag);
    }
}