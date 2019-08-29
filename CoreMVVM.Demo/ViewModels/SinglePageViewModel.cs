namespace CoreMVVM.Demo.ViewModels
{
    internal class SinglePageViewModel : BaseModel
    {
        public SinglePageViewModel()
        {
            Text = "Hello World!";
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
    }
}