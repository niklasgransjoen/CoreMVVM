namespace CoreMVVM.Demo.ViewModels
{
    internal class SinglePageViewModel : BaseModel
    {
        public SinglePageViewModel()
        {
            Text = "Hello World!";
        }

        private string text;

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }
    }
}