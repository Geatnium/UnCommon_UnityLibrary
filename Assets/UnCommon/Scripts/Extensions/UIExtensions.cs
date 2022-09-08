using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnCommon
{
    public static class UIExtensions
    {
        /// <summary>
        /// Imageのアルファ値を変更
        /// </summary>
        /// <param name="image"></param>
        /// <param name="alpha">アルファ値</param>
        public static void SetAlpha(this Image image, float alpha)
        {
            Color temp = image.color;
            temp.a = alpha;
            image.color = temp;
        }

        /// <summary>
        /// Textのアルファ値を変更
        /// </summary>
        /// <param name="text"></param>
        /// <param name="alpha">アルファ値</param>
        public static void SetAlpha(this Text text, float alpha)
        {
            Color temp = text.color;
            temp.a = alpha;
            text.color = temp;
        }
    }
}