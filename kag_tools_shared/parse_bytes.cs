using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kag_tools_shared
{
    public class parse_bytes
    {

        #region 判断字符编码
        /// <summary>
        /// 检测字符编码
        /// </summary>
        /// <param name="src">二进制原文</param>
        /// <returns>字符编码</returns>
        public string DetectUnicode(byte[] src)
        {
            // utf8 with bom
            if (src[0] == 239 && src[1] == 187 && src[2] == 191) return "utf8";
            // unicode big endian
            else if (src[0] == 254 && src[1] == 255) return "utf16be";
            // unicode little endian
            else if (src[0] == 255 && src[1] == 254) return "utf16le";
            // utf8 without bom
            else if (IsUtf8NoBom(src) == true) return "utf8";
            //ansi
            else return "ansi";
        }
        #endregion

        #region 判断 UTF-8 without BOM
        /// <summary>
        /// 判断是否为无 BOM 类型的 UTF-8 编码
        /// </summary>
        /// <param name="src2">待检测的内容</param>
        /// <returns>UTF-8 编码与否</returns>
        private bool IsUtf8NoBom(byte[] src2)
        {
            for (int i = 0; i < src2.Length; i++)
            {
                if ((src2[i] & 0xE0) == 0xC0)    // 110x xxxx 10xx xxxx  
                {
                    if ((src2[i + 1] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
                else if ((src2[i] & 0xF0) == 0xE0)  // 1110 xxxx 10xx xxxx 10xx xxxx  
                {
                    if ((src2[i + 1] & 0x80) != 0x80 || (src2[i + 2] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
                else if ((src2[i] & 0xF8) == 0xF0)  // 1111 0xxx 10xx xxxx 10xx xxxx 10xx xxxx  
                {
                    if ((src2[i + 1] & 0x80) != 0x80 || (src2[i + 2] & 0x80) != 0x80 || (src2[i + 3] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region 转换二进制内容为文本
        /// <summary>
        /// 转换二进制内容为文本
        /// </summary>
        /// <param name="src">二进制内容</param>
        /// <param name="encoding">字符编码</param>
        /// <returns>二进制内容所对应的文本</returns>
        public string byte2str(byte[] src, string encoding)
        {
            string str;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding cp = GetEncoding(encoding);
            switch(encoding)
            {
                case "utf8":
                    {
                        str = Encoding.UTF8.GetString(src);
                        break;
                    }
                case "utf16le":
                    {
                        str = Encoding.Unicode.GetString(src);
                        break;
                    }
                case "utf16be":
                    {
                        str = Encoding.BigEndianUnicode.GetString(src);
                        break;
                    }
                default:
                    {
                        str = cp.GetString(src);
                        break;
                    }
            }
            return str;
        }
        #endregion

        #region 获取具体的 ANSI 编码
        /// <summary>
        /// 获取具体的 ANSI 编码
        /// </summary>
        /// <param name="encoding">字符编码名称</param>
        /// <returns>代码页</returns>
        private Encoding GetEncoding(string encoding)
        {
            switch(encoding)
            {
                case "gbk":
                    {
                        return Encoding.GetEncoding(936);   //GB 2312、GBK、GB18030
                    }
                case "big5":
                    {
                        return Encoding.GetEncoding(950);   //Big 5
                    }
                case "sjis":
                    {
                        return Encoding.GetEncoding(932);   //Shift-JIS
                    }
                case "euckr":
                    {
                        return Encoding.GetEncoding(949);   //EUC-KR
                    }
                default:
                    {
                        return Encoding.GetEncoding(437);   //Windows 默认编码
                    }
            }
        }
        #endregion

    }
}
