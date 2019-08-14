using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace kag_tools_shared
{
    public class loadsave
    {

        #region 异步保存 .ks 文件
        /// <summary>
        /// 保存 .ks 文件
        /// </summary>
        /// <param name="perline">传入需要保存的 List</param>
        /// <returns>string result（保存情况）</returns>
        public async Task<string> save_ksasync(List<ks_perlines> perline)
        {
            FileSavePicker fos = new FileSavePicker();
            fos.FileTypeChoices.Add("KAG Script", new List<string> { ".ks" });
            StorageFile sf = await fos.PickSaveFileAsync();
            if (sf != null)
            {
                Encoding encoding = Encoding.Unicode;   //默认保存为 UTF-16 LE 编码
                byte[] datas;
                string save_file = null;
                for (int i = 0; i < perline.Count; i++)
                {
                    if (i == perline.Count - 1)
                        save_file = save_file + perline[i].texts_dst;
                    else
                        save_file = save_file + perline[i].texts_dst + "\r\n";
                }
                datas = encoding.GetBytes(save_file);
                try
                {
                    await FileIO.WriteBytesAsync(sf, datas);
                    return string.Format("成功，保存文件为：{0}。",sf.Path);
                }
                catch(Exception ex)
                {
                    return string.Format("错误：{0}。", ex.Message);
                }
            }
            else
            {
                return "取消操作。";
            }
        }
        #endregion

        #region 异步打开 .ks 文件
        /// <summary>
        /// 打开 .ks 文件
        /// </summary>
        /// <returns>string files.filename（文件名）、byte[] files.srcode（文件的二进制内容）</returns>
        public async Task<files> load_ksasync()
        {
            // 打开文件
            FileOpenPicker fop = new FileOpenPicker();
            fop.FileTypeFilter.Add(".ks");
            StorageFile sf = await fop.PickSingleFileAsync();
            byte[] src;
            files files;
            string filename;

            if (sf != null)
            {

                //把文件转换为 ibuffer
                IBuffer buffer = await FileIO.ReadBufferAsync(sf);
                parse_bytes parse_bytes = new parse_bytes();

                //以二进制方式读取文件
                using (DataReader datareader = DataReader.FromBuffer(buffer))
                {
                    //DataReader datareader = DataReader.FromBuffer(buffer);
                    src = new byte[datareader.UnconsumedBufferLength];
                    datareader.ReadBytes(src);
                }
                filename = sf.Name;
            }
            else
            {
                //由于直接返回 null 会导致 System.NullReferenceException，所以随便返回一个
                src = Encoding.ASCII.GetBytes("error");
                filename = "empty";
            }


            files.filename = filename;
            files.srcode = src;
            return files;
        }
        #endregion

    }

    //由于要传两个参数回去，所以改为结构
    public struct files
    {
        public string filename;
        public byte[] srcode;
    }
}
