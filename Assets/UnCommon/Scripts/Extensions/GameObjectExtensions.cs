using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnCommon
{
    /// <summary>
    /// インターフェース呼び出しのタイプ
    /// </summary>
    public enum ExecuteType
    {
        [Tooltip("最初に見つかった指定インターフェースのみ")]
        OnlyOne,
        [Tooltip("ゲームオブジェクトについている指定インターフェース全てに対して")]
        AllComponents,
        [Tooltip("子のゲームオブジェクトまで含めた全てのインターフェースに対して")]
        AllChildrenComponents
    }

    /// <summary>
    /// ゲームオブジェクト関連の拡張関数クラス
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 指定のインターフェースを実装したコンポーネントの関数を呼び出す. <br/>
        /// gameObject.Execute<インターフェース>(r => r.Method());
        /// </summary>
        /// <typeparam name="T">指定のインターフェース</typeparam>
        /// <param name="gameObject">対象のゲームオブジェクト</param>
        /// <param name="type">呼び出しタイプ</param>
        /// <param name="functor">呼び出し可能な場合の処理</param>
        /// <returns>呼び出しが成功したか</returns>
        public static bool Execute<T>(this GameObject gameObject, ExecuteType type, Action<T> functor) where T : IEventSystemHandler
        {
            // ゲームオブジェクトの参照が取れなかったら何もしない
            if (!gameObject.IsValid()) return false;

            switch (type)
            {
                case ExecuteType.OnlyOne:
                    T target = gameObject.GetComponent<T>();
                    if (!target.IsValid()) return false;
                    functor.Invoke(target);
                    return true;
                case ExecuteType.AllComponents:
                    T[] targetAllComponents = gameObject.GetComponents<T>();
                    if (!targetAllComponents.IsValid()) return false;
                    for (int i = 0; i < targetAllComponents.Length; i++)
                    {
                        functor.Invoke(targetAllComponents[i]);
                    }
                    return true;
                case ExecuteType.AllChildrenComponents:
                    T[] targetAllChildrenComponents = gameObject.GetComponentsInChildren<T>();
                    if (!targetAllChildrenComponents.IsValid()) return false;
                    for (int i = 0; i < targetAllChildrenComponents.Length; i++)
                    {
                        functor.Invoke(targetAllChildrenComponents[i]);
                    }
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 指定のインターフェースを実装したコンポーネントを持っているか
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool ImplementsInterface<T>(this GameObject gameObject) where T : IEventSystemHandler
        {
            return gameObject.TryGetComponent(out T t);
        }
    }
}