using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_UNIRX_7_1
using UniRx;

namespace UnityUtilities.Persistence
{
    public static class ReactiveTest
    {
        public static void Test<T>(this IReactiveProperty<T> r)
        {

        }
    }
}
#endif
