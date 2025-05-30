using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Tests.AutofacTests.Common.Types
{
    public class TypesExpectedToBeResolved : TypesExpectedTestCaseSource 
    {
        private readonly IEnumerable<Type> _types;

        public TypesExpectedToBeResolved(IEnumerable<Type> types)
        {
            _types = types;
        }

        public override IEnumerable<Type> Types()
        {
            return _types;
        }
    }
}
