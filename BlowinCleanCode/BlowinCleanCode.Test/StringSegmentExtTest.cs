using System;
using System.Collections;
using System.Collections.Generic;
using BlowinCleanCode.Extension;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace BlowinCleanCode.Test
{
    public class StringSegmentExtTest
    {
        [Theory]
        [ClassData(typeof(StartWithTestData))]
        public void StartWith(StringSegment lft, StringSegment rgt)
        {
            var startWithResult = lft.StartsWith(rgt, StringComparison.InvariantCultureIgnoreCase);
            var startWithStringResult = lft.ToString().StartsWith(rgt.ToString(), StringComparison.InvariantCultureIgnoreCase);
            
            Assert.Equal(startWithStringResult, startWithResult);
        }
        
        public class StartWithTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new StringSegment(""), new StringSegment("") };
                yield return new object[] { new StringSegment("Dima"), new StringSegment("D") };
                yield return new object[] { new StringSegment("1Dima"), new StringSegment("D") };
                yield return new object[] { new StringSegment("hom"), new StringSegment("hom") };
                yield return new object[] { new StringSegment("shom"), new StringSegment("hom") };
                yield return new object[] { new StringSegment("hom"), new StringSegment("homss") };
                yield return new object[] { new StringSegment("DDD"), new StringSegment("homss") };
                yield return new object[] { new StringSegment("s"), new StringSegment("homss") };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}