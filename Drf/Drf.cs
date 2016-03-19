using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Drf
{
	public class Drf
	{
		private const int BufferSize = 262144;
		private readonly HashAlgorithm _crypt;

		public Drf()
		{
			_crypt = MD5.Create();
		}

		public string[] Search(string path)
		{
			var itemSizes = new Dictionary<long, string>();
			var fileHashs = new Dictionary<string, string>();
			var dubs = new List<string>();

			string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				var fi = new FileInfo(file);
				var length = fi.Length;
				if (itemSizes.ContainsKey(length))
				{
					var cand = itemSizes[length];
					if (!fileHashs.ContainsValue(cand))
					{
						var candHash = GetHash(_crypt, cand);
						fileHashs.Add(candHash, cand);
					}
					var fileHash = GetHash(_crypt, file);
					if (fileHashs.ContainsKey(fileHash))
					{
						dubs.Add(file);
					}
					else
					{
						fileHashs.Add(fileHash, file);
					}
				}
				else
				{
					itemSizes.Add(length, file);
				}
			}
			return dubs.ToArray();
		}

		private string GetHash(HashAlgorithm crypt, string fileName)
		{
			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.SequentialScan))
			{
				var hash = crypt.ComputeHash(fs);
				return fs.Length + BitConverter.ToString(hash);
			}
		}
	}
}
