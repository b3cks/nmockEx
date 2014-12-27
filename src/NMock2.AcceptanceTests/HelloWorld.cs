using System;
using System.Collections.Generic;
using System.Text;
using NMock2.Attribute;

namespace NMock2.AcceptanceTests
{
    class HelloWorld : IHelloWorld
    {

        private int num;

        public HelloWorld()
        {
        }
        
        public HelloWorld(int num)
        {
            this.num = num;
        }

        public void Hello()
        {

        }

        public void Umm()
        {

        }

        public void Err()
        {

        }

        public void Ahh()
        {

        }

        public void Goodbye()
        {

        }

        public string Ask(string question)
        {
            if (question.Equals("Name"))
            {
                return "Alice";
            }
            else
            {
                return "";
            }
        }

        public void SetNum(int num)
        {
            this.num = num;
        }

        public int GetNum()
        {
            return num;
        }
    
    }
}
