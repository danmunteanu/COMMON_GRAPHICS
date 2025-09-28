namespace CommonGraphics
{
    public partial class EditorBase: UserControl
    {
        //  ERROR STACK
        protected Stack<string> mErrorStack = new();

        public EditorBase()
        {
            InitializeComponent();
        }

        public virtual void LoadState(object item)
            => throw new NotImplementedException("LoadState is not implemented");

        public virtual void SaveState(object item)
            => throw new NotImplementedException("SaveState is not implemented");

        //  Resets the editor state - override in derived classes
        public virtual void ResetState() {}

        public virtual bool ValidateState() => true;

        public void PushError(string error)
            => mErrorStack.Push(error);

        public string PopError()
            => mErrorStack.Pop();

        public bool HasErrors()
            => mErrorStack.Count > 0;

        public void ClearErrors()
            => mErrorStack.Clear();
    }
}
