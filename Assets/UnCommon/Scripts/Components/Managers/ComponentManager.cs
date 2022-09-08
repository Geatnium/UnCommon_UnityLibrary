using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Events;
using UniRx;
using Cysharp.Threading.Tasks;

namespace UnCommon
{

    /// <summary>
    /// ComponentBaseのイベントを管理するコンポーネント
    /// </summary>
    public partial class ComponentManager : ManagerBase<IComponentManager>, IComponentManager
    {
        //---------------------------- メンバー変数 ----------------------------//
        #region メンバー変数

        /// <summary>
        /// ComponentBaseのOnUpdateを呼び出すイベントリスト
        /// </summary>
        private LinkedList<OrderEventList<Func<UniTask>>> updateList;

        /// <summary>
        /// ComponentBaseのOnUpdateJobを呼び出すイベントリスト
        /// </summary>
        private LinkedList<OrderEventList<UnityAction>> updateJobList;

        /// <summary>
        /// ComponentBaseのOnLateUpdateを呼び出すイベントリスト
        /// </summary>
        private LinkedList<OrderEventList<Func<UniTask>>> lateUpdateList;

        /// <summary>
        /// ComponentBaseのOnFixedUpdateを呼び出すイベントリスト
        /// </summary>
        private LinkedList<OrderEventList<Func<UniTask>>> fixedUpdateList;

        /// <summary>
        /// ComponentBaseのOnUpdateJobを呼び出すイベントリスト
        /// </summary>
        private LinkedList<OrderEventList<UnityAction>> fixedUpdateJobList;

        /// <summary>
        /// ComponentBaseのOnTickを呼び出すイベントリスト
        /// </summary>
        private Dictionary<Func<UniTask>, IDisposable> tickList;

        /// <summary>
        /// 1回目のUpdateが終わったか
        /// </summary>
        private bool isEndedFirstUpdate;

        #endregion

        //---------------------------- エディタ ----------------------------//
        #region エディタ

#if UNITY_EDITOR

        protected override void OnReset()
        {
            base.OnReset();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            SetComponentEventsEnabled(
                isUpdateEnabled: true,
                isUpdateJobEnabled: true,
                isLateUpdateEnabled: true,
                isFixedUpdateEnabled: true,
                isFixedUpdateJobEnabled: true,
                isTickEnabled: true);
            SetComponentEventsOrder(
                updateOrder: 0,
                fixedUpdateOrder: 0);
            isResident = false;
        }

        protected override void OnDebugDraw()
        {
            base.OnDebugDraw();
        }

#endif

        #endregion


        //---------------------------- メンバー関数 ----------------------------//
        #region メンバー関数

        /// <summary>
        /// イベントリストにオーダーとイベントを追加する。<br></br>
        /// </summary>
        /// <param name="orderEventLists">追加したいイベントリストを参照で渡す</param>
        /// <param name="order">オーダー</param>
        /// <param name="registerEvent">登録したいイベント</param>
        private void AddEventListItem<T>(ref LinkedList<OrderEventList<T>> orderEventLists, int order, T registerEvent)
        {
            LinkedListNode<OrderEventList<T>> linkedListNode = orderEventLists.First;
            LinkedList<T> linkedList;
            int prevOrder = 0;
            while (linkedListNode.IsValid())
            {
                int currentOrder = linkedListNode.Value.order;
                if (currentOrder == order)
                {
                    linkedListNode.Value.AddEventUnique(registerEvent);
                    return;
                }
                else
                {
                    if (prevOrder < order && order < currentOrder)
                    {
                        linkedListNode = linkedListNode.Previous;
                        linkedList = new();
                        linkedList.AddFirst(registerEvent);
                        orderEventLists.AddAfter(linkedListNode, new OrderEventList<T>(order, linkedList));
                        return;
                    }
                    prevOrder = currentOrder;
                }
                linkedListNode = linkedListNode.Next;
            }
            linkedList = new();
            linkedList.AddFirst(registerEvent);
            orderEventLists.AddLast(new OrderEventList<T>(order, linkedList));
        }

        /// <summary>
        /// イベントリストの中からオーダーを探してイベントを削除する。
        /// </summary>
        /// <param name="orderEventLists">削除したいイベントリストを参照で渡す</param>
        /// <param name="order">オーダー</param>
        /// <param name="removeEvent">削除したいイベント</param>
        private void RemoveEventListItem<T>(ref LinkedList<OrderEventList<T>> orderEventLists, int order, T removeEvent)
        {
            LinkedListNode<OrderEventList<T>> linkedListNode = orderEventLists.First;
            while (linkedListNode.IsValid())
            {
                int currentOrder = linkedListNode.Value.order;
                if (currentOrder == order)
                {
                    linkedListNode.Value.RemoveEvent(removeEvent);
                    return;
                }
                linkedListNode = linkedListNode.Next;
            }
        }

        #endregion


        //---------------------------- インターフェース関数 ----------------------------//
        #region インターフェース関数

        public void AddUpdateEvent(int updateOrder, Func<UniTask> registerUpdateEvent, UnityAction registerUpdateJobEvent)
        {
            if (registerUpdateEvent == OnUpdate) return;

            AddEventListItem(ref updateList, updateOrder, registerUpdateEvent);

            if (registerUpdateJobEvent.IsValid())
            {
                AddEventListItem(ref updateJobList, updateOrder, registerUpdateJobEvent);
            }
        }

        public void RemoveUpdateEvent(int updateOrder, Func<UniTask> removeUpdateEvent, UnityAction removeUpdateJobEvent)
        {
            RemoveEventListItem(ref updateList, updateOrder, removeUpdateEvent);

            if (removeUpdateJobEvent.IsValid())
            {
                RemoveEventListItem(ref updateJobList, updateOrder, removeUpdateJobEvent);
            }
        }

        public void AddLateUpdateEvent(int updateOrder, Func<UniTask> registerLateUpdateEvent)
        {
            if (registerLateUpdateEvent == OnLateUpdate) return;

            AddEventListItem(ref lateUpdateList, updateOrder, registerLateUpdateEvent);
        }

        public void RemoveLateUpdateEvent(int updateOrder, Func<UniTask> removeLateUpdateEvent)
        {
            RemoveEventListItem(ref lateUpdateList, updateOrder, removeLateUpdateEvent);
        }

        public void AddFixedUpdateEvent(int fixedUpdateOrder, Func<UniTask> registerFixedUpdateEvent, UnityAction registerFixedUpdateJobEvent)
        {
            if (registerFixedUpdateEvent == OnFixedUpdate) return;

            AddEventListItem(ref fixedUpdateList, fixedUpdateOrder, registerFixedUpdateEvent);

            if (registerFixedUpdateJobEvent.IsValid())
            {
                AddEventListItem(ref fixedUpdateJobList, updateOrder, registerFixedUpdateJobEvent);
            }
        }

        public void RemoveFixedUpdateEvent(int fixedUpdateOrder, Func<UniTask> removeFixedUpdateEvent, UnityAction removeFixedUpdateJobEvent)
        {
            RemoveEventListItem(ref fixedUpdateList, fixedUpdateOrder, removeFixedUpdateEvent);

            if (removeFixedUpdateJobEvent.IsValid())
            {
                RemoveEventListItem(ref fixedUpdateJobList, updateOrder, removeFixedUpdateJobEvent);
            }
        }

        public void AddTickEvent(Func<UniTask> registerTickEvent, float tickInterval)
        {
            if (registerTickEvent == OnTick) return;

            if (tickList.ContainsKey(registerTickEvent)) return;

            Func<UniTask> tickTask = registerTickEvent;
            IObservable<long> observable = Observable.Interval(TimeSpan.FromSeconds(tickInterval));
            IDisposable disposable = observable.Subscribe(_ => tickTask.Invoke()).AddTo(this);
            tickList.Add(registerTickEvent, disposable);
        }

        public void RemoveTickEvent(Func<UniTask> removeTickEvent)
        {
            if (tickList.TryGetValue(removeTickEvent, out IDisposable tick))
            {
                tick.Dispose();
                tickList.Remove(removeTickEvent);
            }
        }

        #endregion


        //---------------------------- イベント ----------------------------//
        #region イベント

        protected override void Init()
        {
            base.Init();
            // リスト初期化
            updateList = new();
            updateJobList = new();
            lateUpdateList = new();
            fixedUpdateList = new();
            fixedUpdateJobList = new();
            tickList = new();

            // オーダーが0だけ枠を作っておく
            updateList.AddFirst(new OrderEventList<Func<UniTask>>(0, new()));
            updateJobList.AddFirst(new OrderEventList<UnityAction>(0, new()));
            lateUpdateList.AddFirst(new OrderEventList<Func<UniTask>>(0, new()));
            fixedUpdateList.AddFirst(new OrderEventList<Func<UniTask>>(0, new()));
            fixedUpdateJobList.AddFirst(new OrderEventList<UnityAction>(0, new()));
        }

        private void Update()
        {
            if (!isEndedFirstUpdate)
            {
                isEndedFirstUpdate = true;
                GC.Collect();
            }
            LinkedListNode<OrderEventList<UnityAction>> updateJobListNode;
            LinkedListNode<UnityAction> updateJobEventListNode;
            updateJobListNode = updateJobList.First;
            while (updateJobListNode.IsValid())
            {
                updateJobEventListNode = updateJobListNode.Value.eventList.First;
                while (updateJobEventListNode.IsValid())
                {
                    updateJobEventListNode.Value.Invoke();
                    updateJobEventListNode = updateJobEventListNode.Next;
                }
                updateJobListNode = updateJobListNode.Next;
            }

            NativeArray<JobHandle> jobHandles;
            if (JobUtility.GetJobsNativeArray(out jobHandles))
            {
                JobHandle.ScheduleBatchedJobs();
                JobHandle.CompleteAll(jobHandles);
                JobUtility.ClearAllJobs();
                jobHandles.Dispose();
            }

            LinkedListNode<OrderEventList<Func<UniTask>>> updateListNode;
            LinkedListNode<Func<UniTask>> updateEventListNode;
            updateListNode = updateList.First;
            while (updateListNode.IsValid())
            {
                updateEventListNode = updateListNode.Value.eventList.First;
                while (updateEventListNode.IsValid())
                {
                    updateEventListNode.Value.Invoke().Forget();
                    updateEventListNode = updateEventListNode.Next;
                }
                updateListNode = updateListNode.Next;
            }
        }

        private void LateUpdate()
        {
            LinkedListNode<OrderEventList<Func<UniTask>>> lateUpdateListNode = lateUpdateList.First;
            LinkedListNode<Func<UniTask>> lateUpdateEventListNode;
            while (lateUpdateListNode.IsValid())
            {
                lateUpdateEventListNode = lateUpdateListNode.Value.eventList.First;
                while (lateUpdateEventListNode.IsValid())
                {
                    lateUpdateEventListNode.Value.Invoke().Forget();
                    lateUpdateEventListNode = lateUpdateEventListNode.Next;
                }
                lateUpdateListNode = lateUpdateListNode.Next;
            }
        }

        private void FixedUpdate()
        {
            LinkedListNode<OrderEventList<UnityAction>> fixedUpdateJobListNode = fixedUpdateJobList.First;
            LinkedListNode<UnityAction> fixedUpdateJobEventListNode;
            while (fixedUpdateJobListNode.IsValid())
            {
                fixedUpdateJobEventListNode = fixedUpdateJobListNode.Value.eventList.First;
                while (fixedUpdateJobEventListNode.IsValid())
                {
                    fixedUpdateJobEventListNode.Value.Invoke();
                    fixedUpdateJobEventListNode = fixedUpdateJobEventListNode.Next;
                }
                fixedUpdateJobListNode = fixedUpdateJobListNode.Next;
            }

            NativeArray<JobHandle> jobHandles;
            if (JobUtility.GetJobsNativeArray(out jobHandles))
            {
                JobHandle.ScheduleBatchedJobs();
                JobHandle.CompleteAll(jobHandles);
                JobUtility.ClearAllJobs();
                jobHandles.Dispose();
            }

            LinkedListNode<OrderEventList<Func<UniTask>>> fixedUpdateListNode = fixedUpdateList.First;
            LinkedListNode<Func<UniTask>> fixedUpdateEventListNode;
            while (fixedUpdateListNode.IsValid())
            {
                fixedUpdateEventListNode = fixedUpdateListNode.Value.eventList.First;
                while (fixedUpdateEventListNode.IsValid())
                {
                    fixedUpdateEventListNode.Value.Invoke().Forget();
                    fixedUpdateEventListNode = fixedUpdateEventListNode.Next;
                }
                fixedUpdateListNode = fixedUpdateListNode.Next;
            }
        }

        #endregion
    }
}