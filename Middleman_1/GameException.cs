using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    public class GameException : Exception
    {
        public GameException(string message)
            : base(message)
        { }

        protected GameException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }
}
