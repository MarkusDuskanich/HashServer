using System.Text;

namespace BL.Hash.SHA256
{
    public class SHA256 : IHash
    {
        private UInt32 _hash0;
        private UInt32 _hash1;
        private UInt32 _hash2;
        private UInt32 _hash3;
        private UInt32 _hash4;
        private UInt32 _hash5;
        private UInt32 _hash6;
        private UInt32 _hash7;

        private readonly UInt32[] _roundConstants = new UInt32[64]
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
        };

        private Int64 _messageLengthBeforePadding;
        private List<byte> _messageAsBytes = default!;
        private List<UInt32> _messageAsUInt32s = default!;

        private UInt32[][] _chunks = default!;

        public string ComputeHash(string value)
        {
            Init(value);
            Padding();
            ConvertMessageToUInt32Array();
            HashMessage();
            return HashValue();
        }

        private void Init(string value)
        {
            _hash0 = 0x6a09e667;
            _hash1 = 0xbb67ae85;
            _hash2 = 0x3c6ef372;
            _hash3 = 0xa54ff53a;
            _hash4 = 0x510e527f;
            _hash5 = 0x9b05688c;
            _hash6 = 0x1f83d9ab;
            _hash7 = 0x5be0cd19;

            _messageAsBytes = new(Encoding.UTF8.GetBytes(value));
            _messageLengthBeforePadding = _messageAsBytes.Count;
        }


        #region Padding

        private void Padding()
        {
            PaddingAdd1();
            PaddingAddZeros();
            PaddingAddMessageLengthAsInt64();
        }

        private void PaddingAddMessageLengthAsInt64()
        {
            BitConverter.GetBytes(_messageLengthBeforePadding).ToList().ForEach(b => _messageAsBytes.Add(b));
        }

        private void PaddingAddZeros()
        {
            //append zeros
            int padding = 0;
            while ((_messageLengthBeforePadding + 1 + padding + 8) % 64 != 0)
            {
                padding++;
                _messageAsBytes.Add(0);
            }
        }

        private void PaddingAdd1()
        {
            _messageAsBytes.Add(0b10000000);
        }
        #endregion

        private void ConvertMessageToUInt32Array()
        {
            var messageAsBytesArray = _messageAsBytes.ToArray();
            _messageAsUInt32s = new();
            for (int i = 0; i <= messageAsBytesArray.Length - 4; i += 4)
            {
                _messageAsUInt32s.Add(BitConverter.ToUInt32(messageAsBytesArray, i));
            }
        }

        private void HashMessage()
        {
            CreateChunksFromMessage();
            CompressMessage();
        }

        private void CreateChunksFromMessage()
        {
            var chunkCount = _messageAsUInt32s.Count / 16;
            _chunks = new UInt32[chunkCount][];
            for (int i = 0; i < chunkCount; i++)
            {
                _chunks[i] = new UInt32[16];
                for (int j = 0; j < 16; j++)
                {
                    _chunks[i][j] = _messageAsUInt32s[i * 16 + j];
                    _chunks[i][j] = _messageAsUInt32s[i * 16 + j];
                }
            }
        }

        private void CompressMessage()
        {
            foreach (var chunk in _chunks)
            {
                //create a 64 - entry message schedule array w[0..63] of 32 - bit words
                var words = CreateMessageScheduleArray(chunk);

                // Initialize working variables to current hash value
                var a = _hash0;
                var b = _hash1;
                var c = _hash2;
                var d = _hash3;
                var e = _hash4;
                var f = _hash5;
                var g = _hash6;
                var h = _hash7;

                //Compression function main loop:
                for (int i = 0; i < 63; i++)
                {
                    var S1 = RightRotate(e, 6) ^ RightRotate(e, 11) ^ RightRotate(e, 25);
                    var ch = e & f ^ ~e & g;
                    var temp1 = h + S1 + ch + _roundConstants[i] + words[i];
                    var S0 = RightRotate(a, 2) ^ RightRotate(a, 13) ^ RightRotate(a, 22);
                    var maj = a & b ^ a & c ^ b & c;
                    var temp2 = S0 + maj;

                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;
                }

                _hash0 += a;
                _hash1 += b;
                _hash2 += c;
                _hash3 += d;
                _hash4 += e;
                _hash5 += f;
                _hash6 += g;
                _hash7 += h;
            }
        }

        private static UInt32[] CreateMessageScheduleArray(UInt32[] chunk)
        {
            UInt32[] words = new UInt32[64];

            //copy chunk into first 16 words w[0..15] of the message schedule array
            for (int i = 0; i < 16; i++)
            {
                words[i] = chunk[i];
            }

            //Extend the first 16 words into the remaining 48 words w[16..63] of the message schedule array:
            for (int i = 16; i < 64; i++)
            {
                var s0 = RightRotate(words[i - 15], 7) ^ RightRotate(words[i - 15], 18) ^ words[i - 15] >> 3;
                var s1 = RightRotate(words[i - 2], 17) ^ RightRotate(words[i - 2], 19) ^ words[i - 2] >> 10;

                words[i] = words[i - 16] + s0 + words[i - 7] + s1;
            }

            return words;
        }

        private string HashValue()
        {
            return Convert.ToString(_hash0, 16)
                + Convert.ToString(_hash1, 16)
                + Convert.ToString(_hash2, 16)
                + Convert.ToString(_hash3, 16)
                + Convert.ToString(_hash4, 16)
                + Convert.ToString(_hash5, 16)
                + Convert.ToString(_hash6, 16)
                + Convert.ToString(_hash7, 16);
        }

        private static UInt32 RightRotate(UInt32 word32bit, byte count) => word32bit >> count | word32bit << 32 - count;
        //private static UInt32 RotateLeft(UInt32 word32bit, byte count) => word32bit << count | word32bit >> 32 - count;
    }
}
