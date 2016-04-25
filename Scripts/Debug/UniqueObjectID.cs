using System;
using System.Collections.Generic;

namespace Voltage.Common.ID
{
	using System.Runtime.Serialization;

    public class UniqueObjectID
    {
        private static readonly UniqueObjectID instance = new UniqueObjectID();
        public static UniqueObjectID Default { get { return instance; } }

        static UniqueObjectID () {}

		private readonly ObjectIDGenerator _idGenerator;

		private UniqueObjectID () 
		{
			_idGenerator = new ObjectIDGenerator ();
		}  


		public long GetID(object obj)
		{
			bool exists = false;
			return _idGenerator.GetId (obj, out exists);
		}
    }

}

