using System;
using System.Collections.Generic;
using System.Text;

namespace NMock2.AcceptanceTests
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Mockery mocks = new Mockery();
            IHelloWorld helloWorld = (IHelloWorld)mocks.NewMock(typeof(IHelloWorld), typeof(HelloWorld), null);

            //HelloWorld hw = (HelloWorld)mocks.NewMock(typeof(HelloWorld));

            //HelloWorld real = new HelloWorld();

            Stub.On(helloWorld).Method("Ask").With("Name").Will(Return.Value("Bob"));
            Stub.On(helloWorld).Method("Ask").With("Age").Will(Return.Value("30"));
            
            //ContractVerify.Check(real, helloWorld);

            //helloWorld.SetNum(6);
            Console.WriteLine(helloWorld.GetNum());

            Console.WriteLine(helloWorld.Ask("Name"));

            Console.ReadLine();
        }
    }
}
