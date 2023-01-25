using System;
using System.Collections.Generic;

namespace BlowinCleanCode.Model
{
    public static class Box
    {
        public static Box<T> From<T>(T from)
            where T : struct
            => new Box<T>(from);
    }
}
