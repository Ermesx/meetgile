using Xunit;

namespace Meetgile.Domain.Test
{
    using System;
    using System.Collections;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public class PerformanceBitArrayVSlong
    {
        [Fact(Skip = "Benchmark")]
        public void ByteArrayVSlongTest()
        {
            BenchmarkRunner.Run<BitArrayVSlong>();
        }

        [Fact]
        public void HowAndWorking()
        {
            // Arrange
            var test = new BitArrayVSlong();

            // Act 
            var bitArray = test.BitArray();
            var @long = test.Long();

            // Assert
            var empty = new BitArray(new[] { 0b0000 });

            Assert.Equal(empty, bitArray);
            Assert.Equal(0L, @long);
            Assert.True(@long == GetIntFromBitArray(bitArray));
        }

        private static long GetIntFromBitArray(BitArray bitArray)
        {
            var array = new byte[8];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToInt64(array, 0);
        }

        public class BitArrayVSlong
        {
            private long[] mask1 = { 1073741824 };
            private long[] mask2 = { 68719476736 };

            private BitArray maskBytes1 = new BitArray(new [] { 0b0010 });
            private BitArray maskBytes2 = new BitArray(new[] { 0b0001 });

            [Benchmark]
            public BitArray BitArray() => maskBytes1.And(maskBytes2);

            [Benchmark]
            public long Long() => (mask1[0] & mask2[0]);
        }
    }
}
