
namespace Astro.CQRS
{
    using System.Threading.Tasks;

    public interface IWorker
    {
        Task StartAsync();

        void Stop();
    }
}
