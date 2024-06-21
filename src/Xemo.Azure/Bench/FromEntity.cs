//using System.Reflection;
//using Azure.Data.Tables;
//using Tonga.Map;
//using Tonga.Scalar;
//using Xemo.Bench;
//using Xemo.Grip;

//namespace Xemo.Azure.Bench
//{
//    public class AzureMerge<TTarget> : IBench<TTarget, TableEntity>
//    {
//        private readonly TTarget target;

//        public AzureMerge(TTarget target)
//        {
//            this.target = target;
//        }

//        public TTarget Post(TableEntity patch)
//        {
//            TTarget result = default(TTarget);
//            if (this.target.GetType().IsArray)
//                result = (TTarget)MergedArray(this.target.GetType(), this.target, patch);
//            else
//                result = (TTarget)MergedObject(this.target.GetType(), this.target, patch);
//            return result;
//        }
//    }
//}

