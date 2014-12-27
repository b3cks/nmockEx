using System.ComponentModel.Design;
using NUnit.Framework;

namespace NMock2.AcceptanceTests
{
	[TestFixture]
	public class MockeryAcceptanceTest
	{
		[Test]
		public void CallingVerifyOnMockeryShouldEnableMockeryToBeUsedSuccessfullyForOtherTests()
		{
			Mockery mocks = new Mockery();
			IMockedType mockWithUninvokedExpectations = (IMockedType) mocks.NewMock(typeof(IMockedType));
			Expect.Once.On(mockWithUninvokedExpectations).Method("Method").WithNoArguments();
			try
			{
				mocks.VerifyAllExpectationsHaveBeenMet();
				Assert.Fail("Expected ExpectationException to be thrown");
			}
			catch(NMock2.Internal.ExpectationException expected)
			{
				Assert.IsTrue(expected.Message.IndexOf("not all expected invocations were performed") != -1);
			}
			
			IMockedType mockWithInvokedExpectations = (IMockedType) mocks.NewMock(typeof(IMockedType));
			Expect.Once.On(mockWithInvokedExpectations).Method("Method").WithNoArguments();
			mockWithInvokedExpectations.Method();
			mocks.VerifyAllExpectationsHaveBeenMet();
		}

       [Test]
       public void MockObjectsMayBePlacedIntoServiceContainers()
       {
           Mockery mocks = new Mockery();
           ServiceContainer container = new ServiceContainer();

           IMockedType mockedType =
               mocks.NewMock(typeof (IMockedType)) as IMockedType;

           container.AddService(typeof (IMockedType), mockedType);

           Assert.AreSame(mockedType, container.GetService(typeof(IMockedType)));
       }
	}
	
	public interface IMockedType
	{
		void Method();
	}
}
