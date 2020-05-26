using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectingApps.IntegrationFixture.Shared
{
    public class IntegrationFixtureException : Exception
    {
        public IntegrationFixtureException(string message) : base(message)
        {
        }
    }
}
