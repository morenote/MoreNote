﻿using github.hyfree.SDFWrapper;
using github.hyfree.SDFWrapper.Model;

using MoreNote.Common.Utils;
using MoreNote.Logic.Service;
using MoreNote.Models.Entity.ConfigFile;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreNote.CryptographyProvider.EncryptionMachine.StandardCyptographicDevice
{
    public class SDFProvider : ICryptographyProvider
    {
        private static Object Locker = new object();
        private static bool isInitDo = false;

        private void CheckInitSDF()
        {
            if (!isInitDo)
            {
                lock (Locker)
                {
                    if (isInitDo)
                    {
                        //写入配置文件
                        var configText = File.ReadAllText(sdfConfig.ConfigFilePath);
                        var fileName = Path.GetFileName(sdfConfig.ConfigFilePath);
                        File.WriteAllText(fileName, configText);
                        isInitDo = true;
                    }
                }
            }
        }

        private SDFConfig sdfConfig;

        private SDFHelper sdfHelper;

        public SDFProvider(ConfigFileService configFileService)
        {
            sdfConfig = configFileService.WebConfig.SDFConfig;
            CheckInitSDF();
            sdfHelper = new SDFHelper(sdfConfig.SDFDLLFilePath);
        }

        public byte[] Hmac(byte[] data)
        {
            var hamc = sdfHelper.SDF_HMAC(data);

            return hamc;
        }

        public byte[] SM4Decrypt(byte[] data, byte[] iv)
        {
            var dec = sdfHelper.SM4_Encrypt(iv, data, true);
            return dec;
        }

        public byte[] SM4Encrypt(byte[] data, byte[] iv)
        {
            var enc = sdfHelper.SM4_Encrypt(iv, data, true);
            return enc;
        }

        public bool VerifyHmac(byte[] data, byte[] mac)
        {
            var temp = Hmac(data);

            return SecurityUtil.SafeCompareByteArray(temp, mac);
        }

        public byte[] TransEncrypted(byte[] data , byte[] iv)
        {
            if (iv.Length!=16)
            {
                throw new ArgumentException("iv len !=16");
            }
        
            var plain = SM2Decrypt(data);
            var enc= SM4Encrypt(data, iv);
            return enc;
        }

        public byte[] SM2Encrypt(byte[] data)
        {
            var sm2 = sdfHelper.SM2_Encrypt(data, 11);

            return sm2.ToByteArrayC1C3C2();
        }

        public byte[] SM2Decrypt(byte[] data)
        {
            SM2Cipher sm2= SM2Cipher.InsanceC1C3C2(data,true);
            var dec=sdfHelper.SM2_Decrypt(sm2,11);
            return dec;
        }
    }
}