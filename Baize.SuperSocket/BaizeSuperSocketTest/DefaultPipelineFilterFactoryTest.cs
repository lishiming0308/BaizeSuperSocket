using Baize.IPlugin.Model;
using Baize.IPlugin.SuperSocket;
using Baize.SuperSocket.Channel;
using Baize.SuperSocket.PipelineFilter;
using Baize.SuperSocket.Primitives;
using Baize.SuperSocket.ProtoBase;
using Microsoft.Extensions.Options;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BaizeSuperSocketTest
{
    public class DefaultPipelineFilterFactoryTest
    {
        public int GetBodyLengthFromHeader(ReadOnlySequence<byte> data)
        {
            return 100;
        }
        public int GetBodyLengthFromHeader2(ReadOnlySequence<byte> data)
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void  TestCreate()
        {
            ChannelOptions channelOptions = new ChannelOptions();
            DefaultPipelineFilterFactory defaultPipelineFilterFactory = new DefaultPipelineFilterFactory(channelOptions);
            List<ProductConfig>  productInfos=CreateProductInfos();
            var var= defaultPipelineFilterFactory.Create(productInfos);
            Assert.Equal(8, var.Count);
        }
        /// <summary>
        /// 创建参数信息
        /// </summary>
        private List<ProductConfig> CreateProductInfos()
        {
            List<ProductConfig> productInfos = new List<ProductConfig>();
            ProductConfig productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { },
                    EndMark = new byte[] { },
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.ShortMessage,
                    Size = 0,
                    Terminator = new byte[] { }
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
             productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { },
                    EndMark = new byte[] { },
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.ShortMessage,
                    Size = 2,
                    Terminator = new byte[] { }
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = null,
                    EndMark = null,
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.ShortMessage,
                    Size = 0,
                    Terminator = null
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = null,
                    EndMark = null,
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.FixSize,
                    Size = 100,
                    Terminator = null
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = null,
                    EndMark = null,
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.FixSize,
                    Size = 80,
                    Terminator = null
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = null,
                    EndMark = null,
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.Terminator,
                    Size = 80,
                    Terminator = new byte[] { (byte)'\r', (byte)'\n' }
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = null,
                    EndMark = null,
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.Terminator,
                    Size = 80,
                    Terminator = new byte[] { (byte)'\r', (byte)'\n' }
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { (byte)'\r' },
                    EndMark = new byte[] { (byte)'\n' },
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.BeginEndMark,
                    Size = 80,
                    Terminator = null
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { (byte)'\r' },
                    EndMark = new byte[] { (byte)'\n' },
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.BeginEndMark,
                    Size = 80,
                    Terminator = null
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { (byte)'\r' },
                    EndMark = new byte[] { (byte)'\0' },
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.BeginEndMark,
                    Size = 80,
                    Terminator = null
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { (byte)'\r' },
                    EndMark = new byte[] { (byte)'\0' },
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.FixHeaderSize,
                    Size = 80,
                    Terminator = null
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfo.BasePortocalFilterInfo.GetBodyLengthFromHeader = GetBodyLengthFromHeader;
            productInfos.Add(productInfo);
            productInfo = new ProductConfig()
            {
                BasePortocalFilterInfo = new BasePortocalFilterInfo()
                {
                    BeginMark = new byte[] { (byte)'\r' },
                    EndMark = new byte[] { (byte)'\0' },
                    GetBodyLengthFromHeader = null,
                    ProtoType = ProtoType.FixHeaderSize,
                    Size = 80,
                    Terminator = null
                },
                ProductOID = Guid.NewGuid().ToString()
            };
            productInfo.BasePortocalFilterInfo.GetBodyLengthFromHeader = GetBodyLengthFromHeader2;
            productInfos.Add(productInfo);
            return productInfos;
        }
      
    }
}
