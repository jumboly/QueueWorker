using System;
using System.Threading.Tasks;
using Library.AsyncJob.Worker;
using Microsoft.Extensions.Logging;
using SampleJob.Job;
using SampleJob.Job.Dto;
using SampleJob.Worker.Services;

namespace SampleJob.Worker.Consumers
{
    public class CalcConsumer : ConsumerBase, ICalcJob
    {
        private readonly ILogger<CalcConsumer> _logger;
        private readonly ICalcService _calcService;

        public CalcConsumer(ILogger<CalcConsumer> logger, ICalcService calcService)
        {
            _logger = logger;
            _calcService = calcService;
        }

        public Task Add(CalcParameter parameter)
        {
            _logger.LogInformation("Add: {a} + {b} = {c}", 
                parameter.A, parameter.B, _calcService.Add(parameter.A, parameter.B));
            
            return Task.CompletedTask;
        }
        
        public Task Sub(CalcParameter parameter)
        {
            _logger.LogInformation("Sub: {a} - {b} = {c}", 
                parameter.A, parameter.B, _calcService.Sub(parameter.A, parameter.B));
            
            return Task.CompletedTask;
        }
    }
}