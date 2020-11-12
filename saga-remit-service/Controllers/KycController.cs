using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rabbitmqbus;
using remitservice.Models;
using remitservice.Repository;
using sagamessages;

namespace remitservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KycController : ControllerBase
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IRemitterDataAccess _remitterDataAccess;

        public KycController(ISendEndpointProvider sendEndpointProvider, IRemitterDataAccess remitterDataAccess)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _remitterDataAccess = remitterDataAccess;
        }
        public IActionResult Get()
        {
            return Ok("hello");
        }
        [HttpGet]
        [Route("GetRemitter")]
        public async Task<IActionResult> GetCustomer(Guid guid)
        {
            return Ok(await _remitterDataAccess.GetRemitterById(guid));
        }
        [HttpDelete]
        [Route("DeleteRemitter")]
        public async Task<IActionResult> DeleteRemitter(Guid guid)
        {
            return Ok(await _remitterDataAccess.DeleteRemitter(guid));
        }
        [HttpGet]
        [Route("GetRemitters")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _remitterDataAccess.GetRemitterList());
        }
        [HttpPost]
        [Route("validatekyc")]
        public async Task<IActionResult> Post(RemitterModel remitter)
        {
            try
            {
                Guid guid = await _remitterDataAccess.SaveRemitter(remitter);

                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:" + BusConfiguration.QueueName));
                await endpoint.Send<IRemitterMessage>(new
                {
                    RemitterID = remitter.ID,
                    RemitterFirstName = remitter.FirstName,
                    RemitterLastName = remitter.LastName,
                    RemitterCountry = remitter.Country,
                    RemitterStatus = remitter.Status,
                });
                return Ok(guid);
            }
            catch (Exception ex)
            {

                return Ok(ex.InnerException.Message);
            }
         
        }
        [HttpPost]
        [Route("validatekycsaga")]
        public async Task<IActionResult> ValidateKycSaga(RemitterModel remitter)
        {
            try
            {
               // Guid guid = await _remitterDataAccess.SaveRemitter(remitter);

                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:" + BusConfiguration.SagaQueue));
                await endpoint.Send<IAmlValidationEvent>(new
                {
                    RemitterID = remitter.ID,
                    RemitterFirstName = remitter.FirstName,
                    RemitterLastName = remitter.LastName,
                    RemitterCountry = remitter.Country,
                    RemitterStatus = remitter.Status,
                });
                return Ok(remitter.ID);
            }
            catch (Exception ex)
            {

                return Ok(ex.InnerException.Message);
            }

        }
    }
}
