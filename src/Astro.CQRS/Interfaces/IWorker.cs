
namespace Astro.CQRS
{
    public interface IWorker
    {
        void Start();

        void Stop();
    }
}
