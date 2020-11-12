using MassTransit;
using remitservice.Repository;
using sagamessages;
using System.Threading.Tasks;

namespace remitservice
{

    public class RmReceiveConsumer : IConsumer<IAmlData>
    {
      
        public async Task Consume(ConsumeContext<IAmlData> context)
        {
            RemitterData remDataAccess = new RemitterData();
            var amlData = context.Message;
        bool status=   await remDataAccess.UpdateRemitter(amlData.RemitterID,"Block");
            await Task.FromResult(status);
        }
    }
}
