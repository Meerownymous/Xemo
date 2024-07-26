using System;
namespace Xemo
{
    /// <summary>
    /// Identifies by Kind and ID. Used to identify Xemos in a cluster and relations.
    /// </summary>
    public interface IGrip
    {
        /// <summary>
        /// Unique ID of something.
        /// </summary>
        /// <returns></returns>
        public string ID();

        /// <summary>
        /// Kind of something, always the same along a cluster.
        /// </summary>
        public string Kind();

        public string Combined();
    }
}

