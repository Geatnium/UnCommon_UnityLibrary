using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// Actorのインターフェース
    /// </summary>
    public interface IActor : IComponent
    {
        /// <summary>
        /// 名前を設定する
        /// </summary>
        /// <param name="name">名前</param>
        void SetName(Name name);

        /// <summary>
        /// 名前を取得する
        /// </summary>
        /// <returns></returns>
        Name GetName();

        /// <summary>
        /// 生成したときに誰が生成したのかを設定する
        /// </summary>
        /// <param name="gameObject"></param>
        void SetSpawner(GameObject gameObject);

        /// <summary>
        /// このアクターを誰が生成したのかを取得
        /// </summary>
        /// <param name="ownerGameObject"></param>
        /// <returns>設定されていない場合false（最初から配置されていたなど）</returns>
        GameObject GetSpawner();
    }
}