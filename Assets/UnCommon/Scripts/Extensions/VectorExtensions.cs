using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// ベクトル計算関連の拡張関数
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// ベクトルの高さ(y軸)を0.0にして平面にしたベクトルを返す
        /// </summary>
        /// <param name="vector3">ベクトル</param>
        /// <returns></returns>
        public static Vector3 To2D(this Vector3 vector3)
        {
            return new(vector3.x, 0.0f, vector3.z);
        }

        /// <summary>
        /// ベクトルの大きさをXZ平面で取得する
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="result">出力先</param>
        public static float Magnitude2D(this Vector3 vector3)
        {
            Vector3 temp2D = vector3.To2D();
            return temp2D.magnitude;
        }

        /// <summary>
        /// (x, y, z) を (x, z) に変換する
        /// </summary>
        /// <param name="vector3">3次元ベクトル</param>
        /// <returns></returns>
        public static Vector2 ToVector2XZ(this Vector3 vector3)
        {
            return new(vector3.x, vector3.z);
        }

        /// <summary>
        /// (x, y) を　(x, 0.0f, y) に変換する
        /// </summary>
        /// <param name="vector2">2次元ベクトル</param>
        /// <returns></returns>
        public static Vector3 ToVector3XZ(this Vector2 vector2)
        {
            return new(vector2.x, 0.0f, vector2.y);
        }

        /// <summary>
        /// <br>位置ベクトルから位置ベクトルへの方向ベクトルを返す。</br>
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static Vector3 UnitDirection(this Vector3 from, Vector3 to)
        {
            Vector3 difference = to - from;
            return difference.normalized;
        }

        /// <summary>
        /// 位置ベクトルから位置ベクトルへの方向ベクトルを返す（高さを無視）
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static Vector3 UnitDirection2D(this Vector3 from, Vector3 to)
        {
            Vector3 from2D = new(from.x, 0.0f, from.z);
            Vector3 to2D = new(to.x, 0.0f, to.z);
            return from2D.UnitDirection(to2D);
        }

        /// <summary>
        /// 位置ベクトルから位置ベクトルへの方向ベクトルを返す
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static Vector2 UnitDirection(this Vector2 from, Vector2 to)
        {
            Vector2 difference = to - from;
            return difference.normalized;
        }

        /// <summary>
        /// 位置ベクトルから位置ベクトルまでの距離を返す
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static float Distance(this Vector3 from, Vector3 to)
        {
            return Vector3.Distance(from, to);
        }

        /// <summary>
        /// 位置ベクトルから位置ベクトルまでの距離を返す（高さは無視）
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static float Distance2D(this Vector3 from, Vector3 to)
        {
            Vector3 difference = to - from;
            return difference.Magnitude2D();
        }

        /// <summary>
        /// 位置ベクトルから位置ベクトルまでの距離を返す
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static float Distance(this Vector2 from, Vector2 to)
        {
            Vector3 difference = to - from;
            return difference.magnitude;
        }

        /// <summary>
        /// <br>位置ベクトルから位置ベクトルまでの距離の2乗を返す</br>
        /// <br>そのままの距離だと、平方根が含まれ重いため、比較にはこれを使う</br>
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static float SqrDistance(this Vector3 from, Vector3 to)
        {
            Vector3 difference = to - from;
            return difference.sqrMagnitude;
        }

        /// <summary>
        /// <br>位置ベクトルから位置ベクトルまでの距離の2乗を返す（高さを無視）</br>
        /// <br>そのままの距離だと、平方根が含まれ重いため、比較にはこれを使う</br>
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static float SqrDistance2D(this Vector3 from, Vector3 to)
        {
            Vector3 difference = to - from;
            Vector3 difference2D = difference.To2D();
            return difference2D.sqrMagnitude;
        }

        /// <summary>
        /// <br>位置ベクトルから位置ベクトルまでの距離の2乗を返す（高さを無視）</br>
        /// <br>そのままの距離だと、平方根が含まれ重いため、比較にはこれを使う</br>
        /// </summary>
        /// <param name="from">位置ベクトル</param>
        /// <param name="to">位置ベクトル</param>
        /// <returns></returns>
        public static float SqrDistance2D(this Vector2 from, Vector2 to)
        {
            Vector2 difference = to - from;
            return difference.sqrMagnitude;
        }

        /// <summary>
        /// ベクトルを指定の軸周りで回転させる
        /// </summary>
        /// <param name="vector3">回転させたいベクトル</param>
        /// <param name="axis">軸とするベクトル</param>
        /// <param name="angle">回転させる角度</param>
        public static Vector3 RotateAroundAxis(this Vector3 vector3, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * vector3;
        }

        /// <summary>
        /// ベクトルを指定の軸周りで回転させる
        /// </summary>
        /// <param name="vector3">回転させたいベクトル</param>
        /// <param name="axis">軸とするベクトル</param>
        /// <param name="angle">回転させる角度</param>
        public static Vector2 RotateAroundAxis(this Vector2 vector3, float angle)
        {
            return Quaternion.AngleAxis(angle, new Vector3(0.0f, 0.0f, 1.0f)) * vector3;
        }
    }
}