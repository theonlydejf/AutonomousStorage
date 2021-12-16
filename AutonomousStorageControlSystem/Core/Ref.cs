using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core
{
    /// <summary>
    /// Class that can be used to pass any type as a reference
    /// </summary>
    /// <typeparam name="T">Type that is passed as a reference</typeparam>
    public class Ref<T>
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;
        public Ref(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public T Value
        {
            get { return getter(); }
            set { setter(value); }
        }
    }
}
