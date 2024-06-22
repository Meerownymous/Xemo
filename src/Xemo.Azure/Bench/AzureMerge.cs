//using Azure.Data.Tables;

//namespace Xemo.Azure.Bench
//{
//    public class AzureMerge<TTarget> : IBench<TableEntity, TTarget>
//    {
//        private readonly IGrip grip;
//        private readonly TTarget target;

//        public AzureMerge(IGrip grip, TTarget target)
//        {
//            this.grip = grip;
//            this.target = target;
//        }

//        public TableEntity Post(TTarget patch)
//        {
//            var entity = new TableEntity(this.grip.Kind(), this.grip.ID());
//            foreach (var prop in patch.GetType().GetProperties())
//            {
//                if (prop.PropertyType.IsArray)
//                {

//                }
//            }
//        }
//    }
//}

