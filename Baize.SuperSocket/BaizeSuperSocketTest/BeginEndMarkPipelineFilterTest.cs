using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.PipelineFilter;
using Baize.SuperSocket.Primitives;
using Baize.SuperSocket.ProtoBase;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace BaizeSuperSocketTest
{
    public class BeginEndMarkPipelineFilterTest
    {
        private FilterInfo CreateFilterInfo(byte[] beginMark, byte[] endMark)
        {
            FilterInfo filterInfo = new FilterInfo()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = beginMark,
                    EndMark = endMark,
                    ProtoType = ProtoType.BeginEndMark,

                },
                ProductOIDS = new List<string>() { "1", "2" }
            };
            return filterInfo;
        }
        private string GetMarkString(byte[] mark)
        {
            string rtn = "";
            for (int i = 0; i < mark.Length; i++)
            {
                rtn += (char)mark[i];
            }
            return rtn;
        }
        
        /// <summary>
        /// 测试单个起始和结束符不同的用例
        /// </summary>
        [Fact]
        public void TestFilterDifferentSingleMark()
        {
            ChannelOptions channelOptions = new ChannelOptions();
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] beginMark = new byte[] { (byte)'(' };
            byte[] endMark = new byte[] { (byte)')' };
            string beginMarkStr = GetMarkString(beginMark);
            string endMarkStr = GetMarkString(endMark);
            FilterInfo filterInfo = CreateFilterInfo(beginMark,endMark ); 
            IFilter pipelineFilter = new BeginEndMarkPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr+"123456"+endMarkStr, str);
            }

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'9', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr+"12345789"+endMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'(' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'9', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "789" + endMarkStr, str);
            }


            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'6',(byte)')' };
             sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr+"123(56"+endMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'(', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'6', (byte)')',(byte)'7',(byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr+"23(56"+endMarkStr, str);
            }


            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'3', (byte)'5', (byte)'6' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'5', (byte)'5', (byte)'6', (byte)'7',(byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'1', (byte)'(', (byte)'2', (byte)'3', (byte)'4', (byte)')', (byte)'6', (byte)'7', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            if(pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr+"5670123556781(234"+endMarkStr, str);
            }
        }
        /// <summary>
        /// 测试多个起始和结束符不同的用例
        /// </summary>
        [Fact]
        public void TestFilterDifferentMultiMark()
        {
            ChannelOptions channelOptions = new ChannelOptions();
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] beginMark = new byte[] { (byte)'(',(byte)'!' };
            byte[] endMark = new byte[] { (byte)'@',(byte)')' };
            string beginMarkStr = GetMarkString(beginMark);
            string endMarkStr = GetMarkString(endMark);
            FilterInfo filterInfo = CreateFilterInfo(beginMark, endMark);
            IFilter pipelineFilter = new BeginEndMarkPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[] { (byte)'(', (byte)'!', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "2345" + endMarkStr, str);
            }

            data = new byte[] { (byte)'(', (byte)'!', (byte)'2', (byte)'3', (byte)'4', (byte)'5' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "234578" + endMarkStr, str);
            }

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'!' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "78" + endMarkStr, str);
            }


            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'(', (byte)'!', (byte)'5', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "5" + endMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'(', (byte)'!', (byte)'3', (byte)'(', (byte)'5', (byte)'6', (byte)')', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "3(56)" + endMarkStr, str);
            }


            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'3', (byte)'5', (byte)'6' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'!', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'5', (byte)'5', (byte)'6', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'1', (byte)'(', (byte)'2', (byte)'3', (byte)'@', (byte)')', (byte)'6', (byte)'7', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "670123556781(23" + endMarkStr, str);
            }
        }

        /// <summary>
        /// 测试单个起始和结束符相同的用例
        /// </summary>
        [Fact]
        public void TestFilterSameSingleMark()
        {
            ChannelOptions channelOptions = new ChannelOptions();
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] beginMark = new byte[] { (byte)'#' };
            byte[] endMark = new byte[] { (byte)'#' };
            string beginMarkStr = GetMarkString(beginMark);
            string endMarkStr = GetMarkString(endMark);
            FilterInfo filterInfo = CreateFilterInfo(beginMark, endMark);
            IFilter pipelineFilter = new BeginEndMarkPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[] { (byte)'#', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "123456" + endMarkStr, str);
            }

            data = new byte[] { (byte)'#', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'9', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "12345789" + endMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'9', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "789" + endMarkStr, str);
            }


            data = new byte[] { (byte)'#', (byte)'1', (byte)'2', (byte)'3', (byte)'#', (byte)'5', (byte)'6', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "123" + endMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'#', (byte)'2', (byte)'3', (byte)'#', (byte)'5', (byte)'6', (byte)'#', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "23" + endMarkStr, str);
            }


            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'3', (byte)'5', (byte)'6' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'#', (byte)'5', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'5', (byte)'5', (byte)'6', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'1', (byte)'#', (byte)'2', (byte)'3', (byte)'4', (byte)'#', (byte)'6', (byte)'7', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "5670123556781" + endMarkStr, str);
            }
        }
        /// <summary>
        /// 测试多个起始和结束符相同的用例
        /// </summary>
        [Fact]
        public void TestFilterSameMultiMark()
        {
            ChannelOptions channelOptions = new ChannelOptions();
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] beginMark = new byte[] { (byte)'#',(byte)'#' };
            byte[] endMark = new byte[] { (byte)'#',(byte)'#' };
            string beginMarkStr = GetMarkString(beginMark);
            string endMarkStr = GetMarkString(endMark);
            FilterInfo filterInfo = CreateFilterInfo(beginMark, endMark);
            IFilter pipelineFilter = new BeginEndMarkPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[] { (byte)'#', (byte)'#', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'#', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "2345" + endMarkStr, str);
            }

            data = new byte[] { (byte)'#', (byte)'#', (byte)'2', (byte)'3', (byte)'4', (byte)'5' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'#', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "234578" + endMarkStr, str);
            }


            data = new byte[] { (byte)'#', (byte)'1', (byte)'2', (byte)'3', (byte)'#', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'#', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "78" + endMarkStr, str);
            }


            data = new byte[] { (byte)'#', (byte)'1', (byte)'2', (byte)'#', (byte)'#', (byte)'5', (byte)'#', (byte)'#' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "5" + endMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'#', (byte)'2', (byte)'#', (byte)'#', (byte)'5', (byte)'6', (byte)'#', (byte)'#', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "56" + endMarkStr, str);
            }


            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'3', (byte)'5', (byte)'6' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'#', (byte)'#', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'5', (byte)'5', (byte)'6', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'1', (byte)'#', (byte)'2', (byte)'3', (byte)'#', (byte)'#', (byte)'6', (byte)'7', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(beginMarkStr + "670123556781#23" + endMarkStr, str);
            }
        }
        /// <summary>
        /// 测试大数据包
        /// </summary>
        [Fact]
        public void TestFilterBigDataPacket()
        {
            ChannelOptions channelOptions = new ChannelOptions()
            {
                MaxPackageLength = 2048
            };
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] beginMark = new byte[] { (byte)'(' };
            byte[] endMark = new byte[] { (byte)')'};
            string beginMarkStr = GetMarkString(beginMark);
            string endMarkStr = GetMarkString(endMark);
            FilterInfo filterInfo = CreateFilterInfo(beginMark, endMark);
            IFilter pipelineFilter = new BeginEndMarkPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[3000] ;
            for(int i=0;i<data.Length;i++)
            {
                data[i] = (byte)'1';
            }
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[2040];
            for(int i=0;i<data.Length;i++)
            {
                data[i] = (byte)'2';
            }
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[1024];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)'3';
            }
            data[2] = (byte)'(';
            data[3] = (byte)'!';
            data[4] = (byte)'4';
            data[998] = (byte)'4';
            data[999] = (byte)'@';
            data[1000] = (byte)')';
            byte[] contentData = new byte[997] ;
            Array.Copy(data, 3, contentData, 0, contentData.Length);
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                string contentStr = GetMarkString(contentData);
                Assert.Equal(beginMarkStr + contentStr + endMarkStr, str);
            }
        }
        /// <summary>
        /// 测试高频率数据包内存变化
        /// </summary>
        [Fact]
        public void TestFilterHighFreq()
        {
            ChannelOptions channelOptions = new ChannelOptions()
            {
                MaxPackageLength = 2048
            };
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] beginMark = new byte[] { (byte)'(', (byte)'!' };
            byte[] endMark = new byte[] { (byte)'@',(byte)')' };
            string beginMarkStr = GetMarkString(beginMark);
            string endMarkStr = GetMarkString(endMark);
            FilterInfo filterInfo = CreateFilterInfo(beginMark, endMark);
            IFilter pipelineFilter = new BeginEndMarkPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());
            data = new byte[1001];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)'3';
            }
            data[2] = (byte)'(';
            data[3] = (byte)'!';
            data[4] = (byte)'4';
            data[998] = (byte)'4';
            data[999] = (byte)'@';
            data[1000] = (byte)')';
            byte[] contentData = new byte[995];
            Array.Copy(data, 4, contentData, 0, contentData.Length);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int k = 0; k < 100000; k++)
            {
                
                sequence = new ReadOnlySequence<byte>(data);
                if (pipelineFilter.FilterData(baizeSession, sequence))
                {
                    string str = baizeSession.Data.GetString(Encoding.ASCII);
                    string contentStr = GetMarkString(contentData);
                    Assert.Equal(beginMarkStr + contentStr + endMarkStr, str);
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"运行时间:{stopwatch.ElapsedMilliseconds / 1000}s");

        }
    }
}
