namespace Meetgile.Domain.Colision
{
    using System.Collections;

    public class BitArrayBasedSchedule : ISchedule
    {
        private BitArray day = new BitArray(24 * 60);

        public void ReserveTime()
        {
            // check collision
        }

        public void ReleaseTime() { }
    }
}