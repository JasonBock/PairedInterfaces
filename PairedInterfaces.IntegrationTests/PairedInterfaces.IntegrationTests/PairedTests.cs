namespace PairedInterfaces.IntegrationTests
{
	public interface IDummy { }

	internal interface IDummyInternal { }

	public interface IPaired { }

	internal interface IPairedInternal { }

	public class AClass : IPairedInternal { }

	public class AnotherClass : IDummy { }

	public class PairedClass : IPaired { }
}
