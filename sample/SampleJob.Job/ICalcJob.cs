using System;
using System.Threading.Tasks;
using SampleJob.Job.Dto;

namespace SampleJob.Job
{
    public interface ICalcJob
    {
        Task Add(CalcParameter parameter);
        Task Sub(CalcParameter parameter);
    }
}