using System.Threading.Tasks;

namespace Astro.CQRS
{
    public interface IWorker
    {
        Task StartAsync();

        void Stop();
    }
}
