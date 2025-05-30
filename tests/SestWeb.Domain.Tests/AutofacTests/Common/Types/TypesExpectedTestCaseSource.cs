using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Tests.AutofacTests.Common.Types
{
    public abstract class TypesExpectedTestCaseSource : IEnumerable<object[]>
    {
        public abstract IEnumerable<Type> Types();

        public IEnumerator<object[]> GetEnumerator()
        {
            return this.Types()
                .Select(type => new object[] { type })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
