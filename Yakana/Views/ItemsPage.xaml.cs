using System;
using System.IO;
using System.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Xamarin.Forms;

namespace Yakana
{


  public partial class ItemsPage : ContentPage
  {
    ItemsViewModel viewModel;
    private static readonly HttpClient client = new HttpClient();
    private string value;

    public ItemsPage()
    {
      InitializeComponent();

      BindingContext = viewModel = new ItemsViewModel();

    }


    async void AddItem_Clicked(object sender, EventArgs e)
    {
      //await Navigation.PushAsync(new NewItemPage());

      var url = "http://sandbox.afterbar.mx/v1/configurations";
      JsonValue configJson = await FetchConfigAsync(url);
      var configurations = configJson["data"];

      foreach (JsonValue config in configurations)
      {
        if (config["name"] == "OnService")
          Console.Out.WriteLine("OnService: {0}", config["value"]);
        viewModel.onServiceStatus = config["value"];
        value = config["value"];
        button.Text = config["value"];
      }
    }

    async void OnButtonClicked(object sender, EventArgs args)
    {
      JsonValue configJson = await PostConfigAsync();
      var config = configJson["data"];

      value = config["value"];
      button.Text = config["value"];

    }

    private async Task<JsonValue> FetchConfigAsync(string url)
    {
      // Create an HTTP web request using the URL:
      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
      request.ContentType = "application/json";
      request.Method = "GET";

      // Send the request to the server and wait for the response:
      using (WebResponse response = await request.GetResponseAsync())
      {
        // Get a stream representation of the HTTP web response:
        using (Stream stream = response.GetResponseStream())
        {
          // Use this stream to build a JSON document object:
          JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
          Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

          // Return the JSON document:
          return jsonDoc;
        }
      }
    }

    private async Task<JsonValue> PostConfigAsync()
    {

      object data = new
      {
        configuration = new
        {
          name = "OnService",
          value = "true"
        }
      };

      var json = JsonConvert.SerializeObject(data);


      var url = "http://sandbox.afterbar.mx/v1/configurations/2";

      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
      request.ContentType = "application/json";
      request.Accept = "application/json";
      request.Method = "PUT";

      using (var streamWriter = new StreamWriter(request.GetRequestStream()))
      {

        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
      }

      // Send the request to the server and wait for the response:
      using (WebResponse response = await request.GetResponseAsync())
      {
        // Get a stream representation of the HTTP web response:
        using (Stream stream = response.GetResponseStream())
        {
          // Use this stream to build a JSON document object:
          JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
          Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

          // Return the JSON document:
          return jsonDoc;
        }
      }

    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      if (viewModel.Items.Count == 0)
        viewModel.LoadItemsCommand.Execute(null);
    }
  }
}
