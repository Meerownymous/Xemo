//using System;
//using Xemo.IDCard;

//namespace Xemo.Xemo
//{
//    public sealed class OneToOne : IXemo
//    {
//        private readonly string target;

//        public OneToOne(string target)
//        {
//            this.target = target;
//        }

//        public IIDCard Card() =>
//            new LazyIDCard(
//                () => throw new InvalidOperationException("This as relation definition and therefore has no ID."),
//                () => this.target
//            );

//        public TSlice Fill<TSlice>(TSlice wanted) =>
//            throw new InvalidOperationException("Cannot fill from a relation definition.");

//        public IXemo Mutate<TSlice>(TSlice mutation) =>
//            throw new InvalidOperationException("Cannot mutate a relation definition.");

//        public IXemo Schema<TSchema>(TSchema schema) =>
//            throw new InvalidOperationException("Cannot introduce schema to a relation definition.");
//    }
//}

