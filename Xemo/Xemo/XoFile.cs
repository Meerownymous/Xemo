//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Xemo.IDCard;
//using Xemo.Information;
//using Xemo.Bench;

//namespace Xemo.Xemo
//{
//    /// <summary>
//    /// Information stored in a file.
//    /// </summary>
//    public sealed class XoFile : XoEnvelope
//    {
//        public XoFile(FileInfo storage) : this(
//            string.Empty, string.Empty, storage
//        )
//        { }

//        public XoFile(string id, string subject, FileInfo storage) : base(
//            new XoFile<object>(id, subject, storage, new List<object>() { new object() })
//        )
//        { }
//    }

//    /// <summary>
//    /// Information stored in a file.
//    /// </summary>
//    public sealed class XoFile<TContent> : IXemo
//    {
//        private readonly IIDCard passport;
//        private readonly FileInfo storage;
//        private readonly TContent schema;

//        public XoFile(string id, string kind, FileInfo storage) : this(
//            id, kind, storage, default(TContent)
//        )
//        { }

//        public XoFile(string id, string subject, FileInfo memory, TContent schema)
//        {
//            this.passport = new AsIDCard(id, subject);
//            this.storage = memory;
//            this.schema = schema;
//        }

//        public IIDCard Card() => this.passport;

//        public IIDCard IID() => this.passport;

//        public TSlice Fill<TSlice>(TSlice wanted)
//        {
//            if (!this.HasSchema())
//                throw new InvalidOperationException("Cannot fill objects before a schema has been defined.");
//            using (var content = FileContent())
//            {
//                var state =
//                    IsAnonymousType(this.schema.GetType())
//                    ?
//                    JsonConvert.DeserializeAnonymousType(
//                        new StreamReader(content).ReadToEnd(),
//                        this.schema
//                    )
//                    :
//                    JsonConvert.DeserializeObject<TContent>(
//                        new StreamReader(content).ReadToEnd()
//                    );
//                    return
//                        DeepMerge.Schema<TSlice>(wanted, )
                            
//            }
//        }

//        public IXemo Mutate<TSlice>(TSlice mutation)
//        {
//            if (!this.HasSchema())
//                throw new InvalidOperationException("Define a schema prior to first mutation.");
//            using (var content = this.FileContent())
//            using (var writer = new StreamWriter(content))
//            {
//                var newState =
//                    ReflectionMerge.Fill(
//                        JsonConvert.DeserializeObject<TSlice>(
//                            new StreamReader(content).ReadToEnd()
//                        )
//                    ).From(mutation);
//                var newID = ReflectionMerge.Fill(new Identifier()).From(newState).ID;
//                if (newID != this.passport.ID())
//                {
//                    throw new InvalidOperationException("ID change is not supported.");
//                }
//                content.SetLength(0);
//                writer.Write(JsonConvert.SerializeObject(newState));
//            }
//        return this;
//        }

//        public IXemo Schema<TSchema>(TSchema schema) =>
//            new XoFile<TSchema>(
//                this.passport.ID(),
//                this.passport.Kind(),
//                this.storage,
//                schema
//            );

//        private bool IsAnonymousType(Type candidate) => candidate.Namespace == null;

//        private FileStream FileContent()
//        {
//            return
//                File.Open(
//                    this.storage.FullName,
//                    FileMode.Open,
//                    FileAccess.ReadWrite,
//                    FileShare.None
//            );
//        }

//        private bool HasSchema()
//        {
//            return this.schema != null && !this.schema.Equals(default(TContent));
//        }
//    }
//}