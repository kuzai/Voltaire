﻿using CodenameGenerator;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Voltaire.Models;

namespace Voltaire.Controllers.Messages
{
    class PrefixHelper
    {

        public static string ComputePrefix(SocketCommandContext context, Guild guild, string defaultValue = "")
        {
            if (!guild.UseUserIdentifiers)
            {
                return defaultValue;
            }
            var seed = guild.UserIdentifierSeed;
            return Generate(context, seed);
        }

        public static string Generate(SocketCommandContext context, int seed)
        {
            string password = LoadConfig.Instance.config["encryptionKey"];

            //var offset = (ulong)(new Random().Next(0, 10000));
            
            var id = (context.User.Id + (ulong)seed).ToString();

            var bytes = GetHash(id, password);

            var resultString = BitConverter.ToString(bytes);

            var integer = BitConverter.ToInt32(bytes, bytes.Length - 4);

            //Console.WriteLine($"{resultString} {integer} {offset}");

            var generator = new Generator(seed: integer)
            {
                Casing = Casing.PascalCase,
                Parts = new WordBank[] { WordBank.Titles, WordBank.Nouns }
            };
            return generator.Generate();
        }

        public static Byte[] GetHash(String text, String key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return hashBytes;
        }
    }
}
