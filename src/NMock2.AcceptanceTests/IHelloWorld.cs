using NMock2;
using NMock2.Attribute;
namespace NMock2.AcceptanceTests
{
	public interface IHelloWorld
	{
        void Hello();
		void Umm();
		void Err();
		void Ahh();
		void Goodbye();
        [Implemented]
		string Ask(string question);
        [Implemented]
        void SetNum(int i);
        [Implemented]
        int GetNum();
	}
}
