using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Linq;

namespace Diceware {
    public class Diceware
    {

        private string _pattern
        {
            get;
            set;
        }

        private string upperFirst(string input){
            if (input == "" || input == null){
                return "";
            }
            return input.Split("")[0].ToUpper() + input.Substring(1);
        }

        private string ArrElementFromInt(string[] arr, int idx, double intmax = 4294967296, int intmin = 0)
        {
            double ratio = idx / (intmax - intmin);
            int index = (int)(ratio * arr.Length);
            return arr[index];
        }

        private string randomElement(string[] arr)
        {
            System.Random random = new System.Random();
            return arr[random.Next(0, arr.Length)];
        }

        private string[] filterArrByStrLength(string[] arr, int minLength = 0)
        {
            string[] result = Array.FindAll(arr, c => c.Length > minLength);
            return result;
        }

        private string generateMD5(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private string[] getStringsFromArr(string[] input, int minLength)
        {
            return this.filterArrByStrLength(input, minLength);
        }

        private int intFromHexStr(string input, int index = 1, int length = 8)
        {
            string sub = input.Substring(index * length, length);
            int parsed = System.Math.Abs(Convert.ToInt32(sub, 16));
            return parsed;
        }

        private string[] getString(string input, string[] _pattern, int minLength)
        {
            Nouns nouns = new Nouns();
            Adjectives adjectives = new Adjectives();
            Animals animals = new Animals();
            string[] suitableWords;
            string hash = "";

            while (hash.Length < _pattern.Length){
                hash += this.generateMD5(input);
            }

            string[] result = _pattern.Select((wordType, wordIndex) => {
                switch (wordType.ToUpper()){
                    case "ADJ":
                        string[] suitableAdjs = this.getStringsFromArr(adjectives.adjectives, minLength);
                        suitableWords = suitableAdjs.Select((entry, index) => {
                            return ArrElementFromInt(suitableAdjs, intFromHexStr(hash, wordIndex, minLength));
                        }).ToArray();
                        int suitableAdjIndex = intFromHexStr(hash, wordIndex);
                        string outAdj = ArrElementFromInt(suitableWords, suitableAdjIndex);
                        return suitableWords[0];
                    case "NOUN":
                        string[] suitableNouns = this.getStringsFromArr(nouns.nouns, minLength);
                        suitableWords = suitableNouns.Select((entry, index) => {
                            return ArrElementFromInt(suitableNouns, intFromHexStr(hash, wordIndex, minLength));
                        }).ToArray();
                        int suitableNounIndex = intFromHexStr(hash, wordIndex);
                        string outNoun = ArrElementFromInt(suitableWords, suitableNounIndex);
                        return suitableWords[0];
                    case "ANIMAL":
                        string[] suitableAnimals = this.getStringsFromArr(animals.animals, minLength);
                        suitableWords = suitableAnimals.Select((entry, index) => {
                            return ArrElementFromInt(suitableAnimals, intFromHexStr(hash, wordIndex, minLength));
                        }).ToArray();
                        int suitableAnimalIndex = intFromHexStr(hash, wordIndex);
                        string outAnimal = ArrElementFromInt(suitableWords, suitableAnimalIndex);
                        return suitableWords[0];
                    default:
                        return "";
                }
            }).ToArray();
            return result;
        }

        public string GenerateDicewareString(string input, string Structure = "adj noun noun", int minWordLength = 8)
        {
            _pattern = Structure;
            string[] pattern = _pattern.Split(" ");
            string[] output = getString(input, Structure.Split(" "), minWordLength);
            return string.Join("-", output);
        }
    }
}