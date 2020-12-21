using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"fC:\\programFiles";
            var extension = Path.GetExtension(path);

            switch (extension)
            {
                case string x when string.IsNullOrEmpty(x):

                    break;
                default:

                    break;
            }
        }

    }
}
