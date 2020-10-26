using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServiceCommon.Helpers
{
    public static class Util
    {
        public static void Eventually(int iterations, int timeoutInMS, Action action)
        {
            for (int i = 1; i <= iterations; i++)
            {
                try
                {
                    action();
                    break;
                }
                catch (Exception)
                {
                    if (i == iterations)
                    {
                        throw;
                    }
                    Thread.Sleep(timeoutInMS);
                }
            }
        }
    }
}
