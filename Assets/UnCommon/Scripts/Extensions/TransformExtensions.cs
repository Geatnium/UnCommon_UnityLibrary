using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnCommon
{
    /// <summary>
    /// ゲームオブジェクト関連の拡張関数クラス
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// 指定のインターフェースを持つコンポーネントの関数を呼び出す. <br/>
        /// transform.Execute<インターフェース>(r => r.Method());
        /// </summary>
        /// <typeparam name="T">指定のインターフェース</typeparam>
        /// <param name="transform">対象のゲームオブジェクト</param>
        /// <paramref name="type">呼び出しタイプ</paramref>
        /// <param name="functor">呼び出し可能な場合の処理</param>
        /// <returns>呼び出しが成功したか</returns>
        public static bool Execute<T>(this Transform transform, ExecuteType type, Action<T> functor) where T : IEventSystemHandler
        {
            return transform.gameObject.Execute(type, functor);
        }

        /// <summary>
        /// トランスフォームのX座標だけ変更
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="newX"></param>
        public static void SetPositionX(this Transform transform, float newX)
        {
            Vector3 tempPosition = transform.position;
            tempPosition.x = newX;
            transform.position = tempPosition;
        }

        /// <summary>
        /// トランスフォームのX座標だけ変更
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="newY"></param>
        public static void SetPositionY(this Transform transform, float newY)
        {
            Vector3 tempPosition = transform.position;
            tempPosition.y = newY;
            transform.position = tempPosition;
        }

        /// <summary>
        /// トランスフォームのX座標だけ変更
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="newZ"></param>
        public static void SetPositionZ(this Transform transform, float newZ)
        {
            Vector3 tempPosition = transform.position;
            tempPosition.z = newZ;
            transform.position = tempPosition;
        }

        /// <summary>
        /// 高さを無視して水平方向で指定座標の方を向く
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="target"></param>
        public static void LookAt2D(this Transform transform, Vector3 target)
        {
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }
}