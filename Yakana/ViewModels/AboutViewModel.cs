using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Yakana
{
  public class AboutViewModel : BaseViewModel
  {
    public AboutViewModel()
    {
      Title = "About";

      OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://afterbar.com")));
    }

    public ICommand OpenWebCommand { get; }
  }
}
