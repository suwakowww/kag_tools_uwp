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
    public class loaddict
    {

        /// <summary>
        /// 异步加载词典
        /// </summary>
        /// <returns>List<dictlist>（词典类）</returns>
        public async Task<List<dictlist>> loaddictasync()
        {
            List<dictlist> dicts = new List<dictlist>();
            FileOpenPicker fop = new FileOpenPicker();
            fop.FileTypeFilter.Add(".csv");
            StorageFile sf = await fop.PickSingleFileAsync();
            dictlist dictlist = new dictlist();
            parse_bytes parse_bytes = new parse_bytes();
            if (sf != null)
            {
                //如果非空，则开始组合词典
                if (sf.FileType == ".csv")
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(sf);

                    using (DataReader dataReader = DataReader.FromBuffer(buffer))
                    {
                        byte[] csvsrc = new byte[dataReader.UnconsumedBufferLength];
                        dataReader.ReadBytes(csvsrc);
                        string encoding = parse_bytes.DetectUnicode(csvsrc);
                        string csv = parse_bytes.byte2str(csvsrc, encoding);
                        dicts = dictlist.parse_csvdict(csv);
                    }
                }
                return dicts;
            }
            else
                //否则返回空值
                return null;
        }
    }
}
