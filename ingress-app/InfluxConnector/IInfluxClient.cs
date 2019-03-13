using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace webapi.Controllers
{
    /// <summary>
    /// Interface for defining Influx Client Class
    /// </summary>
    public interface IInfluxClient : IDisposable
    {

        /// <summary>
        ///  Enqueue method for adding data to buffer.
        /// </summary>
        /// <param name="points">The List of Influx points</param>
        /// <param name="workerQueue">The Flag for indication if provided points should be enqueued into worker buffer or failure handler buffer</param>
        void Enqueue(IList<string> points, bool workerQueue);

        /// <summary>
        ///  Enqueue method for adding single point to buffer.
        /// </summary>
        /// <param name="point">The single Influx point</param>
        /// <param name="workerQueue">The Flag for indication if provided points should be enqueued into worker buffer or failure handler buffer</param>
        void Enqueue(string point, bool workerQueue);

    }
}