// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Tests.ExtensionsTests
{
    [Collection("DefaultSettings")]
    public class TypeExtensionTest
    {
        private readonly ITestOutputHelper _output;

        public TypeExtensionTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForTypeExtensionGetBaseTypesShouldSucceed()
        {
            yield return new object[]
            {
                "Type with more than one base type",
                typeof(ChildType),
                new List<Type> {typeof(BaseTypeA), typeof(BaseTypeB), typeof(BaseTypeC)}
            };

            yield return new object[]
            {
                "Type one base type",
                typeof(BaseTypeB),
                new List<Type> {typeof(BaseTypeC)}
            };

            yield return new object[]
            {
                "Type one no base type",
                typeof(BaseTypeC),
                new List<Type>()
            };
        }


        [Theory]
        [MemberData(nameof(GetTestCasesForTypeExtensionGetBaseTypesShouldSucceed))]
        public void TypeExtensionGetBaseTypesShouldSucceed(
            string testCaseName,
            Type childType,
            List<Type> expectedBaseTypes)
        {
            _output.WriteLine(testCaseName);

            childType.GetBaseTypes().Should().BeEquivalentTo(expectedBaseTypes);
        }
    }

    public class BaseTypeA : BaseTypeB
    {
    }

    public class BaseTypeB : BaseTypeC
    {
    }

    public class BaseTypeC
    {
    }

    public class ChildType : BaseTypeA
    {
    }
}