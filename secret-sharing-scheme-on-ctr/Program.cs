
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Numerics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace secret_sharing_scheme_on_ctr
{
    class Program
    {

      
        [STAThread]
        static void Main(string[] args)
        {
            string text = "Hello World!!!";
            BigInteger n =  new BigInteger(Encoding.ASCII.GetBytes(text));
            Console.WriteLine("Секрет : " + text);
            Console.WriteLine("Секрет -> Число : " + n);

            Console.WriteLine("--фаза разделения секрета--");
            Console.Write("Введите кол-во участников: ");
            int count = int.Parse(Console.ReadLine());
            
            System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\Users\Konstantin\TestFolder\");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            List<BigInteger> f = new List<BigInteger>();
            
            for (int i = 0; i < count; i++)
            {

                
                BigInteger pi = 4;
                
                while (!pi.IsPrime(23, out double a) || f.Contains(pi))// && (pi<= IRoot(n, 3)) && (pi >=IRoot(n, 2))
                {
                    pi =PrimeExtensions.GenerateRandomBigInteger(PrimeExtensions.IRoot(n, 3), PrimeExtensions.IRoot(n, 2));

                }
                f.Add(pi);
                BigInteger xi = n % pi;
                Console.WriteLine("Участник "+ i+ ": " +"x: " + xi + " p: " + pi);
                
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Konstantin\TestFolder\"+i+".txt", true))
                {
                    file.WriteLine(xi.ToString()+" "+pi.ToString());
                }
            }

            Console.WriteLine("--фаза восстановления секрета--");
            var files = ListFiles();
            
            List<BigInteger> constraints = new List<BigInteger>();
            List<BigInteger> mods = new List<BigInteger>();

            for (int i = 0; i < files.Length; i++)
            {
                Console.Write(files[i]);
                using (StreamReader sr = new StreamReader(files[i]))
                {
                    String line = sr.ReadToEnd();
                    constraints.Add(BigInteger.Parse(line.Split(' ','\r')[0]));
                    mods.Add(BigInteger.Parse(line.Split(' ', '\r')[1]));
                    Console.WriteLine(" Участник " + i + ": " + line);
                }
            }

            BigInteger M = 1;
            for (int i = 0; i < mods.Count; i++)
                M *= mods[i];

            BigInteger[] multInv = new BigInteger[constraints.Count];
            for (int i = 0; i < multInv.Length; i++)
                multInv[i] = euclidean(M / mods[i], mods[i])[0];

            BigInteger x = 0;
            for (int i = 0; i < mods.Count; i++)
                x += (M / mods[i]) * constraints[i] * multInv[i];

            x = leastPosEquiv(x, M);
            
            Console.WriteLine("Секрет -> Число : " + x);
            Console.WriteLine("Секрет : " + Encoding.ASCII.GetString(x.ToByteArray()));
           

            Console.ReadKey();
        }
        public static string[] ListFiles()
        {
            using (OpenFileDialog file = new OpenFileDialog())
            {
                //Allow to select multiple files
                file.Multiselect = true;

                //Allow to select only *.txt Files
                file.Filter = "Only Text Documents | *.txt";

                //Show the Dialog box to selcet file(s)
                file.ShowDialog();

                //return input file names
                return file.FileNames;
            }
        }
       public static BigInteger[] euclidean(BigInteger a, BigInteger b)
        {
            if (b > a)
            {
                BigInteger[] coeffs = euclidean(b, a);
                return new BigInteger[] { coeffs[1], coeffs[0] };
            }
            BigInteger q = a / b;
            BigInteger r = a - q * b;
            if (r == 0)
            {
                return new BigInteger[] { 0, 1 };
            }

            BigInteger[] next = euclidean(b, r);
            BigInteger[] output = { next[1], next[0] - q * next[1] };
            return output;
        }

        public static BigInteger leastPosEquiv(BigInteger a, BigInteger m)
        {
            //a eqivalent to b mod -m <==> a equivalent to b mod m
            if (m < 0)
                return leastPosEquiv(a, -1 * m);
            //if 0 <= a < m, then a is the least positive integer equivalent to a mod m
            if (a >= 0 && a < m)
                return a;

            //for negative a, find the least negative integer equivalent to a mod m
            //then add m
            if (a < 0)
                return -1 * leastPosEquiv(-1 * a, m) + m;

            //the only case left is that of a,m > 0 and a >= m

            //take the remainder according to the Division algorithm
            BigInteger q = a / m;

            /*
             * a = qm + r, with 0 <= r < m
             * r = a - qm is equivalent to a mod m
             * and is the least such non-negative number (since r < m)
             */
            return a - q * m;
        }

    } 
}
