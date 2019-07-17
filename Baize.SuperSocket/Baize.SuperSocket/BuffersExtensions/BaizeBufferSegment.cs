using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Baize.SuperSocket.BuffersExtensions
{
    /// <summary>
    /// 自定义数据缓存段
    /// </summary>
    public class BaizeBufferSegment:ReadOnlySequenceSegment<byte>
    {
        private BaizeBufferSegment _next;
        private int _end;

        /// <summary>
        /// The End represents the offset into AvailableMemory where the range of "active" bytes ends. At the point when the block is leased
        /// the End is guaranteed to be equal to Start. The value of Start may be assigned anywhere between 0 and
        /// Buffer.Length, and must be equal to or less than End.
        /// </summary>
        public int End
        {
            get => _end;
            set
            {
                _end = value;
                Memory = AvailableMemory.Slice(0, value);
            }
        }

        /// <summary>
        /// Reference to the next block of data when the overall "active" bytes spans multiple blocks. At the point when the block is
        /// leased Next is guaranteed to be null. Start, End, and Next are used together in order to create a linked-list of discontiguous
        /// working memory. The "active" memory is grown when bytes are copied in, End is increased, and Next is assigned. The "active"
        /// memory is shrunk when bytes are consumed, Start is increased, and blocks are returned to the pool.
        /// </summary>
        public BaizeBufferSegment NextSegment
        {
            get => _next;
            set
            {
                Next = value;
                _next = value;
            }
        }

        public void SetUnownedMemory(ReadOnlyMemory<byte> memory)
        {
            AvailableMemory = memory;
            End = memory.Length;
        }

        public void ResetMemory()
        {
            // Order of below field clears is significant as it clears in a sequential order
            Next = null;
            RunningIndex = 0;
            Memory = default;
            _next = null;
            _end = 0;
            AvailableMemory = default;
        }

        public ReadOnlyMemory<byte> AvailableMemory { get; private set; }

        public int Length => End;



        public void SetNext(BaizeBufferSegment segment)
        {

            NextSegment = segment;

            segment = this;

            while (segment.Next != null)
            {
                segment.NextSegment.RunningIndex = segment.RunningIndex + segment.Length;
                segment = segment.NextSegment;
            }
        }
    }
}
