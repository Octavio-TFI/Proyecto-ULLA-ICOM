using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Tests
{
    public abstract class BaseTests
    {
        readonly Dictionary<string, APIFactory> _apiFactories = [];

        protected APIFactory CreateAPIFactory(int localLLMPort, int testClientPort)
        {
            string testId = Guid.NewGuid().ToString();

            var apiFactory = new APIFactory(localLLMPort, testClientPort, testId);

            _apiFactories.Add(testId, apiFactory);

            return apiFactory;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var apiFactory in _apiFactories.Values)
            {
                apiFactory.Dispose();
            }
        }
    }
}
