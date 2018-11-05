namespace Gadget.Widgets
{
	public interface IWidgetWithInternet
	{
		int UpdateInternetInterval { get; set; }

		void UpdateInternet();
	}
}
