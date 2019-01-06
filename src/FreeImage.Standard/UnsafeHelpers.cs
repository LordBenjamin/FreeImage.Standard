namespace FreeImageAPI {
	internal unsafe class UnsafeHelpers
	{
		// TODO: This is 5-6x slower than calling RtlCompareMemory on Windows.
		// TODO: there are probably builtin ways to do this in linux/mac...
		// TODO: if not, there's probably a clever way of doing this as longs instead of byte by 
		//       byte (count the bits not set when xor'ing each long?)
		/// <summary>
		/// Compares blocks of memory
		/// and returns the number of bytes that are equivalent.
		/// </summary>
		/// <param name="buf1">A pointer to a block of memory to compare.</param>
		/// <param name="buf2">A pointer to a block of memory to compare.</param>
		/// <param name="count">Specifies the number of bytes to be compared.</param>
		/// <returns>The number of bytes that compare as equal.
		/// If all bytes compare as equal, the input Length is returned.</returns>
		internal static unsafe uint CompareMemory(void* buf1, void* buf2, uint count)
		{
			uint matching = 0;

			byte* buf1Byte = (byte*)buf1;
			byte* buf2Byte = (byte*)buf2;

			byte* buf1ByteEnd = buf1Byte + count;

			while (buf1Byte < buf1ByteEnd)
			{
				if (*buf1Byte++ == *buf2Byte++)
				{
					matching += sizeof(byte);
				}
				else
				{
					break;
				}
			}

			return matching;
		}
	}
}
