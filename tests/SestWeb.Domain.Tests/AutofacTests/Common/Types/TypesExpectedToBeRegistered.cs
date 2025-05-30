using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Tests.AutofacTests.Common.Types
{
    public class TypesExpectedToBeRegistered : TypesExpectedTestCaseSource 
    {
        private readonly IEnumerable<Type> _types;

        public TypesExpectedToBeRegistered(IEnumerable<Type> types)
        {
            _types = types;
        }

        public override IEnumerable<Type> Types()
        {
            return _types;
        }
    }
}
