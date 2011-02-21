#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using NUnit.Framework;

namespace Colombo.Clerk.Client.Tests
{
    [TestFixture]
    public class ClerkMessageAttributeTest
    {
        [Test]
        public void It_should_return_a_basic_message()
        {
            var msg = new ClerkMessageAttribute("Foo");

            Assert.That(msg.GetRealMessage(null, null), Is.EqualTo("Foo"));
        }

        [Test]
        public void It_should_do_string_interpolation_with_simple_message()
        {
            var msg = new ClerkMessageAttribute("$request.FirstName become $response.Name.");

            Assert.That(
                msg.GetRealMessage(
                    new TestRequest { FirstName = "Foo" },
                    new TestResponse { Name = "Bar" }
                ),
                Is.EqualTo("Foo become Bar."));
        }

        [Test]
        public void It_should_allow_string_interpolation_with_methods()
        {
            var msg = new ClerkMessageAttribute("His name is $request.GetName().");

            Assert.That(
                msg.GetRealMessage(
                    new TestRequest { FirstName = "Foo", LastName = "Bar" },
                    null
                ),
                Is.EqualTo("His name is Foo Bar."));
        }

        [Test]
        public void It_should_return_resources()
        {
            var msg = new ClerkMessageAttribute("SimpleTest", typeof (TestsResourceClerkMessage));

            Assert.That(msg.GetRealMessage(null, null), Is.EqualTo("Something"));
        }

        [Test]
        public void It_should_do_string_interpolation_with_resources()
        {
            var msg = new ClerkMessageAttribute("InterpolTest", typeof(TestsResourceClerkMessage));

            Assert.That(
                msg.GetRealMessage(
                    new TestRequest { LastName = "Foo" },
                    new TestResponse { Name = "Bar" }
                ),
                Is.EqualTo("Name is Foo for Bar."));
        }

        [Test]
        public void It_should_not_throw_exception_if_resource_not_found()
        {
            var msg = new ClerkMessageAttribute("NoRes", typeof(TestsResourceClerkMessage));

            Assert.That(msg.GetRealMessage(null, null), Is.Null);
        }

        [Test]
        public void It_should_not_throw_if_interpolation_is_incorrect()
        {
            var msg = new ClerkMessageAttribute("$request.Name");

            Assert.That(msg.GetRealMessage(null, null), Is.EqualTo("$request.Name"));
        }

        public class TestResponse : Response
        {
            public string Name { get; set; }
        }

        public class TestRequest : Request<TestResponse>
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string GetName()
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
