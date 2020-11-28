﻿using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Blake512
{
    class Program
    {
        static void Main(string[] args)
        {
            Blake512Algorithm blake512 = new Blake512Algorithm();
            byte[] pbData1 = new byte[0];
            byte[] pbHash1 = blake512.ComputeHash(pbData1);
            string s = string.Empty;

            foreach(var v in pbHash1)
            {
                s += $"{Convert.ToString(v, 2)}";
            }

            //s = s.Replace("0", "");
            Console.WriteLine(s);
            Console.WriteLine(s.Length);
            BitArray bArray = new BitArray(pbHash1);

            int trueCount = 0;

            foreach (bool v in bArray)
            {
                if (v)
                    trueCount++;
            }

            int sum = 0;

            foreach (bool v in bArray)
            {
                if (v)
                    sum++;
                else
                    sum--;
            }

            Console.WriteLine(trueCount);
            Console.WriteLine(sum);
        }

		//private static void AreEqual(byte[] arr1, byte[] arr2)
		//{
		//	for (int i = 0; i < arr1.Length; i++)
		//	{
  //              if (arr1[i] != arr2[i])
  //              {
  //                  Console.WriteLine(arr1[i] + " != " + arr2[i]);
  //              }
  //          }
		//}

		private static void SelfTestPriv()
		{
			Blake512Algorithm blake512 = new Blake512Algorithm();

            //byte[] pbData = new byte[1] { 0 };
            //byte[] pbExpc = new byte[64] {
            //    0x97, 0x96, 0x15, 0x87, 0xF6, 0xD9, 0x70, 0xFA,
            //    0xBA, 0x6D, 0x24, 0x78, 0x04, 0x5D, 0xE6, 0xD1,
            //    0xFA, 0xBD, 0x09, 0xB6, 0x1A, 0xE5, 0x09, 0x32,
            //    0x05, 0x4D, 0x52, 0xBC, 0x29, 0xD3, 0x1B, 0xE4,
            //    0xFF, 0x91, 0x02, 0xB9, 0xF6, 0x9E, 0x2B, 0xBD,
            //    0xB8, 0x3B, 0xE1, 0x3D, 0x4B, 0x9C, 0x06, 0x09,
            //    0x1E, 0x5F, 0xA0, 0xB4, 0x8B, 0xD0, 0x81, 0xB6,
            //    0x34, 0x05, 0x8B, 0xE0, 0xEC, 0x49, 0xBE, 0xB3
            //};
            //byte[] pbHash = blake512.ComputeHash(pbData);
            //AreEqual(pbExpc, pbHash);


            byte[] pbData1 = new byte[4] { 2, 2, 2, 1 };
            byte[] pbData2 = new byte[4] { 2, 2, 2, 0 };
            byte[] pbExpc = new byte[64] {
                0xA8, 0xCF, 0xBB, 0xD7, 0x37, 0x26, 0x06, 0x2D,
                0xF0, 0xC6, 0x86, 0x4D, 0xDA, 0x65, 0xDE, 0xFE,
                0x58, 0xEF, 0x0C, 0xC5, 0x2A, 0x56, 0x25, 0x09,
                0x0F, 0xA1, 0x76, 0x01, 0xE1, 0xEE, 0xCD, 0x1B,
                0x62, 0x8E, 0x94, 0xF3, 0x96, 0xAE, 0x40, 0x2A,
                0x00, 0xAC, 0xC9, 0xEA, 0xB7, 0x7B, 0x4D, 0x4C,
                0x2E, 0x85, 0x2A, 0xAA, 0xA2, 0x5A, 0x63, 0x6D,
                0x80, 0xAF, 0x3F, 0xC7, 0x91, 0x3E, 0xF5, 0xB8
            };
            byte[] pbHash1 = blake512.ComputeHash(pbData1);
            byte[] pbHash2 = blake512.ComputeHash(pbData2);
            //Console.WriteLine(pbHash);
            //AreEqual(pbHash1, pbHash2);
        }
	}
}
