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
				using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.SequentialScan))
				using (var bs = new BufferedStream(fs))
				{
					var length = fs.Length;
					if (itemSizes.ContainsKey(length))
					{
						var cand = itemSizes[length];
						if (!fileHashs.ContainsValue(cand))
						{
							using (var fs2 = new FileStream(cand, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.SequentialScan))
							using (var bs2 = new BufferedStream(fs2))
							{
								var candHash = GetHash(_crypt, bs2);
								fileHashs.Add(candHash, cand);
							}
						}
						var fileHash = GetHash(_crypt, bs);
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
						itemSizes.Add(fs.Length, file);
					}
				}
			}
			return dubs.ToArray();
		}

		private string GetHash(HashAlgorithm crypt, Stream stream)
		{
			var hash = crypt.ComputeHash(stream);

			return stream.Length + BitConverter.ToString(hash);
		}
	}
}
