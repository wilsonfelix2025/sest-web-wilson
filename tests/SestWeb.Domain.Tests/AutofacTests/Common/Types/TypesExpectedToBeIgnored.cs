using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Tests.AutofacTests.Common.Types
{
    public class TypesExpectedToBeIgnored : TypesExpectedTestCaseSource {
        private readonly IEnumerable<Type> _types;

        public TypesExpectedToBeIgnored(IEnumerable<Type> types)
        {
            _types = types;
        }

        public override IEnumerable<Type> Types()
        {
            return _types;
        }
    }
}
