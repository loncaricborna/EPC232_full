using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPC232
{
    class TdadBText
    {

        public static string[] Gain = new string[16] { "  0dB", "  2dB", "  4dB", "  6dB", "  8dB", " 10dB", " 12dB", " 14dB", " 16dB", " 18dB", " 20dB", " 22dB", " 24dB", " 26dB", " 28dB", " 30dB" };
        public static string[] Volume = new string[14] { "  0dB", "- 1dB", "- 2dB", "- 3dB", "- 4dB", "- 5dB", "- 6dB", "- 7dB", "- 8dB", "-16dB", "-24dB", "-32dB", "-40dB", " MUTE" };
        public static string[] Tone = new string[15] { "-14dB", "-12dB", "-10dB", "- 8dB", "- 6dB", "- 4dB", "- 2dB", "  0dB", "+ 2dB", "+ 4dB", "+ 6dB", "+ 8dB", "+10dB", "+12dB", "+14dB" };
        public static string[] Attenuation = new string[18] { "  0dB", "- 1dB", "- 2dB", "- 3dB", "- 4dB", "- 5dB", "- 6dB", "- 7dB", "- 8dB", "-16dB", "-24dB", "-32dB", "-40dB", "-48dB", "-56dB", "-64dB", "-72dB", " MUTE", };
    }

}
