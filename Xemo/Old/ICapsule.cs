using System;
using Xemo;

namespace Xemo
{
    /// <summary>
    /// Capsule holding information.
    /// </summary>
	public interface ICapsule
	{
        public ICollection<string> TOC();
        public ICapsule With(string name, IContent content);
        public ICapsule With(ICapsule patch);
        public object Value(string name);
        //public TSubject Print<TSubject>(IPrinting<TSubject> printing);
    }
}