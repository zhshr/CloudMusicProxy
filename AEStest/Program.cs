﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace AEStest
{
    class Program
    {
        static void Main(string[] args)
        {
            String text = "24A014CFACF85FFBE94739C5A9168B2E0F629C75E45698F6806E02A40C5847BACCB7DA176A2DF9E7AA821CE886357AA078BE10136C8787449E401E0F0628E629161AE7BB1FACCBC581D490490CE3875491491D19B54A125C52460EE0D689C50BF421C5AE60EDC27F19F55C9BCFAEA3807C4196708F5021945ACBC690109FFA2F48E7BD5BEEFF4B39F89834A1F765D32037C62F711AC0BFC8A1924F8E1D550CFC274AA97832034B71128BEFC3206BABCAAD9A9F926C5E1B3979139277DBCDF27EC418D0E91ADB34A6B08564B68E0F3FC5C1A204FA2832E8AF3F163E20DD7A4AB83900BAF709C9EFF4902F6ADF158D573FBB16E89AA73F534B1C0CCDE50B0B235742B748A907EFE6C49F46F20D90ED5FF471D016265438FA160C28E2FF32238E5498A1C424D56B4139D441195705C91255A9413A17B6C4A04C072A0E5D1B646991703B19B57F3EEE4A49506799FB5E49EF612EABCFE8887EBB05131F119E5488AE";
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                }
            }
        }
    }
}