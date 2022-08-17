
namespace UnCommon
{
    /// <summary>
    /// ActorChildのインターフェース
    /// </summary>
    public interface IActorChild : IComponent
    {
        /// <summary>
        /// 親のアクターを取得する
        /// </summary>
        /// <returns></returns>
        Actor GetParent();
    }
}