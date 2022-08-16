using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;

namespace UnCommon
{
    /// <summary>
    /// ジョブ（マルチスレッド）関連のユーティリティクラス
    /// </summary>
    public static class JobUtility
    {
        /// <summary>
        /// ジョブのリスト（キュー）
        /// </summary>
        private static Queue<JobHandle> jobHandles;

        /// <summary>
        /// ジョブを登録する
        /// </summary>
        /// <param name="jobHandle"></param>
        public static void AddJob(JobHandle jobHandle)
        {
            // リストのインスタンスがなかったら生成
            if (!jobHandles.IsValid())
            {
                jobHandles = new();
            }
            // 追加
            jobHandles.Enqueue(jobHandle);
        }

        /// <summary>
        /// 登録したジョブを全てクリアする
        /// </summary>
        public static void ClearAllJobs()
        {
            jobHandles.Clear();
        }

        /// <summary>
        /// ジョブのリストをNativeArrayにして渡す
        /// </summary>
        /// <param name="output">出力先</param>
        /// <returns>ジョブがなかったらFalseを返す</returns>
        public static bool GetJobsNativeArray(out NativeArray<JobHandle> output)
        {
            // ジョブが一個も登録されていなかったら、配列をすぐに解放しFalseを返す
            if (jobHandles.IsValidIndex(0))
            {
                output = new(1, Allocator.TempJob);
                output.Dispose();
                return false;
            }
            else
            {
                // 登録されたジョブリストを配列に入れていく
                output = new(jobHandles.Count, Allocator.TempJob);
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = jobHandles.Dequeue();
                }
                // 全部使ったので無いはずだけど、念の為クリア
                jobHandles.Clear();
                return true;
            }
        }
    }
}