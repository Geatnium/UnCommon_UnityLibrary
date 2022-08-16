using System.Collections.Generic;

namespace UnCommon
{ 
    public partial class ComponentManager
    {
        //---------------------------- 定義 ----------------------------//
        #region 定義

        /// <summary>
        /// 呼び出し順序の優先順位とそのイベントリストの構造体
        /// </summary>
        private struct OrderEventList<T>
        {
            /// <summary>
            /// 呼び出し順序の優先順位
            /// </summary>
            public int order { get; private set; }

            /// <summary>
            /// イベントリスト
            /// </summary>
            public LinkedList<T> eventList;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="order">オーダー</param>
            /// <param name="eventList">イベントリストで渡す</param>
            public OrderEventList(in int order, in LinkedList<T> eventList)
            {
                this.order = order;
                this.eventList = eventList;
            }

            /// <summary>
            /// イベントリストにイベントをユニークに追加する
            /// </summary>
            /// <param name="item">追加したいイベント</param>
            public void AddEventUnique(in T item)
            {
                eventList.AddUnique(item);
            }

            /// <summary>
            /// イベントリストからイベントを除外する
            /// </summary>
            /// <param name="item">除外したいイベント</param>
            public void RemoveEvent(in T item)
            {
                eventList.Remove(item);
            }
        }

        #endregion
    }
}