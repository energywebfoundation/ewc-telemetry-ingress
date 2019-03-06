using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace webapi.Controllers
{
    public interface IInfluxClient : IDisposable
    {
         //Task WriteToInflux(IList<string> pointsList);
         void Enqueue(IList<string> points, bool workerQueue);

         void Enqueue(string point, bool workerQueue);
        
    }
}