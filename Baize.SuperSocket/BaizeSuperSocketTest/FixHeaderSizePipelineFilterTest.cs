﻿using Baize.IPlugin.SuperSocket;
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
    public class FixHeaderSizePipelineFilterTest
    {
        private FilterInfo CreateFilterInfo(int size)
        {
            FilterInfo filterInfo = new FilterInfo()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    Size = size,
                    ProtoType = ProtoType.FixHeaderSize,

                },
                ProductOIDS = new List<string>() { "1", "2" }
            };
            filterInfo.BasePortocalFilterInfo.GetBodyLengthFromHeader = GetBodyLength;
            return filterInfo;
        }

        public int GetBodyLength(ReadOnlySequence<byte> sequence)
        {
            int rtn= sequence.GetInt16(7);
            return rtn;
        }
        /// <summary>
        /// 测试单个结束符不同的用例
        /// </summary>
        [Fact]
        public void TestFilterDifferentSingleMark()
        {
            ListenOptions channelOptions = new ListenOptions();
            List<byte> data=new List<byte>();
            List<byte> allData = new List<byte>();
            ReadOnlySequence<byte> sequence;
            FilterInfo filterInfo = CreateFilterInfo(9);
            IFilter pipelineFilter = new FixHeaderSizePipelineFilter(channelOptions.ChannelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());
            data.AddRange(  new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)0x02, (byte)0x00, (byte)'9', (byte)('1') });
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                Assert.Equal(Encoding.ASCII.GetString(data.ToArray()), str);
            }

            data.Clear();
            data.AddRange(new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)0x04, (byte)0x00, (byte)'9', (byte)('1') });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data.Clear();
            data.AddRange(new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4' });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            if(pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                string contentStr = Encoding.ASCII.GetString(allData.GetRange(0, 13).ToArray());
                Assert.Equal(contentStr, str);
            }

            data.Clear();
            data.AddRange(new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)0x04  });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data.Clear();
            data.AddRange(new byte[] { (byte)0x00, (byte)'9', (byte)('1'),(byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4' });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                string contentStr = Encoding.ASCII.GetString(allData.GetRange(0, 13).ToArray());
                Assert.Equal(contentStr, str);
                allData.Clear();
            }

            data.Clear();
            data.AddRange(new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6' });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data.Clear();
            data.AddRange(new byte[] { (byte)0x04,(byte)0x00, (byte)'9', (byte)('1'), (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4' });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                string contentStr = Encoding.ASCII.GetString(allData.GetRange(0, 13).ToArray());
                Assert.Equal(contentStr, str);
                allData.Clear();
            }

            //测试包头长度在中间段
            data.Clear();
            data.AddRange(new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6' });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data.Clear();
            data.AddRange(new byte[] { (byte)0x04, (byte)0x00, (byte)'9', (byte)('1') });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            Assert.False(pipelineFilter.FilterData(baizeSession, sequence));
            data.Clear();
            data.AddRange(new byte[] {  (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4' });
            allData.AddRange(data);
            sequence = new ReadOnlySequence<byte>(data.ToArray());
            if (pipelineFilter.FilterData(baizeSession, sequence))
            {
                string str = baizeSession.Data.GetString(Encoding.ASCII);
                string contentStr = Encoding.ASCII.GetString(allData.GetRange(0, 13).ToArray());
                Assert.Equal(contentStr, str);
                allData.Clear();
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
            FilterInfo filterInfo = CreateFilterInfo(9);
            IFilter pipelineFilter = new FixHeaderSizePipelineFilter(channelOptions, filterInfo);
            BaizeSession baizeSession = new BaizeSession(Guid.NewGuid().ToString());
            data = new byte[2048];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Convert.ToByte(i / 256);
            }
            data[7] = 0x00;
            data[8] = 0x02;
            int len = 521;
            byte[] contentData = new byte[len];
            Array.Copy(data, 0, contentData, 0, len);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int k = 0; k < 10000; k++)
            {
                sequence = new ReadOnlySequence<byte>(data);
                if (pipelineFilter.FilterData(baizeSession, sequence))
                {
                    string str = baizeSession.Data.GetString(Encoding.ASCII);
                    string contentStr = Encoding.ASCII.GetString(contentData);
                    Assert.Equal(contentStr, str);
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"运行时间:{stopwatch.ElapsedMilliseconds / 1000}s");

        }
    }
}
