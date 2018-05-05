using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeeInfo.Lib
{
    public static class Tools
    {
        public static string GetRandomColor()
        {
            Random RandomNum_First = new Random();
            Random RandomNum_Sencond = new Random();
            Random RandomNum_Third = new Random();
            int int_Red = RandomNum_First.Next(0, 256);
            int int_Green = RandomNum_Sencond.Next(0, 256);
            int int_Blue = RandomNum_Third.Next(0, 256);
            Color color = Color.FromArgb(int_Red, int_Green, int_Blue);
            string strColor = "#" + Convert.ToString(color.ToArgb(), 16).PadLeft(8, '0').Substring(2, 6);
            return strColor;
        }

        public static int SubstringCount(string str, string substring)
        {
            if (str.Contains(substring))
            {
                string strReplaced = str.Replace(substring, "");
                return (str.Length - strReplaced.Length) / substring.Length;
            }

            return 0;
        }
    }
}
