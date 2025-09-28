namespace CommonGraphics
{
    public static class GenericFactory<TBase>
    {
        //  delegate to a creator which returs an instance of classes derived from TBase
        public delegate TBase Creator();

        //  The dictionary of creators
        private static readonly Dictionary<string, Creator> mCreators = new();

        public static int Count => mCreators.Count;

        //  Registers a creator delegate by id
        public static void Register(string id, Creator creator)
        {
            //  TBD: Localize the exception messages
            const string KMsgNullId = "ID cannot be null or empty.";
            const string KErrDelegateNull = "The creator delegate cannot be null.";
            const string KErrCreatorAlreadyRegistered = "There is a creator already registered with ID {0}";

            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(KMsgNullId, nameof(id));
            
            if (creator == null)
                throw new ArgumentNullException(nameof(creator), KErrDelegateNull);

            if (!mCreators.TryAdd(id, creator))
                throw new InvalidOperationException(string.Format(KErrCreatorAlreadyRegistered, id));
        }

        //  Calls the associated creator for the specified ID
        public static TBase Create(string id)
        {
            if (mCreators.TryGetValue(id, out var creator))
                return creator();

            //  TODO: Localize the exception messages
            throw new KeyNotFoundException($"There is no creator registered with the ID \"{id}\".");
        }

        //  Make the key strings available
        public static IReadOnlyList<string> CreatorKeys 
            => mCreators.Keys.ToList().AsReadOnly();
    }
}
