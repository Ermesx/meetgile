namespace Meetgile.Domain.Colision
{
    public interface ISchedule
    {
        void ReserveTime();
        void ReleaseTime();
    }
}