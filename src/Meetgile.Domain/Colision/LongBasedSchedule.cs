namespace Meetgile.Domain.Colision
{
    public class LongBasedSchedule : ISchedule
    {
        public long[] day = new long[24];

        public void ReserveTime()
        {
            // check collision
        }

        public void ReleaseTime() { }
    }
}