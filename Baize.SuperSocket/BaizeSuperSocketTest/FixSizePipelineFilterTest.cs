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
    public class FixSizePipelineFilterTest
    {
        private FilterInfo CreateFilterInfo(int size)
        {
            FilterInfo filterInfo = new FilterInfo()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    Size=size,
                    ProtoType = ProtoType.FixSize,

                },
                ProductOIDS = new List<string>() { "1", "2" }
            };
            return filterInfo;
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
            FilterInfo filterInfo = CreateFilterInfo(10);
            IFilter pipelineFilter = new FixSizePipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7',(byte)'8',(byte)'9' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("0123456789" , str);
            }

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'6', (byte)'7', (byte)'8', (byte)'9' };
            sequence = new ReadOnlySequence<byte>(data);
            if(pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("0123456789", str);
            }

            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9',(byte)'1',(byte)'2' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("0123456789", str);
            }


            data = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', };
            sequence = new ReadOnlySequence<byte>(data);
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));

            data = new byte[] { (byte)'6', (byte)'7', (byte)'8', (byte)'9',(byte)'1',(byte)'2',(byte)'3' };
            sequence = new ReadOnlySequence<byte>(data);
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal("0123456789", str);
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
            FilterInfo filterInfo = CreateFilterInfo(1024);
            IFilter pipelineFilter = new FixSizePipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());
            data = new byte[2048];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] =Convert.ToByte( i / 256);
            }
            byte[] contentData = new byte[1024];
            Array.Copy(data, 0, contentData, 0, 1024);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int k = 0; k < 1000; k++)
            {

                sequence = new ReadOnlySequence<byte>(data);
                if (pipelineFilter.FilterData(baizeSession, sequence))
                {
                    string str = baizeSession.Data.GetString(Encoding.ASCII);
                    string contentStr = Encoding.ASCII.GetString(contentData);
                    Assert.Equal(contentStr , str);
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"运行时间:{stopwatch.ElapsedMilliseconds / 1000}s");

        }

    }
}
