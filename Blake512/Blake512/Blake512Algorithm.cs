using System;
using System.Security.Cryptography;


namespace Blake512
{
    public sealed class Blake512Algorithm : HashAlgorithm
    {
        private const int _roundCount = 16;
        private ulong[] _h = new ulong[8];
        private ulong[] _s = new ulong[4];
        private ulong _t;
        private int _buffLength;
        private bool _nullT;
        private byte[] _messageBuffer = new byte[128]; 
        private ulong[] _v = new ulong[16];
        private ulong[] _m = new ulong[16];

        private static readonly int[] _sigma = new int[_roundCount * 16] {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            14, 10, 4, 8, 9, 15, 13, 6, 1, 12, 0, 2, 11, 7, 5, 3,
            11, 8, 12, 0, 5, 2, 15, 13, 10, 14, 3, 6, 7, 1, 9, 4,
            7, 9, 3, 1, 13, 12, 11, 14, 2, 6, 5, 10, 4, 0, 15, 8,
            9, 0, 5, 7, 2, 4, 10, 15, 14, 1, 11, 12, 6, 8, 3, 13,
            2, 12, 6, 10, 0, 11, 8, 3, 4, 13, 7, 5, 15, 14, 1, 9,
            12, 5, 1, 15, 14, 13, 4, 10, 0, 7, 6, 3, 9, 2, 8, 11,
            13, 11, 7, 14, 12, 1, 3, 9, 5, 0, 15, 4, 8, 6, 2, 10,
            6, 15, 14, 9, 11, 3, 0, 8, 12, 2, 13, 7, 1, 4, 10, 5,
            10, 2, 8, 4, 7, 6, 1, 5, 15, 11, 9, 14, 3, 12, 13, 0,
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            14, 10, 4, 8, 9, 15, 13, 6, 1, 12, 0, 2, 11, 7, 5, 3,
            11, 8, 12, 0, 5, 2, 15, 13, 10, 14, 3, 6, 7, 1, 9, 4,
            7, 9, 3, 1, 13, 12, 11, 14, 2, 6, 5, 10, 4, 0, 15, 8,
            9, 0, 5, 7, 2, 4, 10, 15, 14, 1, 11, 12, 6, 8, 3, 13,
            2, 12, 6, 10, 0, 11, 8, 3, 4, 13, 7, 5, 15, 14, 1, 9
        };

        private static readonly ulong[] _c = new ulong[16] {
            0x243F6A8885A308D3UL, 
            0x13198A2E03707344UL, 
            0xA4093822299F31D0UL,
            0x082EFA98EC4E6C89UL,
            0x452821E638D01377UL, 
            0xBE5466CF34E90C6CUL,
            0xC0AC29B7C97C50DDUL, 
            0x3F84D5B5B5470917UL, 
            0x9216D5D98979FB1BUL,
            0xD1310BA698DFB5ACUL, 
            0x2FFD72DBD01ADFB7UL, 
            0xB8E1AFED6A267E96UL,
            0xBA7C9045F12C7F99UL, 
            0x24A19947B3916CF7UL,
            0x0801F2E2858EFC16UL,
            0x636920D871574E69UL
        };

        private static readonly byte[] _padding = new byte[128] {
            0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private static readonly ulong[] _iv = new ulong[8]
        {
            0x6A09E667F3BCC908UL,
            0xBB67AE8584CAA73BUL,
            0x3C6EF372FE94F82BUL,
            0xA54FF53A5F1D36F1UL,
            0x510E527FADE682D1UL,
            0x9B05688C2B3E6C1FUL,
            0x1F83D9ABFB41BD6BUL,
            0x5BE0CD19137E2179UL
        };

        public Blake512Algorithm()
        {
            HashSizeValue = 512;
            Initialize();
        }

        /// <summary>
        /// Сбрасывает алгоритм в начальное состояние
        /// </summary>
        public override void Initialize()
        {
            Array.Copy(_iv, 0, _h, 0, _h.Length);
            Array.Clear(_s, 0, _s.Length);
            Array.Clear(_messageBuffer, 0, _messageBuffer.Length);
            _nullT = false;
            _buffLength = 0;
            _t = 0;
        }

        /// <summary>
        /// Вычисляет хэш
        /// </summary>
        /// <param name="array">входные данные, для которых вычисляется хэш</param>
        /// <param name="ibStart">начало чтения данных</param>
        /// <param name="cbSize">количество данных, которое нужно прочитать</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int offset = ibStart; // с какой позции читать байты
            int fill = 128 - _buffLength; // размер свободного место для заполнения
            
            if (_buffLength > 0 && cbSize >= fill)
            {
                Array.Copy(array, offset, _messageBuffer, _buffLength, fill);
                _t += 1024;
                Compress(_messageBuffer, 0);
                offset += fill;
                cbSize -= fill;
                _buffLength = 0;
            }

            // бежим и поблочно сжимаем полученные данные
            while (cbSize >= 128)
            {
                _t += 1024;
                Compress(array, offset);
                offset += 128;
                cbSize -= 128;
            }

            // если что-то осталось, то запоминаем это в буфер
            if (cbSize > 0)
            {
                Array.Copy(array, offset, _messageBuffer, _buffLength, cbSize);
                _buffLength += cbSize;
            }
            else
            {
                _buffLength = 0;
            }
        }

        /// <summary>
        /// Завершает вычисление хэша
        /// </summary>
        /// разбирается с входными данными, добавляя padding, и вычисляет hashcore
        /// <returns>хэш</returns>
        protected override byte[] HashFinal()
        {
            byte[] pbMsg = new byte[16];
            UInt64ToBytes(_t + ((ulong)_buffLength << 3), pbMsg, 8);

            if (_buffLength == 111)
			{
				_t -= 8;
				HashCore(new byte[1] { 0x81 }, 0, 1);
			}
			else
			{
				if(_buffLength < 111)
				{
					if(_buffLength == 0) _nullT = true;
					_t -= 888UL - ((ulong)_buffLength << 3);
					HashCore(_padding, 0, 111 - _buffLength);
				}
				else
				{
					_t -= 1024UL - ((ulong)_buffLength << 3);
					HashCore(_padding, 0, 128 - _buffLength);
					_t -= 888UL;
					HashCore(_padding, 1, 111);
					_nullT = true;
				}
				HashCore(new byte[1] { 0x01 }, 0, 1);
				_t -= 8;
			}

			_t -= 128;
			HashCore(pbMsg, 0, 16);

            byte[] pbDigest = new byte[64];
            for (int i = 0; i < 8; ++i)
                UInt64ToBytes(_h[i], pbDigest, i << 3);
            return pbDigest;
        }

        /// <summary>
        /// Производит логическое сложение результатов побитовых сдвигов исходного числа
        /// </summary>
        /// <param name="u">исходное число</param>
        /// <param name="nBits">на сколько бит сдвиг</param>
        /// <returns></returns>
        private static ulong Shift(ulong u, int nBits)
        {
            return ((u >> nBits) | (u << (64 - nBits)));
        }

        /// <summary>
        /// Блок вычисления
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="r">номер раунда</param>
        /// <param name="i">номер блока</param>
        private void G(int a, int b, int c, int d, int r, int i)
        {
            int p = (r << 4) + i;
            int p0 = _sigma[p];
            int p1 = _sigma[p + 1];

            _v[a] += _v[b] + (_m[p0] ^ _c[p1]);
            _v[d] = Shift(_v[d] ^ _v[a], 32);
            _v[c] += _v[d];
            _v[b] = Shift(_v[b] ^ _v[c], 25);
            _v[a] += _v[b] + (_m[p1] ^ _c[p0]);
            _v[d] = Shift(_v[d] ^ _v[a], 16);
            _v[c] += _v[d];
            _v[b] = Shift(_v[b] ^ _v[c], 11);
        }

        /// <summary>
        /// Записывает обрабатываемый блок сообщения
        /// </summary>
        /// <param name="block">сообщение</param>
        /// <param name="offset">смещение</param>
        private void SetMessage(byte[] block, int offset)
        {
            for (int i = 0; i < 16; ++i)
                _m[i] = BytesToUInt64(block, offset + (i << 3));
        }

        /// <summary>
        /// Устанавливает состояние системы
        /// </summary>
        private void SetVariables()
        {
            Array.Copy(_h, _v, 8);
            _v[8] = _s[0] ^ _c[0];
            _v[9] = _s[1] ^ _c[1];
            _v[10] = _s[2] ^ _c[2];
            _v[11] = _s[3] ^ _c[3];
            _v[12] = _c[4];
            _v[13] = _c[5];
            _v[14] = _c[6];
            _v[15] = _c[7];

            if (!_nullT)
            {
                _v[12] ^= _t;
                _v[13] ^= _t;
                // _v[14] ^= _t[1];
                // _v[15] ^= _t[1];
            }
        }

        /// <summary>
        /// Устанавливает состояние системы (после выполнения 16 серий раундов) в переменные цепочки
        /// </summary>
        private void SetHashValues()
        {
            for (int i = 0; i < 8; ++i) 
                _h[i] ^= _v[i];

            for (int i = 0; i < 8; ++i) 
                _h[i] ^= _v[i + 8];

            for (int i = 0; i < 4; ++i) 
                _h[i] ^= _s[i];

            for (int i = 0; i < 4; ++i) 
                _h[i + 4] ^= _s[i];
        }

        /// <summary>
        /// Устанавливает состояние системы и сообщение, и запускает серию из 16 раундов по 8 блоков вычисления
        /// </summary>
        /// <param name="block"></param>
        /// <param name="offset"></param>
        private void Compress(byte[] block, int offset)
        {
            SetMessage(block, offset);
            SetVariables();

            for (int r = 0; r < _roundCount; ++r)
            {
                G(0, 4, 8, 12, r, 0);
                G(1, 5, 9, 13, r, 2);
                G(2, 6, 10, 14, r, 4);
                G(3, 7, 11, 15, r, 6);

                G(3, 4, 9, 14, r, 14);
                G(2, 7, 8, 13, r, 12);
                G(0, 5, 10, 15, r, 8);
                G(1, 6, 11, 12, r, 10);
            }

            SetHashValues();
        }

        /// <summary>
		/// Convert an <c>UInt64</c> to 8 bytes using big-endian.
		/// </summary>
		private static void UInt64ToBytes(ulong u, byte[] pbOut, int iOffset)
        {
            for (int i = 7; i >= 0; --i)
            {
                pbOut[iOffset + i] = (byte)(u & 0xFF);
                u >>= 8;
            }
        }

        /// <summary>
		/// Convert 8 bytes to an <c>UInt64</c> using big-endian.
		/// </summary>
		private static ulong BytesToUInt64(byte[] pb, int iOffset)
        {
            return ((ulong)pb[iOffset + 7] | ((ulong)pb[iOffset + 6] << 8) |
                ((ulong)pb[iOffset + 5] << 16) | ((ulong)pb[iOffset + 4] << 24) |
                ((ulong)pb[iOffset + 3] << 32) | ((ulong)pb[iOffset + 2] << 40) |
                ((ulong)pb[iOffset + 1] << 48) | ((ulong)pb[iOffset] << 56));
        }
    }
}
