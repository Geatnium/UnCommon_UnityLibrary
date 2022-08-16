using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnCommon
{
    /// <summary>
    /// <br>サービスロケーター</br>
    /// <br>マネージャークラスの格納場所</br>
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// インスタンスを登録する辞書。
        /// </summary>
        static readonly Dictionary<Type, object> instances = new();

        /// <summary>
        /// インスタンスを登録する
        /// </summary>
        /// <param name="instance">登録するインスタンス</param>
        public static void Register<T>(T instance) where T : class, IService
        {
            Type type = typeof(T);
            // すでに登録されていたら警告を出す
            if (instances.ContainsKey(key: type))
            {
                DebugLogger.LogWarning($"サービスロケータにすでに同じ型のインスタンスが登録されている：{type.Name}");
                return;
            }
            // 初期化させて登録
            instance.GetGameObject().SendMessage("InitOnce");
            instances.Add(type, instance);
            DebugLogger.Log($"サービスロケータに登録：{type.Name}");
        }

        /// <summary>
        /// インスタンスの登録を解除
        /// </summary>
        /// <param name="instance">登録を解除するインスタンス</param>
        public static void Unregister<T>(T instance) where T : class, IService
        {
            Type type = typeof(T);
            // 登録されていなければ何もしない
            if (!instances.ContainsKey(type)) return;
            // 登録されている要求された型のインスタンスと渡されたインスタンスが一致しない場合、警告を出す
            if (instances[type] != instance)
            {
                DebugLogger.LogWarning($"サービスロケータに登録されている要求された型のインスタンスと渡されたインスタンスが一致しない：{type.Name}");
                return;
            }
            // 同じインスタンスがあったら除外
            instances.Remove(type);
            DebugLogger.Log($"サービスロケータから解除：{type.Name}");
        }

        /// <summary>
        /// 指定された型のインスタンスがすでに登録されているかをチェックする
        /// </summary>
        /// <returns>指定された型のインスタンスがすでに登録されている場合は true を返す</returns>
        public static bool IsTypeRegisted<T>() where T : class, IService
        {
            return instances.ContainsKey(typeof(T));
        }

        /// <summary>
        /// 渡されたインスタンスがすでに登録されているかをチェックする
        /// </summary>
        /// <param name="instance">登録を確認するインスタンス</param>
        /// <returns>渡されたインスタンスが既に登録されている場合は true を返します。</returns>
        public static bool IsInstanceRegisted<T>(T instance) where T : class, IService
        {
            Type type = typeof(T);
            return instances.ContainsKey(type) && instances[type] == instance;
        }

        /// <summary>
        /// <br>インスタンスを取得する。</br>
        /// <br>取得できなかった場合は、インスタンスを探して登録する。</br>
        /// <br>探してもない場合はエラー。</br>
        /// </summary>
        /// <returns>取得したインスタンスを返す。取得できなかった場合は null を返します。</returns>
        public static T GetInstance<T>() where T : class, IService
        {
            Type type = typeof(T);
            // 登録されていたらそれを返す
            if (instances.ContainsKey(type))
            {
                return instances[type] as T;
            }
            // 登録されていなかったら、シーンから探して見つけたら登録
            if (FindInstance(out T instance))
            {
                Register(instance);
                return instance;
            }
            DebugLogger.LogWarning($"サービスロケータからインスタンスを取得できなかった：{type.Name}");
            return null;
        }

        /// <summary>
        /// インスタンスを取得し、渡された引数に代入する。<br></br>
        /// 取得できなかった場合は null が入る。
        /// </summary>
        /// <param name="instance">取得したインスタンスを入れる変数。</param>
        /// <returns>取得が成功したら true を返します。</returns>
        public static bool TryGetInstance<T>(out T instance) where T : class, IService
        {
            Type type = typeof(T);
            // 登録されていたらそれを渡し、成功
            if (instances.ContainsKey(type))
            {
                instance = instances[type] as T;
                return true;
            }
            // 登録されていなかったら、シーンから探して見つけたら登録して成功
            if (FindInstance(out instance))
            {
                Register(instance);
                return true;
            }
            DebugLogger.LogWarning($"サービスロケータからインスタンスを取得できなかった：{type.Name}");
            instance = null;
            return false;
        }

        /// <summary>
        /// 指定のインターフェースの型を持ったサービスを呼び出す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functor">実行する処理</param>
        /// <returns></returns>
        public static bool Execute<T>(in Action<T> functor) where T : class, IService
        {
            // インスタンスを取得できたらfunctorの処理を発行
            if (TryGetInstance(out T instance))
            {
                functor.Invoke(instance);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 指定のインターフェースを持ったサービスのインスタンスを探す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">取得したインスタンスを入れる変数</param>
        /// <returns></returns>
        private static bool FindInstance<T>(out T instance) where T : class, IService
        {
            // シーンからインスタンスを探し、取得できたらそれを返す
            ServiceBase<T> serviceBase = GameObject.FindObjectOfType<ServiceBase<T>>();
            if (serviceBase.IsValid())
            {
                instance = serviceBase.GetComponent<T>();
                return true;
            }
            instance = null;
            return false;
        }
    }
}