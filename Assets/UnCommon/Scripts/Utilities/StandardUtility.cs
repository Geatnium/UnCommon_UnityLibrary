using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// 基本的な便利関数
    /// </summary>
    public static class StandardUtility
    {
        /// <summary>
        /// float計算の許容範囲（比較とか）
        /// </summary>
        public static readonly float Tolerance = 0.000001f;

        /// <summary>
        /// 0
        /// </summary>
        public static readonly int Zero = 0;

        /// <summary>
        /// 1
        /// </summary>
        public static readonly int One = 1;

        /// <summary>
        /// 2
        /// </summary>
        public static readonly int Two = 2;

        /// <summary>
        /// 0.0f
        /// </summary>
        public static readonly float Zerof = 0.0f;

        /// <summary>
        /// 1.0f
        /// </summary>
        public static readonly float Onef = 1.0f;

        /// <summary>
        /// 2.0f
        /// </summary>
        public static readonly float Twof = 2.0f;

        /// <summary>
        /// floatの秒をミリ秒のintに変換する
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static int ToMilliSecondsInt(this float seconds)
        {
            return (int)(seconds * 1000.0f);
        }

        /// <summary>
        /// ゲームオブジェクトを生成する
        /// </summary>
        /// <param name="owner">生成命令を出したゲームオブジェクトを設定</param>
        /// <param name="original">生成したいオリジナルのオブジェクト</param>
        /// <param name="position">生成する座標</param>
        /// <param name="rotation">生成する回転</param>
        /// <param name="instanced">生成したゲームオブジェクト</param>
        /// <returns>生成に成功したらtrue</returns>
        public static bool CustomInstantiate(GameObject owner, GameObject original, Vector3 position, Quaternion rotation, out GameObject instanced)
        {
            // インスタンス
            instanced = GameObject.Instantiate(original, position, rotation);
            // 生成できていなかったら失敗を返す
            if (!instanced.IsValid()) return false;
            // アクターだったら生みの親を渡す
            instanced.Execute<IActor>(ExecuteType.OnlyOne, (r) => r.SetSpawner(owner));
            return true;
        }

        /// <summary>
        /// ゲームオブジェクトを生成する
        /// </summary>
        /// <param name="owner">生成命令を出したゲームオブジェクトを設定</param>
        /// <param name="original">生成したいオリジナルのオブジェクト</param>
        /// <param name="parent">親トランスフォーム</param>
        /// <param name="instanced">生成したゲームオブジェクト</param>
        /// <returns>生成に成功したらtrue</returns>
        public static bool CustomInstantiate(GameObject owner, GameObject original, Transform parent, out GameObject instanced)
        {
            // インスタンス
            instanced = GameObject.Instantiate(original, parent);
            // 生成できていなかったら失敗を返す
            if (!instanced.IsValid()) return false;
            // アクターだったら生みの親を渡す
            instanced.Execute<IActor>(ExecuteType.OnlyOne, (r) => r.SetSpawner(owner));
            return true;
        }

        /// <summary>
        /// 指定のインターフェースを実装したゲームオブジェクトを取得する（最初に見つけたやつを取得）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static GameObject FindGameObjectOfInterface<T>()
        {
            GameObject[] finds = GameObject.FindObjectsOfType<GameObject>();
            for (int i = 0; i < finds.Length; i++)
            {
                if (finds[i].TryGetComponent(out T t))
                {
                    return finds[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 指定のインターフェースを実装したゲームオブジェクトを全て取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static GameObject[] FindGameObjectsOfInterface<T>()
        {
            List<GameObject> list = new List<GameObject>();
            GameObject[] finds = GameObject.FindObjectsOfType<GameObject>();
            for (int i = 0; i < finds.Length; i++)
            {
                if (finds[i].TryGetComponent(out T t))
                {
                    list.Add(finds[i]);
                }
            }
            return list.ToArray();
        }
    }
}