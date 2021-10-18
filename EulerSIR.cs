using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EulerSIR
{
    class Program
    {
        // Creates a new instance of EulerSIR, sets the stepsize and parameters, solves the equation and writes it to a csv file at @"c:\myfile.csv" 
        static void Main(string[] args)
        {
            try
            {
                EulerSIR e = new EulerSIR();
                e.Stepsize = 0.01;
                e.SetInit(0.999, 0, 0.001);
                e.SetParams(1, 1.0 / 14.0, 1000);
                e.Solve();
                e.WriteToFile(@"c:\myfile.csv");

            }
            catch (Exception x)
            {
                Console.WriteLine("Error Encountered : {0}", x.Message);
            }
           
        }
        class EulerSIR
        {
            /* System of equations : 
             * Sj+1 = Sj + h(-BSjIj)
             * Ij+1 = Ij + h(BSjIj - GIj)
             * Rj+1 = Rj + h(GIj)
             * xj+1 = xj + h
             * 
             */

            public double Stepsize;
            private double S0 = 0;
            private double R0 = 0;
            private double I0 = 0;
            private double G = 1.0/14.0;
            private double B = 1.0;
            public int numsteps = 100;
            public double[,] data;
            public double Sj, Rj, Ij, xj, Sjp1, Rjp1, Ijp1;

            // Uses Euler's method to solve the SIR model equations, stores them in a 2D array with entries of Susceptible, Infected, Recovered and Time, Time is the change in x.
            public void Solve()
            {
                Sj = S0;
                Rj = R0;
                Ij = I0;
                xj = 0;
                data = new double[numsteps, 4];
                int i;

                for (i = 0; i < numsteps; i++)
                {
                    Sjp1 = Sj + Stepsize * (-B * Sj * Ij);
                    Ijp1 = Ij + Stepsize * (B * Sj * Ij - G * Ij);
                    Rjp1 = Rj + Stepsize * (G * Ij);
                    xj += Stepsize;

                    Sj = Sjp1;
                    Rj = Rjp1;
                    Ij = Ijp1;

                    data[i, 0] = Sjp1;
                    data[i, 1] = Ijp1;
                    data[i, 2] = Rjp1;
                    data[i, 3] = xj;
                }
            }

            // The indexer which allows the user to get and set values in the 2D data array
            public double this[int row, int col]
            {
                get
                {
                    if (row < numsteps && col < 3)
                        return data[row,col];
                    else
                        return 0;
                }
                set
                {
                    if (row < numsteps && col < 3)
                        data[row, col] = value;
                }
            }

            // Allows the user to set the initial values for S, I and R
            public void SetInit(double a, double b, double c)
            {
                S0 = a;
                R0 = b;
                I0 = c;
            }
            /* Allows the user to set the values for the number of steps, the value of Beta and the value of Gamma
            *(I decided to implement this instead of just having one value for the number of steps, Beta and Gamma. The user may set them despite their default values)
            */
            public void SetParams(double _B, double _G, int _numsteps)
            {
                B = _B;
                G = _G;
                numsteps = _numsteps;
            }

            // Outputs to the console the array representing the solutions to the SIR model in (S, I, R, Time) format
            public void ShowArray()
            {
                Console.WriteLine("Susceptible, Infected, Recovered, Time");
                for(int i = 0; i < numsteps; i++)
                {
                    Console.WriteLine("({0}, {1}, {2}, {3})", data[i, 0], data[i, 1], data[i, 2], data[i, 3]);
                }
            }

            // Creates a path using the path input, writes the array of values for S, I, R and x to a .csv file, closes the stream to the file. 
            public void WriteToFile(string path)
            {
                path = Path.GetFullPath(path);
                StreamWriter sw = new StreamWriter(path, false);
                sw.WriteLine("Susceptible, Infected, Recovered, Time");

                for (int i = 0; i < numsteps; i++)
                {
                    sw.WriteLine("{0},{1},{2},{3}", data[i, 0], data[i, 1], data[i, 2], data[i, 3]);
                }
                sw.Close();
            }

        }
    }
}
