using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace webapi.Controllers
{
    public interface IInfluxClient : IDisposable
    {
         //Task WriteToInflux(IList<string> pointsList);
         void Enqueue(IList<string> point, bool workerQueue);
        
    }
}