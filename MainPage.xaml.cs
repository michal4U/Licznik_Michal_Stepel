using System.IO;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace Counter
{
    public partial class MainPage : ContentPage
    {
        private const string FileName = "counters.json";

        public MainPage()
        {
            InitializeComponent();
            LoadCounters();
        }


        private void OnAddCounterClicked(object sender, EventArgs e)
        {
            var counterName = CounterNameEntry.Text;
            var initialValueText = CounterValueEntry.Text;

            if (string.IsNullOrWhiteSpace(counterName))
            {
                DisplayAlert("Błąd", "Proszę podać nazwę licznika", "OK");
                return;
            }

            int initialValue;

            if (string.IsNullOrWhiteSpace(initialValueText))
            {
                initialValue = 0;
            }
            else
            {
                initialValue = int.Parse(initialValueText);
            }

            AddCounter(counterName, initialValue);
            CounterNameEntry.Text = string.Empty;
            CounterValueEntry.Text = string.Empty;
        }


        // dodawanie licznika
        private void AddCounter(string counterName, int initialValue)
        {
            var counterLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

            var nameLabel = new Label
            {
                Text = counterName,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10) 
            };

            var valueLabel = new Label
            {
                Text = initialValue.ToString(), 
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            var minusButton = new Button
            {
                Text = "-",
                Command = new Command(() =>
                {
                    var currentValue = int.Parse(valueLabel.Text);
                    valueLabel.Text = (currentValue - 1).ToString();
                    SaveCounters(); 
                })
            };

            var plusButton = new Button
            {
                Text = "+",
                Command = new Command(() =>
                {
                    var currentValue = int.Parse(valueLabel.Text);
                    valueLabel.Text = (currentValue + 1).ToString();
                    SaveCounters(); 
                })
            };

            counterLayout.Children.Add(nameLabel);
            counterLayout.Children.Add(minusButton);
            counterLayout.Children.Add(valueLabel);
            counterLayout.Children.Add(plusButton);

            CountersLayout.Children.Add(counterLayout);
            SaveCounters();
        }
        //zapisywanie
        private async void SaveCounters()
        {
            var counters = new List<CounterData>();

            foreach (StackLayout counterLayout in CountersLayout.Children)
            {
                var nameLabel = (Label)counterLayout.Children[0];
                var valueLabel = (Label)counterLayout.Children[2];

                var counterData = new CounterData
                {
                    Name = nameLabel.Text,
                    Value = int.Parse(valueLabel.Text)
                };
                counters.Add(counterData);
            }

            var json = JsonSerializer.Serialize(counters);
            var filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);

            await File.WriteAllTextAsync(filePath, json);
        }
        //ladowanie
        private async void LoadCounters()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    var counters = JsonSerializer.Deserialize<List<CounterData>>(json);
                    foreach (var counter in counters)
                    {
                        AddCounter(counter.Name, counter.Value);
                    }
                }
            }
        }
    }
    //przechowywanie danych
    public class CounterData
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
