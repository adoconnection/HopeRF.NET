using System.Threading;
using System.Threading.Tasks;

namespace RFMLib
{
    public interface ITransceiver
    {
        void Initialize();
        Task<RawData> Recieve();
        Task<RawData> Recieve(CancellationToken token);
        Task<bool> Transmit(byte[] buffer);
        Task<bool> Transmit(byte[] buffer, CancellationToken token);
    }
}