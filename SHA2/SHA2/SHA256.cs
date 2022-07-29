using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitCollections;

namespace SHA2
{
    public class SHA256
    {

        private UInt32 _hash0 = 0x6a09e667;
        private UInt32 _hash1 = 0xbb67ae85;
        private UInt32 _hash2 = 0x3c6ef372;
        private UInt32 _hash3 = 0xa54ff53a;
        private UInt32 _hash4 = 0x510e527f;
        private UInt32 _hash5 = 0x9b05688c;
        private UInt32 _hash6 = 0x1f83d9ab;
        private UInt32 _hash7 = 0x5be0cd19;

        private BitList _message;
        private readonly Int64 _originalMessageLength;

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

        private List<BitList> _chunks = new();

        #region Constructors

        public SHA256(string message)
        {
            _message = new BitList(Encoding.ASCII.GetBytes(message));
            _originalMessageLength = _message.Count;
        }
        

        public SHA256(byte[] message)
        {
            throw new NotImplementedException();
        }

        public SHA256(BitList message)
        {
            throw new NotImplementedException();
        }
        #endregion
        public string Hash()
        {
            Padding();
            ProcessMessage();
            return FinalHashValue();
        }

        private string FinalHashValue()
        {
            var h0 = BitList.CreateFromUInt32(_hash0);
            var h1 = BitList.CreateFromUInt32(_hash1);
            var h2 = BitList.CreateFromUInt32(_hash2);
            var h3 = BitList.CreateFromUInt32(_hash3);
            var h4 = BitList.CreateFromUInt32(_hash4);
            var h5 = BitList.CreateFromUInt32(_hash5);
            var h6 = BitList.CreateFromUInt32(_hash6);
            var h7 = BitList.CreateFromUInt32(_hash7);

            var res = new BitList(0);

            foreach (var bit in h0)
            {
                res.Add(bit);
            }
            foreach (var bit in h1)
            {
                res.Add(bit);
            }
            foreach (var bit in h2)
            {
                res.Add(bit);
            }
            foreach (var bit in h3)
            {
                res.Add(bit);
            }
            foreach (var bit in h4)
            {
                res.Add(bit);
            }
            foreach (var bit in h5)
            {
                res.Add(bit);
            }
            foreach (var bit in h6)
            {
                res.Add(bit);
            }
            foreach (var bit in h7)
            {
                res.Add(bit);
            }

            return res.GetHexString().ToLower();
        }

        private void ProcessMessage()
        {
            //break message into chunks
            var chunkCount = _message.Count / 512;
            for (int i = 0; i < chunkCount; i++)
            {
                _chunks.Add(new BitList(512));
                for (int j = 0; j < 512; j++)
                {
                    _chunks[i][j] = _message[i * 512 + j];
                }
            }

            foreach (var chunk in _chunks)
            {
                //create a 64 - entry message schedule array w[0..63] of 32 - bit words
                BitList[] words = new BitList[64];
                for (int i = 0; i < 64; i++)
                {
                    words[i] = new(32, false);
                }

                //copy chunk into first 16 words w[0..15] of the message schedule array
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        words[i][j] = chunk[i * 32 + j];
                    }
                }

                //Extend the first 16 words into the remaining 48 words w[16..63] of the message schedule array:
                for (int i = 16; i < 64; i++)
                {
                    var s0 = words[i - 15].RightRotate(7).XOR(words[i - 15].RightRotate(18)).XOR(words[i - 15].RightShift(3));
                    var s1 = words[i - 2].RightRotate(17).XOR(words[i - 2].RightRotate(19)).XOR(words[i - 2].RightShift(10));

                    UInt32 result = words[i - 16].GetUInt32() + s0.GetUInt32() + words[i - 7].GetUInt32() + s1.GetUInt32();
                    words[i] = BitList.CreateFromUInt32(result);
                }

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
                    var s1 = BitList.CreateFromUInt32(e).RightRotate(6).XOR(BitList.CreateFromUInt32(e).RightRotate(11)).XOR(BitList.CreateFromUInt32(e).RightRotate(25));

                    var ch = BitList.CreateFromUInt32(e).And(BitList.CreateFromUInt32(f)).XOR(BitList.CreateFromUInt32(e).Not().And(BitList.CreateFromUInt32(g)));
                    var temp1 = h + s1.GetUInt32() + ch.GetUInt32() + _roundConstants[i] + words[i].GetUInt32();
                    
                    var s0 = BitList.CreateFromUInt32(a).RightRotate(2).XOR(BitList.CreateFromUInt32(a).RightRotate(13)).XOR(BitList.CreateFromUInt32(a).RightRotate(22));
                    var maj = BitList.CreateFromUInt32(a).And(BitList.CreateFromUInt32(b)).XOR(BitList.CreateFromUInt32(a).And(BitList.CreateFromUInt32(c))).XOR(BitList.CreateFromUInt32(b).And(BitList.CreateFromUInt32(c)));
                    
                    var temp2 = s0.GetUInt32() + maj.GetUInt32();

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

        #region Padding

        private void Padding()
        {
            PaddingAddSingle1();
            PaddingAddZeros();
            PaddingAddMessageLengthAsInt64();
        }

        private void PaddingAddMessageLengthAsInt64()
        {
            //append length of original message as 64 big endian bit int
            var bytes = BitConverter.GetBytes(_originalMessageLength);
            if (BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            var bits = new BitArray(bytes);
            foreach (var bit in bits)
            {
                _message.Add((bool)bit);
            }
        }

        private void PaddingAddZeros()
        {
            //append zeros
            int padding = 0;
            while ((_originalMessageLength + 1 + padding + 64) % 512 != 0)
            {
                padding++;
            }

            for (int i = 0; i < padding; i++)
            {
                _message.Add(false);
            }
        }

        private void PaddingAddSingle1()
        {
            //append a single 1
            _message.Add(true);
        }
        #endregion
    }
}
