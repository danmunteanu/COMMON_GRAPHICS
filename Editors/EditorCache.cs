namespace CommonGraphics
{
    public class EditorCache
    {
        //  Cache of Editors
        private Dictionary<string, EditorBase> mEditors = new();

        //  Tries to find an editor in the cache,
        //  if not found, it creates the editor
        //  if found, returns the instance
        public EditorBase? FindOrCreateEditor(string name)
        {
            EditorBase? editor = null;
            if (!mEditors.TryGetValue(name, out editor))
            {
                editor = EditorFactory.Create(name);
                mEditors.Add(name, editor);
            }
            return editor;
        }

        public void ClearEditors()
        {
            mEditors.Clear();
        }

        public void ResetEditors()
        {
            //  Resets all editors by calling ResetState on each
            foreach (var keyValuePair in mEditors)
                keyValuePair.Value.ResetState();
        }

    }
}
