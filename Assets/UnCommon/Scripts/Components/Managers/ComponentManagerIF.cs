using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.Jobs;
using Cysharp.Threading.Tasks;

namespace UnCommon
{
    /// <summary>
    /// UpdateManagerのインターフェース
    /// </summary>
    public interface IComponentManager : IManager
    {
        /// <summary>
        /// Updateイベントを行うリストに登録する
        /// </summary>
        /// <param name="updateOrder">呼び出す順番の優先順位</param>
        /// <param name="registerUpdateEvent">登録したいUpdateイベント</param>
        /// <param name="registerUpdateJobEvent">登録したいUpdateJobイベント</param>
        void AddUpdateEvent(int updateOrder, Func<UniTask> registerUpdateEvent, UnityAction registerUpdateJobEvent = null);

        /// <summary>
        /// Updateイベントのリストから除外する
        /// </summary>
        /// <param name="updateOrder">呼び出す順番の優先順位</param>
        /// <param name="removeUpdateEvent">除外したいUpdateイベント</param>
        /// <param name="removeUpdateJobEvent">除外したいUpdateJobイベント</param>
        void RemoveUpdateEvent(int updateOrder, Func<UniTask> removeUpdateEvent, UnityAction removeUpdateJobEvent = null);


        /// <summary>
        /// LateUpdateイベントを行うリストに登録する
        /// </summary>
        /// <param name="updateOrder">呼び出す順番の優先順位</param>
        /// <param name="registerLateUpdateEvent">登録したいLateUpdateイベント</param>
        void AddLateUpdateEvent(int updateOrder, Func<UniTask> registerLateUpdateEvent);

        /// <summary>
        /// LateUpdateイベントのリストから除外する
        /// </summary>
        /// <param name="updateOrder">呼び出す順番の優先順位</param>
        /// <param name="removeLateUpdateEvent">除外したいLateUpdateイベント</param>
        void RemoveLateUpdateEvent(int updateOrder, Func<UniTask> removeLateUpdateEvent);


        /// <summary>
        /// FixedUpdateイベントを行うリストに登録する
        /// </summary>
        /// <param name="fixedUpdateOrder">呼び出す順番の優先順位</param>
        /// <param name="registerFixedUpdateEvent">登録したいFixedUpdateイベント</param>
        /// <param name="registerFixedUpdateJobEvent">登録したいFixedUpdateJobイベント</param>
        void AddFixedUpdateEvent(int fixedUpdateOrder, Func<UniTask> registerFixedUpdateEvent, UnityAction registerFixedUpdateJobEvent = null);

        /// <summary>
        /// FixedUpdateイベントのリストから除外する
        /// </summary>
        /// <param name="fixedUpdateOrder">呼び出す順番の優先順位</param>
        /// <param name="removeFixedUpdateEvent">除外したいFixedUpdateイベント</param>
        /// <param name="removeFixedUpdateJobEvent">除外したいFixedUpdateJobイベント</param>
        void RemoveFixedUpdateEvent(int fixedUpdateOrder, Func<UniTask> removeFixedUpdateEvent, UnityAction removeFixedUpdateJobEvent = null);


        /// <summary>
        /// Tickイベントを行うリストに登録する
        /// </summary>
        /// <param name="registerTickEvent">登録したいTickイベント</param>
        /// <param name="tickInterval">登録するイベントの時間間隔</param>
        void AddTickEvent(Func<UniTask> registerTickEvent, float tickInterval);

        /// <summary>
        /// Tickイベントのリストから除外する
        /// </summary>
        /// <param name="removeTickEvent">除外したいTickイベント</param>
        void RemoveTickEvent(Func<UniTask> removeTickEvent);
    }
}