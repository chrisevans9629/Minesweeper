using System;
using System.Collections.Generic;
using System.Text;

namespace Minesweeper
{
    public static class Ext
    {
        public static double[] ToDoubleArray(this int[] array)
        {
            var list = new Double[array.Length];
            for (var i = 0; i < list.Length; i++)
            {
                list[i] = array[i];
            }
            return list;
        }
        public static string ArrayContentString<T>(this T[] array)
        {
            var builder = new StringBuilder();
            for (var index = 0; index < array.Length; index++)
            {
                var x1 = array[index];
                builder.Append("{ " + x1 + " }");
            }
            return builder.ToString();
        }
    }
}