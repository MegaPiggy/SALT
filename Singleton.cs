using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SALT
{
    /// <summary>
    /// Makes a singleton class, said class will only allow a single instance of itself
    /// </summary>
    /// <typeparam name="T">The type of the class turned into a singleton</typeparam>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        /// <summary>The instance of this singleton</summary>
        public static T Instance { get; private set; }

        /// <summary>The constructor that sets the instance value</summary>
        protected Singleton()
        {
            if ((object)Singleton<T>.Instance != null)
                Console.Console.LogWarning(string.Format("Trying to create a new instance of {0} while there can only be one!", (object)this.GetType()));
            else
                Singleton<T>.Instance = (T)this;
        }
    }
}
