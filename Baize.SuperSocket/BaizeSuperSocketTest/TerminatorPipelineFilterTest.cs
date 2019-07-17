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
    public class TerminatorPipelineFilterTest
    {
        private FilterInfo CreateFilterInfo(byte[] endMark)
        {
            FilterInfo filterInfo = new FilterInfo()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    Terminator = endMark,
                    ProtoType = ProtoType.Terminator,

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
        /// 测试单个结束符不同的用例
        /// </summary>
        [Fact]
        public void TestFilterDifferentSingleMark()
        {
            ChannelOptions channelOptions = new ChannelOptions();
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] terminatorMark = new byte[] { (byte)')' };
            string terminatorMarkStr = GetMarkString(terminatorMark);
            FilterInfo filterInfo = CreateFilterInfo( terminatorMark);
            IFilter pipelineFilter = new TerminatorPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(123456" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)')',(byte)'1',(byte)'2' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(123456" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)')', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)')', (byte)'1', (byte)'2' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(  "" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'9', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal( "(12345789" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'(' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'9', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal( "01234(789" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'6', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(123(56" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'(', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'6', (byte)')', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal( "0(23(56" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'3', (byte)'5', (byte)'6' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'5', (byte)'5', (byte)'6', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'1', (byte)'(', (byte)'2', (byte)'3', (byte)'4', (byte)')', (byte)'6', (byte)'7', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal( "01233560123(5670123556781(234" + terminatorMarkStr, str);
            }
        }

        /// <summary>
        /// 测试多个结束符不同的用例
        /// </summary>
        [Fact]
        public void TestFilterDifferentMultiMark()
        {
            ChannelOptions channelOptions = new ChannelOptions();
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] terminatorMark = new byte[] { (byte)'@',(byte)')' };
            string terminatorMarkStr = GetMarkString(terminatorMark);
            FilterInfo filterInfo = CreateFilterInfo(terminatorMark);
            IFilter pipelineFilter = new TerminatorPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(12345" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'@', (byte)')', (byte)'1', (byte)'2' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(12345" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)')', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'@', (byte)')', (byte)'1', (byte)'2' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(")12345" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(1234578" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'(' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("01234(78" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'@', (byte)')' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(123(5" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'(', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'@', (byte)')', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("0(23(5" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'3', (byte)'5', (byte)'6' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'6', (byte)'7' };
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
                Assert.Equal("01233560123(5670123556781(23" + terminatorMarkStr, str);
            }
        }

        /// <summary>
        /// 测试多个结束符不同的用例
        /// </summary>
        [Fact]
        public void TestFilterSameMultiMark()
        {
            ChannelOptions channelOptions = new ChannelOptions();
            byte[] data;
            ReadOnlySequence<byte> sequence;
            byte[] terminatorMark = new byte[] { (byte)'@', (byte)'@' };
            string terminatorMarkStr = GetMarkString(terminatorMark);
            FilterInfo filterInfo = CreateFilterInfo(terminatorMark);
            IFilter pipelineFilter = new TerminatorPipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'@', (byte)'@' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(12345" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'@', (byte)'@', (byte)'1', (byte)'2' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(12345" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)')', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'@', (byte)'@', (byte)'1', (byte)'2' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(")12345" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'@', (byte)'@' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(1234578" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'(' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'7', (byte)'8', (byte)'@', (byte)'@' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("01234(78" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'(', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'@', (byte)'@' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("(123(5" + terminatorMarkStr, str);
            }

            data = new byte[] { (byte)'0', (byte)'(', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'@', (byte)'@', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("0(23(5" + terminatorMarkStr, str);
            }


            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'3', (byte)'5', (byte)'6' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'(', (byte)'5', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'5', (byte)'5', (byte)'6', (byte)'7', (byte)'8' };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'1', (byte)'(', (byte)'2', (byte)'3', (byte)'@', (byte)'@', (byte)'6', (byte)'7', (byte)'6', (byte)'7' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("01233560123(5670123556781(23" + terminatorMarkStr, str);
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
            byte[] endMark = new byte[] { (byte)'@', (byte)')' };
            string endMarkStr = GetMarkString(endMark);
            FilterInfo filterInfo = CreateFilterInfo( endMark);
            IFilter pipelineFilter = new TerminatorPipelineFilter(channelOptions, filterInfo);
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
            byte[] contentData = new byte[999];
            Array.Copy(data, 0, contentData, 0, contentData.Length);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int k = 0; k < 1000; k++)
            {

                sequence = new ReadOnlySequence<byte>(data);
                if (pipelineFilter.FilterData(baizeSession, sequence))
                {
                    string str = baizeSession.Data.GetString(Encoding.ASCII);
                    string contentStr = GetMarkString(contentData);
                    Assert.Equal(  contentStr + endMarkStr, str);
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"运行时间:{stopwatch.ElapsedMilliseconds / 1000}s");

        }
    }
}
