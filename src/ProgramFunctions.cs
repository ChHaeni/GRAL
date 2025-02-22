#region Copyright
///<remarks>
/// <Graz Lagrangian Particle Dispersion Model>
/// Copyright (C) [2019]  [Dietmar Oettl, Markus Kuntner]
/// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
/// the Free Software Foundation version 3 of the License
/// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
/// You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.
///</remarks>
#endregion

using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Threading;
using System.Threading.Tasks;
namespace GRAL_2001
{
    partial class Program
    {
        /// <summary>
        /// Generate the vertical grid for GRAL calculations for flat terrain
        /// </summary>
        private static int GenerateVerticalGridFlat()
        {
            int nkk = 0; // number of vertical grid cells
            int flexstretchindex = 0;
            float stretching = 1;

            if (LogLevel > Consts.LogLevelOff)
            {
                Console.WriteLine("Fact  \t Height");
            }
            for (int i = 2; i < VerticalCellMaxBound; i++)
            {
                if (StretchFF > 0.99)
                {
                    if (LogLevel > Consts.LogLevelOff)
                    {
                        Console.Write(StretchFF.ToString() + "\t");
                    }
                    DZK[i] = DZK[i - 1] * StretchFF;
                }
                else
                {
                    if (flexstretchindex < StretchFlexible.Count - 1)
                    {
                        if (StretchFlexible[flexstretchindex + 1][1] > 0.99 && HOKART[i - 1] > StretchFlexible[flexstretchindex + 1][0])
                        {
                            stretching = StretchFlexible[flexstretchindex + 1][1];
                            flexstretchindex++;
                        }
                    }
                    if (LogLevel > Consts.LogLevelOff)
                    {
                        Console.Write(stretching.ToString() + "\t");
                    }
                    DZK[i] = DZK[i - 1] * stretching;
                }

                HOKART[i] = HOKART[i - 1] + DZK[i];
                if (LogLevel > Consts.LogLevelOff)
                {
                    Console.WriteLine(HOKART[i].ToString());
                }
                if (HOKART[i] >= 800)
                {
                    nkk = i;
                    break;
                }
            }
            nkk = Math.Min(nkk, VerticalCellMaxBound - 2);
            return nkk;
        }

        /// <summary>
        /// Generate the vertical grid for GRAL calculations with terrain
        /// </summary>
        private static int GenerateVerticalGridTerrain()
        {
            int nkk = 0; // number of vertical grid cells
            int flexstretchindex = 0;
            float stretching = 1;

            if (LogLevel > Consts.LogLevelOff)
            {
                Console.WriteLine("Fact  \t Height");
            }
            for (int i = 2; i < VerticalCellMaxBound; i++)
            {
                if (StretchFF > 0.99)
                {
                    if (LogLevel > Consts.LogLevelOff)
                    {
                        Console.Write(StretchFF.ToString() + "\t");
                    }
                    DZK[i] = DZK[i - 1] * StretchFF;
                }
                else
                {
                    if (flexstretchindex < StretchFlexible.Count - 1)
                    {
                        if (StretchFlexible[flexstretchindex + 1][1] > 0.99 && HOKART[i - 1] > StretchFlexible[flexstretchindex + 1][0])
                        {
                            stretching = StretchFlexible[flexstretchindex + 1][1];
                            flexstretchindex++;
                        }
                    }
                    if (LogLevel > Consts.LogLevelOff)
                    {
                        Console.Write(stretching.ToString() + "\t");
                    }
                    DZK[i] = DZK[i - 1] * stretching;
                }

                HOKART[i] = HOKART[i - 1] + DZK[i];
                if (LogLevel > Consts.LogLevelOff)
                {
                    Console.WriteLine(HOKART[i].ToString());
                }
                // 300 m above highest elevation and 800 m above lowest elevation 
                if ((HOKART[i] >= (AHMAX - AHMIN + 300)) && (HOKART[i] >= 800))
                {
                    nkk = i;
                    ModelTopHeight = HOKART[i] + AHMIN;
                    break;
                }
            }

            nkk = Math.Min(nkk, VerticalCellMaxBound - 2);
            return nkk;
        }

        /// <summary>
        /// Delete temporary files for transient calculations
        /// </summary>
        private static void Delete_Temp_Files()
        {
            if (TransientTempFileDelete)
            {
                try
                {
                    if (File.Exists("Transient_Concentrations1.tmp"))
                    {
                        File.Delete("Transient_Concentrations1.tmp");
                    }
                    if (File.Exists("Transient_Concentrations2.tmp"))
                    {
                        File.Delete("Transient_Concentrations2.tmp");
                    }
                    if (File.Exists("Vertical_Concentrations.tmp"))
                    {
                        File.Delete("Vertical_Concentrations.tmp");
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Output for logging level 01
        /// </summary>
        private static void LOG01_Output()
        {
            Console.WriteLine("Refl.Nr. > 400000: " + LogReflexions[5].ToString());
            Console.WriteLine("Refl.Nr. > 100000: " + LogReflexions[4].ToString());
            Console.WriteLine("Refl.Nr. >  50000: " + LogReflexions[3].ToString());
            Console.WriteLine("Refl.Nr. >  10000: " + LogReflexions[2].ToString());
            Console.WriteLine("Refl.Nr. >   5000: " + LogReflexions[1].ToString());
            Console.WriteLine("Refl.Nr. >    500: " + LogReflexions[0].ToString());
            Console.WriteLine("Time-Step.Nr. > 100000000: " + Log_Timesteps[5].ToString());
            Console.WriteLine("Time-Step.Nr. >  10000000: " + Log_Timesteps[4].ToString());
            Console.WriteLine("Time-Step.Nr. >   1000000: " + Log_Timesteps[3].ToString());
            Console.WriteLine("Time-Step.Nr. >    100000: " + Log_Timesteps[2].ToString());
            Console.WriteLine("Time-Step.Nr. >     10000: " + Log_Timesteps[1].ToString());
            Console.WriteLine("Time-Step.Nr. >      1000: " + Log_Timesteps[0].ToString());
            Array.Clear(LogReflexions, 0, LogReflexions.Length);
            Array.Clear(Log_Timesteps, 0, Log_Timesteps.Length);
        }

        /// <summary>
        /// Console output of meteo data for the recent situation
        /// </summary>
        private static void OutputOfMeteoData()
        {
            Console.WriteLine("Obukhov length [m]:        " + Ob[1][1].ToString("0.0"));
            Console.WriteLine("Friction velocity [m/s]:   " + Ustern[1][1].ToString("0.00"));
            Console.WriteLine("Boundary-layer height [m]: " + BdLayHeight.ToString("0"));
            if (WindVelGral > 0)
            {
                if (WindVelGral < 20)
                {
                    Console.Write("Init meteo:  wind speed [m/s]: " + WindVelGral.ToString("F2"));
                    double dir = Math.Round(270 - WindDirGral * 180 / Math.PI, 1);
                    if (dir < 0)
                    {
                        dir = 270 + 360 + dir;
                    }

                    if (dir > 360)
                    {

                        int k = (int)(dir / 360); // Ku: 10.5.2020 avoid dir >> 360°
                        dir -= 360 * k;
                    }

                    Console.Write("  direction [deg]: " + dir.ToString("F0"));
                }
                else
                {
                    Console.Write("Init meteo:  weather situation token: " + WindVelGral.ToString("F2"));
                }

                if (StabClass > 0)
                {
                    Console.Write("  Stability class [-]: " + StabClass.ToString("F0"));
                }

                //Show GRAMM stab class when reading a .gff file
                if (WindVelGramm < -1 && StabClassGramm > 0)
                {
                    Console.Write("  GRAMM SC: " + StabClassGramm.ToString("F0"));
                }

                Console.WriteLine("");
            }
            if (WindVelGramm > -1)
            {
                Console.Write("GRAMM meteo: wind speed [m/s]: " + WindVelGramm.ToString("F2"));
                if (WindDirGramm >= 0)
                {
                    Console.Write("  direction [deg]: " + WindDirGramm.ToString("F0"));
                }

                if (StabClassGramm > 0)
                {
                    Console.Write("  Stability class [-]: " + StabClassGramm.ToString("F0"));
                }

                Console.WriteLine("");
            }
            Console.WriteLine("                           ----------");
        }

        /// <summary>
        /// Calculate the potential temperature gradient
        /// </summary>
        private static void CalculateTemperatureGradient()
        {
            double z_L = 1 / Ob[1][1];
            if ((z_L <= 0.005) && (z_L > -0.02))
            {
                PotdTdz = 0;
            }
            else if ((z_L <= -0.02) && (z_L > -0.06))
            {
                PotdTdz = (float)((0.9 - 1.6) * 0.01);
            }
            else if ((z_L <= -0.06) && (z_L > -0.1))
            {
                PotdTdz = (float)((0.9 - 1.8) * 0.01);
            }
            else if (z_L <= -0.1)
            {
                PotdTdz = (float)((0.9 - 2) * 0.01);
            }
            else if ((z_L <= 0.02) && (z_L > 0.005))
            {
                PotdTdz = (float)((0.9 + 1) * 0.01);
            }
            else if ((z_L <= 0.05) && (z_L > 0.02))
            {
                PotdTdz = (float)((0.9 + 2.5) * 0.01);
            }
            else if (z_L > 0.05)
            {
                PotdTdz = (float)((0.9 + 5) * 0.01);
            }
        }

        /// <summary>
        /// Calculate the boundary layer height in steady state mode
        /// </summary>
        private static void CalculateBoudaryLayerHeight()
        {
            if (IStatistics == Consts.MeteoZR)
            { }
            else
            {
                if (Ob[1][1] >= 0)
                {
                    BdLayFlat = (float)Math.Min(0.4 * Math.Sqrt(Ustern[1][1] * Ob[1][1] / CorolisParam), 800);
                }
                else
                {
                    BdLayFlat = (float)(Math.Min(0.4 * Math.Sqrt(Ustern[1][1] * 1000 / CorolisParam), 800) + 300 * Math.Pow(2.72, Ob[1][1] * 0.01F));
                }
                BdLayHeight = BdLayFlat;
            }
            BdLayH10 = BdLayHeight * 0.1F;
        }

        /// <summary>
        /// Check SIMD status
        /// </summary>
        /// <returns>Number of elements stroed in a float vector</returns>
        private static int CheckSIMD()
        {
            int SIMD = Vector<float>.Count;
            if (Program.UseVector512Class && Vector512.IsHardwareAccelerated)
            {
                SIMD = Vector512<float>.Count;
                Console.WriteLine("SIMD - support: " + SIMD.ToString() + " float values  Using Vector512 extension");
            }
            else
            {
                SIMD = Vector<float>.Count;
                Console.WriteLine("SIMD - support: " + SIMD.ToString() + " float values  IsHardwareAccelerated: " + Vector.IsHardwareAccelerated.ToString());
            }
            if (Vector.IsHardwareAccelerated == false)
            {
                string err = "Your system does not support vectors. This results in a very slow flow field calculation. \n Try to use the latest .NetCore framework \n Press a key to continue.";
                Console.WriteLine(err);
                ProgramWriters.LogfileGralCoreWrite(err);
                if (Program.IOUTPUT <= 0 && Program.WaitForConsoleKey) // not for Soundplan or no keystroke
                {
                    Console.ReadKey(true); 	// wait for a key input
                }
            }
            return SIMD;
        }

        /// <summary>
        /// Check command line arguments
        /// </summary>
        private static int CheckCommandLineArguments(string[] args)
        {
            int LogLevel = 0;
            if (args.Length > 0)
            {
                int _off = 0;
                if (Directory.Exists(args[0]) == true) // arg[0] = Directory!
                {
                    Directory.SetCurrentDirectory(args[0]);
                    _off = 1;
                }
                else if (!args[0].Contains("LOGLEVEL"))
                {
                    string err = "! The command line argument is not a valid directory: " + args[0];
                    Console.WriteLine(err);
                    ProgramWriters.LogfileGralCoreWrite(err);
                    err = "! GRAL starts in the application folder !";
                    Console.WriteLine(err);
                    ProgramWriters.LogfileGralCoreWrite(err);
                }
                if (args.Length > _off) // additional arguments
                {
                    if (args[0 + _off].ToUpper().Contains("LOGLEVEL01") == true) // Loglevel 1
                    {
                        LogLevel = 1;
                        Console.WriteLine("LOGLEVEL01");
                        Console.WriteLine("");
                    }
                    if (args[0 + _off].ToUpper().Contains("LOGLEVEL02") == true) // Loglevel 2
                    {
                        LogLevel = 2;
                        Console.WriteLine("LOGLEVEL02");
                        Console.WriteLine("");
                    }
                    if (args[0 + _off].ToUpper().Contains("LOGLEVEL03") == true) // Loglevel 3
                    {
                        LogLevel = 3;
                        Console.WriteLine("LOGLEVEL03");
                        Console.WriteLine("");
                    }
                }
            }
            return LogLevel;
        }

        /// <summary>
        /// Find internal source group index by external SG number -> just sources with source groups defined in Program.SourceGroups are used in the simulation
        /// </summary>
        /// <param name="Real_SG_Number">Source group number as used in the GUI and input/output files</param>
        /// <returns>Internal contiguous source group number, starting with 0</returns>
        public static int Get_Internal_SG_Number(int Real_SG_Number)
        {
            int SG_Internal = -1; // if SG is not indicated to be computed
            for (int im = 0; im < Program.SourceGroups.Count; im++)
            {
                if (Real_SG_Number == Program.SourceGroups[im])
                {
                    SG_Internal = im;
                }
            }
            return SG_Internal;
        }

        /// <summary>
        /// Initialze a jagged array
        /// </summary>
        public static T[] CreateArray<T>(int cnt, Func<T> itemCreator)
        {
            T[] result = new T[cnt];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = itemCreator();
            }
            return result;
        }

        /// <summary>
        /// Create large jagged arrays 
        /// </summary>        
        private static void CreateLargeArrays()
        {
            //array definition block
            NX1 = NX + 1;
            NY1 = NY + 1;
            NZ1 = NZ + 1;
            NX2 = NX + 2;
            NY2 = NY + 2;
            NZ2 = NZ + 2;
            Conz3d = CreateArray<float[][][]>(NXL + 2, () => CreateArray<float[][]>(NYL + 2, () => CreateArray<float[]>(NS, () => new float[Program.SourceGroups.Count])));
            XKO = new double[NX2];
            YKO = new double[NY2];
            ZKO = new double[NZ2];
            AH = CreateArray<float[]>(NX1, () => new float[NY1]);
            AHE = CreateArray<float[][]>(NX2, () => CreateArray<float[]>(NY2, () => new float[NZ2]));
            ZSP = CreateArray<float[][]>(NX1, () => CreateArray<float[]>(NY1, () => new float[NZ1]));
            DDX = new float[NX1];
            DDY = new float[NY1];
            UWIN = CreateArray<float[][]>(NX1, () => CreateArray<float[]>(NY1, () => new float[NZ1]));
            VWIN = CreateArray<float[][]>(NX1, () => CreateArray<float[]>(NY1, () => new float[NZ1]));
            WWIN = CreateArray<float[][]>(NX1, () => CreateArray<float[]>(NY1, () => new float[NZ1]));
            //TKE = CreateArray<float[][]>(NX1, () => CreateArray<float[]>(NY1, () => new float[NZ1]));
            //DISS = CreateArray<float[][]>(NX1, () => CreateArray<float[]>(NY1, () => new float[NZ1]));
            HorSlices = new float[NS];
            Z0Gramm = CreateArray<float[]>(NX1, () => new float[NY1]);
            Ustern = CreateArray<float[]>(NX1, () => new float[NY1]);
            Ob = CreateArray<float[]>(NX1, () => new float[NY1]);
            SC_Gral = CreateArray<byte[]>(NX1, () => new byte[NY1]); //11.9.2017 Kuntner
            AKL_GRAMM = new int[NX1, NY1];

            //create additional concentration arrays for calculating concentration gradients used for odour-hour modelling
            if (Odour == true)
            {
                Conz3dp = CreateArray<float[][][]>(NXL + 2, () => CreateArray<float[][]>(NYL + 2, () => CreateArray<float[]>(NS, () => new float[Program.SourceGroups.Count])));
                Conz3dm = CreateArray<float[][][]>(NXL + 2, () => CreateArray<float[][]>(NYL + 2, () => CreateArray<float[]>(NS, () => new float[Program.SourceGroups.Count])));
                Q_cv0 = CreateArray<float[]>(NS, () => new float[Program.SourceGroups.Count]);
                DisConcVar = CreateArray<float[]>(NS, () => new float[Program.SourceGroups.Count]);
            }

            //array for computing deposition
            Depo_conz = CreateArray<double[][]>(NXL + 2, () => CreateArray<double[]>(NYL + 2, () => new double[Program.SourceGroups.Count]));
        }

        /// <summary>
        /// Init GRAL settings for a calculation in complex terrain
        /// </summary>
        /// <param name="SIMD">Number of float values within a vector</param>
        private static void InitGralTopography(int SIMD)
        {
            //get the minimum and maximum elevation of the GRAMM orography
            for (int i = 1; i <= NI; i++)
            {
                for (int j = 1; j <= NJ; j++)
                {
                    if ((GrammWest + DDX[1] * i >= XsiMinGral) && (GrammWest + DDX[1] * (i - 1) <= XsiMaxGral) &&
                        (GrammSouth + DDY[1] * j >= EtaMinGral) && (GrammSouth + DDY[1] * (j - 1) <= EtaMaxGral))
                    {
                        if (AH[i][j] < AHMIN)
                        {
                            AHMIN = AH[i][j];
                        }

                        AHMAX = Math.Max(AH[i][j], AHMAX);
                    }
                }
            }

            //number of grid cells of the GRAL microscale flow field
            NII = (int)((XsiMaxGral - XsiMinGral) / DXK);
            NJJ = (int)((EtaMaxGral - EtaMinGral) / DYK);
            //heights of the grid levels of the GRAL microscale flow field (HOKART[0]=0)
            HOKART[1] = DZK[1];

            NKK = GenerateVerticalGridTerrain();
            NKK = Math.Max(SIMD, Math.Min(NKK, VerticalCellMaxBound - 2)); // at least SIMD vertical cells
                                                                           //array declaration block
            Console.WriteLine();
            Console.Write("Array declarations...");
            // create jagged arrays manually to keep memory areas of similar indices togehter -> reduce false sharing & 
            // save memory because of the unused index 0  
            Program.UK = new float[NII + 2][][];
            Program.VK = new float[NII + 2][][];
            Program.WK = new float[NII + 2][][];
            for (int i = 1; i < NII + 2; ++i)
            {
                Program.UK[i] = new float[NJJ + 2][];
                Program.VK[i] = new float[NJJ + 2][];
                Program.WK[i] = new float[NJJ + 2][];
                for (int j = 1; j < NJJ + 2; ++j)
                {
                    Program.UK[i][j] = new float[NKK + 1];
                    Program.VK[i][j] = new float[NKK + 1];
                    Program.WK[i][j] = new float[NKK + 2];
                }
                if (i % 50 == 0)
                {
                    Console.Write(".");
                }
            }
            //TURB = CreateArray<float[][]>(NII + 2, () => CreateArray<float[]>(NJJ + 2, () => new float[NKK + 2]));
            //Console.Write(".");
            //TDISS = CreateArray<float[][]>(NII + 2, () => CreateArray<float[]>(NJJ + 2, () => new float[NKK + 2]));
            //Console.Write(".");
            AHK = CreateArray<float[]>(NII + 2, () => new float[NJJ + 2]);
            Console.Write(".");
            //KKART = CreateArray<Int16[]>(NII + 2, () => new Int16[NJJ + 2]);
            //Console.Write(".");
            //ADVDOM = CreateArray<Byte[]>(NII + 2, () => new Byte[NJJ + 2]);
            ADVDOM = new byte[NII + 2][];
            KKART = new short[NII + 2][];
            for (int i = 0; i < NII + 2; ++i)
            {
                ADVDOM[i] = GC.AllocateArray<byte>(NJJ + 2);
            }
            for (int i = 0; i < NII + 2; ++i)
            {
                KKART[i] = GC.AllocateArray<short>(NJJ + 2);
            }
            Console.Write(".");
            BUI_HEIGHT = CreateArray<float[]>(NII + 2, () => new float[NJJ + 2]);
            Console.Write(".");
            CUTK = CreateArray<float[]>(NII + 2, () => new float[NJJ + 2]);
            Console.Write(".");
        }

        /// <summary>
        /// Init GRAL settings for a calculation in flat terrain
        /// </summary>
        private static void InitGralFlat()
        {
            //number of grid cells of the GRAL microscale flow field
            NII = (int)((XsiMaxGral - XsiMinGral) / DXK);
            NJJ = (int)((EtaMaxGral - EtaMinGral) / DYK);
            //heights of the grid levels of the GRAL microscale flow field (HOKART[0]=0)
            HOKART[1] = DZK[1];

            NKK = GenerateVerticalGridFlat();

            //array declaration block
            Console.WriteLine();
            Console.Write("Array declarations...");
            // create jagged arrays manually to keep memory areas of similar indices togehter -> reduce false sharing & 
            // save memory because of the unused index 0  
            Program.UK = new float[NII + 2][][];
            Program.VK = new float[NII + 2][][];
            Program.WK = new float[NII + 2][][];
            for (int i = 1; i < NII + 2; ++i)
            {
                Program.UK[i] = new float[NJJ + 2][];
                Program.VK[i] = new float[NJJ + 2][];
                Program.WK[i] = new float[NJJ + 2][];
                for (int j = 1; j < NJJ + 2; ++j)
                {
                    Program.UK[i][j] = new float[NKK + 1];
                    Program.VK[i][j] = new float[NKK + 1];
                    Program.WK[i][j] = new float[NKK + 2];
                }
                if (i % 50 == 0)
                {
                    Console.Write(".");
                }
            }
            //TURB = CreateArray<float[][]>(NII + 2, () => CreateArray<float[]>(NJJ + 2, () => new float[NKK + 2]));
            //Console.Write(".");
            //TDISS = CreateArray<float[][]>(NII + 2, () => CreateArray<float[]>(NJJ + 2, () => new float[NKK + 2]));
            //Console.Write(".");
            AHK = CreateArray<float[]>(NII + 2, () => new float[NJJ + 2]);
            Console.Write(".");
            //KKART = CreateArray<Int16[]>(NII + 2, () => new Int16[NJJ + 2]);
            //Console.Write(".");
            //ADVDOM = CreateArray<Byte[]>(NII + 2, () => new Byte[NJJ + 2]);
            ADVDOM = new byte[NII + 2][];
            KKART = new short[NII + 2][];
            for (int i = 0; i < NII + 2; ++i)
            {
                ADVDOM[i] = GC.AllocateArray<byte>(NJJ + 2);
            }
            for (int i = 0; i < NII + 2; ++i)
            {
                KKART[i] = GC.AllocateArray<short>(NJJ + 2);
            }
            Console.Write(".");
            BUI_HEIGHT = CreateArray<float[]>(NII + 2, () => new float[NJJ + 2]);
            Console.Write(".");
            CUTK = CreateArray<float[]>(NII + 2, () => new float[NJJ + 2]);
            Console.Write(".");
        }

        /// <summary>
        /// Init the GRAL borders
        /// </summary>
        private static void InitGralBorders()
        {
            if (Topo == Consts.TerrainFlat)
            {
                //Set GRAMM bounds to GRAL bounds in flat terrain
                IKOOAGRAL = (int)XsiMinGral;
                JKOOAGRAL = (int)EtaMinGral;
                GrammWest = (int)XsiMinGral;
                GrammSouth = (int)EtaMinGral;
                DDX[1] = (float)(XsiMaxGral - XsiMinGral);
                DDY[1] = (float)(EtaMaxGral - EtaMinGral);
                XsiMinGral = 0;
                XsiMaxGral -= (float)GrammWest;
                EtaMinGral = 0;
                EtaMaxGral -= (float)GrammSouth;
                NI = 1;
                NJ = 1;
            }
            else
            {
                XsiMinGramm = 0;
                EtaMinGramm = 0;
                IKOOAGRAL = (int)XsiMinGral;
                JKOOAGRAL = (int)EtaMinGral;
                XsiMinGral = 0;
                XsiMaxGral -= (float)IKOOAGRAL;
                EtaMinGral = 0;
                EtaMaxGral -= (float)JKOOAGRAL;
                XsiMaxGramm = DDX[1] * (float)NX;
                EtaMaxGramm = DDY[1] * (float)NY;

                if (GrammWest > GralWest || GrammSouth > GralSouth || (GrammWest + XsiMaxGramm) < GralEast || (GrammSouth + EtaMaxGramm) < GralNorth)
                {
                    string err = "WARNING: the GRAL domain area is larger than GRAMM domain area!";
                    Console.WriteLine(err);
                    ProgramWriters.LogfileGralCoreWrite(err);
                    err = "The calculation outside the GRAMM area will be interpolated! It is recommended to cancel the calculation.";
                    Console.WriteLine(err);
                    ProgramWriters.LogfileGralCoreWrite(err);

                    Console.WriteLine("Press a key to continue or close the calculation terminal");
                    Console.ReadKey(true); 	// wait for a key input
                    //Environment.Exit(-1);
                }
            }
        }

        /// <summary>
        /// Read the user defined meteo data
        /// </summary>
        private static void ReadMeteoData()
        {
            //read meteorological input data (wind speed, wind direction, stability class)
            if (IStatistics == Consts.MeteoPgtAll) // meteopgt.all 
            {
                Input_MeteopgtAll.Read();
            }
            //read meteorological input data (friction velocity, Obukhov length, boundary-layer height, standard deviation of horizontal wind fluctuations, u- and v-wind components)
            if (IStatistics == Consts.MeteoZR)
            {
                Input_zr.Read();
            }
            //read meteorological input data (friction velocity, Obukhov length, standard deviation of horizontal wind fluctuations, u- and v-wind components)
            if (IStatistics == Consts.MeteoEKI)
            {
                Input_Eki.Read();
            }
            //read meteorological input data (velocity, direction, friction velocity, standard deviation of horizontal and vertical wind fluctuations, Obukhov length, meandering parameters)
            if (IStatistics == Consts.MeteoSonic)
            {
                Input_Sonic.Read();
            }
        }

        /// <summary>
        /// Read the user defined path for the GRAMM windfields
        /// </summary>
        public static string ReadWindfeldTXT()
        {
            string path = string.Empty;
            if (File.Exists("windfeld.txt"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader("windfeld.txt"))
                    {
                        path = sr.ReadLine();  // read path
                        if (Program.RunOnUnix) // it is possible to use a 2nd line for compatibility to Windows
                        {
                            if (!Directory.Exists(path))
                            {
                                path = sr.ReadLine();  // read path
                            }
                        }
                    }
                }
                catch
                {
                    path = string.Empty;
                }
                if (string.IsNullOrEmpty(path)) // if ReadLine() was at the end of the stream
                {
                    path = string.Empty;
                }

                if (!Directory.Exists(path))
                {
                    string err = "Path from windfeld.txt not available: " + path;
                    Console.WriteLine(err);
                    ProgramWriters.LogfileGralCoreWrite(err);
                    path = string.Empty;
                }
            }
            return path;
        }

        /// <summary>
        /// Compute the wind direction
        /// </summary>
        /// <param name="u">u value of wind vector</param> 
        /// <param name="v">v value of wind vector</param> 
        public static double Winkel(double u, double v)
        {
            double winkel = 0;
            if (v == 0)
            {
                winkel = 90;
            }
            else
            {
                winkel = Math.Abs(Math.Atan(u / v)) * 180 / Math.PI;
            }

            if ((v > 0) && (u <= 0))
            {
                winkel = 180 - winkel;
            }

            if ((v >= 0) && (u > 0))
            {
                winkel = 180 + winkel;
            }

            if ((v < 0) && (u >= 0))
            {
                winkel = 360 - winkel;
            }

            return winkel;
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]

        /// <summary>
        /// Get the max value of two floats
        /// </summary>
        /// <param name="val1">1st value</param> 
        /// <param name="val2">2nd value</param> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FloatMax(float val1, float val2)
        {
            if (val1 > val2)
            {
                return val1;
            }
            return val2;
        }

        /// <summary>
    	/// Return an integer, similar to Convert.ToInt32, but faster
    	/// </summary>
    	/// <param name="val1">float value</param> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ConvToInt(float val1)
        {
            if (val1 < 0)
            {
                return ((int)(val1 - 0.5F));
            }
            else
            {
                return ((int)(val1 + 0.5F));
            }
        }
        /// <summary>
    	/// Return an integer, similar to Convert.ToInt32, but faster
    	/// </summary>
    	/// <param name="val1">double value</param> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ConvToInt(double val1)
        {
            if (val1 < 0)
            {
                return ((int)(val1 - 0.5));
            }
            else
            {
                return ((int)(val1 + 0.5));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //[MethodImpl(MethodImplOptions.NoInlining)]
        public static double Pow5(double x1)
        {
            return (x1 * x1 * x1 * x1 * x1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //[MethodImpl(MethodImplOptions.NoInlining)]
        public static float Pow5(float x1)
        {
            return (x1 * x1 * x1 * x1 * x1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //[MethodImpl(MethodImplOptions.NoInlining)]
        public static double Pow4(double x1)
        {
            return (x1 * x1 * x1 * x1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //[MethodImpl(MethodImplOptions.NoInlining)]
        public static double Pow3(double x1)
        {
            return (x1 * x1 * x1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow3(float x1)
        {
            return (x1 * x1 * x1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [MethodImpl(MethodImplOptions.NoInlining)]
        public static double Pow2(double x1)
        {
            return (x1 * x1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow2(float x1)
        {
            return (x1 * x1);
        }

        /// <summary>
        /// Count the number of lines in a text file
        /// </summary>
        /// <param name="filename">Full path and name of a text file</param> 
        public static int CountLinesInFile(string filename)
        {
            int count = 0;
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        count++;
                    }
                }
            }
            catch
            { }
            return count;
        }

        /// <summary>
        /// Get a hash code of the running app
        /// </summary>
        public static string GetAppHashCode()
        {
            string filename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string hashstring = string.Empty;

            if (File.Exists(filename))
            {
                try
                {
                    using (System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create())
                    {
                        byte[] hash;

                        using (FileStream stream = File.OpenRead(filename))
                        {
                            hash = sha.ComputeHash(stream);
                        }

                        foreach (byte item in hash)
                        {
                            hashstring += item.ToString("x2");
                        }
                    }
                }
                catch { }
            }
            return hashstring;
        }

        /// <summary>
        /// Transfer non-steady-state concentration fields and start writing of conz4d array
        /// </summary>
        private static void TransferNonSteadyStateConcentrations(ProgramWriters WriteClass, ref Thread TreadWriteConz4dFile)
        {
            //Create a shallow copy
            float[][][][] copytemp = (float[][][][])Conz4d.Clone();
            Conz4d = Conz5d;
            Conz5d = copytemp;

            //Clear array Conz5d
            Parallel.For(1, Program.NII + 2, Program.pOptions, i =>
            {
                for (int j = 1; j <= Program.NJJ + 1; j++)
                {
                    float[][] conz5d_L = Conz5d[i][j];
                    for (int k = 1; k <= NKK_Transient; k++)
                    {
                        Array.Clear(conz5d_L[k]);
                    }
                }
            });

            ConzSumCounter++; // increase number of SumCounter;
            if (IWET % TransientTempFileInterval == 0) // each TransientTempFileInterval (default 24) situations -> store arrays temporarily
            {
                if (WriteVerticalConcentration) // write concentration array
                {
                    WriteClass.Write3DTempConcentrations();
                }
                //write conz4d File 
                TreadWriteConz4dFile = new Thread(() => WriteClass.WriteTransientConcentration(IWET));
                TreadWriteConz4dFile.Start(); // start writing thread
            }
        }

        /// <summary>
        /// Volume correction nearby buildings if the flow field grid is smaller than the concentration grid
        /// </summary>
        private static void VolumeCorrection()
        {
            if (Program.BuildingsExist == true)
            {
                ProgramHelper prgH = new ProgramHelper();

                if (GralDx != DXK || GralDy != DYK) // if flow field grid is smaller than the concentration grid
                {
                    prgH.VolumeConcCorrection(Conz3d, 0);
                    if (Program.Odour == true)
                    {
                        prgH.VolumeConcCorrection(Conz3dm, -Program.GralDz);
                        prgH.VolumeConcCorrection(Conz3dp, Program.GralDz);
                    }

                    // Correction for all receptors
                    if (Program.ReceptorsAvailable)
                    {
                        for (int irec = 1; irec < ReceptorNearbyBuilding.Length; irec++)
                        {
                            double x0 = Program.ReceptorX[irec] - Program.IKOOAGRAL - GralDx * 0.5;
                            double xmax = x0 + GralDx;
                            double VolumeReduction = 0;
                            float HMin = Program.ReceptorZ[irec] - GralDz * 0.5F;
                            float HMax = Program.ReceptorZ[irec] + GralDz * 0.5F;
                            double y0 = Program.ReceptorY[irec] - Program.JKOOAGRAL - GralDy * 0.5;
                            double ymax = y0 + GralDy;

                            // loop over all flow field cells inside the receptor cell
                            for (double xi = x0; xi < xmax; xi += DXK)
                            {
                                int IndexI = (int)(xi / DXK) + 1;
                                if (IndexI > 0 && IndexI < NII + 1)
                                {
                                    for (double yi = y0; yi < ymax; yi += DYK)
                                    {
                                        int IndexJ = (int)(yi / DYK) + 1;
                                        if (IndexJ > 0 && IndexJ < NJJ + 1)
                                        {
                                            float buidingheight = BUI_HEIGHT[IndexI][IndexJ];
                                            if (buidingheight > HMin)
                                            {
                                                VolumeReduction += DXK * DYK * (Math.Min(buidingheight, HMax) - HMin);
                                            }
                                        }
                                    }
                                }
                            }
                            if (VolumeReduction > 0)
                            {
                                // Limit factor to 10, because in these cases, just a small portion of volume is left
                                float factor = (float)Math.Max(1, Math.Min(10, (GridVolume / Math.Max(0.01F, GridVolume - VolumeReduction))));
                                for (int IQ = 0; IQ < Program.SourceGroups.Count; IQ++)
                                {
                                    Program.ReceptorConc[irec][IQ] *= factor;
                                }
                            }
                        }
                    } // Receptor correction
                }
                else // if flow field grid and concentration grid match
                {
                    prgH.VolumeConcCorrection2(Conz3d, 0);
                    if (Program.Odour == true)
                    {
                        prgH.VolumeConcCorrection2(Conz3dm, -Program.GralDz);
                        prgH.VolumeConcCorrection2(Conz3dp, Program.GralDz);
                    }

                    // Correction for all receptors
                    if (Program.ReceptorsAvailable)
                    {
                        for (int irec = 1; irec < ReceptorNearbyBuilding.Length; irec++)
                        {
                            int IndexI = ReceptorIInd[irec];
                            if (IndexI > 0 && IndexI < NII + 1)
                            {
                                int IndexJ = ReceptorJInd[irec];
                                if (IndexJ > 0 && IndexJ < NJJ + 1)
                                {
                                    float HMin = Program.ReceptorZ[irec] - Program.GralDz * 0.5F;
                                    float HMax = Program.ReceptorZ[irec] + Program.GralDz * 0.5F - 0.2F;

                                    float buidingheight = BUI_HEIGHT[IndexI][IndexJ];

                                    if (buidingheight > HMin && buidingheight < HMax)
                                    {
                                        // Limit factor to 10, because in these cases, just a small portion of volume is left
                                        float VolumeReduction = MathF.Max(1, MathF.Min(10, Program.GralDz / (Program.GralDz - (buidingheight - HMin))));
                                        if (VolumeReduction > 0)
                                        {
                                            for (int IQ = 0; IQ < Program.SourceGroups.Count; IQ++)
                                            {
                                                Program.ReceptorConc[irec][IQ] *= VolumeReduction;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } // receptor correction

                }

            }
        }

        private static void ShowCopyright(string[] args)
        {
            Console.WriteLine("[GRAL]  Copyright (C) <2019>  <Dietmar Oettl, Markus Kuntner>");
            Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY; for details start GRAL with a startup parameter ‘show_w’");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; start GRAL with a startup parameter ‘show_c’ for details. )");

            if (args.Length > 0 && args[0].Contains("show_w"))
            {
                Console.WriteLine("This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of" +
                                  " MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.");
            }
            else if (args.Length > 0 && args[0].Contains("show_c"))
            {
                Console.WriteLine("This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by" +
                                  "the Free Software Foundation, either version 3 of the License.");
                Console.WriteLine("You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Parallel loop for the release of particles from all sources
        /// </summary>
        /// <param name="inclusiveLowerBound">Start index of the loop</param>
        /// <param name="exclusiveUpperBound">Final index of the loop + 1</param>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void ParallelParticleDriver(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            if (Program.UseFixedRndSeedVal)
            {
                // Custom ligthweight parallel loop for the release of particles from all sources
                using (ManualResetEvent manResetEvent = new ManualResetEvent(false))
                {
                    int remainingWorkItems = Program.pOptions.MaxDegreeOfParallelism;
                    int nextIteration = inclusiveLowerBound;
                    int percent10 = (int)(Program.NTEILMAX * 0.1F);

                    // Create each of the work items up to MaxDegreeOfParallelism
                    for (int p = 0; p < Program.pOptions.MaxDegreeOfParallelism; p++)
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            int index;
                            while ((index = Interlocked.Increment(ref nextIteration) - 1) < exclusiveUpperBound)
                            {
                                // start the particle driver for the particle No. index
                                Zeitschleife.Calculate(index);
                                // show progress bar
                                if (index % percent10 == 0 && index > 0)
                                {
                                    Console.Write("I");
                                }
                            }
                            if (Interlocked.Decrement(ref remainingWorkItems) == 0)
                            {
                                manResetEvent.Set();
                            }
                        });
                    }
                    // Wait for all threads to complete
                    manResetEvent.WaitOne();
                }
            }
            else
            {
                // default parallel loop
                int IPERC = 0;
                int advance = 0;
                int percent10 = (int)(NTEILMAX * 0.1F);
                ParallelOptions pOpt = Program.pOptions;
                int locker = 0;
                Parallel.For(inclusiveLowerBound, exclusiveUpperBound, pOpt, nteil =>
                {
                    Interlocked.Increment(ref advance);
                    if (advance > percent10)
                    {
                        Interlocked.Exchange(ref advance, 0); // set advance to 0
                        Interlocked.Add(ref IPERC, 10);
                        Console.Write("I");
                        if (IPERC % 20 == 0 && locker == 0)
                        {
                            Interlocked.Increment(ref locker);
                            try
                            {
                                using (StreamWriter sr = new StreamWriter("Percent.txt", false))
                                {
                                    if (ISTATIONAER == Consts.TransientMode)
                                    {
                                        sr.Write((50 + MathF.Round(IPERC * 0.5F)).ToString());
                                    }
                                    else
                                    {
                                        sr.Write(IPERC.ToString());
                                    }
                                }
                            }
                            catch { }
                            Interlocked.Decrement(ref locker);
                        }
                    }
                    Zeitschleife.Calculate(nteil);
                });

                Console.Write("I");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Parallel loop for the release of particles from all transient cells
        /// </summary>
        /// <param name="inclusiveLowerBound">Start index of the loop</param>
        /// <param name="exclusiveUpperBound">Final index of the loop + 1</param>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void ParallelTransientParticleDriver(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            if (Program.UseFixedRndSeedVal)
            {
                // Custom ligthweight parallel loop for the release of particles from all sources
                using (ManualResetEvent manResetEvent = new ManualResetEvent(false))
                {
                    const int batchSize = 16;
                    int remainingWorkItems = Program.pOptions.MaxDegreeOfParallelism;
                    int nextIteration = inclusiveLowerBound;
                    int percent10 = (int)(exclusiveUpperBound * 0.1F);

                    // Create each of the work items up to MaxDegreeOfParallelism
                    for (int p = 0; p < Program.pOptions.MaxDegreeOfParallelism; p++)
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            int indexbatch;
                            while ((indexbatch = Interlocked.Add(ref nextIteration, batchSize) - batchSize) < exclusiveUpperBound)
                            {
                                //internal loop batchSize end
                                int end = indexbatch + batchSize;
                                if (end >= exclusiveUpperBound)
                                {
                                    end = exclusiveUpperBound;
                                }
                                for (int index = indexbatch; index < end; index++)
                                {
                                    // indices of recent cellNr
                                    int i = 1 + (index % Program.NII);
                                    int j = 1 + (int)(index / Program.NII);
                                    for (int k = Program.NKK_Transient; k > 0; k--)
                                    {
                                        for (int IQ = 0; IQ < Program.SourceGroups.Count; IQ++)
                                        {
                                            if (Program.Conz4d[i][j][k][IQ] >= Program.TransConcThreshold)
                                            {
                                                // start the particle driver for the cell [i,j], height k, source group IQ
                                                ZeitschleifeNonSteadyState.Calculate(i, j, k, IQ, Program.Conz4d[i][j][k][IQ]);
                                            }
                                        }
                                    }
                                    // show progress bar
                                    if (index % percent10 == 0)
                                    {
                                        Console.Write("X");
                                    }
                                }
                            }
                            if (Interlocked.Decrement(ref remainingWorkItems) == 0)
                            {
                                manResetEvent.Set();
                            }
                        });
                    }
                    // Wait for all threads to complete
                    manResetEvent.WaitOne();
                }
            }
            else
            {
                // default parallel loop
                int IPERCnss = 0;
                int advancenss = 0;
                int cellNr = NII * NJJ;
                int percent10nss = (int)(cellNr * 0.1F);
                ParallelOptions pOpt = Program.pOptions;
                int locker = 0;
                Parallel.For(inclusiveLowerBound, exclusiveUpperBound, pOpt, cell =>
                {
                    Interlocked.Increment(ref advancenss);
                    if (advancenss > percent10nss)
                    {
                        Interlocked.Exchange(ref advancenss, 0); // set advance to 0
                        Interlocked.Add(ref IPERCnss, 10);
                        if (IPERCnss < 100)
                        {
                            Console.Write("X");
                            if (IPERCnss % 20 == 0 && locker == 0)
                            {
                                Interlocked.Increment(ref locker);
                                try
                                {
                                    using (StreamWriter sr = new StreamWriter("Percent.txt", false))
                                    {
                                        sr.Write(MathF.Round(IPERCnss * 0.5F).ToString());
                                    }
                                }
                                catch { }
                                Interlocked.Decrement(ref locker);
                            }
                        }
                    }

                    // indices of recent cellNr
                    int i = 1 + (cell % NII);
                    int j = 1 + (int)(cell / NII);
                    for (int k = 1; k <= NKK_Transient; k++)
                    {
                        for (int IQ = 0; IQ < Program.SourceGroups.Count; IQ++)
                        {
                            if (Conz4d[i][j][k][IQ] >= TransConcThreshold)
                            {
                                ZeitschleifeNonSteadyState.Calculate(i, j, k, IQ, Conz4d[i][j][k][IQ]);
                            }
                        }
                    }
                });
                Console.Write("X");
            }
        }
    }
    /// <summary>
    /// Some helper functions for Program
    /// </summary>
    class ProgramHelper
    {
        /// <summary>
        /// Volume correction for a concentration array if GralDx != DXK || GralDy != DYK
        /// </summary>
        /// <param name="Conc">Concentration array</param>
        /// <param name="DeltaH">DeltaH for vertical height of the concentration array</param>
        public void VolumeConcCorrection(float[][][][] Conc, float DeltaH)
        {
            // double[,] corr = new double[NXL + 1, NYL + 1]; // DEBUG
            // loop over all concentration cells
            Parallel.For(0, Program.NXL, Program.pOptions, i =>
            {
                double x0 = i * Program.GralDx;
                double xmax = x0 + Program.GralDx;

                Span<double> VolumeReduction = new double[Program.NS];
                Span<float> HMin = new float[Program.NS];
                Span<float> HMax = new float[Program.NS];

                for (int k = 0; k < HMin.Length; k++)
                {
                    HMin[k] = Program.HorSlices[k] - Program.GralDz * 0.5F + DeltaH;
                    HMax[k] = Program.HorSlices[k] + Program.GralDz * 0.5F + DeltaH;
                }

                for (int j = 0; j < Program.NYL; ++j)
                {
                    double y0 = j * Program.GralDy;
                    double ymax = y0 + Program.GralDy;
                    VolumeReduction.Clear();

                    // loop over all flow field cells inside the concentration cell
                    for (double xi = x0; xi < xmax; xi += Program.DXK)
                    {
                        int IndexI = (int)(xi / Program.DXK) + 1;
                        if (IndexI > 0 && IndexI < Program.NII + 1)
                        {
                            for (double yi = y0; yi < ymax; yi += Program.DYK)
                            {
                                int IndexJ = (int)(yi / Program.DYK) + 1;
                                if (IndexJ > 0 && IndexJ < Program.NJJ + 1)
                                {
                                    float buidingheight = Program.BUI_HEIGHT[IndexI][IndexJ];
                                    // loop over all slices
                                    for (int k = 0; k < HMin.Length; k++)
                                    {
                                        if (buidingheight > HMin[k])
                                        {
                                            VolumeReduction[k] += Program.DXK * Program.DYK * (Math.Min(buidingheight, HMax[k]) - HMin[k]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    int iko = i + 1;
                    int jko = j + 1;
                    // Correction of the concentration
                    for (int k = 0; k < HMin.Length; k++)
                    {
                        if (VolumeReduction[k] > 0)
                        {
                            for (int IQ = 0; IQ < Program.SourceGroups.Count; IQ++)
                            {
                                // Limit factor to 10, because in these cases, just a small portion of volume is left
                                float factor = (float)Math.Max(1, Math.Min(10, (Program.GridVolume / Math.Max(0.01F, Program.GridVolume - VolumeReduction[k]))));
                                Conc[iko][jko][k][IQ] *= factor;
                                // corr[iko, jko] = factor; // DEBUG
                            }
                        }
                    }

                }
            });

            // string Filename = "VolumeCorrection.txt"; // DEBUG
            // StringBuilder SB = new StringBuilder();
            // using (StreamWriter myWriter = new StreamWriter(Filename))
            // {
            //     // Header
            //     myWriter.WriteLine("ncols         " + Convert.ToString(Program.NXL, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("nrows         " + Convert.ToString(Program.NYL, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("xllcorner     " + Convert.ToString(Program.GralWest, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("yllcorner     " + Convert.ToString(Program.GralSouth, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("cellsize      " + Convert.ToString(Program.GralDx, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("NODATA_value  -9999");

            //     for (int j = Program.NYL; j > 0; j--)
            //     {
            //         SB.Clear();
            //         for (int i = 1; i <= Program.NXL; i++)
            //         {
            //             SB.Append(corr[i, j].ToString(CultureInfo.InvariantCulture));
            //             SB.Append(" ");
            //         }
            // 		myWriter.WriteLine(SB.ToString());
            //     }

            // }
            // SB = null;
        }

        /// <summary>
        /// Volume correction for a concentration array if GralDx == DXK || GralDy == DYK
        /// </summary>
        /// <param name="Conc">Concentration array</param>
        /// <param name="DeltaH">DeltaH for vertical height of the concentration array</param>
        public void VolumeConcCorrection2(float[][][][] Conc, float DeltaH)
        {
            //double[,] corr = new double[Program.NXL + 1, Program.NYL + 1]; // DEBUG

            // loop over all concentration cells
            Parallel.For(0, Program.NXL, Program.pOptions, i =>
            {
                Span<float> HMin = new float[Program.NS];
                Span<float> HMax = new float[Program.NS];

                for (int k = 0; k < HMin.Length; k++)
                {
                    HMin[k] = Program.HorSlices[k] - Program.GralDz * 0.5F + DeltaH;
                    HMax[k] = Program.HorSlices[k] + Program.GralDz * 0.5F + DeltaH - 0.2F;
                }

                for (int j = 0; j < Program.NYL; ++j)
                {
                    float buidingheight = Program.BUI_HEIGHT[i][j];

                    // loop over all slices
                    for (int k = 0; k < HMin.Length; k++)
                    {
                        if (buidingheight > HMin[k] && buidingheight < HMax[k])
                        {
                            // Limit factor to 10, because in these cases, just a small portion of volume is left
                            float VolumeReduction = MathF.Max(1, MathF.Min(10, Program.GralDz / (Program.GralDz - (buidingheight - HMin[k]))));
                            for (int IQ = 0; IQ < Program.SourceGroups.Count; IQ++)
                            {
                                Conc[i][j][k][IQ] *= VolumeReduction;
                                //corr[i, j] = VolumeReduction; // DEBUG
                            }
                        }
                    }
                }
            });

            // string Filename = "VolumeCorrection.txt"; // DEBUG
            // StringBuilder SB = new StringBuilder();
            // using (StreamWriter myWriter = new StreamWriter(Filename))
            // {
            //     // Header
            //     myWriter.WriteLine("ncols         " + Convert.ToString(Program.NXL, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("nrows         " + Convert.ToString(Program.NYL, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("xllcorner     " + Convert.ToString(Program.GralWest, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("yllcorner     " + Convert.ToString(Program.GralSouth, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("cellsize      " + Convert.ToString(Program.GralDx, CultureInfo.InvariantCulture));
            //     myWriter.WriteLine("NODATA_value  -9999");

            //     for (int j = Program.NYL; j > 0; j--)
            //     {
            //         SB.Clear();
            //         for (int i = 1; i <= Program.NXL; i++)
            //         {
            //             SB.Append(corr[i, j].ToString(CultureInfo.InvariantCulture));
            //             SB.Append(" ");
            //         }
            // 		myWriter.WriteLine(SB.ToString());
            //     }

            // }
            // SB = null;
        }
    }
}