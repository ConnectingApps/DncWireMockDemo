using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ConnectingApps.IntegrationFixture.Shared.GraphQl
{
    public class GraphQlResponse
    {
        public HttpStatusCode StatusCode { get; internal set; }

        public string ResponseContent { get; internal set; }
    }
}
