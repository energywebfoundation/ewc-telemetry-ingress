using System.Collections.Generic;

namespace webapi.Controllers
{
    public interface IInfluxConnector
    {
        void Record(List<string> influxLines);
    }
}