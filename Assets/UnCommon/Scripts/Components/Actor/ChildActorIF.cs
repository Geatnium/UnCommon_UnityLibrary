
namespace UnCommon
{
    /// <summary>
    /// ActorChildのインターフェース
    /// </summary>
    public interface IChildActor : IComponent
    {
        /// <summary>
        /// 親のアクターを取得する
        /// </summary>
        /// <returns></returns>
        Actor GetParent();
    }
}